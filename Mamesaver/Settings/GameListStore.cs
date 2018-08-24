using System.Collections.Generic;
using Mamesaver.Models;

namespace Mamesaver.Settings
{
    public class GameListStore : SettingsStore<List<SelectableGame>>
    {
        public override string Filename => "gamelist.xml";
    }
}