using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_2026_SGGW.Core
{
    public class SolutionRunner
    {
        public MethodInfo FindMethod(Assembly asm, string taskName)
        {
            var nameNormalized = taskName.Replace(" ", "").Replace("_", "");
            foreach (var tpye in asm.GetTypes())
            {
                foreach (var method in tpye.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    // Czy Main jest potrzebny? Podobno zalecany.
                    if (method.Name.Equals("Main", StringComparison.OrdinalIgnoreCase)) continue;
                    if (method.Name.Replace("_", "").Equals(nameNormalized, StringComparison.OrdinalIgnoreCase)) return method;
                }
            }

            return null;
        }

        public object[] ParseArgs(string paramsCsv, ParameterInfo[] paramInfos)
        {
            if (String.IsNullOrWhiteSpace(paramsCsv))
            {
                if (paramInfos.Length == 0) return new object[0];
                throw new ArgumentException($"Metoda oczekuje {paramInfos.Length} argumentów. Dostarczono 0.");
            }

            var tokens = SplitRespectingBrackets(paramsCsv); // stringi
            if (tokens.Count != paramInfos.Length)
            {
                throw new ArgumentException($"Metoda oczekuje {paramInfos.Length} argumentów. Dostarczono {tokens.Count}.");
            }


            var toRet = new object[tokens.Count];
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i].Trim();
                var type = paramInfos[i].ParameterType;


                if (type.IsArray)
                {
                    var elemType = type.GetElementType();
                    var parts = PrepareXlsxInput(token);
                    var arr = Array.CreateInstance(elemType, parts.Length);

                    for (int j = 0; j < parts.Length; j++)
                    {
                        arr.SetValue(Convert.ChangeType(parts[j], elemType, CultureInfo.InvariantCulture), j);
                    }
                    toRet[i] = arr;

                }
                else
                {
                    toRet[i] = Convert.ChangeType(token, type, CultureInfo.InvariantCulture);
                }
                
            }


            return toRet;
        }

        [DebuggerHidden]
        public RunResult InvokeWithTimeout(MethodInfo method, object[] args, int timeoutMs)
        {
            object instance = null;
            object result = null;
            Exception exp = null;

            if (!method.IsStatic)
            {
                instance = Activator.CreateInstance(method.DeclaringType);
            }

            void Worker()
            {
                try
                {
                    result = method.Invoke(instance, args);
                }
                catch (TargetInvocationException tie)
                {
                    exp = tie.InnerException ?? tie;
                }
                catch (Exception e)
                {
                    exp = e;
                }
            }

            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Start();

            if (!thread.Join(timeoutMs))
            {
                try // Jest bardzo problematyczne! Nie łatwo zmienić, a nie ma kompatybilności z wersjami .NET 5+
                {
                    thread.Abort();
                }
                catch { }
                return new RunResult { Status = RunStatus.Timeout };
            }

            if (exp != null)
            {
                return new RunResult
                {
                    Status = RunStatus.Wyjatek,
                    ErrorMessage = exp.GetType().Name + ": " + exp.Message,
                };
            }

            return new RunResult { Status = RunStatus.Ok, Value = result };
        }


        public bool IsCorrect(object actual, string expectedFromXlsx, Type returnType)
        {
            if (actual == null) return string.IsNullOrWhiteSpace(expectedFromXlsx);

            if (actual is Array actualArr)
            {
                var expectedParts = PrepareXlsxInput(expectedFromXlsx);
                if (actualArr.Length != expectedParts.Length) return false;


                var elemType = returnType.GetElementType();
                for (int i = 0; i < actualArr.Length; i++)
                {
                    var expEl = Convert.ChangeType(expectedParts[i], elemType, CultureInfo.InvariantCulture);
                    if (elemType == typeof(double) || elemType == typeof(float))
                    {
                        if (Math.Abs(Convert.ToDouble(actualArr.GetValue(i)) - Convert.ToDouble(expEl)) >= 1e-6) return false;
                    }
                    else if (!actualArr.GetValue(i).Equals(expEl)) return false;
                }
                return true;
            }


            if (returnType == typeof(double) || returnType == typeof(float))
            {
                if (double.TryParse(expectedFromXlsx, NumberStyles.Any, CultureInfo.InvariantCulture, out var exp))
                {
                    return Math.Abs(Convert.ToDouble(actual) - exp) < 1e-6;
                }
            }


            return actual.ToString().Trim().Equals(expectedFromXlsx.Trim(), StringComparison.OrdinalIgnoreCase);
        }




        public class RunResult
        {
            public RunStatus Status { get; set; }
            public object Value { get; set; }
            public string ErrorMessage { get; set; }
        }


        private static List<string> SplitRespectingBrackets(string s)
        // MultiD nie jest wspierane
        {
            var toRet = new List<string>();
            var sb = new StringBuilder();
            int depth = 0;

            foreach (char c in s)
            {
                if (c == '[') depth++;
                else if (c == ']') depth--;
                if (depth > 1) throw new NotImplementedException("Tablice wielowymiarowe nie są obecnie wspierane.");

                if (c == ',' && depth == 0)
                {
                    toRet.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0) { toRet.Add(sb.ToString()); }
            return toRet;
        }

        private static string[] PrepareXlsxInput(string xlsxString)
        {
            var expectedInner = xlsxString.Trim().TrimStart('[').TrimEnd(']');
            var expectedParts = expectedInner.Split(',').Select(s => s.Trim()).ToArray();

            return expectedParts;
        }
    }
}
