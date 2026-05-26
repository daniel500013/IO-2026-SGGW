using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using ClosedXML.Excel;
using IO_2026_SGGW.Core;

class Program
{
    static void Main()
    {
        string path = Path.Combine(Path.GetTempPath(), "repro_klucz.xlsx");

        try
        {
            Console.WriteLine("[1] Tworzenie XLSX (ClosedXML zapis)...");
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Zadanie1a");
                ws.Cell(1, 1).Value = "Zadanie";
                ws.Cell(1, 2).Value = "Parametry";
                ws.Cell(1, 3).Value = "Odpowiedz";
                ws.Cell(2, 1).Value = "Zadanie1a";
                ws.Cell(2, 2).Value = "5, 10";
                ws.Cell(2, 3).Value = "15";
                ws.Cell(3, 1).Value = "Zadanie1a";
                ws.Cell(3, 2).Value = "[1,2,3]";
                ws.Cell(3, 3).Value = "6";
                wb.SaveAs(path);
            }
            Console.WriteLine("    OK -> " + path);

            Console.WriteLine("[2] AnswerKeyLoader.Load (ClosedXML odczyt)...");
            var key = new AnswerKeyLoader().Load(path);
            Console.WriteLine("    OK -> zakladek: " + key.Tasks.Count + ", test case'ow: " + key.Tasks[0].TestCases.Count);

            Console.WriteLine("[3] Kompilacja Roslyn + ocenianie (GradingService)...");
            var students = new List<StudentSolution>
            {
                new StudentSolution
                {
                    StudentId = "student1",
                    FilePath = "student1.cs",
                    SourceCode = "using System.Linq; public class Sol { public int Zadanie1a(int a, int b) { return a + b; } public int Zadanie1a(int[] t) { return t.Sum(); } }",
                    LastModified = DateTime.Now
                }
            };

            var results = new BindingList<ResultRow>();
            var progress = new Progress<int>(p => { });
            new GradingService().RunAsync(students, key, 3000, results, progress).GetAwaiter().GetResult();

            System.Threading.Thread.Sleep(300);

            Console.WriteLine("    Wynikow: " + results.Count);
            foreach (var r in results)
                Console.WriteLine($"    [{r.Status}] {r.Student} {r.Zadanie} params={r.Parametry} oczek={r.Oczekiwany} uzysk={r.Uzyskany} pkt={r.Punkty}");

            Console.WriteLine("WSZYSTKO OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine("!!! WYJATEK: " + ex.GetType().FullName);
            Console.WriteLine("    " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
