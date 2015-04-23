namespace Errlock.Lib.Sessions
{
    public class SessionScanOptions
    {
        public int RecursionDepth { get; set; }
        public int FetchPerPage { get; set; }
        public int MaxLinks { get; set; }
        public bool UseRandomLinks { get; set; }
        public bool IngoreAnchors { get; set; }
    }
}