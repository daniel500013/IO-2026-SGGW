using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Eksportuje wyniki oceny do pliku Excel (<c>.xlsx</c>) przy użyciu biblioteki ClosedXML.
    /// </summary>
    /// <remarks>
    /// Tworzony plik zawiera dwa arkusze: "Wyniki" ze szczegółowym wykazem wszystkich przypadków testowych
    /// (z kolorowaniem komórki statusu) oraz "Podsumowanie" z procentem dla każdego zadania osobno
    /// (zielony przy 100%, czerwony przy 0%, żółty pośrodku) oraz wynikiem ogólnym ("Całość") dla każdego studenta.
    /// </remarks>
    public class ResultsExporter
    {
        /// <summary>
        /// Zapisuje przekazane wiersze wyników do nowego pliku Excel.
        /// </summary>
        /// <param name="rows">Wiersze wyników do wyeksportowania.</param>
        /// <param name="path">Docelowa ścieżka pliku <c>.xlsx</c> (istniejący plik zostanie nadpisany).</param>
        /// <remarks>
        /// Arkusz "Wyniki" zawiera kolumny: Student, Zadanie, Parametry, Oczekiwany, Uzyskany, Punkty, Status,
        /// a komórka statusu jest kolorowana przez <see cref="ColorForStatus"/>. Arkusz "Podsumowanie" grupuje
        /// wiersze po studencie i dla każdego zadania wylicza procent zdanych przypadków (komórka zielona przy
        /// 100%, czerwona przy 0%, żółta pośrodku) oraz wynik ogólny "Całość" (suma punktów / liczba przypadków).
        /// </remarks>
        public void Export(IList<ResultRow> rows, string path)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Wyniki");
                ws.Cell(1, 1).Value = "Student";
                ws.Cell(1, 2).Value = "Zadanie";
                ws.Cell(1, 3).Value = "Parametry";
                ws.Cell(1, 4).Value = "Oczekiwany";
                ws.Cell(1, 5).Value = "Uzyskany";
                ws.Cell(1, 6).Value = "Punkty";
                ws.Cell(1, 7).Value = "Status";
                ws.Range(1, 1, 1, 7).Style.Font.Bold = true;
                int r = 2;
                foreach (var row in rows)
                {
                    ws.Cell(r, 1).Value = row.Student;
                    ws.Cell(r, 2).Value = row.Zadanie;
                    ws.Cell(r, 3).Value = row.Parametry;
                    ws.Cell(r, 4).Value = row.Oczekiwany;
                    ws.Cell(r, 5).Value = row.Uzyskany;
                    ws.Cell(r, 6).Value = row.Punkty;
                    ws.Cell(r, 7).Value = row.Status.ToString();
                    ws.Cell(r, 7).Style.Fill.BackgroundColor =
                    ColorForStatus(row.Status);
                    r++;
                }
                ws.Columns().AdjustToContents();
                var summary = wb.Worksheets.Add("Podsumowanie");
                var zadania = rows.Select(x => x.Zadanie).Distinct().ToList();
                int colPct = 2 + zadania.Count;
                summary.Cell(1, 1).Value = "Student";
                for (int i = 0; i < zadania.Count; i++)
                {
                    summary.Cell(1, 2 + i).Value = zadania[i];
                }
                summary.Cell(1, colPct).Value = "Całość";
                summary.Range(1, 1, 1, colPct).Style.Font.Bold = true;
                int sr = 2;
                foreach (var grp in rows.GroupBy(x => x.Student))
                {
                    summary.Cell(sr, 1).Value = grp.Key;
                    for (int i = 0; i < zadania.Count; i++)
                    {
                        var caseRows = grp.Where(x => x.Zadanie == zadania[i]).ToList();
                        var cell = summary.Cell(sr, 2 + i);
                        if (caseRows.Count == 0)
                        {
                            cell.Value = "-";
                            continue;
                        }
                        int passed = caseRows.Sum(x => x.Punkty);
                        cell.Value = (double)passed / caseRows.Count;
                        cell.Style.NumberFormat.Format = "0.00%";
                        cell.Style.Fill.BackgroundColor = ColorForScore(passed, caseRows.Count);
                    }
                    int total = grp.Count();
                    int sum = grp.Sum(x => x.Punkty);
                    summary.Cell(sr, colPct).Value = total > 0 ? (double)sum / total : 0;
                    summary.Cell(sr, colPct).Style.NumberFormat.Format = "0.00%";
                    sr++;
                }
                summary.Columns().AdjustToContents();
                wb.SaveAs(path);
            }
        }
        /// <summary>
        /// Zwraca kolor tła komórki statusu w eksporcie, odpowiadający danemu <see cref="RunStatus"/>.
        /// </summary>
        /// <param name="s">Status wykonania przypadku testowego.</param>
        /// <returns>
        /// Kolor ClosedXML przypisany do statusu; dla statusów bez własnego koloru
        /// (m.in. <see cref="RunStatus.BrakMetody"/>, <see cref="RunStatus.ZlyFormatParametrow"/>)
        /// zwracany jest jasnoszary.
        /// </returns>
        private static XLColor ColorForStatus(RunStatus s)
        {
            switch (s)
            {
                case RunStatus.Ok: return XLColor.LightGreen;
                case RunStatus.Bledny: return XLColor.LightCoral;
                case RunStatus.Timeout: return XLColor.Orange;
                case RunStatus.Wyjatek: return XLColor.LightYellow;
                case RunStatus.BladKompilacji: return XLColor.DarkGray;
                default: return XLColor.LightGray;
            }
        }
        /// <summary>
        /// Zwraca kolor tła komórki zadania w arkuszu "Podsumowanie" zależnie od liczby zdanych przypadków.
        /// </summary>
        /// <param name="passed">Liczba zdanych przypadków testowych zadania.</param>
        /// <param name="total">Łączna liczba przypadków testowych zadania.</param>
        /// <returns>
        /// Zielony przy komplecie zdanych przypadków (100%), czerwony przy zerze (0%),
        /// żółty dla wyniku częściowego.
        /// </returns>
        private static XLColor ColorForScore(int passed, int total)
        {
            if (passed == total) return XLColor.LightGreen;
            if (passed == 0) return XLColor.LightCoral;
            return XLColor.Yellow;
        }
    }
}