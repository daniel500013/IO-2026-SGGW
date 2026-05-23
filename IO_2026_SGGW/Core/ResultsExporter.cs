using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
namespace IO_2026_SGGW.Core
{
    public class ResultsExporter
    {
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
                summary.Cell(1, 1).Value = "Student";
                summary.Cell(1, 2).Value = "Suma punktów";
                summary.Cell(1, 3).Value = "Liczba zadań";
                summary.Cell(1, 4).Value = "Procent";
                summary.Range(1, 1, 1, 4).Style.Font.Bold = true;
                int sr = 2;
                foreach (var grp in rows.GroupBy(x => x.Student))
                {
                    int total = grp.Count();
                    int sum = grp.Sum(x => x.Punkty);
                    summary.Cell(sr, 1).Value = grp.Key;
                    summary.Cell(sr, 2).Value = sum;
                    summary.Cell(sr, 3).Value = total;
                    summary.Cell(sr, 4).Value = total > 0 ? (double)sum / total
                    : 0;
                    summary.Cell(sr, 4).Style.NumberFormat.Format = "0.00%";
                    sr++;
                }
                summary.Columns().AdjustToContents();
                wb.SaveAs(path);
            }
        }
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
    }
}