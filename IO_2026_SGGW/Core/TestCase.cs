namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Pojedynczy przypadek testowy zadania: surowe (tekstowe) parametry wejściowe
    /// oraz oczekiwany wynik, odczytane bezpośrednio z komórek arkusza Excel.
    /// </summary>
    /// <remarks>
    /// Wartości przechowywane są jako nieprzetworzony tekst z pliku XLSX. Parsowaniem parametrów
    /// na argumenty właściwych typów zajmuje się <see cref="SolutionRunner.ParseArgs"/>, a porównaniem
    /// wyniku zajmuje się <see cref="SolutionRunner.IsCorrect"/>.
    /// </remarks>
    public class TestCase
    {
        /// <summary>
        /// Surowy, tekstowy zapis parametrów wejściowych z kolumny 2 arkusza
        /// (np. <c>"2, 3"</c> albo <c>"[1, 2, 3]"</c> dla tablicy).
        /// </summary>
        public string ParametersRaw { get; set; }

        /// <summary>
        /// Surowy, tekstowy zapis oczekiwanego wyniku z kolumny 3 arkusza
        /// (np. <c>"5"</c> albo <c>"[1, 2, 3]"</c>).
        /// </summary>
        public string ExpectedRaw { get; set; }
    }
}
