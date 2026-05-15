using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace IO_2026_SGGW.Core
{
    public class AnswerKeyLoader
    {
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