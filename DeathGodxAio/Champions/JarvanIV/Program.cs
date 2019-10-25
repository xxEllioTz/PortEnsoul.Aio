using System;
using System.Linq;
using System.Collections.Generic;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using Color = System.Drawing.Color;
using static EnsoulSharp.SDK.Items;
using SharpDX.Direct3D9;

namespace StonedJarvan
{
    class Program
    {
        private const string Champion = "JarvanIV";

        private static List<Spell> SpellList = new List<Spell>();

        private static Spell Q;

        private static Spell W;

        private static Spell E;

        private static Spell R;

        public static SpellSlot IgniteSlot;

        public static Menu Menu, ComboMenu, DrawMenu, MiscMenu, LaneMenu, JungleMenu, Misc, KillStealMenu, Drawings;

        private static AIHeroClient Player;

     
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static void JarvanGame_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (!_Player.CharacterName.Contains("JarvanIV")) return;
            Q = new Spell(SpellSlot.Q, 770);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 650);
            IgniteSlot = Player.GetSpellSlot("SummonerDot");
            Q.SetSkillshot(0.25f, 70f, 1450f, false, false, SkillshotType.Line);
            E.SetSkillshot(0.5f, 175f, int.MaxValue, false, false, SkillshotType.Circle);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            Menu = new Menu("Stoned", "StonedJarvan", true);

