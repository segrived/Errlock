using System.Resources;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules
{
    public abstract class ModuleNotice
    {
        private static readonly ResourceManager NoticeDescriptions
            = Resources.NoticeDescriptions.ResourceManager;

        public Session Session { get; set; }
        public string LinkedUrl { get; private set; }
        public abstract NoticePriority Priority { get; }
        public string Information { get; private set; }

        public virtual string Text
        {
            get { return string.Format("[{0}] | {1}", this.Priority, this.LinkedUrl); }
        }

        protected ModuleNotice(Session session, string linkedUrl)
        {
            this.Session = session;
            this.LinkedUrl = linkedUrl;

            string resName = this.GetType().Name.TrimEnd("Notice");
            this.Information = NoticeDescriptions.GetString(resName)
                               ?? "Дополнительная информация не предоставлена.";
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}