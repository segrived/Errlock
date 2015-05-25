namespace Errlock.Lib.DomParser
{
    public class DomParser
    {
        private string _htmlRawData { get; set; }

        public DomParser(string htmlRawData)
        {
            this._htmlRawData = htmlRawData;
        }
    }
}
