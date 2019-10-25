using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PortAIO.Dual_Port
{
    class Misc
    {
        public static Menu info;
        public static void Load()
        {

            info = new Menu("PAIOInfo", "[~] EnsoulAIO - Info", true);
            info.Add(new MenuSeparator("aioBerb", "PortAIO - By xxEliotz 1"));
            info.Add(new MenuSeparator("aioVersion", "Version : " + Game.BuildVersion));
            info.Add(new MenuSeparator("aioNote", "Note : Make sure you're in Borderless!"));
            info.Attach();

        }


    }
}
