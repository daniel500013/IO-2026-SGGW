using System;
using System.Collections.Generic;
using System.IO;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Reguły decydujące, czy dany plik nadaje się do dodania jako rozwiązanie studenta.
    /// Wydzielone z <see cref="IO_2026_SGGW.MainForm"/> (metoda AddCsFiles), aby logikę intake
    /// dało się testować jednostkowo (obszar testów T1).
    /// </summary>
    public static class CsFileRules
    {
        /// <summary>
        /// Sprawdza, czy ścieżka wskazuje na akceptowalny plik <c>.cs</c>, którego nie dodano jeszcze wcześniej.
        /// </summary>
        /// <param name="path">Ścieżka rozważanego elementu (plik lub – błędnie – folder).</param>
        /// <param name="alreadyAdded">Ścieżki już dodanych plików (do wykrycia duplikatu).</param>
        /// <returns><c>true</c>, gdy plik wolno dodać; w przeciwnym razie <c>false</c>.</returns>
        public static bool IsAcceptableCsFile(string path, IEnumerable<string> alreadyAdded)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            if (Directory.Exists(path)) return false;                        // T1-11: odrzuć folder (np. "Testy.CS")
            if (!File.Exists(path)) return false;
            if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) return false;

            if (alreadyAdded != null)
            {
                foreach (var p in alreadyAdded)
                    if (string.Equals(p, path, StringComparison.OrdinalIgnoreCase)) return false; // duplikat
            }

            return true;
        }
    }
}
