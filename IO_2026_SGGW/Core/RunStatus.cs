namespace IO_2026_SGGW.Core
{
    /// <summary>
    /// Status wykonania pojedynczego przypadku testowego, ustalany przez <see cref="GradingService"/>
    /// oraz <see cref="SolutionRunner"/>. Decyduje o liczbie punktów oraz o kolorowaniu wierszy
    /// w interfejsie i w eksporcie do Excela.
    /// </summary>
    public enum RunStatus
    {
        /// <summary>Metoda wykonała się i zwróciła wynik zgodny z kluczem odpowiedzi (1 punkt).</summary>
        Ok,

        /// <summary>Metoda wykonała się poprawnie, ale zwrócony wynik różni się od oczekiwanego (0 punktów).</summary>
        Bledny,

        /// <summary>Wykonanie przekroczyło dozwolony limit czasu i zostało przerwane (0 punktów).</summary>
        Timeout,

        /// <summary>W rozwiązaniu studenta nie znaleziono metody odpowiadającej nazwie zadania (0 punktów).</summary>
        BrakMetody,

        /// <summary>Metoda rzuciła wyjątek w trakcie wykonywania (0 punktów).</summary>
        Wyjatek,

        /// <summary>Kod źródłowy studenta nie skompilował się, co dotyczy wszystkich zadań tego studenta (0 punktów).</summary>
        BladKompilacji,

        /// <summary>Nie udało się sparsować parametrów z klucza do typów wymaganych przez metodę (0 punktów).</summary>
        ZlyFormatParametrow
    }
}
