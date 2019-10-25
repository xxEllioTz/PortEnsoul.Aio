
using System;
using System.Linq;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using Color = System.Drawing.Color;
using static EnsoulSharp.SDK.Items;
using SharpDX.Direct3D9;
using static EnsoulSharp.SDK.Interrupter;
using EnsoulSharp.SDK.Prediction;

namespace The_Horizon_Leona
{
    class Program
    {
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static Menu StartMenu, ComboMenu, DrawingsMenu, ActivatorMenu;

        public static Spell _Q;
        public static Spell _W;
        public static Spell _E;
        public static Spell _R;
        public static Spell _Ignite;

     

        public static void LeonaLoading_OnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Leona"))
            {
                return;
            }
            Game.Print("The Horizon Leona - Loaded, PORTED by DEATHGODX");
            Game.Print("For best experience left-click target.");

            _Q = new Spell(SpellSlot.Q, GameObjects.Player.GetRealAutoAttackRange());
            _W = new Spell(SpellSlot.W, 405);
            _E = new Spell(SpellSlot.E, 800);//, SkillShotType.Linear, 250, 2000, 70);
            _R = new Spell(SpellSlot.R, 1200);//, SkillShotType.Circular, castDelay: 250, spellWidth: 200);
            _Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);

            StartMenu = new Menu("The Horizon Leona", "The Horizon Leona");
            ComboMenu = new Menu("Combo", "Combo");
            DrawingsMenu = new Menu("Drawings", "Drawings");
            ActivatorMenu = new Menu("Activator", "Activator");
            var Menuleo = new Menu("The Horizon Leona", "The Horizon Leona",true);
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuSeparator("Tick for enable/disable spells in Combo", "Tick for enable/disable spells in Combo"));
            ComboMenu.Add(new MenuBool("UseQ", "Use [Q]"));
            ComboMenu.Add(new MenuBool("UseW", "Use [W]"));
            ComboMenu.Add(new MenuBool("UseE", "Use [E]"));
            ComboMenu.Add(new MenuBool("UseR", "Use [R]"));
            Menuleo.Add(ComboMenu);
            DrawingsMenu.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            DrawingsMenu.Add(new MenuSeparator("Tick for enable/disable Draw Spell Range", "Tick for enable/disable Draw Spell Range"));
            DrawingsMenu.Add(new MenuBool("DQ", "- Draw [Q] range"));
            DrawingsMenu.Add(new MenuBool("DW", "- Draw [W] range"));
            DrawingsMenu.Add(new MenuBool("DE", "- Draw [E] range"));
            DrawingsMenu.Add(new MenuBool("DR", "- Draw [R] range"));
            Menuleo.Add(DrawingsMenu);
            ActivatorMenu.Add(new MenuSeparator("Activator Settings", "Activator Settings"));
            ActivatorMenu.Add(new MenuSeparator("Use Summoner Spell", "Use Summoner Spell"));
            ActivatorMenu.Add(new MenuBool("IGNI", "- Use Ignite if enemy is killable"));
            Menuleo.Add(ActivatorMenu);
            Menuleo.Attach();


            Game.OnUpdate += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private static void Game_OnTick(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
            Activator();
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Physical);
            if (DrawingsMenu["DQ"].GetValue<MenuBool>().Enabled && _Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _Q.Range, Color.Red, 1);
            }
            if (DrawingsMenu["DW"].GetValue<MenuBool>().Enabled && _W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _W.Range, Color.Red, 1);
            }
            if (DrawingsMenu["DE"].GetValue<MenuBool>().Enabled && _E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _E.Range, Color.Red, 1);
            }
            if (DrawingsMenu["DR"].GetValue<MenuBool>().Enabled && _R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _R.Range, Color.Red, 1);
            }

        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(_E.Range, DamageType.Magical);

            if (target == null)
                return;
            if (ComboMenu["UseQ"].GetValue<MenuBool>().Enabled)
            {
                if (target.IsValidTarget(_Q.Range) && _Q.IsReady() && _Player.Distance(target) > 125)

                {
                    _Q.Cast();
                }



            }
            if (ComboMenu["UseW"].GetValue<MenuBool>().Enabled)
            {
                if (target.IsValidTarget(_W.Range) && _W.IsReady())
                {

                    _W.Cast();



                }
            }
            if (ComboMenu["UseE"].GetValue<MenuBool>().Enabled)
            {
                if (target.IsValidTarget(_E.Range) && _E.IsReady())
                    _E.Cast(target.Position);
            }
            if (ComboMenu["UseR"].GetValue<MenuBool>().Enabled)
            {
                if (target.IsValidTarget(_R.Range))
                    {

                        _R.Cast(target);



                    }
            }





        }

        public static void Activator()
        {
            var target = TargetSelector.GetTarget(_Ignite.Range, DamageType.True);
            if (_Ignite != null && ActivatorMenu["IGNI"].GetValue<MenuBool>().Enabled && _Ignite.IsReady())
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
