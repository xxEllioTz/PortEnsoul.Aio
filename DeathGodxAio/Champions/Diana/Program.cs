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
using static SharpDX.Color;

namespace Diana___Bloody_Lunari
{
    internal class Program

    {
        //   private static Menu HorizonU, ActivatorM;
        private static Menu StartMenu, ComboMenu, DrawingsMenu, AHarrasM, ActivatorMenu, HarrasMenu, LCMenu, AntiSpellMenu, LastHitM, KSMenu;
        public static Spell _Q;
        public static Spell _W;
        public static Spell _E;
        public static Spell _R;
        public static Spell _Ignite;
        private static Spell _RM;
        public static AIHeroClient _Playere
        {
            get { return ObjectManager.Player; }
        }

        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
  
        public static void DianaLoading_OnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Diana"))
            {
                return;
            }

            Game.Print("Diana - Bloody Lunari Loaded!", System.Drawing.Color.Crimson); /*Color.Crimson);*/
            Game.Print("Good luck and have fun, DEATHGODX.", System.Drawing.Color.DarkViolet); /*Color.DarkViolet);*/

            _Q = new Spell(SpellSlot.Q, 900);//, SkillShotType.Circular, 250, 1400, 200);
            _W = new Spell(SpellSlot.W, 200);
            _E = new Spell(SpellSlot.E, 450);
            _R = new Spell(SpellSlot.R, 825);
            _Q.SetSkillshot(0.60f, 95f, float.MaxValue, true, true, SkillshotType.Line);
            _Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            //Diana 
            StartMenu = new Menu("Diana", "Diana");
            ComboMenu = new Menu("Combo Settings", "Combo Settings");
            HarrasMenu = new Menu("Harras Settings", "Harras Settings");
            AHarrasM = new Menu("AutoHarras Settings", "AutoHarras Settings");
            LastHitM = new Menu("Last Hit Settings", "Last Hit Settings");
            LCMenu = new Menu("LaneClear Settings", "LaneClear Settings");
            KSMenu = new Menu("KillSteall Settings", "KillSteall Settings");
            AntiSpellMenu = new Menu("AntiSpell Settings", "AntiSpell Settings");
            ActivatorMenu = new Menu("Activator Settings", "Activator Settings");
            DrawingsMenu = new Menu("Drawings Settings", "Drawings Settings");

            // Horizon Utility
            //      HorizonU = MainMenu.AddMenu("Horizon Utility", "Horizon Utility");
            //     ActivatorM = HorizonU.Add(new Menu("Activator", "Activator");


            var Menudia = new Menu("Diana", "Diana", true);
            StartMenu.Add(new MenuSeparator("Diana - Bloody Lunari", "Diana - Bloody Lunari"));
            StartMenu.Add(new MenuSeparator("Made By", "Made By"));
            StartMenu.Add(new MenuSeparator("- Horizon", "- Horizon"));
            StartMenu.Add(new MenuSeparator("- Radi", "- Radi"));

