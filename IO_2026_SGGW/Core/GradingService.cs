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
                int total = students.Count * SumTestCases(key);
                int done = 0;

                foreach (var student in students)
                {
                    var comp = compiler.Compile(student.SourceCode);

                    if (!comp.Success)
                    {
                        EmitCompilationFailureForAllTasks(student, key, comp.ErrorMessage, ui, results);
                        done += SumTestCases(key);
                        progress?.Report(done * 100 / Math.Max(1, total));
                        continue;
                    }

                    foreach (var sheet in key.Tasks)
                    {
                        var method = runner.FindMethod(comp.Assembly, sheet.Name);

                        foreach (var tc in sheet.TestCases)
                        {
                            var row = (method == null)
                                ? MakeRow(student, sheet, tc, RunStatus.BrakMetody, 0, "Brak metody " + sheet.Name.Replace(" ", ""))
                                : GradeOne(student, sheet, tc, method, timeoutMs);

                            ui.Post(_ => results.Add(row), null);
                            done++;
                            progress?.Report(done * 100 / Math.Max(1, total));
                        }
                    }
                }
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
        private ResultRow GradeOne(StudentSolution student, TaskSheet sheet, TestCase tc, MethodInfo method, int timeoutMs)
        {
            object[] args;
            try
            {
                args = runner.ParseArgs(tc.ParametersRaw, method.GetParameters());
            }
            catch (Exception ex)
            {
                return MakeRow(student, sheet, tc, RunStatus.ZlyFormatParametrow, 0, ex.Message);
            }

            var run = runner.InvokeWithTimeout(method, args, timeoutMs);

            if (run.Status == RunStatus.Timeout)
                return MakeRow(student, sheet, tc, RunStatus.Timeout, 0, "Przekroczono limit czasu");

            if (run.Status == RunStatus.Wyjatek)
                return MakeRow(student, sheet, tc, RunStatus.Wyjatek, 0, run.ErrorMessage);

            bool correct = runner.IsCorrect(run.Value, tc.ExpectedRaw, method.ReturnType);

            return correct
                ? MakeRow(student, sheet, tc, RunStatus.Ok, 1, FormatValue(run.Value))
                : MakeRow(student, sheet, tc, RunStatus.Bledny, 0, FormatValue(run.Value));
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
            string errorMessage, SynchronizationContext ui, BindingList<ResultRow> results)
        {
            foreach (var sheet in key.Tasks)
            {
                foreach (var tc in sheet.TestCases)
                {
                    var row = MakeRow(student, sheet, tc, RunStatus.BladKompilacji, 0, errorMessage);
                    ui.Post(_ => results.Add(row), null);
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
    }
}
