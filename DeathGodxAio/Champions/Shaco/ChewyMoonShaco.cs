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

namespace ChewyMoonsShaco
{
    internal class ChewyMoonShaco
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Menu;
        public static Menu illuminatiMenu, comboMenu, drawingMenu, escapeMenu, ActivatorMenu, harassMenu, miscMenu, AntiSpellMenu, LastHitM, ksMenu;
        // public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList;
        public static Items.Item Tiamat;
        public static Items.Item Hydra;
        public static int cloneAct = 0;
        public static AIHeroClient player = ObjectManager.Player;

        public static void ShacoOnGameLoad()
        {
            if (player.CharacterName != "Shaco")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 425);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 200);
            SpellList = new List<Spell> { Q, E, W, R };
            CreateMenu();
            //Illuminati.Init();
            //Tiamat = ItemData.Tiamat_Melee_Only.GetItem();
            //Hydra = ItemData.Ravenous_Hydra_Melee_Only.GetItem();
            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            //Orbwalker.OnAction += OrbwalkingOnAfterAttack;
            Game.Print("<font color=\"#6699ff\"><b>ChewyMoon's Shaco: PORTED By DEATHGODX</b></font> <font color=\"#FFFFFF\">" + "loaded!" +"</font>");
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }


        static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!escapeMenu["Evade"].GetValue<MenuBool>().Enabled) return;
            if (sender.IsAlly) return;
            //Need to calc Delay/Time for misille to hit !

            if (DangerDB.TargetedList.Contains(args.SData.Name))
            {
                if (args.Target.IsMe)
                    R.Cast();
            }

            if (DangerDB.CircleSkills.Contains(args.SData.Name))
            {
                if (player.Distance(args.End) < args.SData.LineWidth)
                    R.Cast();
            }

            if (DangerDB.Skillshots.Contains(args.SData.Name))
            {
                if (new Geometry.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(player))
                {
                    R.Cast();
                }
            }
        }


        private static void OrbwalkingOnAfterAttack(Object unit, OrbwalkerActionArgs targeti)
        {
            var target = TargetSelector.GetTarget(400);
            if (!(target is AIHeroClient))
            {
                return;
            }

            if (!target.IsValidTarget())
            {
                return;
            }

            if (Hydra.IsReady)
            {
                Hydra.Cast();
            }
            else if (Tiamat.IsReady)
            {
                Tiamat.Cast();
            }
        }

        private static void CreateMenu()
        {
            Menu = new Menu("[Chewy's Shaco]", "Chewy Moons Shaco", true);

            // Target Selector


            // Orbwalking


            // Combo
            comboMenu = new Menu("Combo", "Combo");
            comboMenu.Add(new MenuBool("useQ", "Use Q"));
            comboMenu.Add(new MenuBool("useW", "Use W"));
            comboMenu.Add(new MenuBool("useE", "Use E"));
            comboMenu.Add(new MenuBool("useR", "Use R"));
            comboMenu.Add(new MenuBool("cloneOrb", "Clone Orbwalking"));
            comboMenu.Add(new MenuBool("useItems", "Use items"));
            Menu.Add(comboMenu);

            // Harass
            var harassMenu = new Menu("Harass", "Harass");
            harassMenu.Add(new MenuBool("useEHarass", "Use E"));
            Menu.Add(harassMenu);

            // Ks
            ksMenu = new Menu("KS", "KillSteal");
            ksMenu.Add(new MenuBool("ksE", "Use E"));
            Menu.Add(ksMenu);

            //Escape
            escapeMenu = new Menu("Escape", "Flee Espace");
            escapeMenu.Add(new MenuKeyBind("Escape", "Escape", System.Windows.Forms.Keys.Z, KeyBindType.Press)).Permashow();
            escapeMenu.Add(new MenuKeyBind("EscapeR", "Escape With Ultimate",System.Windows.Forms.Keys.X, KeyBindType.Press)).Permashow();
            escapeMenu.Add(new MenuBool("Evade", "Evade With Ultimate"));
            Menu.Add(escapeMenu);
            // ILLUMINATI
            illuminatiMenu = new Menu("Illuminati", "Box Settings Illimunati");
            illuminatiMenu.Add(new MenuKeyBind("PlaceBox", "Place Box", System.Windows.Forms.Keys.C, KeyBindType.Press)).Permashow();
            illuminatiMenu.Add(new MenuBool("RepairTriangle", "Repair Triangle & Auto Form Triangle"));
            illuminatiMenu.Add(new MenuSlider("BoxDistance", "Box Distance", 600, 101, 1200));
            /*illuminatiMenu["BoxDistance"].GetValue<MenuSlider>().Value += delegate (object sender, MenuValueChangedEventArgs args)
                {
                    Illuminati.TriangleLegDistance = args.GetNewValue<MenuSlider>().Value;
                };*/
            Menu.Add(illuminatiMenu);
            // Drawing
            drawingMenu = new Menu("Drawings", "Drawing");
            drawingMenu.Add(new MenuBool("drawQ", "Draw Q"));
            drawingMenu.Add(new MenuBool("drawQPos", "Draw Q Pos"));
            drawingMenu.Add(new MenuBool("drawW", "Draw W"));
            drawingMenu.Add(new MenuBool("drawE", "Draw E"));
            Menu.Add(drawingMenu);

            // Misc
            miscMenu = new Menu("Misc", "Misc");
            miscMenu.Add(new MenuBool("usePackets", "Use packets"));
            miscMenu.Add(new MenuBool("stuff", "Let me know of any"));
            miscMenu.Add(new MenuBool("stuff2", "other misc features you want"));
            miscMenu.Add(new MenuBool("stuff3", "on the thread or IRC"));
            miscMenu.Add(new MenuBool("stuff4", "Modded by XcxooxL"));
            Menu.Add(miscMenu);
            Menu.Attach();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qCircle = drawingMenu["drawQ"].GetValue<MenuBool>().Enabled;
            var wCircle = drawingMenu["drawW"].GetValue<MenuBool>().Enabled;
            var eCircle = drawingMenu["drawE"].GetValue<MenuBool>().Enabled;
            var qPosCircle = drawingMenu["drawQPos"].GetValue<MenuBool>().Enabled;

            var pos = player.Position;

            if (qCircle)
            {
                Render.Circle.DrawCircle(pos, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (wCircle)
            {
                Render.Circle.DrawCircle(pos, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (eCircle)
            {
                Render.Circle.DrawCircle(pos, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (!qPosCircle)
            {
                return;
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget()))
            {
                Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(ShacoUtil.GetQPos(enemy, false)), 2, Color.Aquamarine);
            }
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            if (escapeMenu["EscapeR"].GetValue<MenuKeyBind>().Active)
            {
                if (R.IsReady() && Q.IsReady())
                {
                    R.Cast();
                }
                Escape();
            }

            if (escapeMenu["Escape"].GetValue<MenuKeyBind>().Active)
            {
                Escape();
            }


            if (ksMenu["ksE"].GetValue<MenuBool>().Enabled)
            {
                KillSecure();
            }

            if (illuminatiMenu["PlaceBox"].GetValue<MenuKeyBind>().Active)
            {
                Illuminati.PlaceInitialBox();
            }

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;

                case OrbwalkerMode.Harass:
                    Harass();
                    break;
            }
        }

        public static void Escape()
        {
            Q.Cast(Game.CursorPos);
            player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);

            var clone = getClone();

            if (clone != null)
            {

                var pos =Game.CursorPos.Extend(clone.Position, clone.Distance(Game.CursorPos) + 2000);
                R.Cast(pos);

            }


        }

        public static AIBaseClient getClone()
        {
            AIBaseClient Clone = null;
            foreach (var unit in ObjectManager.Get<AIBaseClient>().Where(clone => !clone.IsMe && clone.Name == player.Name))
            {
                Clone = unit;
            }

            return Clone;

        }

        private static void KillSecure()
        {
            if (!E.IsReady())
            {
                return;
            }

            foreach (var target in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsEnemy)
                    .Where(x => !x.IsDead)
                    .Where(x => x.Distance(player) <= E.Range)
                    .Where(target => player.GetSpellDamage(target, SpellSlot.E) > target.Health))
            {
                E.CastOnUnit(target, miscMenu["usePackets"].GetValue<MenuBool>().Enabled);
                return;
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var useQ = comboMenu["useQ"].GetValue<MenuBool>().Enabled;
            var useW = comboMenu["useW"].GetValue<MenuBool>().Enabled;
            var useE = comboMenu["useE"].GetValue<MenuBool>().Enabled;
            var packets = miscMenu["usePackets"].GetValue<MenuBool>().Enabled;

            foreach (var spell in SpellList.Where(x => x.IsReady()))
            {
                if (spell.Slot == SpellSlot.Q && useQ)
                {
                    if (!target.IsValidTarget(Q.Range))
                    {
                        continue;
                    }

                    var pos = ShacoUtil.GetQPos(target, true);
                    Q.Cast(pos, packets);
                }


                if (target != null)
                    if (spell.Slot == SpellSlot.R && target.IsValidTarget() && player.Distance(target) < 400 &&
                        player.HasBuff("Deceive") && comboMenu["useR"].GetValue<MenuBool>().Enabled)
                    {
                        R.Cast();
                    }

                if (spell.Slot == SpellSlot.W && useW)
                {
                    //TODO: Make W based on waypoints
                    if (!target.IsValidTarget(W.Range))
                    {
                        continue;
                    }

                    var pos = ShacoUtil.GetQPos(target, true, 100);
                    W.Cast(pos, packets);
                }

                if (spell.Slot != SpellSlot.E || !useE)
                {
                    continue;
                }
                if (!target.IsValidTarget(E.Range))
                {
                    continue;
                }

                E.CastOnUnit(target);
            }



        }

        private static void Harass()
        {
            var useE = harassMenu["useEHarass"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (!target.IsValidTarget(E.Range))
            {
                return;
            }

            if (useE && E.IsReady())
            {
                E.CastOnUnit(target);
            }
        }

       /* public static bool hasClone()
        {
            return player.GetSpell(SpellSlot.R).Name.Equals("hallucinateguide");
        }*/
    }
}