            // HorizonU.Add(new MenuSeparator("Welcomy to Utlility Beta made by Horizon!");


            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuSeparator("Q Spell Settings", "Q Spell Settings"));
            ComboMenu.Add(new MenuBool("UseQ", "Use [Q]"));
            ComboMenu.Add(new MenuSeparator("W Spell Settings", "W Spell Settings"));
            ComboMenu.Add(new MenuBool("UseW", "Use [W]"));
            ComboMenu.Add(new MenuSeparator("E Spell Settings", ("E Spell Settings")));
            ComboMenu.Add(new MenuBool("UseE", "Use [E] (when enemy out of AA range)"));
            ComboMenu.Add(new MenuSeparator("R Spell Settings", "R Spell Settings"));
            ComboMenu.Add(new MenuBool("UseR", "Use [R] on Combo"));
            ComboMenu.Add(new MenuBool("CR", "Use Second Ult in combo if enemy is killable"));
            ComboMenu.Add(new MenuKeyBind("Misaya", "Use Misaya Combo (R->Q->R)", System.Windows.Forms.Keys.T, KeyBindType.Press)).Permashow();
            ComboMenu.Add(new MenuSeparator("R Spell Settings - tick only one option", "R Spell Settings - tick only one option"));
            ComboMenu.Add(new MenuList("Combos", "Use R", new[] { "Only with Q Mark", "Always" }) {Index = 0 });
            //   ComboMenu.Add(new MenuBool("RONLY", "Use [R] (only when target got Q mark)"));
            // ComboMenu.Add(new MenuBool("RNO", "Use [R] (always) ", false));
            Menudia.Add(ComboMenu);
            HarrasMenu.Add(new MenuSeparator("Harras Settings", "Harras Settings"));
            HarrasMenu.Add(new MenuSeparator("Q Spell Settings", "Q Spell Settings"));
            HarrasMenu.Add(new MenuBool("UseQH", "Use [Q] for Harras"));
            HarrasMenu.Add(new MenuSeparator("W Spell Settings", "W Spell Settings"));
            HarrasMenu.Add(new MenuBool("UseWH", "Use [W] for Harras"));
            HarrasMenu.Add(new MenuSeparator("E Spell Settings", "E Spell Settings"));
            HarrasMenu.Add(new MenuBool("UseEH", "Use [E] for Harras"));
            Menudia.Add(HarrasMenu);
            AHarrasM.Add(new MenuSeparator("Auto Harras Settings", "Auto Harras Settings"));
            AHarrasM.Add(new MenuKeyBind("AHQ", "Use Auto Harras",System.Windows.Forms.Keys.A, KeyBindType.Toggle)).Permashow();
            AHarrasM.Add(new MenuSeparator("Q Spell Settings", "Q Spell Settings"));
            AHarrasM.Add(new MenuBool("QAO", "Use [Q] for Auto Harras"));
            AHarrasM.Add(new MenuSeparator("Mana settings", "Mana settings"));
            AHarrasM.Add(new MenuSlider("AHQM", "Minimum mana percentage for use [Q] in Auto Harras (%{0})", 50, 1));
            Menudia.Add(AHarrasM);
            LastHitM.Add(new MenuSeparator("Last Hit Settings", "Last Hit Settings"));
            LastHitM.Add(new MenuSeparator("Q Spell Settings", "Q Spell Settings"));
            LastHitM.Add(new MenuBool("Qlh", "Use Q to Last Hit"));
            LastHitM.Add(new MenuSeparator("W Spell Settings", "W Spell Settings"));
            LastHitM.Add(new MenuBool("Elh", "Use W to Last Hit"));
            LastHitM.Add(new MenuSeparator("Mana settings", "Mana settings"));
            LastHitM.Add(new MenuSlider("manalh", "Minimum mana percentage for use [Q] [W] in Auto Harras (%{0})", 50, 1));
            Menudia.Add(LastHitM);
            LCMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LCMenu.Add(new MenuBool("LCQ", "Use [Q] for Lane Clear"));
            LCMenu.Add(new MenuSlider("LCQM", "Minimum mana percentage for use [Q] in Lane Clear (%{0})", 50, 1));
            LCMenu.Add(new MenuBool("LCW", "Use [W] for Lane Clear"));
            LCMenu.Add(new MenuSlider("LCWM", "Minimum mana percentage for use [W] in Lane Clear (%{0})", 50, 1));
            LCMenu.Add(new MenuBool("JGCQ", "Use [Q] for Jungle clear"));
            LCMenu.Add(new MenuSlider("JGCQM", "Minimum mana percentage for use [Q] in Jungle Clear (%{0})", 50, 1));
            LCMenu.Add(new MenuBool("JGCW", "Use [W] for Jungle clear"));
            LCMenu.Add(new MenuSlider("JGCWM", "Minimum mana percentage for use [W] in Jungle Clear (%{0})", 50, 1));
            LCMenu.Add(new MenuBool("JGCR", "Use [R] for Jungle clear"));
            LCMenu.Add(new MenuSlider("JGCRM", "Minimum mana percentage for use [R] in Jungle Clear (%{0})", 0, 50, 100));
            Menudia.Add(LCMenu);
            KSMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KSMenu.Add(new MenuSeparator("Q Spell Settings", "Q Spell Settings"));
            KSMenu.Add(new MenuBool("KSQ", " - KillSteal with Q"));
            KSMenu.Add(new MenuSeparator("W Spell Settings", "W Spell Settings"));
            KSMenu.Add(new MenuBool("KSW", " - KillSteal with W"));
            KSMenu.Add(new MenuSeparator("R Spell Settings", "R Spell Settings"));
            KSMenu.Add(new MenuBool("KSR", " - KillSteal with R"));
            Menudia.Add(KSMenu);
            AntiSpellMenu.Add(new MenuSeparator("Anti Spell Settings", "Anti Spell Settings"));
            AntiSpellMenu.Add(new MenuSeparator("Use shield when playing against Morgana or Lux", "Use shield when playing against Morgana or Lux"));
            AntiSpellMenu.Add(new MenuBool("ASLux", "- Anti Lux Passive"));
            AntiSpellMenu.Add(new MenuBool("ASMorgana", "- Anti Morgana"));
            AntiSpellMenu.Add(new MenuSeparator("Mana settings", "Mana settings"));
            AntiSpellMenu.Add(new MenuSlider("ASPM", "Minimum mana percentage for Shield (%{0})", 80, 1));
            Menudia.Add(AntiSpellMenu);
            ActivatorMenu.Add(new MenuSeparator("Activator Settings", "Activator Settings"));
            ActivatorMenu.Add(new MenuSeparator("Use Summoner Spell", "Use Summoner Spell"));
            ActivatorMenu.Add(new MenuBool("IGNI", "- Use Ignite if enemy is killable"));
            Menudia.Add(ActivatorMenu);
            DrawingsMenu.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            DrawingsMenu.Add(new MenuSeparator("Tick for enable/disable spell drawings", "Tick for enable/disable spell drawings"));
            DrawingsMenu.Add(new MenuBool("DQ", "- Draw [Q] range"));
            DrawingsMenu.Add(new MenuBool("DW", "- Draw [W] range"));
            DrawingsMenu.Add(new MenuBool("DE", "- Draw [E] range"));
            DrawingsMenu.Add(new MenuBool("DR", "- Draw [R] range"));
            DrawingsMenu.Add(new MenuBool("DM", "- Draw [R] Mode"));
            Menudia.Add(DrawingsMenu);
            Menudia.Attach();
            Game.OnUpdate += Game_OnUpdate;
            //Game.OnUpdate += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harras();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }
            AHarra();
            KillSteal();
            Activator();
            AntiSpell();
            if (ComboMenu["Misaya"].GetValue<MenuKeyBind>().Active)
            {
                Misaya();
            }
        }
        private static void Game_OnTick(EventArgs args)
        {
        }

        public static void Misaya()
        {
            var UR = ComboMenu["UseR"].GetValue<MenuBool>().Enabled;
            var UQ = ComboMenu["UseQ"].GetValue<MenuBool>().Enabled;
            var targetR = TargetSelector.GetTarget(_R.Range, DamageType.Physical);
            var targetQ = TargetSelector.GetTarget(_Q.Range, DamageType.Physical);
            if (targetR == null)
            {
                return;
            }
            if (targetQ == null)
            {
                return;
            }
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var rDelay = _R.Delay = 500;
            if (UR && UQ)
            {
                if (targetQ.IsValidTarget(_Q.Range) && targetQ.IsValidTarget() && _Q.IsReady())
                {
                    _Q.Cast(targetQ.Position);
                }
                if (targetR.IsValidTarget() && _R.IsReady() && _Q.Instance.State == SpellState.Cooldown)
                {
                    _R.Cast(targetR);
                }
                if (targetR.IsValidTarget() && _R.IsReady())
                {
                    _R.Cast(targetR);
                }

            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var RQ = ComboMenu["Combos"].GetValue<MenuList>().Index == 0;
            var RA = ComboMenu["Combos"].GetValue<MenuList>().Index == 1;
            var DM = DrawingsMenu["DM"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Physical);
            if (DrawingsMenu["DQ"].GetValue<MenuBool>().Enabled && _Q.IsReady())

            {
                if (!_Q.IsReady())

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _Q.Range, Color.DarkRed, 1);

                else if (_Q.IsReady())
                {

                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _Q.Range, Color.DarkRed, 1);
                }


            }
            if (DrawingsMenu["DW"].GetValue<MenuBool>().Enabled && _W.IsReady())
            {
                if (!_W.IsReady())
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _W.Range, Color.DarkRed, 1);

                else if (_W.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _W.Range, Color.DarkRed, 1);
                }
            }
            if (DrawingsMenu["DE"].GetValue<MenuBool>().Enabled && _E.IsReady())
            {
                if (!_E.IsReady())
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _E.Range, Color.DarkRed, 1);

                else if (_E.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _E.Range, Color.DarkRed, 1);
                }
            }
            if (DrawingsMenu["DR"].GetValue<MenuBool>().Enabled && _R.IsReady())
            {
                if (!_R.IsReady())
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _R.Range, Color.DarkRed, 1);

                else if (_R.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _R.Range, Color.DarkRed, 1);
                }
            }

            if (RQ && DM)
            {
                Drawing.DrawText(Drawing.WorldToScreen(_Player.Position).X - 50,
                Drawing.WorldToScreen(_Player.Position).Y + 10,
                System.Drawing.Color.White,
                "     R Mode: Mark ");

            }
            if (RA && DM)
            {
                Drawing.DrawText(Drawing.WorldToScreen(_Player.Position).X - 50,
                 Drawing.WorldToScreen(_Player.Position).Y + 10,
                 System.Drawing.Color.White,
                 "     R Mode: Always ");
            }
        }
        public static void AHarra()
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            {
                if (_Player.ManaPercent > AHarrasM["AHQM"].GetValue<MenuSlider>().Value &&
                    AHarrasM["AHQ"].GetValue<MenuKeyBind>().Active && AHarrasM["QAO"].GetValue<MenuBool>().Enabled)
                {
                    var Qpred = _Q.GetPrediction(target);
                    if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                    {
                        if (target.IsValidTarget(_Q.Range) && _Q.IsReady())
                        {
                            _Q.Cast(target);
                        }
                    }
                }
            }
        }

        public static void AntiSpell()
        {
            var ManaAutoS = AntiSpellMenu["ASPM"].GetValue<MenuSlider>().Value;

            if (ManaAutoS < _Player.ManaPercent && _W.IsReady())
            {
                if (AntiSpellMenu["ASLux"].GetValue<MenuBool>().Enabled)
                {
                    if (_Player.HasBuff("LuxLightBindingMis"))


                    {
                        _W.Cast();
                    }
                }
            }
            else if (ManaAutoS < _Player.ManaPercent && _W.IsReady())
            {
                if (AntiSpellMenu["ASMorgana"].GetValue<MenuBool>().Enabled)
                {
                    if (_Player.HasBuff("Dark Binding"))
                    {
                        _W.Cast();
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(_Q.Range) && e.IsMinion())
                            .Cast<AIBaseClient>().ToList();
            var qFarmLocation = _Q.GetLineFarmLocation(minions, _Q.Width);
            if (LCMenu["LCQ"].GetValue<MenuBool>().Enabled)
            {
                if (!_Q.IsReady())
                {
                    _Q.Cast(qFarmLocation.Position);
                }
                
            }

            if (_Player.ManaPercent > LCMenu["LCWM"].GetValue<MenuSlider>().Value && LCMenu["LCW"].GetValue<MenuBool>().Enabled)
            {
                if (!_W.IsReady())
                {
                    return;
                }
                var minionsList = GameObjects.EnemyMinions.Where(x => x.IsValidTarget()).Where(minion => minion.IsValidTarget(_W.Range)).ToList();

                if (qFarmLocation.MinionsHit>= 3)
                {
                    _W.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var jungleMonsters = GameObjects.Jungle.Where(j => j.IsValidTarget(_Q.Range)).FirstOrDefault(j => j.IsValidTarget(_Q.Range));

                if (_Player.ManaPercent > LCMenu["JGCQM"].GetValue<MenuSlider>().Value && LCMenu["JGCQ"].GetValue<MenuBool>().Enabled && jungleMonsters.IsValidTarget(_Q.Range) && _Q.IsReady())
                {
                    _Q.Cast(jungleMonsters.Position);
                }
            if (_Player.ManaPercent > LCMenu["JGCRM"].GetValue<MenuSlider>().Value && LCMenu["JGCR"].GetValue<MenuBool>().Enabled && jungleMonsters.IsValidTarget(_R.Range))
            {
                if (jungleMonsters.HasBuff("DianaMoonlight") && jungleMonsters.IsValidTarget(_R.Range) && _R.IsReady())
                {
                    _R.Cast(jungleMonsters);
                }
            }
            if (_Player.ManaPercent > LCMenu["JGCWM"].GetValue<MenuSlider>().Value && LCMenu["JGCW"].GetValue<MenuBool>().Enabled && jungleMonsters.IsValidTarget(_W.Range) && _W.IsReady())
            {
                _W.Cast();
            }
        }
        private static void Combo()
        {
            var RQ = ComboMenu["Combos"].GetValue<MenuList>().Index == 0;
            var RA = ComboMenu["Combos"].GetValue<MenuList>().Index == 1;
            var CR = ComboMenu["CR"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            var targetR = TargetSelector.GetTarget(_R.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (targetR == null)
            {
                return;
            }
            if (ComboMenu["UseQ"].GetValue<MenuBool>().Enabled && _Q.IsReady())
            {
                var Qpred = _Q.GetPrediction(target);
                //var wheretocastt = _Player.Position.Extend(ObjectManager.Player, Qpred.CastPosition.Distance(ObjectManager.Player) + 125).To3DWorld();
                if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                {
                    if (!target.IsValidTarget(_Q.Range) && _Q.IsReady())
                    {
                        return;
                    }
                }
                {
                    _Q.Cast(Qpred.CastPosition);
                }
            }

            if (ComboMenu["UseR"].GetValue<MenuBool>().Enabled && RQ && _R.IsReady())
            {
                if (target.HasBuff("DianaMoonlight") && target.IsValidTarget(_R.Range) & _R.IsReady())
                {
                    _R.Cast(target);
                }
            }

            if (RA && ComboMenu["UseR"].GetValue<MenuBool>().Enabled && _R.IsReady())
            {
                if (target.IsValidTarget(_R.Range) && target.IsValidTarget() && _R.IsReady())
                {
                    _R.Cast(target);
                }
            }
            if (ComboMenu["UseE"].GetValue<MenuBool>().Enabled && _E.IsReady())
            {
                if (_E.IsInRange(target) && !ObjectManager.Player.InAutoAttackRange())
                {
                    _E.Cast();
                }
            }

            if (ComboMenu["UseW"].GetValue<MenuBool>().Enabled)
            {
                if (!target.IsValidTarget(_W.Range) && _W.IsReady())
                {
                    _W.Cast();
                }
                {
                    return;
                }
            }

            if (ComboMenu["UseR"].GetValue<MenuBool>().Enabled && CR)
            {
                if (target.IsValidTarget(_R.Range) && _R.IsReady() && targetR.Health + targetR.PhysicalShield < _Player.GetSpellDamage(targetR, SpellSlot.R))
                {
                    _R.Cast(targetR);
                }
            }
        }


        private static void LastHit()
        {
            var jungleMonsters = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= _Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (jungleMonsters != null)
            {
                if (LastHitM["Qlh"].GetValue<MenuBool>().Enabled && _Q.IsReady() && ObjectManager.Player.ManaPercent > LastHitM["manalh"].GetValue<MenuSlider>().Value && jungleMonsters.IsValidTarget(_Q.Range) &&
                    ObjectManager.Player.GetSpellDamage(jungleMonsters, SpellSlot.Q) >= jungleMonsters.Health + jungleMonsters.AllShield)

                {
                    _Q.Cast(jungleMonsters);
                }

                if (LastHitM["Elh"].GetValue<MenuBool>().Enabled && _W.IsReady() && ObjectManager.Player.GetSpellDamage(jungleMonsters, SpellSlot.W) >= jungleMonsters.Health + jungleMonsters.AllShield &&
                    ObjectManager.Player.ManaPercent > LastHitM["manalh"].GetValue<MenuSlider>().Value)
                {
                    _W.Cast();
                }
            }
        }

        private static void Harras()
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (HarrasMenu["UseQH"].GetValue<MenuBool>().Enabled)
            {
                var Qpred = _Q.GetPrediction(target);
                if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                {
                    if (!target.IsValidTarget(_Q.Range) && _Q.IsReady())
                    {
                        return;
                    }
                }
                {
                    _Q.Cast(target);
                }
            }
            if (HarrasMenu["UseEH"].GetValue<MenuBool>().Enabled)
            {
                if (!target.IsValidTarget(_E.Range) && _E.IsReady())
                {
                    return;
                }
                {
                    _E.Cast();
                }
            }
            if (HarrasMenu["UseWH"].GetValue<MenuBool>().Enabled)
            {
                if (!target.IsValidTarget(_W.Range) && _W.IsReady())
                {
                    return;
                }
                {
                    _W.Cast();
                }
            }
        }



        public static void KillSteal()
        {
            var targetQ = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            var targetW = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
            var targetR = TargetSelector.GetTarget(_R.Range, DamageType.Magical);
            if (targetQ == null)
            {
                return;
            }
            if (targetW == null)
            {
                return;
            }
            if (targetR == null)
            {
                return;
            }
            if (KSMenu["KSQ"].GetValue<MenuBool>().Enabled)
            {
                var Qpred = _Q.GetPrediction(targetQ);
                if (Qpred.Hitchance >= HitChance.High && targetQ.IsValidTarget(_Q.Range))
                {
                    if (targetQ.Health + targetQ.PhysicalShield < _Player.GetSpellDamage(targetQ, SpellSlot.Q))
                    {
                        if (!targetQ.IsValidTarget(_Q.Range) && _Q.IsReady())
                        {
                            _Q.Cast(targetQ);
                        }
                    }
                }
                return;
            }

            if (KSMenu["KSW"].GetValue<MenuBool>().Enabled)
            {
                if (targetW.Health + targetW.PhysicalShield < _Player.GetSpellDamage(targetW, SpellSlot.W))
                {
                    if (!targetW.IsValidTarget(_W.Range) && _W.IsReady())
                    {
                        return;
                    }
                }
                {
                    _W.Cast();
                }
            }

            if (KSMenu["KSR"].GetValue<MenuBool>().Enabled)
            {
                if (targetR.Health + targetR.PhysicalShield < _Player.GetSpellDamage(targetR, SpellSlot.R))
                {
                    if (!targetR.IsValidTarget(_R.Range) && _R.IsReady())
                    {
                        return;
                    }
                }
                {
                    _R.Cast(targetR);
                }
            }




        }
        // end region of Diana

        //Utility region
        public static void Activator()
        {
            var target = TargetSelector.GetTarget(_Ignite.Range, DamageType.True);
            if (target == null)
            {
                return;
            }
            if (ActivatorMenu["IGNI"].GetValue<MenuBool>().Enabled && _Ignite.IsReady() && target.IsValidTarget())

            {
                if (target.Health + target.PhysicalShield <
                    _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                {
                    _Ignite.Cast(target);
                }
            }
        }
    }
}