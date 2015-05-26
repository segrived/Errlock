namespace Errlock.Lib.Modules.PublicFinderModule.Notices
{
    public class OpenResourceNotice : ModuleNotice
    {
        private const string TextFormat = "Возможно открытый доступ по URL '{0}', " +
                                          "рекомендуется проверить";

        public override NoticePriority Priority
        {
            get { return NoticePriority.Medium; }
        }

        public override string Text
        {
            get { return string.Format(TextFormat, this.LinkedUrl); }
        }

        public OpenResourceNotice(string linkedUrl)
            : base(linkedUrl) { }
    }
}