using System;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using Color = System.Drawing.Color;
using static EnsoulSharp.SDK.Items;
using SPrediction;

namespace ElRumble
{
    public class ElRumbleMenu
    {
        private static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, heatMenu, clearMenu, miscMenu;

        public static void Initialize()
        {

            Menu = new Menu("ElRumble", "ElRumble",true);

            comboMenu = new Menu("Combo", "Combo");
            comboMenu.Add(new MenuBool("ElRumble.Combo.Q", "Use Q"));
            comboMenu.Add(new MenuBool("ElRumble.Combo.W", "Use W"));
            comboMenu.Add(new MenuBool("ElRumble.Combo.E", "Use E"));
            comboMenu.Add(new MenuBool("ElRumble.Combo.R", "Use R"));
            comboMenu.Add(new MenuSlider("ElRumble.Combo.Count.Enemies", "Enemies in range for R", 2, 1, 5));
            comboMenu.Add(new MenuBool("ElRumble.Combo.Ignite", "Use Ignite"));
            Menu.Add(comboMenu);
            harassMenu = new Menu("Harass", "Harass");
            harassMenu.Add(new MenuBool("ElRumble.Harass.Q", "Use Q"));
            harassMenu.Add(new MenuBool("ElRumble.Harass.E", "Use E"));
            Menu.Add(harassMenu);
            heatMenu = new Menu("Heat", "Heat");
            heatMenu.Add(new MenuKeyBind("ElRumble.KeepHeat.Activated", "Auto harass", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            heatMenu.Add(new MenuBool("ElRumble.Heat.Q", "Use Q"));
            heatMenu.Add(new MenuBool("ElRumble.Heat.W", "Use W"));
            Menu.Add(heatMenu);
            clearMenu = new Menu("Clear", "Clear");
            clearMenu.Add(new MenuBool("ElRumble.LastHit.E", "Lasthit with E"));
            clearMenu.Add(new MenuBool("ElRumble.LaneClear.Q", "Use Q LaneClear"));
            clearMenu.Add(new MenuBool("ElRumble.LaneClear.E", "Use E LaneClear"));
            clearMenu.Add(new MenuBool("ElRumble.JungleClear.Q", "Use Q JungleClear"));
            clearMenu.Add(new MenuBool("ElRumble.JungleClear.E", "Use E JungleClear"));
            Menu.Add(clearMenu);
            miscMenu = new Menu("Misc", "Misc");
            miscMenu.Add(new MenuBool("ElRumble.Draw.Q", "Draw Q"));
            miscMenu.Add(new MenuBool("ElRumble.Draw.E", "Draw E"));
            miscMenu.Add(new MenuBool("ElRumble.Draw.R", "Draw R"));
            miscMenu.Add(new MenuKeyBind("ElRumble.Misc.R", "Manual R", System.Windows.Forms.Keys.L, KeyBindType.Press));
            Menu.Add(miscMenu);
            Menu.Attach();
        }
    }
}