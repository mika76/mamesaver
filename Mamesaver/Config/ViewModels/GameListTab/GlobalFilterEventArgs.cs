namespace Mamesaver.Config.ViewModels.GameListTab
{
    public class GlobalFilterEventArgs
    {
        public GlobalFilterEventArgs(FilterMode filterOption) => FilterMode = filterOption;
        public FilterMode FilterMode { get; }
    }
}