            ComboMenu = (new Menu("Combo", "Combo"));
            ComboMenu.Add(new MenuBool("UseQCombo", "Use Q"));
            ComboMenu.Add(new MenuBool("UseWCombo", "Use W"));
            ComboMenu.Add(new MenuBool("UseECombo", "Use E"));
            ComboMenu.Add(new MenuBool("UseEQCombo", "Use EQ"));
            ComboMenu.Add(new MenuBool("UseRCombo", "Use R"));
            ComboMenu.Add(new MenuKeyBind("ActiveCombo", "Combo!", System.Windows.Forms.Keys.Space, KeyBindType.Press)).Permashow();
            Menu.Add(ComboMenu);
            LaneMenu = new Menu("Laneclear Settings", "Clear");
            LaneMenu.Add(new MenuSeparator("Laneclear Settings", "Laneclear Settings"));
            LaneMenu.Add(new MenuBool("ClearQ", "Use [Q] Laneclear", false));
            LaneMenu.Add(new MenuBool("ClearE", "Use [E] Laneclear", false));
            LaneMenu.Add(new MenuSlider("manaFarm", "Min Mana For LaneClear", 50, 0, 100));
            Menu.Add(LaneMenu);
            JungleMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleMenu.Add(new MenuBool("jungleQ", "Use [Q] JungleClear"));
            JungleMenu.Add(new MenuBool("jungleE", "Use [E] JungleClear"));
            JungleMenu.Add(new MenuBool("jungleW", "Use [W] JungleClear", false));
            JungleMenu.Add(new MenuSlider("manaJung", "Min Mana For JungleClear", 50, 0, 100));
            Menu.Add(JungleMenu);
            DrawMenu = new Menu("Drawings", "Drawings");
            DrawMenu.Add(new MenuBool("DrawQ", "Draw Q"));
            DrawMenu.Add(new MenuBool("DrawW", "Draw W"));
            DrawMenu.Add(new MenuBool("DrawE", "Draw E"));
            DrawMenu.Add(new MenuBool("DrawR", "Draw R"));
            DrawMenu.Add(new MenuBool("CircleLag", "Lag Free Circles"));
            DrawMenu.Add(new MenuSlider("CircleQuality", "Circles Quality", 100, 100, 10));
            DrawMenu.Add(new MenuSlider("CircleThickness", "Circles Thickness", 1, 10, 1));
            Menu.Add(DrawMenu);
            MiscMenu = new Menu("Misc", "Misc");
            MiscMenu.Add(new MenuKeyBind("EQmouse", "EQ To Mouse", System.Windows.Forms.Keys.G, KeyBindType.Press));
            MiscMenu.Add(new MenuBool("Ignite", "Use Ignite"));
            Menu.Add(MiscMenu);
            Menu.Attach();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            //AIHeroClient.OnCreate += OnCreateObj;
            //AIHeroClient.OnDelete += OnDeleteObj;
            Game.Print("<font color='#FF00BF'>Stoned Jarvan Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>DEATHGODx</font><font color='#40FF00'>Style</font>");
        }

        /* WIP
         * private static void OnCreateObj(GameObject sender, EventArgs args)
         {
             throw new NotImplementedException();
         }

         private static void OnDeleteObj(GameObject sender, EventArgs args)
         {
             throw new NotImplementedException();
         } 
         */



        private static void OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }
            if (MiscMenu["EQmouse"].GetValue<MenuKeyBind>().Active)
            {
                EQMouse();
            }
            Player = ObjectManager.Player;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null) return;
            Orbwalker.AttackState = true;
            if (ComboMenu["ActiveCombo"].GetValue<MenuKeyBind>().Active)
            {
                Combo();
            }
            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && MiscMenu["Ignite"].GetValue<MenuBool>().Enabled)
            {
                Player.Spellbook.CastSpell(IgniteSlot, target);
            }

        }
        private static void LaneClear()
        {
            var useQ = LaneMenu["ClearQ"].GetValue<MenuBool>().Enabled;
            var useE = LaneMenu["ClearE"].GetValue<MenuBool>().Enabled;
            var mana = LaneMenu["manaFarm"].GetValue<MenuSlider>().Value;
            foreach (var minion in GameObjects.EnemyMinions.Where(e => e.IsValidTarget(E.Range)))
            {
                if (useE && E.IsReady() && minion.HealthPercent >= 70 && minion.IsValidTarget(E.Range) && Player.ManaPercent >= mana)
                {
                    E.CastOnUnit(minion);
                }

                if (useQ && Q.IsReady() && minion.IsValidTarget(E.Range))
                {
                    Q.Cast(minion);
                }
            }
        }

        private static void JungleClear()
        {
            var monster = GameObjects.Jungle.Where(j => j.IsValidTarget(E.Range)).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            var useQ = JungleMenu["jungleQ"].GetValue<MenuBool>().Enabled;
            var useW = JungleMenu["jungleW"].GetValue<MenuBool>().Enabled;
            var useE = JungleMenu["jungleE"].GetValue<MenuBool>().Enabled;
            var mana = JungleMenu["manaJung"].GetValue<MenuSlider>().Value;

                if (useQ && Q.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }

                if (Player.ManaPercent < mana) return;

                if (useW && W.IsReady() && monster.IsValidTarget(W.Range))
                {
                    W.CastOnUnit(monster);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(monster);
                }
            }

        private static void EQMouse()
        {
            if (E.IsReady() && Q.IsReady())
            {
                E.Cast(Game.CursorPos);
                Q.Cast(Game.CursorPos);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null) return;

            if (E.IsReady() && Q.IsReady())
            {
                if (Player.Distance(target) <= E.Range && (ComboMenu["UseEQCombo"].GetValue<MenuBool>().Enabled))
                {
                    E.Cast(target);
                    Q.Cast(target);
                }
            }
            else if (Player.Distance(target) <= E.Range && E.IsReady() && (ComboMenu["UseECombo"].GetValue<MenuBool>().Enabled))
            {
                E.Cast(target);
            }
            else if (Player.Distance(target) <= Q.Range && Q.IsReady() && (ComboMenu["UseQCombo"].GetValue<MenuBool>().Enabled))
            {
                Q.Cast(target);
            }
            if (Player.Distance(target) <= R.Range && (ComboMenu["UseRCombo"].GetValue<MenuBool>().Enabled))
            {
                R.Cast(target);
            }
            if (Player.Distance(target) <= W.Range && ComboMenu["UseWCombo"].GetValue<MenuBool>().Enabled)
            {
                W.Cast();
            }


        }

        private static void OnDraw(EventArgs args)
        {
            if (DrawMenu["CircleLag"].GetValue<MenuBool>().Enabled)
            {
                if (DrawMenu["DrawQ"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.DarkRed, DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value + DrawMenu["CircleQuality"].GetValue<MenuSlider>().Value);
                }
                if (DrawMenu["DrawW"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.DarkRed, DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value + DrawMenu["CircleQuality"].GetValue<MenuSlider>().Value);
                }
                if (DrawMenu["DrawE"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.DarkRed, DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value + DrawMenu["CircleQuality"].GetValue<MenuSlider>().Value);
                }
                if (DrawMenu["DrawR"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.DarkRed, DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value + DrawMenu["CircleQuality"].GetValue<MenuSlider>().Value);
                }
            }
            else
            {
                if (DrawMenu["DrawQ"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.DarkRed);
                }
                if (DrawMenu["DrawW"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.DarkRed);
                }
                if (DrawMenu["DrawE"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.DarkRed);
                }
                if (DrawMenu["DrawR"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.DarkRed);
                }
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (IgniteSlot == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(IgniteSlot) != SpellState.Ready) return 0f;
            return (float)Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite);
        }
    }
}
