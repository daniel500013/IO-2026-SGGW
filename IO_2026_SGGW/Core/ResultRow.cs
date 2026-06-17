namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Pojedynczy wiersz wyniku oceny: rezultat uruchomienia jednego przypadku testowego
    /// dla jednego studenta i jednego zadania.
    /// </summary>
    /// <remarks>
    /// Wiersze powstają w <see cref="GradingService"/>, są wyświetlane w tabeli (<c>DataGridView</c>)
    /// głównego okna i mogą zostać wyeksportowane do Excela przez <see cref="ResultsExporter"/>.
    /// Nazwy właściwości celowo pozostają po polsku, bo stanowią zarazem nagłówki kolumn w interfejsie
    /// oraz w eksporcie.
    /// </remarks>
    public class ResultRow
    {
        /// <summary>Identyfikator studenta (zwykle nazwa pliku bez rozszerzenia).</summary>
        public string Student { get; set; }

        /// <summary>Nazwa ocenianego zadania (nazwa arkusza z klucza odpowiedzi).</summary>
        public string Zadanie { get; set; }

        /// <summary>Parametry wejściowe przypadku testowego w postaci tekstowej (z klucza).</summary>
        public string Parametry { get; set; }

        /// <summary>Wynik oczekiwany według klucza odpowiedzi.</summary>
        public string Oczekiwany { get; set; }

        /// <summary>
        /// Wynik faktycznie uzyskany z metody studenta albo komunikat błędu/diagnostyczny
        /// (np. treść wyjątku, informacja o braku metody).
        /// </summary>
        public string Uzyskany { get; set; }

        /// <summary>Liczba punktów za ten przypadek: 1 za poprawny wynik, 0 w pozostałych sytuacjach.</summary>
        public int Punkty { get; set; }

        /// <summary>Status wykonania przypadku testowego, decydujący m.in. o kolorze wiersza w UI.</summary>
        public RunStatus Status { get; set; }
    }
}
