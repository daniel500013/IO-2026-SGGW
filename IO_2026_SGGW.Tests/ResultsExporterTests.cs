using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using IO_2026_SGGW.Core;
using Xunit;
public class ResultsExporterTests
{
private static List<ResultRow> Sample() => new List<ResultRow>
{
new ResultRow { Student="Jan", Zadanie="Z1", Parametry="5, 10", Oczekiwany="15", Uzyskany="15",
Punkty=1, Status=RunStatus.Ok },
new ResultRow { Student="Jan", Zadanie="Z2", Parametry="2", Oczekiwany="4", Uzyskany="5",
Punkty=0, Status=RunStatus.Bledny },
};
private static string TempPath() =>
Path.Combine(Path.GetTempPath(), "exp_" + Guid.NewGuid().ToString("N") + ".xlsx");
[Fact] // [OK]
public void Export_TworzyDwaArkusze_ZNaglowkami()
{
var path = TempPath();
try
{
new ResultsExporter().Export(Sample(), path);
Assert.True(File.Exists(path));
using (var wb = new XLWorkbook(path))
{
Assert.True(wb.TryGetWorksheet("Wyniki", out var w));
Assert.True(wb.TryGetWorksheet("Podsumowanie", out _));
Assert.Equal("Student", w.Cell(1, 1).GetString());
Assert.Equal("Status", w.Cell(1, 7).GetString());
Assert.Equal("15", w.Cell(2, 5).GetString()); // Uzyskany pierwszego wiersza
}
}
finally { File.Delete(path); }
}
[Fact] // [OK]
public void Export_Podsumowanie_LiczyProcent()
{
var path = TempPath();
try
{
new ResultsExporter().Export(Sample(), path);
using (var wb = new XLWorkbook(path))
{
var s = wb.Worksheet("Podsumowanie");
Assert.Equal("Jan", s.Cell(2, 1).GetString());
Assert.Equal(1, s.Cell(2, 2).GetValue<int>()); // suma punktów
Assert.Equal(2, s.Cell(2, 3).GetValue<int>()); // liczba zadań
Assert.Equal(0.5, s.Cell(2, 4).GetValue<double>(), 3); // 50%
}
}
finally { File.Delete(path); }
}
[Fact] // [OK]
public void Export_PustaLista_NieRzuca()
{
var path = TempPath();
try
{
var ex = Record.Exception(() => new ResultsExporter().Export(new List<ResultRow>(), path));
Assert.Null(ex);
Assert.True(File.Exists(path));
}
finally { if (File.Exists(path)) File.Delete(path); }
}
[Fact] // [OK]
public void Export_KolorStatusu_Ok_Jasnozielony()
{
var path = TempPath();
try
{
new ResultsExporter().Export(Sample(), path);
using (var wb = new XLWorkbook(path))
{
var w = wb.Worksheet("Wyniki");
Assert.Equal(XLColor.LightGreen, w.Cell(2, 7).Style.Fill.BackgroundColor);
}
}
finally { File.Delete(path); }
}
}