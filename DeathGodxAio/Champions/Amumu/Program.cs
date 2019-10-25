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


namespace StonedAmumu
{
    internal class Program
    {



        private const string Champion = "Amumu";

        //private static Orbwalking.Orbwalker Orbwalker;

        private static List<Spell> SpellList = new List<Spell>();

        private static Spell Q;

        private static Spell W;

        private static Spell E;

        private static Spell R;

        private static Menu Config, JungleMenu, ComboMenu, LaneClearMenu, DrawMenu;

        private static Items.Item RDO;

        private static Items.Item DFG;

        private static Items.Item YOY;

        private static Items.Item BOTK;

        private static Items.Item HYD;

        private static Items.Item CUT;

        private static Items.Item TYM;

        private static AIHeroClient Player;

      
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

       public static void AmumuGame_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (!_Player.CharacterName.Contains("Amumu")) return;

            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 525);

            Q.SetSkillshot(0.250f, 80, 2000, true, true, SkillshotType.Line);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            RDO = new Items.Item(3143, 490f);
            HYD = new Items.Item(3074, 175f);
            DFG = new Items.Item(3128, 750f);
            YOY = new Items.Item(3142, 185f);
            BOTK = new Items.Item(3153, 450f);
            CUT = new Items.Item(3144, 450f);
            TYM = new Items.Item(3077, 175f);

            //Menu Amumu
            Config = new Menu(Champion, "StonedAmumu", true);

            //Combo Menu
            ComboMenu = new Menu("Combo", "Combo");
            ComboMenu.Add(new MenuBool("UseQCombo", "Use Q"));
            ComboMenu.Add(new MenuBool("UseWCombo", "Use W"));
            ComboMenu.Add(new MenuBool("UseECombo", "Use E"));
            ComboMenu.Add(new MenuBool("UseItems", "Use Items"));
            ComboMenu.Add(new MenuBool("AutoR", "Auto R"));
            ComboMenu.Add(new MenuSlider("CountR", "Num of Enemy in Range to Ult", 1, 1, 5));
            ComboMenu.Add(new MenuKeyBind("ActiveCombo", "Combo!", System.Windows.Forms.Keys.Space, KeyBindType.Press));
            Config.Add(ComboMenu);
            //JungleClear
            JungleMenu = new Menu("Jungle Clear", "Jungle");
            JungleMenu.Add(new MenuBool("UseQClear", "Use Q"));
            JungleMenu.Add(new MenuBool("UseWClear", "Use W"));
            JungleMenu.Add(new MenuBool("UseEClear", "Use E"));
            JungleMenu.Add(new MenuKeyBind("ActiveClear", "Jungle Key!", System.Windows.Forms.Keys.V, KeyBindType.Press));
            Config.Add(JungleMenu);
            LaneClearMenu = new Menu("Wave Clear", "Wave");
            LaneClearMenu.Add(new MenuBool("UseQWave", "Use Q"));
            LaneClearMenu.Add(new MenuBool("UseWWave", "Use W"));
            LaneClearMenu.Add(new MenuBool("UseEWave", "Use E"));
            LaneClearMenu.Add(new MenuKeyBind("ActiveWave", "WaveClear Key!", System.Windows.Forms.Keys.V, KeyBindType.Press));
            Config.Add(LaneClearMenu);
            //Drawings
            DrawMenu = new Menu("Drawings", "Drawings");
            DrawMenu.Add(new MenuBool("DrawQ", "Draw Q"));
            DrawMenu.Add(new MenuBool("DrawW", "Draw W"));
            DrawMenu.Add(new MenuBool("DrawE", "Draw E"));
            DrawMenu.Add(new MenuBool("DrawR", "Draw R"));
            DrawMenu.Add(new MenuBool("CircleLag", "Lag Free Circles"));
            DrawMenu.Add(new MenuSlider("CircleQuality", "Circles Quality", 100, 100, 10));
            DrawMenu.Add(new MenuSlider("CircleThickness", "Circles Thickness", 1, 10, 1));
            Config.Add(DrawMenu);
            Config.Attach();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;



