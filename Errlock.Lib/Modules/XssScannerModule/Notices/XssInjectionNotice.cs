using System;

namespace Errlock.Lib.Modules.XssScannerModule.Notices
{
    public class XssInjectionNotice : ModuleNotice
    {
        private readonly string _queryString;

        public XssInjectionNotice(string linkedUrl, string queryString) : base(linkedUrl)
        {
            this._queryString = queryString;
        }

        private const string TextFormat = "Данный сайт подвержен XSS-инъекциям.\n" +
                                          "URL: {0}\n" +
                                          "Строка запроса: {1}";

        public override string Text
        {
            get { return String.Format(TextFormat, LinkedUrl, _queryString); }
        }

        public override NoticePriority Priority
        {
            get { return NoticePriority.High; }
        }
    }
}
