using System.Resources;

namespace Errlock.Lib.Modules
{
    public abstract class ModuleNotice
    {
        private static readonly ResourceManager NoticeDescriptions
            = Resources.NoticeDescriptions.ResourceManager;

        public string LinkedUrl { get; private set; }
        public abstract string Text { get; }
        public abstract NoticePriority Priority { get; }
        public string Information { get; set; }

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