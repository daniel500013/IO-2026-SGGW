using ClosedXML.Excel;
using System;
using System.Globalization;
using System.IO;

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

                    // NAPRAWA T2-06: Jawny zakres wierszy od nagłówka do końca użytego obszaru
                    var firstRow = ws.FirstRowUsed()?.RowNumber() ?? 1;
                    var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;

                    for (int r = firstRow + 1; r <= lastRow; r++) // +1 pomija wiersz nagłówka
                    {
                        try
                        {
                            // NAPRAWA T2-15, T2-17, T2-18: Czytanie przez ReadCellInvariant
                            string parameters = ReadCellInvariant(ws.Cell(r, 2));
                            string expected = ReadCellInvariant(ws.Cell(r, 3));

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
                            throw new Exception($"Błąd odczytu danych w arkuszu '{ws.Name}', wiersz: {r}.", ex);
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

        // NAPRAWA T2-15, T2-17, T2-18: Metoda czytająca komórkę niezależnie od kultury
        // Czyta wartość komórki niezależnie od kultury systemu i formatu:
        // liczba 3,14 (PL) -> "3.14" (kropka, zgodne z Convert.ChangeType(InvariantCulture))
        // data -> ISO "yyyy-MM-dd" (nie psuje parametrów)
        // formuła =5*2 -> policzony wynik "10" (ClosedXML zwraca typ i wartość wyniku, nie tekst formuły)
        private static string ReadCellInvariant(IXLCell cell)
        {
            if (cell == null) return "";

            switch (cell.DataType)
            {
                case XLDataType.Number:
                    return cell.GetValue<double>().ToString(CultureInfo.InvariantCulture);
                case XLDataType.Boolean:
                    return cell.GetValue<bool>() ? "true" : "false";
                case XLDataType.DateTime:
                    return cell.GetValue<DateTime>().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                default:
                    return cell.GetString().Trim();
            }
        }
    }
}