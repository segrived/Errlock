using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.ConfigurationTestModule.Notices
{
    class SpecialHeadersNotice : ModuleNotice
    {
        private readonly List<string> _headerKeys;

        private const string TextFormat = "В ответе сервера найдены нестандартные заголовки. " +
                                          "Список нестандартных заголовков: {0}";

        public SpecialHeadersNotice(Session session, string linkedUrl, List<string> headerKeys) 
            : base(session, linkedUrl)
        {
            _headerKeys = headerKeys;
        }

        public override string Text
        {
            get { return String.Format(TextFormat, String.Join(", ", this._headerKeys)); }
        }

        public override NoticePriority Priority
        {
            get { return NoticePriority.Info; }
        }
    }
}
