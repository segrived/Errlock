using System.Resources;

namespace Errlock.Lib.Modules
{
    public abstract class ModuleNotice
    {
        private static readonly ResourceManager NoticeDescriptions
            = Resources.NoticeDescriptions.ResourceManager;

        protected string LinkedUrl { get; private set; }
        protected abstract NoticePriority Priority { get; }
        private string Information { get; set; }

        public virtual string Text
        {
            get { return string.Format("[{0}] | {1}", this.Priority, this.LinkedUrl); }
        }

        protected ModuleNotice(string linkedUrl)
        {
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