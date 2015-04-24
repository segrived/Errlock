using System;
using System.IO;

namespace Errlock.Lib.Sessions
{
    public class SessionLogFile
    {
        public string Module { get; set; }
        public string LogFilePath { get; set; }
        public DateTime CreatingDate { get; set; }
        public Lazy<string> Content
        {
            get { return new Lazy<String>(() => File.ReadAllText(this.LogFilePath)); }
        }

        public SessionLogFile(string logFile)
        {
            this.LogFilePath = logFile;
            this.Module = Directory.GetParent(logFile).Name;
            string fileName = Path.GetFileNameWithoutExtension(logFile);
            this.CreatingDate = Double.Parse(fileName).UnixTimestampToDateTime();
        }
    }
}
