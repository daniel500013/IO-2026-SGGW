using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IO_2026_SGGW.Core
{
    public class GradingService
    {
        private readonly SolutionCompiler compiler = new SolutionCompiler();
        private readonly SolutionRunner runner = new SolutionRunner();

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

        private static int SumTestCases(AnswerKey key)
        {
            int sum = 0;
            foreach (var sheet in key.Tasks)
            {
                sum += sheet.TestCases.Count;
            }
            return sum;
        }

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
