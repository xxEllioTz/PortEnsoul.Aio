using EnsoulSharp.SDK;
using EnsoulSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortEnsoulSharp.AIO
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += GameEvent_OnGameLoad;
            //Events.OnLoad += Events_OnLoad;

        }

        private static void GameEvent_OnGameLoad()
        {
            Console.WriteLine("Done");
            PortAIO.Init.Initialize();
        }

        //private static void Events_OnLoad(object sender, EventArgs e)
        //{
        //    Console.WriteLine("Done");
        //}
    }
}
