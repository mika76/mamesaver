/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Mamesaver
{
    public class GameTimer : Timer
    {
        protected List<Game> gameList = null;
        private Process process = null;

        public GameTimer(int interval, List<Game> gameList)
            : base()
        {
            this.Interval = interval;
            this.gameList = gameList;
        }

        public Process Process
        {
            get { return process; }
            set { process = value; }
        }

        public List<Game> GameList
        {
            get { return gameList; }
            set { gameList = value; }
        }
    }
}
