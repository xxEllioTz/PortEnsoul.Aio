using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.DamageJson;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using System.Threading.Tasks;
using EnsoulSharp.SDK.Clipper;
using System.Text;
using SharpDX;
using Color = System.Drawing.Color;

using xxEliot.Core;

namespace BubbaKush
{

   internal class Program
    {
        internal const int FlashRange = 425, IgniteRange = 600, SmiteRange = 570;

        internal static Items.Item Bilgewater, BotRuinedKing, Youmuu, Tiamat, Ravenous, Titanic;

        internal static SpellSlot Flash = SpellSlot.Unknown, Ignite = SpellSlot.Unknown, Smite = SpellSlot.Unknown;

        internal static Menu MainMenu;

        internal static AIHeroClient Player;

        internal static Spell Q, Q2, Q3, W, W2, E, E2, R, R2, R3;

        private static bool isSkinReset = true;

        private static void InitItem()
        {
            Bilgewater = new Items.Item(ItemId.Bilgewater_Cutlass, 550);
            BotRuinedKing = new Items.Item(ItemId.Blade_of_the_Ruined_King, 550);
            Youmuu = new Items.Item(ItemId.Youmuus_Ghostblade, 0);
            Tiamat = new Items.Item(ItemId.Tiamat_Melee_Only, 400);
            Ravenous = new Items.Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Items.Item(3748, 0);
        }



    }
}
