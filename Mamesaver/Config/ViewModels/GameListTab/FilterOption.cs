namespace Mamesaver.Config.ViewModels.GameListTab
{
    public class FilterOption
    {
        public FilterOption(string text, FilterMode filterMode)
        {
            Text = text;
            FilterMode = filterMode;
        }

        public string Text { get; }
        public FilterMode FilterMode { get; }
    }
}