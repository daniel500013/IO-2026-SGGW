using System.Collections.Generic;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Pojedyncze zadanie z klucza odpowiedzi, odpowiadające jednemu arkuszowi pliku XLSX.
    /// </summary>
    /// <remarks>
    /// Nazwa zadania (<see cref="Name"/>) jest jednocześnie nazwą szukanej metody w rozwiązaniu
    /// studenta: <see cref="SolutionRunner.FindMethod"/> dopasowuje metodę po znormalizowanej nazwie.
    /// </remarks>
    public class TaskSheet
    {
        /// <summary>
        /// Nazwa zadania (nazwa arkusza w pliku XLSX). Służy do wyszukania odpowiadającej metody
        /// w skompilowanym rozwiązaniu studenta.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Zestaw przypadków testowych dla tego zadania (kolejne wiersze arkusza).
        /// Domyślnie pusta lista (nigdy <c>null</c>).
        /// </summary>
        public List<TestCase> TestCases { get; set; } = new List<TestCase>();
    }
}
