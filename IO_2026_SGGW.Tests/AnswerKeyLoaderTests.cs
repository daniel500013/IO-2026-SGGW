using System;
using System.Globalization;
using System.IO;
using System.Threading;
using ClosedXML.Excel;
using IO_2026_SGGW.Core;
using Xunit;

public class AnswerKeyLoaderTests
{
    private static string MakeXlsx(string sheetName, Action<IXLWorksheet> fill)
    {
        var path = Path.Combine(Path.GetTempPath(), "key_" + Guid.NewGuid().ToString("N") + ".xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add(sheetName);
            ws.Cell(1, 1).Value = "Zadanie";
            ws.Cell(1, 2).Value = "Parametry";
            ws.Cell(1, 3).Value = "Odpowiedz";
            fill(ws);
            wb.SaveAs(path);
        }
        return path;
    }

    [Fact] // [OK]
    public void Load_ZlaSciezka_RzucaFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() =>
            new AnswerKeyLoader().Load(@"C:\nie\ma\pliku.xlsx"));
    }

    [Fact] // [OK]
    public void Load_PustyPlik_RzucaInvalidData()
    {
        var path = MakeXlsx("Zadanie1", ws => { });
        try { Assert.Throws<InvalidDataException>(() => new AnswerKeyLoader().Load(path)); }
        finally { File.Delete(path); }
    }

    [Fact] // [REGRESJA T2-06]
    public void Load_DwaWiersze_WczytujeOba()
    {
        var path = MakeXlsx("Zadanie1", ws =>
        {
            ws.Cell(2, 2).Value = "5, 10"; ws.Cell(2, 3).Value = "15";
            ws.Cell(3, 2).Value = "1, 2"; ws.Cell(3, 3).Value = "3";
        });
        try
        {
            var key = new AnswerKeyLoader().Load(path);
            Assert.Single(key.Tasks);
            Assert.Equal(2, key.Tasks[0].TestCases.Count);
        }
        finally { File.Delete(path); }
    }

    [Fact] // [REGRESJA T2-18]
    public void Load_Liczba314_CzytanaZKropka_PodKulturaPL()
    {
        var prev = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("pl-PL");
        var path = MakeXlsx("Zadanie1", ws =>
        {
            ws.Cell(2, 2).Value = 3.14;
            ws.Cell(2, 3).Value = "ok";
        });
        try
        {
            var key = new AnswerKeyLoader().Load(path);
            Assert.Equal("3.14", key.Tasks[0].TestCases[0].ParametersRaw);
        }
        finally { Thread.CurrentThread.CurrentCulture = prev; File.Delete(path); }
    }

    [Fact] // [REGRESJA T2-15]
    public void Load_KomorkaData_FormatISO()
    {
        var path = MakeXlsx("Zadanie1", ws =>
        {
            ws.Cell(2, 2).Value = new DateTime(2026, 4, 1);
            ws.Cell(2, 3).Value = "x";
        });
        try
        {
            var key = new AnswerKeyLoader().Load(path);
            Assert.Equal("2026-04-01", key.Tasks[0].TestCases[0].ParametersRaw);
        }
        finally { File.Delete(path); }
    }
}