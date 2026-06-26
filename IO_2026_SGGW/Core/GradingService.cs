using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Orkiestruje cały proces oceny: kompiluje rozwiązania studentów, odnajduje i uruchamia metody
    /// dla kolejnych zadań oraz przypadków testowych, a wyniki na bieżąco dodaje do listy powiązanej z UI.
    /// </summary>
    /// <remarks>
    /// To centralny element przetwarzania, spinający <see cref="SolutionCompiler"/> i <see cref="SolutionRunner"/>.
    /// Praca wykonywana jest w tle (<see cref="Task.Run(System.Action)"/>), a aktualizacje wyników i postępu
    /// trafiają do wątku interfejsu poprzez przechwycony <see cref="SynchronizationContext"/>.
    /// </remarks>
    public class GradingService
    {
        /// <summary>Kompilator kodu źródłowego studentów (Roslyn).</summary>
        private readonly SolutionCompiler compiler = new SolutionCompiler();

        /// <summary>Komponent uruchamiający metody i weryfikujący ich wyniki.</summary>
        private readonly SolutionRunner runner = new SolutionRunner();

        /// <summary>
        /// Asynchronicznie ocenia komplet rozwiązań studentów względem klucza odpowiedzi.
        /// </summary>
        /// <param name="students">Lista rozwiązań do oceny.</param>
        /// <param name="key">Klucz odpowiedzi z zadaniami i przypadkami testowymi.</param>
        /// <param name="timeoutMs">Limit czasu (w ms) na wykonanie pojedynczego przypadku testowego.</param>
        /// <param name="results">
        /// Kolekcja powiązana z UI, do której dopisywane są kolejne wiersze wyników (aktualizowana w wątku interfejsu).
        /// </param>
        /// <param name="progress">Raportowanie postępu w procentach (0-100); może być <c>null</c>.</param>
        /// <returns>Zadanie reprezentujące trwające obliczenia.</returns>
        /// <remarks>
        /// Dla każdego studenta kod jest najpierw kompilowany. Przy błędzie kompilacji dla wszystkich zadań
        /// i przypadków zapisywany jest status <see cref="RunStatus.BladKompilacji"/>. W przeciwnym razie dla
        /// każdego zadania wyszukiwana jest metoda (jej brak daje <see cref="RunStatus.BrakMetody"/>), a następnie
        /// oceniany jest każdy przypadek testowy. Postęp liczony jest względem łącznej liczby przypadków
        /// (liczba studentów × liczba wszystkich przypadków w kluczu).
        /// </remarks>
        public Task RunAsync(List<StudentSolution> students, AnswerKey key, int timeoutMs,
            BindingList<ResultRow> results, IProgress<int> progress)
        {
            var ui = SynchronizationContext.Current ?? new SynchronizationContext();

            return Task.Run(() =>
            {
                int casesPerStudent = SumTestCases(key);
                int total = students.Count * casesPerStudent;
                var buffer = new List<ResultRow>(); // Bufor dla batchowania UI

                for (int i = 0; i < students.Count; i++)
                {
                    var student = students[i];
                    int initialDone = i * casesPerStudent;
                    int done = initialDone;
                    string dllPath = null;

                    try
                    {
                        // Kompilacja raz na studenta do pliku tymczasowego
                        dllPath = compiler.CompileToTempFile(student.SourceCode, out string compileError);

                        if (dllPath == null)
                        {
                            EmitCompilationFailureForAllTasks(student, key, compileError, buffer);
                            done += casesPerStudent;
                            progress?.Report(done * 100 / Math.Max(1, total));
                            continue;
                        }

                        // Weryfikacja zadań przy użyciu wyizolowanego procesu
                        foreach (var sheet in key.Tasks)
                        {
                            foreach (var tc in sheet.TestCases)
                            {
                                var row = GradeOne(student, sheet, tc, dllPath, timeoutMs);
                                buffer.Add(row);
                                done++;

                                // Zrzut (Flush) bufora co 50 wyników, zamiast pojedynczo
                                if (buffer.Count >= 50) FlushBuffer(ui, results, buffer);

                                progress?.Report(done * 100 / Math.Max(1, total));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ochrona przed "trucizną"
                        buffer.Add(new ResultRow { Student = student.StudentId, Zadanie = "BŁĄD KRYTYCZNY", Status = RunStatus.Wyjatek, Uzyskany = ex.Message });
                        done = initialDone + casesPerStudent;
                        progress?.Report(done * 100 / Math.Max(1, total));
                    }
                    finally
                    {
                        FlushBuffer(ui, results, buffer);

                        // Sprzątanie pliku DLL z dysku
                        if (!string.IsNullOrEmpty(dllPath) && System.IO.File.Exists(dllPath))
                        {
                            try { System.IO.File.Delete(dllPath); } catch { }
                        }
                    }
                }

                // Upewniamy się na koniec, że pasek dobił do 100%
                progress?.Report(100);
            });
        }

        /// <summary>
        /// Ocenia pojedynczy przypadek testowy: parsuje parametry, wywołuje metodę z limitem czasu
        /// i porównuje wynik z oczekiwanym.
        /// </summary>
        /// <param name="student">Oceniane rozwiązanie studenta.</param>
        /// <param name="sheet">Zadanie, którego dotyczy przypadek.</param>
        /// <param name="tc">Przypadek testowy (parametry i oczekiwany wynik).</param>
        /// <param name="method">Metoda studenta odpowiadająca zadaniu.</param>
        /// <param name="timeoutMs">Limit czasu wykonania w milisekundach.</param>
        /// <returns>
        /// Gotowy <see cref="ResultRow"/> z odpowiednim statusem i liczbą punktów: 1 punkt dla
        /// <see cref="RunStatus.Ok"/>, w pozostałych przypadkach 0.
        /// </returns>
        private ResultRow GradeOne(StudentSolution student, TaskSheet sheet, TestCase tc, string dllPath, int timeoutMs)
        {
            // Wywołanie izolowanego procesu - to on robi parsowanie, wywołanie z limitem i ocenę
            var run = runner.RunIsolated(dllPath, sheet.Name, tc.ParametersRaw, tc.ExpectedRaw, timeoutMs);

            string uzyskany;
            switch (run.Status)
            {
                case RunStatus.Ok:
                case RunStatus.Bledny:
                    // W RunIsolated wartość Value jest już rzutowana na string przez proces dziecka
                    uzyskany = run.Value as string ?? "";
                    break;
                case RunStatus.Timeout:
                    uzyskany = "Przekroczono limit czasu";
                    break;
                case RunStatus.BrakMetody:
                    uzyskany = "Brak metody " + sheet.Name.Replace(" ", "");
                    break;
                default:
                    uzyskany = run.ErrorMessage ?? "Błąd wykonania";
                    break;
            }

            int punkty = run.Status == RunStatus.Ok ? 1 : 0;
            return MakeRow(student, sheet, tc, run.Status, punkty, uzyskany);
        }

        /// <summary>
        /// Zapisuje status <see cref="RunStatus.BladKompilacji"/> dla wszystkich zadań i przypadków testowych
        /// studenta, którego kod się nie skompilował.
        /// </summary>
        /// <param name="student">Student z błędem kompilacji.</param>
        /// <param name="key">Klucz odpowiedzi (źródło listy zadań i przypadków).</param>
        /// <param name="errorMessage">Komunikat błędu kompilacji wpisywany do każdego wiersza.</param>
        /// <param name="ui">Kontekst synchronizacji UI, przez który dopisywane są wiersze.</param>
        /// <param name="results">Kolekcja wyników powiązana z UI.</param>
        private static void EmitCompilationFailureForAllTasks(StudentSolution student, AnswerKey key,
                    string errorMessage, List<ResultRow> buffer)
        {
            foreach (var sheet in key.Tasks)
            {
                foreach (var tc in sheet.TestCases)
                {
                    var row = MakeRow(student, sheet, tc, RunStatus.BladKompilacji, 0, errorMessage);
                    buffer.Add(row); // Dodajemy do bufora zamiast bezpośrednio powiadamiać UI
                }
            }
        }

        /// <summary>
        /// Tworzy i wypełnia wiersz wyniku (<see cref="ResultRow"/>) na podstawie studenta, zadania
        /// i przypadku testowego.
        /// </summary>
        /// <param name="student">Oceniany student.</param>
        /// <param name="sheet">Zadanie.</param>
        /// <param name="tc">Przypadek testowy.</param>
        /// <param name="status">Status wykonania.</param>
        /// <param name="punkty">Przyznane punkty (0 lub 1).</param>
        /// <param name="uzyskany">Uzyskany wynik lub komunikat diagnostyczny.</param>
        /// <returns>Wypełniony wiersz wyniku.</returns>
        private static ResultRow MakeRow(StudentSolution student, TaskSheet sheet, TestCase tc,
            RunStatus status, int punkty, string uzyskany)
        {
            return new ResultRow
            {
                Student = student.StudentId,
                Zadanie = sheet.Name,
                Parametry = tc.ParametersRaw,
                Oczekiwany = tc.ExpectedRaw,
                Uzyskany = uzyskany,
                Punkty = punkty,
                Status = status
            };
        }

        /// <summary>
        /// Zlicza łączną liczbę przypadków testowych we wszystkich zadaniach klucza.
        /// </summary>
        /// <param name="key">Klucz odpowiedzi.</param>
        /// <returns>Suma liczby przypadków testowych ze wszystkich zadań.</returns>
        private static int SumTestCases(AnswerKey key)
        {
            int sum = 0;
            foreach (var sheet in key.Tasks)
            {
                sum += sheet.TestCases.Count;
            }
            return sum;
        }

        /// <summary>
        /// Formatuje wartość zwróconą przez metodę studenta do postaci tekstowej prezentowanej w wynikach.
        /// </summary>
        /// <param name="value">Wartość do sformatowania; tablice zapisywane są jako <c>"[a, b, c]"</c>.</param>
        /// <returns>
        /// Tekstowa reprezentacja wartości (z <see cref="CultureInfo.InvariantCulture"/>) lub pusty tekst dla <c>null</c>.
        /// </returns>
        private static string FormatValue(object value)
        {
            if (value == null) return "";

            if (value is Array arr)
            {
                var parts = new List<string>();
                foreach (var item in arr)
                {
                    parts.Add(Convert.ToString(item, CultureInfo.InvariantCulture));
                }
                return "[" + string.Join(", ", parts) + "]";
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Zrzuca zebrane wyniki do głównej listy asynchronicznie poprzez wątek UI.
        /// </summary>
        private static void FlushBuffer(SynchronizationContext ui, BindingList<ResultRow> results, List<ResultRow> buffer)
        {
            if (buffer.Count == 0) return;
            var copy = buffer.ToArray();
            buffer.Clear();

            ui.Post(_ =>
            {
                foreach (var r in copy) results.Add(r);
            }, null);
        }
    }
}
