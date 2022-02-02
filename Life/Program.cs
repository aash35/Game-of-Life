using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Display;

namespace Life
{
    class Program
    {
        /// <summary>
        /// Runs the life program by proccessing the arguments handed to it, then it creates a universe and 
        /// renders the universe which behaves in different ways depending on the settings. 
        /// </summary>
        /// <param name="args">arguments that are given to the program through the command line interface</param>
        static void Main(string[] args)
        {
            Settings UniverseSettings = new Settings(args);

            Console.WriteLine(UniverseSettings.ToString());

            Universe universe = new Universe(UniverseSettings);

            // Wait for user to press a spacebar
            universe.Spacebar("start");

            universe.UniverseDisplayer();
        }
        
    }
}
