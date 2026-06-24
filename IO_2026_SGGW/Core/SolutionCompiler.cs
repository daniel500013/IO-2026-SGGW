using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Wynik kompilacji kodu źródłowego studenta, zwracany przez <see cref="SolutionCompiler.Compile"/>.
    /// </summary>
    /// <remarks>
    /// Przy powodzeniu ustawione jest pole <see cref="Assembly"/>, a <see cref="ErrorMessage"/> jest puste;
    /// przy niepowodzeniu jest odwrotnie. Najszybciej rozróżnia je właściwość <see cref="Success"/>.
    /// </remarks>
    public class CompilationResult
    {
        /// <summary>
        /// Skompilowany podzespół (assembly) załadowany do pamięci, gotowy do wykonania przez refleksję.
        /// Wartość <c>null</c>, jeśli kompilacja się nie powiodła.
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// Złączone komunikaty błędów kompilatora (oddzielone "; "). Puste, gdy kompilacja się powiodła.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// <c>true</c>, jeśli kompilacja zakończyła się sukcesem (czyli <see cref="Assembly"/> nie jest <c>null</c>).
        /// </summary>
        public bool Success => Assembly != null;
    }

    /// <summary>
    /// Kompiluje w pamięci kod źródłowy C# studenta przy użyciu kompilatora Roslyn
    /// (<c>Microsoft.CodeAnalysis</c>), bez zapisywania jakichkolwiek plików na dysku.
    /// </summary>
    /// <remarks>
    /// Powstały podzespół jest ładowany bezpośrednio ze strumienia pamięci, dzięki czemu
    /// <see cref="SolutionRunner"/> może odnaleźć i wywołać metody studenta przez refleksję.
    /// </remarks>
     
    public class SolutionCompiler
    {
        /// <summary>
        /// Kompiluje podany kod źródłowy do podzespołu w pamięci.
        /// </summary>
        /// <param name="sourceCode">Pełna treść pliku <c>.cs</c> studenta.</param>
        /// <returns>
        /// <see cref="CompilationResult"/> z załadowanym podzespołem przy powodzeniu albo
        /// z komunikatami błędów w <see cref="CompilationResult.ErrorMessage"/> przy niepowodzeniu.
        /// </returns>
        /// <remarks>
        /// Do kompilacji dołączane są podstawowe referencje (m.in. <c>System.Runtime</c>, LINQ i kolekcje),
        /// a kod budowany jest jako biblioteka DLL
        /// (<see cref="Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary"/>). Jako błędy traktowane
        /// są wyłącznie diagnostyki o wadze <see cref="Microsoft.CodeAnalysis.DiagnosticSeverity.Error"/>.
        /// </remarks>
        // Obrona w głąb (T3-16 / T3-17): odrzuć kod sięgający po groźne API.
        private static readonly string[] ForbiddenApis =
        {
            "Environment.Exit", "Environment.FailFast",
            "Process.Start", "Process.GetCurrentProcess", "Process.Kill",
            "File.Delete", "File.WriteAllText", "File.WriteAllBytes", "File.Create", "File.Open",
            "Directory.Delete", "Directory.CreateDirectory",
            "Registry.", "DllImport", "Marshal."
        };
        private static string FindForbiddenApi(string sourceCode)
        {
            foreach (var api in ForbiddenApis)
                if (sourceCode.IndexOf(api, StringComparison.Ordinal) >= 0)
                    return api;
            return null;
        }

        public CompilationResult Compile(string sourceCode)
        {

            // Bezpiecznik bezpieczeństwa - odrzuć groźny kod zanim w ogóle go skompilujemy.
            var forbidden = FindForbiddenApi(sourceCode);
            if (forbidden != null)
            {
                return new CompilationResult
                {
                    ErrorMessage = $"Kod odrzucony ze względów bezpieczeństwa: użyto zabronionego API'{forbidden}'."
                };
            }
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Runtime.dll"))
            };

            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(),
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                if (!emitResult.Success)
                {
                    var errors = emitResult.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d => d.GetMessage());

                    return new CompilationResult { ErrorMessage = string.Join("; ", errors) };
                }

                ms.Seek(0, SeekOrigin.Begin);
                return new CompilationResult { Assembly = Assembly.Load(ms.ToArray()) };
            }
        }

        public string CompileToTempFile(string sourceCode, out string error)
        {
            error = null;
            var forbidden = FindForbiddenApi(sourceCode); // bezpiecznik z Zad.3
            if (forbidden != null) { error = $"Zabronione API '{forbidden}'."; return null; }
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Runtime.dll"))
            };
            var path = Path.Combine(Path.GetTempPath(), "io_" + Guid.NewGuid().ToString("N") + ".dll");
            var compilation = CSharpCompilation.Create(
            Path.GetFileNameWithoutExtension(path),
            new[] { syntaxTree }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var emit = compilation.Emit(fs);
                if (!emit.Success)
                {
                    error = string.Join("; ", emit.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.GetMessage()));
                    return null;
                }
            }
            return path;
        }

    }
}