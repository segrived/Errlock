using System;
using System.Collections.Generic;

namespace Errlock.Lib.Modules.ConfigurationTestModule.Notices
{
    class SpecialHeadersNotice : ModuleNotice
    {
        private readonly List<string> _headerKeys;

        private const string TextFormat = "В ответе сервера найдены нестандартные заголовки. " +
                                          "Список нестандартных заголовков: {0}";

        public SpecialHeadersNotice(string linkedUrl, List<string> headerKeys) 
            : base(linkedUrl)
        {
            _headerKeys = headerKeys;
        }

        public override string Text
        {
            get { return String.Format(TextFormat, String.Join(", ", this._headerKeys)); }
        }

        protected override NoticePriority Priority
        {
            get { return NoticePriority.Info; }
        }
    }
}
