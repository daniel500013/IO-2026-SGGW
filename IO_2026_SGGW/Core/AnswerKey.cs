using System.Collections.Generic;

namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Klucz odpowiedzi wczytany z pliku Excel: komplet zadań wraz z ich przypadkami testowymi.
    /// </summary>
    /// <remarks>
    /// Tworzony przez <see cref="AnswerKeyLoader.Load"/> (jeden arkusz pliku XLSX = jedno
    /// <see cref="TaskSheet"/>). Stanowi wzorzec, względem którego <see cref="GradingService"/>
    /// ocenia rozwiązania studentów.
    /// </remarks>
    public class AnswerKey
    {
        /// <summary>
        /// Lista zadań wchodzących w skład klucza. Każdy element odpowiada jednemu arkuszowi w pliku XLSX.
        /// Domyślnie pusta lista (nigdy <c>null</c>).
        /// </summary>
        public List<TaskSheet> Tasks { get; set; } = new List<TaskSheet>();
    }
}