            Game.Print("<font color='#FF00BF'>Stoned Amumu Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>DEATHGODX</font><font color='#40FF00'>Style</font>");
        }

        private static void OnGameUpdate(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                //WaveClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }

        }

        private static void WaveClear()
        {
        
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);

            var minionQ = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range) && e.IsMinion())
                          .Cast<AIBaseClient>().ToList();
            var useQ = LaneClearMenu["UseQClear"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["UseWClear"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["UseEClear"].GetValue<MenuBool>().Enabled;


            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= Q.Range)
                {
                    Q.Cast(minion.Position);
                }

                if (useW && W.IsReady() && minion.IsValidTarget())
                {
                    if (Player.Distance(minion) <= W.Range && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1))
                    {
                        W.Cast();
                    }
                    else if (Player.Distance(minion) > W.Range && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2))
                    {
                        W.Cast();
                    }

                }

                if (useE && E.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= E.Range)
                {
                    E.Cast();
                }
            }
        }

        private static void JungleClear() //Credits To Flapperdoodle! 
        {
            var useQ = JungleMenu["UseQClear"].GetValue<MenuBool>().Enabled;
            var useW = JungleMenu["UseWClear"].GetValue<MenuBool>().Enabled;
            var useE = JungleMenu["UseEClear"].GetValue<MenuBool>().Enabled;

            var allminions = GameObjects.Jungle.Where(j => j.IsValidTarget(Q.Range)).OrderByDescending(a => a.MaxHealth).FirstOrDefault();


            if (useQ && Q.IsReady() && allminions.IsValidTarget() && Player.Distance(allminions) <= Q.Range)
            {
                Q.Cast(allminions.Position);
            }

            if (useW && W.IsReady() && allminions.IsValidTarget())
            {
                if (Player.Distance(allminions) <= W.Range && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1))
                {
                    W.Cast();
                }
                else if (Player.Distance(allminions) > W.Range && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2))
                {
                    W.Cast();
                }

            }

            if (useE && E.IsReady() && allminions.IsValidTarget() && Player.Distance(allminions) <= E.Range)
            {
                E.Cast();
            }

        }



        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null) return;

            //Combo
            if (Player.Distance(target) <= Q.Range && Q.IsReady() && (ComboMenu["UseQCombo"].GetValue<MenuBool>().Enabled))
            {

                Q.Cast(target);

            }
            if (W.IsReady() && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1) && (ComboMenu["UseWCombo"].GetValue<MenuBool>().Enabled))
                if (Player.Position.Distance(target.Position) < W.Range)
                {

                    W.Cast();

                }
            if (W.IsReady() && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2) && (ComboMenu["UseWCombo"].GetValue<MenuBool>().Enabled))
                if (Player.Position.Distance(target.Position) > W.Range)
                {


                    W.Cast();

                }
            if (Player.Distance(target) <= E.Range && E.IsReady() && (ComboMenu["UseECombo"].GetValue<MenuBool>().Enabled))
            {

                E.Cast();

            }
            if (ComboMenu["UseItems"].GetValue<MenuBool>().Enabled)
            {
                if (Player.Distance(target) <= RDO.Range)
                {
                    RDO.Cast(target);
                }
                if (Player.Distance(target) <= HYD.Range)
                {
                    HYD.Cast(target);
                }
                if (Player.Distance(target) <= DFG.Range)
                {
                    DFG.Cast(target);
                }
                if (Player.Distance(target) <= BOTK.Range)
                {
                    BOTK.Cast(target);
                }
                if (Player.Distance(target) <= CUT.Range)
                {
                    CUT.Cast(target);
                }
                if (Player.Distance(target) <= 125f)
                {
                    YOY.Cast();
                }
                if (Player.Distance(target) <= TYM.Range)
                {
                    TYM.Cast(target);
                }
            }
            if (ComboMenu["AutoR"].GetValue<MenuBool>().Enabled)
            {

                if (GetNumberHitByR(target) >= ComboMenu["CountR"].GetValue<MenuSlider>().Value)
                {
                    R.Cast(target);
                }
            }


        }

        private static int GetNumberHitByR(AIBaseClient target) // Credits to Trelli For helping me with this one!
        {
            int totalHit = 0;
            foreach (AIHeroClient current in ObjectManager.Get<AIHeroClient>())
            {
                if (current.IsEnemy && Vector3.Distance(Player.Position, current.Position) <= R.Range)
                {
                    totalHit = totalHit + 1;
                }
            }
            return totalHit;
        }




        private static void OnDraw(EventArgs args)
        {
            if (DrawMenu["CircleLag"].GetValue<MenuBool>().Enabled) // Credits to SKOBOL
            {
                if (DrawMenu["DrawQ"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White,
                        DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (DrawMenu["DrawW"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White,
                        DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (DrawMenu["DrawE"].GetValue<MenuBool>().Enabled)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White,
                        DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (DrawMenu["DrawR"].GetValue<MenuBool>().Enabled)
                {
                    //Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White, DrawMenu["CircleThickness"].GetValue<MenuSlider>().Value);
                }
            }
            else
            {
                if (DrawMenu["DrawQ"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White);
                }
                if (DrawMenu["DrawW"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White);
                }
                if (DrawMenu["DrawE"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White);
                }
                if (DrawMenu["DrawR"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White);
                }

            }
        }

    }
}
