namespace IO_2026_SGGW.Core
{
    public class ResultRow
    {
        public string Student { get; set; }
        public string Zadanie { get; set; }
        public string Parametry { get; set; }
        public string Oczekiwany { get; set; }
        public string Uzyskany { get; set; }
        public int Punkty { get; set; }
        public RunStatus Status { get; set; }
    }
}
