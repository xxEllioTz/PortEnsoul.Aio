
using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using Color = System.Drawing.Color;
namespace LeeSin
{
    internal class AssassinManager
    {
        public AssassinManager()
        {
            Load();
        }

        private static void Load()
        {
            Program.TargetSelectorMenu.Add(new MenuSeparator("MenuAssassin", "Assassin Manager"));
            Program.TargetSelectorMenu.Add(new MenuBool("AssassinActive", "Active", true));
            Program.TargetSelectorMenu.Add(new MenuList("AssassinSelectOption", "Set: ", new[] { "Single Select", "Multi Select" }));
            Program.TargetSelectorMenu.Add(new MenuBool("AssassinSetClick", "Add/Remove with Right-Click"));
            Program.TargetSelectorMenu.Add(new MenuKeyBind("AssassinReset", "Reset List", System.Windows.Forms.Keys.K, KeyBindType.Press));

            Program.TargetSelectorMenu.Add(new MenuSeparator("Draw", "Draw:"));

            Program.TargetSelectorMenu.Add(new MenuColor("DrawSearch", "Search Range", SharpDX.Color.Gray));
            Program.TargetSelectorMenu.Add(new MenuColor("DrawActive", "Active Enemy", SharpDX.Color.GreenYellow));
            Program.TargetSelectorMenu.Add(new MenuColor("DrawNearest", "Nearest Enemy", SharpDX.Color.DarkSeaGreen));

            Program.TargetSelectorMenu.Add(new MenuSeparator("AssassinMode", "Assassin List:"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                Program.TargetSelectorMenu.Add(
                        new MenuBool("Assassin" + enemy.CharacterName, enemy.CharacterName));

            }
            Program.TargetSelectorMenu.Add(new MenuSlider("AssassinSearchRange", "Search Range", 0, 1300, 2000));
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;

        }
        static void ClearAssassinList()
        {
            foreach (
                var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {

                Program.TargetSelectorMenu["Assassin" + enemy.CharacterName].GetValue<MenuBool>().SetValue(false);

            }
        }

        private static void OnUpdate(EventArgs args)
        {
        }

        private static void Game_OnWndProc(GameWndProcEventArgs args)
        {

            if (Program.TargetSelectorMenu["AssassinReset"].GetValue<MenuKeyBind>().Active)
            {
                ClearAssassinList();
                Game.Print(
                    "<font color='#FFFFFF'>Reset Assassin List is Complete! Click on the enemy for Add/Remove.</font>");
            }

            if (args.Msg != 0x201)
            {
                return;
            }

            if (Program.TargetSelectorMenu["AssassinSetClick"].GetValue<MenuBool>())
            {
                foreach (var objAiHero in from hero in ObjectManager.Get<AIHeroClient>()
                                          where hero.IsValidTarget()
                                          select hero
                                              into h
                                          orderby h.Distance(Game.CursorPos) descending
                                          select h
                                                  into enemy
                                          where enemy.Distance(Game.CursorPos) < 150f
                                          select enemy)
                {
                    if (objAiHero != null && objAiHero.IsVisible && !objAiHero.IsDead)
                    {
                        var xSelect =
                            Program.TargetSelectorMenu["AssassinSelectOption"].GetValue<MenuList>().Index;

                        switch (xSelect)
                        {
                            case 0:
                                ClearAssassinList();
                                Program.TargetSelectorMenu["Assassin" + objAiHero.CharacterName].GetValue<MenuBool>().SetValue(true);
                                Game.Print(
                                    string.Format(
                                        "<font color='FFFFFF'>Added to Assassin List</font> <font color='#09F000'>{0} ({1})</font>",
                                        objAiHero.Name, objAiHero.CharacterName));
                                break;
                            case 1:
                                var menuStatus = Program.TargetSelectorMenu["Assassin" + objAiHero.CharacterName].GetValue<MenuBool>().Enabled;
                                Program.TargetSelectorMenu["Assassin" + objAiHero.CharacterName].GetValue<MenuBool>().SetValue(!menuStatus);
                                Game.Print(
                                    string.Format("<font color='{0}'>{1}</font> <font color='#09F000'>{2} ({3})</font>",
                                        !menuStatus ? "#FFFFFF" : "#FF8877",
                                        !menuStatus ? "Added to Assassin List:" : "Removed from Assassin List:",
                                        objAiHero.Name, objAiHero.CharacterName));
                                break;
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Program.TargetSelectorMenu["AssassinActive"].GetValue<MenuBool>().Enabled)
                return;

            var drawSearch = Program.TargetSelectorMenu["DrawSearch"].GetValue<MenuColor>();
            var drawActive = Program.TargetSelectorMenu["DrawActive"].GetValue<MenuColor>();
            var drawNearest = Program.TargetSelectorMenu["DrawNearest"].GetValue<MenuColor>();

            var drawSearchRange = Program.TargetSelectorMenu["AssassinSearchRange"].GetValue<MenuSlider>().Value;

            if (!drawSearch.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, drawSearchRange, Color.Gray);
            }

            foreach (
               var enemy in
                   ObjectManager.Get<AIHeroClient>()
                       .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                       .Where(
                           enemy =>
                               enemy.IsVisible &&
                               Program.TargetSelectorMenu["Assassin" + enemy.CharacterName] != null &&
                               !enemy.IsDead)
                       .Where(
                           enemy => Program.TargetSelectorMenu["Assassin" + enemy.CharacterName].GetValue<MenuBool>().Enabled))
            {
                if (ObjectManager.Player.Distance(enemy.Position) < drawSearchRange)
                {
                    if (!drawActive.Active)
                        Render.Circle.DrawCircle(enemy.Position, 85f, Color.GreenYellow);
                }
                else if (ObjectManager.Player.Distance(enemy.Position) > drawSearchRange &&
                         ObjectManager.Player.Distance(enemy.Position) < drawSearchRange + 400)
                {
                    if (!drawNearest.Active)
                        Render.Circle.DrawCircle(enemy.Position, 85f, Color.DarkSeaGreen);
                }
            }
        }
    }
}