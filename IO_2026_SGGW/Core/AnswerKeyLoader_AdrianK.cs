using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Wczytuje klucz odpowiedzi z pliku Excel (<c>.xlsx</c>) i buduje na jego podstawie
    /// obiekt <see cref="AnswerKey"/>.
    /// </summary>
    /// <remarks>
    /// Konwencja pliku: każdy arkusz to jedno zadanie (jego nazwa = nazwa arkusza), pierwszy wiersz
    /// jest nagłówkiem i jest pomijany, kolumna 2 zawiera parametry, a kolumna 3 zawiera oczekiwany wynik.
    /// Do odczytu wykorzystywana jest biblioteka ClosedXML. Arkusze bez żadnego niepustego przypadku
    /// testowego są pomijane.
    /// </remarks>
    public class AnswerKeyLoader
    {
        /// <summary>
        /// Wczytuje klucz odpowiedzi z podanego pliku XLSX.
        /// </summary>
        /// <param name="xlsxPath">Ścieżka do pliku Excel z kluczem odpowiedzi.</param>
        /// <returns>
        /// Wypełniony <see cref="AnswerKey"/> zawierający wszystkie arkusze, które miały co najmniej
        /// jeden niepusty przypadek testowy.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException">Gdy plik o podanej ścieżce nie istnieje.</exception>
        /// <exception cref="System.IO.InvalidDataException">Gdy plik nie zawiera żadnego poprawnego zadania.</exception>
        /// <exception cref="System.Exception">
        /// Gdy odczyt konkretnego wiersza arkusza się nie powiedzie, komunikat zawiera nazwę arkusza
        /// i numer wiersza, a pierwotna przyczyna jest dostępna jako <see cref="System.Exception.InnerException"/>.
        /// </exception>
        public AnswerKey Load(string xlsxPath)
        {
            if (!File.Exists(xlsxPath))
            {
                throw new FileNotFoundException("Nie znaleziono pliku XLSX", xlsxPath);
            }

            var key = new AnswerKey();

            using (var wb = new XLWorkbook(xlsxPath))
            {
                foreach (var ws in wb.Worksheets)
                {
                    var sheet = new TaskSheet { Name = ws.Name };

                    var usedRows = ws.RowsUsed().Skip(1);

                    foreach (var row in usedRows)
                    {
                        try
                        {
                            string parameters = row.Cell(2).GetString().Trim();
                            string expected = row.Cell(3).GetString().Trim();

                            if (!string.IsNullOrWhiteSpace(parameters) || !string.IsNullOrWhiteSpace(expected))
                            {
                                sheet.TestCases.Add(new TestCase
                                {
                                    ParametersRaw = parameters,
                                    ExpectedRaw = expected
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Błąd odczytu danych w arkuszu '{ws.Name}', wiersz: {row.RowNumber()}.", ex);
                        }
                    }

                    if (sheet.TestCases.Count > 0)
                    {
                        key.Tasks.Add(sheet);
                    }
                }
            }

            if (key.Tasks.Count == 0)
            {
                throw new InvalidDataException("Plik XLSX nie zawiera żadnych poprawnych zadań do załadowania.");
            }

            return key;
        }
    }
}