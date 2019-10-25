#region
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
using static EnsoulSharp.SDK.Geometry;

#endregion

namespace ChewyMoonsLux
{
    internal class ChewyMoonsLux
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Ignite;
        private static GameObject _eGameObject;
        public static Spell Q, W, E, R;
        public static Menu drawingMenu, tsMenu, comboMenu, harassMenu, ksMenu, itemsMenu, miscMenu, LaneClearMenu, JungleClearMenu;
        public static bool PacketCast = false;

        public static bool Debug
        {
            get { return miscMenu["debug"].GetValue<MenuBool>().Enabled; }
        }

        public static void LuxOnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Lux")
            {
                return;
            }

            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Q = new Spell(SpellSlot.Q, 1175);
            W = new Spell(SpellSlot.W, 1075);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 3340);
            Q.SetSkillshot(0.25f, 80f, 1200f, false, false, SkillshotType.Line); // to get collision objects
            W.SetSkillshot(0.25f, 150f, 1200f, false,false, SkillshotType.Line);
            R.SetSkillshot(1.35f, 190f, float.MaxValue, false,false, SkillshotType.Line);
            SetupMenu();
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += QGapCloser.OnEnemyGapCloser;
            Game.OnUpdate += Game_OnUpdate;

            Utilities.PrintChat("Loaded. PORT by DeathGODX");
        }
        private static void AutoShield()
        {
            // linq op babbyyy
            foreach (var teamMate in
                from teamMate in ObjectManager.Get<AIBaseClient>().Where(teamMate => teamMate.IsAlly && teamMate.IsValid)
                let hasToBePercent = ChewyMoonsLux.miscMenu["autoShieldPercent"].GetValue<MenuSlider>().Value
                let ourPercent = teamMate.Health / teamMate.MaxHealth * 100
                where ourPercent <= hasToBePercent &&W.IsReady()
                select teamMate)
            {
               W.Cast(teamMate, PacketCast);
            }
        }

        private static void KillSecure()
        {
            // KILL SECURE MY ASS LOOL
            foreach (var hero in
                ObjectManager.Get<AIHeroClient>()
                    .Where(hero => hero.IsValidTarget())
                    .Where(
                        hero =>
                            ObjectManager.Player.Distance(hero) <=R.Range &&
                            ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q) >= hero.Health &&
                           R.IsReady()))
            {
                R.Cast(hero, PacketCast);
            }
        }

        private static void Harass()
        {
            var useQ = ChewyMoonsLux.harassMenu["useQHarass"].GetValue<MenuBool>().Enabled;
            var useE = ChewyMoonsLux.harassMenu["useEHarass"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(1000);
            if (target == null)
            {
                return;
            }

            if (HasPassive(target))
            {
                return;
            }

            if (useQ &Q.IsReady() && !HasPassive(target))
            {
                SpellCombo.CastQ(target);
            }

            if (!useE || !ChewyMoonsLux.E.IsReady() || HasPassive(target) || _eGameObject != null)
            {
                return;
            }
           E.Cast(target, PacketCast);
        }
        private static void LaneClear()
        {
            var useE = LaneClearMenu["E"].GetValue<MenuBool>().Enabled;
            var minE = LaneClearMenu["mine"].GetValue<MenuSlider>().Value;
            var minionsx = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(W.Range) && e.IsMinion())
                .Cast<AIBaseClient>().ToList();
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(E.Range));
            var ECanCast = W.GetLineFarmLocation(minionsx, W.Width);
            if (ECanCast.Position.IsValid())
                foreach (var minion in minions)
                {
                    if (useE && E.IsReady() && minion.IsValidTarget(E.Range) && ECanCast.MinionsHit >= minE)
                    {
                        E.Cast(minion);
                    }
                }
        }
        private static void Combo()
        {
            var useQ = ChewyMoonsLux.comboMenu["useQ"].GetValue<MenuBool>();
            var useW = ChewyMoonsLux.comboMenu["useW"].GetValue<MenuBool>();
            var useE = ChewyMoonsLux.comboMenu["useE"].GetValue<MenuBool>();
            var useR = ChewyMoonsLux.comboMenu["useR"].GetValue<MenuBool>();
            var target = TargetSelector.GetTarget(1000);
            var useDfg = ChewyMoonsLux.itemsMenu["useDFG"].GetValue<MenuBool>();

            // Pop e
            if (_eGameObject != null)
            {
                var targetsInE =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(hero => hero.IsValidTarget(_eGameObject.BoundingRadius))
                        .ToList();
                if (ChewyMoonsLux.E.IsReady() && useE && !HasPassive(target))
                {
                    if (targetsInE.Any(leTarget => !HasPassive(leTarget)))
                    {
                       E.Cast(PacketCast);
                    }
                }
            }

 
            if (target == null)
            {
                return;
            }
            if (HasPassive(target))
            {
                return;
            }

            if (useDfg)
            {
                /* if (Items.CanUseItem(3128) && Items.HasItem(3128))
                 {
                     Items.UseItem(3128, target);
                 }*/
            }

            if (ChewyMoonsLux.Q.IsReady() && useQ && !HasPassive(target))
            {
                SpellCombo.CastQ(target);
            }

            if (ChewyMoonsLux.E.IsReady() && useE && !HasPassive(target))
            {
               E.Cast(target, PacketCast);
            }

            if (ChewyMoonsLux.W.IsReady() && useW)
            {
               W.Cast(Game.CursorPos, PacketCast);
            }

            if (target.IsDead)
            {
                return;
            }
            if (!ChewyMoonsLux.R.IsReady() || !useR || HasPassive(target))
            {
                return;
            }

            if (ChewyMoonsLux.comboMenu["onlyRIfKill"].GetValue<MenuBool>().Enabled)
            {
                if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) >= target.Health)
                {
                   R.Cast(target, PacketCast);
                }
            }
            else
            {
               R.Cast(target, PacketCast);
            }
        }
        private static bool HasPassive(AIBaseClient target)
        {
            return target.HasBuff("luxilluminatingfraulein");
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (ChewyMoonsLux.ksMenu["ultKS"].GetValue<MenuBool>().Enabled)
            {
                KillSecure();
            }
            foreach (var targetz in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(R.Range) && !hero.HasBuff("FioraW") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.HasBuff("SpellShield") && !hero.HasBuff("NocturneShield") && !hero.IsDead && !hero.IsDead))
                if (targetz.Health + targetz.AllShield < _Player.GetSummonerSpellDamage(targetz, SummonerSpell.Ignite))
                {
                    Ignite.Cast(targetz);
                }
            
            // should be reversed but w/e
            if (ChewyMoonsLux.miscMenu["autoShield"].GetValue<MenuKeyBind>().Active)
            {
                    AutoShield();
            
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
        }
        private static void OnDraw(EventArgs args)
        {

            var drawQ = drawingMenu["drawQ"].GetValue<MenuBool>().Enabled;
            var drawW = drawingMenu["drawW"].GetValue<MenuBool>().Enabled;
            var drawE = drawingMenu["drawE"].GetValue<MenuBool>().Enabled;
            var drawR = drawingMenu["drawR"].GetValue<MenuBool>().Enabled;

            var qColor = drawingMenu["qColor"].GetValue<MenuBool>().Enabled;
            var wColor = drawingMenu["wColor"].GetValue<MenuBool>().Enabled;
            var eColor = drawingMenu["eColor"].GetValue<MenuBool>().Enabled;
            var rColor = drawingMenu["rColor"].GetValue<MenuBool>().Enabled;

            var position = ObjectManager.Player.Position;

            if (drawQ)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Red, 1);
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Red, 1);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Red, 1);
            }

            if (drawR)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Red, 1);
            }
        }
        public static void JungleClear()
        {
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(E.Range));
            if (monster != null)
            {
                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.Cast(monster.Position);
                    E.Cast(monster.Position);
                }
            }
        }
        private static void SetupMenu()
        {
            var Menu = new Menu("[Chewy's Lux]", "cmLux", true);
            tsMenu = new Menu("[Chewy's Lux] - TS", "cmLuxTs");
            Menu.Add(tsMenu);
            // Combo settings
            comboMenu = new Menu("[Chewy's Lux] - Combo", "cmLuxCombo");
            comboMenu.Add(new MenuBool("useQ", "Use Q"));
            comboMenu.Add(new MenuBool("useW", "Use W"));
            comboMenu.Add(new MenuBool("useE", "Use E"));
            comboMenu.Add(new MenuBool("useR", "Use R"));
            comboMenu.Add(new MenuBool("onlyRIfKill", "Use R to kill only"));
            comboMenu.Add(new MenuBool("useIgnite", "Use ignite in combo"));
            Menu.Add(comboMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] [Q] JungleClear"));
            Menu.Add(JungleClearMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("E", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("mine", "Min Hit Minions Use [E]", 2, 1, 6));
            Menu.Add(LaneClearMenu);
            // Harass Settings
            harassMenu = new Menu("[Chewy's Lux] - Harass", "cmLuxHarass");
            harassMenu.Add(new MenuBool("useQHarass", "Use Q"));
            harassMenu.Add(new MenuBool("useEHarass", "Use E"));
            Menu.Add(harassMenu);
            // KS / Finisher Settings
            ksMenu = new Menu("[Chewy's Lux] - KS", "cmLuxKS");
            ksMenu.Add(new MenuBool("ultKS", "KS with R"));
           // ksMenu.Add(new MenuBool("recallExploitKS", "KS enemies recalling"));
            Menu.Add(ksMenu);
            // Items
            itemsMenu = new Menu("[Chewy's Lux] - Items", "cmLuxItems");
            itemsMenu.Add(new MenuBool("useDFG", "Use DFG"));
            Menu.Add(itemsMenu);
            //Drawing
            drawingMenu = new Menu("[Chewy's Lux] - Drawing", "cmLuxDrawing");
            drawingMenu.Add(new MenuBool("drawQ", "Draw Q"));
            drawingMenu.Add(new MenuBool("drawW", "Draw W"));
            drawingMenu.Add(new MenuBool("drawE", "Draw E"));
            drawingMenu.Add(new MenuBool("drawR", "Draw R"));
            drawingMenu.Add(new MenuBool("qColor", "Q Color"));
            drawingMenu.Add(new MenuBool("wColor", "W Color"));
            drawingMenu.Add(new MenuBool("eColor", "E Color"));
            drawingMenu.Add(new MenuBool("rColor", "R Color"));
            Menu.Add(drawingMenu);
            // Misc
            miscMenu = new Menu("[Chewy's Lux] - Misc", "cmLuxMisc");
            miscMenu.Add(new MenuBool("antiGapCloserQ", "Stun all gap closers"));
            miscMenu.Add(new MenuBool("packetCast", "Use packets for spells"));
            miscMenu.Add(new MenuKeyBind("autoShield", "Auto-shield allies", System.Windows.Forms.Keys.C, KeyBindType.Toggle));
            miscMenu.Add(new MenuSlider("autoShieldPercent", "Auto Shield %" ,0,20,100 ));
            miscMenu.Add(new MenuBool("debug", "Debug"));
            Menu.Add(miscMenu);
            // Finalize
            Menu.Attach();
        }
    }
}
