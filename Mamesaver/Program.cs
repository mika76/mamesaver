/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mamesaver
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string[] arguments = new string[] {"/c"};

            if (args.Length != 0) //default to config if no options passed
                arguments = args;

            Mamesaver saver = new Mamesaver();

            switch (arguments[0].Trim().Substring(0, 2).ToLower())
            {
                case "/c":
                    //TODO: Catch display properties window handle and set it as parent
                    saver.ShowConfig();
                    break;

                case "/s":
                    saver.Run();
                    break;


                case "/p":
                    // do nothing
                    break;
            }
        }
    }
}
