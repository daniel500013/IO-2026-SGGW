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
    /// <summary>
    /// Uruchamia metody ze skompilowanego rozwiązania studenta i porównuje ich wyniki z kluczem.
    /// Odpowiada za odnajdywanie metod przez refleksję, parsowanie parametrów z postaci tekstowej,
    /// bezpieczne wywołanie z limitem czasu oraz weryfikację poprawności wyniku.
    /// </summary>
    /// <remarks>
    /// Współpracuje z <see cref="GradingService"/>, który dla każdego przypadku testowego wywołuje
    /// kolejno: <see cref="FindMethod"/>, <see cref="ParseArgs"/>, <see cref="InvokeWithTimeout"/>
    /// oraz <see cref="IsCorrect"/>.
    /// </remarks>
    public class SolutionRunner
    {
        /// <summary>
        /// Wyszukuje w podzespole metodę odpowiadającą nazwie zadania.
        /// </summary>
        /// <param name="asm">Skompilowany podzespół rozwiązania studenta.</param>
        /// <param name="taskName">Nazwa zadania (nazwa arkusza z klucza).</param>
        /// <returns>
        /// Pasująca <see cref="MethodInfo"/> albo <c>null</c>, jeśli żadna metoda nie odpowiada nazwie zadania.
        /// </returns>
        /// <remarks>
        /// Dopasowanie ignoruje wielkość liter oraz spacje i podkreślenia w nazwie (np. zadanie
        /// "Suma Tablicy" pasuje do metody <c>SumaTablicy</c> lub <c>suma_tablicy</c>). Przeszukiwane są
        /// wszystkie typy podzespołu i ich metody publiczne (statyczne oraz instancyjne) zadeklarowane
        /// bezpośrednio w typie; metoda <c>Main</c> jest pomijana.
        /// </remarks>
        public MethodInfo FindMethod(Assembly asm, string taskName)
        {
            var target = Normalize(taskName);
            MethodInfo prefixMatch = null;
            foreach (var type in asm.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static
                | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (method.Name.Equals("Main", StringComparison.OrdinalIgnoreCase)) continue;
                    var name = Normalize(method.Name);
                    if (name.Equals(target, StringComparison.OrdinalIgnoreCase))
                        return method; // pełne dopasowanie
                                       // T2-16: Excel obcina nazwy arkuszy do 31 znaków -> dopuszczamy dopasowanie po prefiksie
                    if (target.Length >= 31 && name.StartsWith(target, StringComparison.OrdinalIgnoreCase))
                        prefixMatch = prefixMatch ?? method;
                }
            }
            return prefixMatch;
        }
        private static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Trim().Replace(" ", "").Replace("_", "");
        }


        /// <summary>
        /// Zamienia tekstowy zapis parametrów z klucza na tablicę argumentów o typach wymaganych przez metodę.
        /// </summary>
        /// <param name="paramsCsv">
        /// Parametry rozdzielone przecinkami w postaci tekstowej (np. <c>"2, 3"</c>); tablicę zapisuje się
        /// w nawiasach kwadratowych, np. <c>"[1, 2, 3]"</c>.
        /// </param>
        /// <param name="paramInfos">Informacje o parametrach docelowej metody (z <see cref="System.Reflection.MethodBase.GetParameters"/>).</param>
        /// <returns>Tablica argumentów gotowa do przekazania do <see cref="InvokeWithTimeout"/>.</returns>
        /// <exception cref="System.ArgumentException">
        /// Gdy liczba dostarczonych parametrów nie zgadza się z liczbą parametrów metody.
        /// </exception>
        /// <exception cref="System.NotImplementedException">
        /// Gdy parametr jest tablicą wielowymiarową (zagnieżdżone nawiasy kwadratowe), co nie jest obsługiwane.
        /// </exception>
        /// <remarks>
        /// Konwersja typów odbywa się przez <see cref="System.Convert.ChangeType(object, System.Type, System.IFormatProvider)"/>
        /// z <see cref="CultureInfo.InvariantCulture"/>, dzięki czemu separatorem dziesiętnym jest kropka.
        /// Dla parametrów tablicowych każdy element konwertowany jest na typ elementu tablicy.
        /// </remarks>
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

        /// <summary>
        /// Wywołuje metodę na osobnym wątku z ograniczeniem czasu, przechwytując ewentualne wyjątki.
        /// </summary>
        /// <param name="method">Metoda do wywołania.</param>
        /// <param name="args">Argumenty wywołania (zob. <see cref="ParseArgs"/>).</param>
        /// <param name="timeoutMs">Maksymalny czas wykonania w milisekundach.</param>
        /// <returns>
        /// <see cref="RunResult"/> ze statusem <see cref="RunStatus.Ok"/> i zwróconą wartością przy powodzeniu,
        /// <see cref="RunStatus.Timeout"/> po przekroczeniu czasu albo <see cref="RunStatus.Wyjatek"/>,
        /// gdy metoda rzuciła wyjątek.
        /// </returns>
        /// <remarks>
        /// Dla metody instancyjnej tworzony jest egzemplarz typu deklarującego. Metoda wykonywana jest na
        /// wątku w tle, na który oczekuje się przez <see cref="Thread.Join(int)"/>. Uwaga: próba przerwania
        /// przekroczonego wątku korzysta z <see cref="Thread.Abort()"/>, które na platformie .NET 5+ nie jest
        /// wspierane i rzuca wyjątek (przechwytywany po cichu), w takim wypadku wątek (uruchomiony jako tło)
        /// może nadal działać, mimo że metoda zwraca już status <see cref="RunStatus.Timeout"/>.
        /// </remarks>
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


        /// <summary>
        /// Sprawdza, czy faktyczny wynik metody jest zgodny z wartością oczekiwaną z klucza.
        /// </summary>
        /// <param name="actual">Wartość zwrócona przez metodę studenta.</param>
        /// <param name="expectedFromXlsx">Oczekiwany wynik w postaci tekstowej z klucza.</param>
        /// <param name="returnType">Typ zwracany metody, decydujący o sposobie porównania.</param>
        /// <returns><c>true</c>, jeśli wynik jest uznawany za poprawny; w przeciwnym razie <c>false</c>.</returns>
        /// <remarks>
        /// Reguły porównania: dla <c>null</c> wynik jest poprawny tylko wtedy, gdy oczekiwana wartość jest pusta;
        /// tablice porównywane są element po elemencie (po wcześniejszym sprawdzeniu długości); wartości
        /// zmiennoprzecinkowe (<see cref="double"/>/<see cref="float"/>) porównywane są z tolerancją 1e-6;
        /// pozostałe typy porównywane są tekstowo, bez uwzględniania wielkości liter i z przycięciem białych znaków.
        /// </remarks>
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

            if (returnType == typeof(bool))
            {
                var exp = expectedFromXlsx.Trim();
                bool expected;
                if (exp == "1") expected = true;
                else if (exp == "0") expected = false;
                else if (!bool.TryParse(exp, out expected)) return false;
                return Convert.ToBoolean(actual) == expected;
            }



            return actual.ToString().Trim().Equals(expectedFromXlsx.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public RunResult RunIsolated(string dllPath, string taskName, string paramsRaw, string expectedRaw,
int timeoutMs)
        {
            var psi = new ProcessStartInfo
            {
                FileName = typeof(SolutionRunner).Assembly.Location, // exe aplikacji (IO_2026_SGGW.exe), nie host testowy
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            psi.Arguments = "--sandbox \"" + dllPath + "\" " + Enc(taskName) + " " + Enc(paramsRaw) + " " + Enc(expectedRaw);
            using (var proc = Process.Start(psi))
            {
                var stdout = proc.StandardOutput.ReadToEndAsync();
                if (!proc.WaitForExit(timeoutMs))
                {
                    try { proc.Kill(); } catch { } // nieskończona pętla niezabijalny wątek
                return new RunResult { Status = RunStatus.Timeout };
                }
                return MapSandboxOutput(stdout.GetAwaiter().GetResult().Trim());
            }
        }
        private static string Enc(string s) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s ?? ""));
        private static RunResult MapSandboxOutput(string outp)
        {
            if (string.IsNullOrEmpty(outp)) // proces padł (Exit/StackOverflow) bez wyniku
        return new RunResult
        {
            Status = RunStatus.Wyjatek,
            ErrorMessage = "Proces wykonawczy zakończył się awaryjnie." };
        if (outp.StartsWith("OK|")) return new RunResult
        {
            Status = RunStatus.Ok,
            Value =
        outp.Substring(3)
        };
            if (outp.StartsWith("BLEDNY|")) return new RunResult
            {
                Status = RunStatus.Bledny,
                Value =
            outp.Substring(7)
            };
            if (outp.StartsWith("WYJATEK|")) return new RunResult
            {
                Status = RunStatus.Wyjatek,
                ErrorMessage = outp.Substring(8)
            };
            if (outp.StartsWith("ZLYFORMAT|")) return new RunResult
            {
                Status = RunStatus.ZlyFormatParametrow,
                ErrorMessage = outp.Substring(10)
            };
            if (outp == "BRAKMETODY") return new RunResult { Status = RunStatus.BrakMetody };
            return new RunResult { Status = RunStatus.Wyjatek, ErrorMessage = outp };
        }




        /// <summary>
        /// Wynik pojedynczego wywołania metody, zwracany przez <see cref="InvokeWithTimeout"/>.
        /// </summary>
        public class RunResult
        {
            /// <summary>
            /// Status wykonania (np. <see cref="RunStatus.Ok"/>, <see cref="RunStatus.Timeout"/>,
            /// <see cref="RunStatus.Wyjatek"/>).
            /// </summary>
            public RunStatus Status { get; set; }

            /// <summary>
            /// Wartość zwrócona przez metodę przy statusie <see cref="RunStatus.Ok"/>;
            /// w pozostałych przypadkach zwykle <c>null</c>.
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// Komunikat błędu (typ i treść wyjątku) ustawiany przy statusie <see cref="RunStatus.Wyjatek"/>.
            /// </summary>
            public string ErrorMessage { get; set; }
        }


        /// <summary>
        /// Dzieli tekst parametrów po przecinkach, ignorując przecinki znajdujące się wewnątrz
        /// nawiasów kwadratowych (czyli wewnątrz tablic).
        /// </summary>
        /// <param name="s">Tekstowy zapis listy parametrów.</param>
        /// <returns>Lista pojedynczych tokenów-parametrów (jeszcze nieobciętych z białych znaków).</returns>
        /// <exception cref="System.NotImplementedException">
        /// Gdy zagnieżdżenie nawiasów przekracza jeden poziom (tablice wielowymiarowe nie są obsługiwane).
        /// </exception>
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

        /// <summary>
        /// Przygotowuje tekst z pliku XLSX reprezentujący tablicę do dalszej konwersji: usuwa zewnętrzne
        /// nawiasy kwadratowe i dzieli zawartość po przecinkach, obcinając białe znaki.
        /// </summary>
        /// <param name="xlsxString">Tekstowy zapis tablicy, np. <c>"[1, 2, 3]"</c>.</param>
        /// <returns>Tablica tekstowych elementów (np. <c>"1"</c>, <c>"2"</c>, <c>"3"</c>).</returns>
        private static string[] PrepareXlsxInput(string xlsxString)
        {
            var expectedInner = xlsxString.Trim().TrimStart('[').TrimEnd(']');
            var expectedParts = expectedInner.Split(',').Select(s => s.Trim()).ToArray();

            return expectedParts;
        }
    }
}
