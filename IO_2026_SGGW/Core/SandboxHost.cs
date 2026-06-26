using System;
using System.Reflection;
using System.Text;

namespace IO_2026_SGGW.Core
{
    public static class SandboxHost
    {
        public static int Run(string[] args)
        {
            try
            {
                string dll = args[1];
                string taskName = Dec(args[2]);
                string paramsRaw = Dec(args[3]);
                string expected = Dec(args[4]);
                var asm = Assembly.LoadFrom(dll);
                var runner = new SolutionRunner();
                var method = runner.FindMethod(asm, taskName);
                if (method == null) { Console.Out.Write("BRAKMETODY"); return 0; }
                object[] callArgs;
                try { callArgs = runner.ParseArgs(paramsRaw, method.GetParameters()); }
                catch (Exception ex) { Console.Out.Write("ZLYFORMAT|" + ex.Message); return 0; }
                object instance = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);
                object value;
                try { value = method.Invoke(instance, callArgs); }
                catch (TargetInvocationException tie)
                {
                    var inner = tie.InnerException ?? tie;
                    Console.Out.Write("WYJATEK|" + inner.GetType().Name + ": " + inner.Message);
                    return 0;
                }
                bool ok = runner.IsCorrect(value, expected, method.ReturnType);
                Console.Out.Write((ok ? "OK|" : "BLEDNY|") + (value?.ToString() ?? ""));
                return 0;
            }
            catch (Exception ex)
            {
                Console.Out.Write("WYJATEK|" + ex.GetType().Name + ": " + ex.Message);
                return 0;
            }
        }
        private static string Dec(string b64) =>
            Encoding.UTF8.GetString(Convert.FromBase64String(b64));
    }
}
