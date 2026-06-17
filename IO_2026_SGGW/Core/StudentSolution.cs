using System;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Reprezentuje pojedyncze rozwiązanie studenta: wczytany plik źródłowy <c>.cs</c>
    /// wraz z metadanymi potrzebnymi do kompilacji i oceny.
    /// </summary>
    /// <remarks>
    /// Obiekty tej klasy tworzy <see cref="IO_2026_SGGW.MainForm"/> podczas dodawania plików
    /// (przeciągnięcie na panel lub wybór z okna dialogowego), a następnie przekazuje je do
    /// <see cref="GradingService.RunAsync"/>, gdzie pole <see cref="SourceCode"/> jest kompilowane
    /// przez <see cref="SolutionCompiler"/>.
    /// </remarks>
    public class StudentSolution
    {
        /// <summary>
        /// Identyfikator studenta. Domyślnie nazwa pliku bez rozszerzenia (np. <c>"Kowalski_Jan"</c>).
        /// Trafia do kolumny "Student" w wynikach.
        /// </summary>
        public string StudentId { get; set; }

        /// <summary>
        /// Pełna ścieżka do pliku źródłowego <c>.cs</c> na dysku. Używana także do wykrywania duplikatów
        /// przy ponownym dodawaniu tych samych plików.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Treść kodu źródłowego wczytana z pliku. To ją kompiluje <see cref="SolutionCompiler.Compile"/>.
        /// </summary>
        public string SourceCode { get; set; }

        /// <summary>
        /// Data ostatniej modyfikacji pliku na dysku (odczytana przez <see cref="System.IO.File.GetLastWriteTime"/>).
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
