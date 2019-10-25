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
using SPrediction;

namespace The_Horizon_Sona
{
    class Program
    {
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static Menu StartMenu, ComboMenu, DrawingsMenu, ActivatorMenu, AHarrasMenu;

        public static Spell _Q;
        public static Spell _W;
        public static Spell _E;
        public static Spell _R;
        public static Spell _FlashR;
        public static Spell _Ignite;
        public static Spell _Flash = new Spell(ReturnSlot("summonerflash"), 425);
        public static SpellSlot ReturnSlot(string Name)
        {
            return ObjectManager.Player.GetSpellSlot(Name);
        }

        public static void SonaLoading_OnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Sona"))
            {
                return;
            }
            Game.Print("The Horizon Sona - Loaded ! PORTED By DEATHGODx");
            _Q = new Spell(SpellSlot.Q, 825);
            _W = new Spell(SpellSlot.W, 1000);
            _E = new Spell(SpellSlot.E, 425);
            _R = new Spell(SpellSlot.R, 900);//, SkillShotType.Linear, 250, 2400, 140);
            _Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            _FlashR = new Spell(SpellSlot.R, 1050);//, SkillShotType.Linear, 250, 2400, 140);
            StartMenu = new Menu("The Horizon Sona", "The Horizon Sona", true);
            ComboMenu = new Menu("Combo", "Combo");
            AHarrasMenu = new Menu("Auto Harras", "Auto Harras");
            DrawingsMenu = new Menu("Drawings", "Drawings");
            ActivatorMenu = new Menu("Activator", "Activator");

            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuSeparator("Q Settings", "Q Settings"));
            ComboMenu.Add(new MenuBool("UseQ", "Use [Q]"));
            ComboMenu.Add(new MenuSeparator("W Settings", "W Settings"));
            ComboMenu.Add(new MenuBool("UseW", "Use [W]"));
            ComboMenu.Add(new MenuSlider("HPW", "Ally Minimum Health  Percentage %{0} Use W ", 60, 1));
            ComboMenu.Add(new MenuSlider("HPWS", "Sona Minimum Health  Percentage %{0} Use W ", 80, 1));
            ComboMenu.Add(new MenuSeparator("E Settings", "E Settings"));
            ComboMenu.Add(new MenuBool("UseE", "Use [E] in combo", false));
            ComboMenu.Add(new MenuBool("UseEA", "Use [E] for ally"));
            ComboMenu.Add(new MenuSlider("HPE", " Ally Minimum Health  Percentage %{0} Use E ", 30, 1));
            ComboMenu.Add(new MenuSeparator("R Settings", "R Settings"));
            ComboMenu.Add(new MenuBool("UseR", "Use [R] in combo", false));
            ComboMenu.Add(new MenuSeparator("Use for cast Flash + R To enemy ", "Use for cast Flash + R To enemy "));
            ComboMenu.Add(new MenuKeyBind("FlashR", "Use Flash + R", System.Windows.Forms.Keys.T, KeyBindType.Press));
            ComboMenu.Add(new MenuSeparator("Use for cast Ultimate to enemy by Prediction ", "Use for cast Ultimate to enemy by Prediction "));
            ComboMenu.Add(new MenuKeyBind("UltA", "Use Ultimate Assistance", System.Windows.Forms.Keys.G, KeyBindType.Press));
            ComboMenu.Add(new MenuBool("UseUA", "Tick for enable/disable Ultimate Assistance"));
            StartMenu.Add(ComboMenu);
            AHarrasMenu.Add(new MenuSeparator("Auto Harras Settings", "Auto Harras Settings"));
            AHarrasMenu.Add(new MenuSeparator("Tick for enable/disable auto harras with Q when enemy is in Range", "Tick for enable/disable auto harras with Q when enemy is in Range"));
            AHarrasMenu.Add(new MenuBool("AHQ", "- Use [Q] For Auto Harras"));
            AHarrasMenu.Add(new MenuSlider("AHQM", " Minimum Mana  Percentage %{0} Use [Q] ", 65, 1));
            StartMenu.Add(AHarrasMenu);
            DrawingsMenu.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            DrawingsMenu.Add(new MenuSeparator("Tick for enable/disable Draw Spell Range", "Tick for enable/disable Draw Spell Range"));
            DrawingsMenu.Add(new MenuBool("DQ", "- Draw [Q] range"));
            DrawingsMenu.Add(new MenuBool("DW", "- Draw [W] range"));
            DrawingsMenu.Add(new MenuBool("DE", "- Draw [E] range"));
            DrawingsMenu.Add(new MenuBool("DR", "- Draw [R] range"));
            StartMenu.Add(DrawingsMenu);
            ActivatorMenu.Add(new MenuSeparator("Activator Settings", "Activator Settings"));
            ActivatorMenu.Add(new MenuSeparator("Use Summoner Spell", "Use Summoner Spell"));
            ActivatorMenu.Add(new MenuBool("IGNI", "- Use Ignite if enemy is killable"));
            StartMenu.Add(ActivatorMenu);
            StartMenu.Attach();
            Game.OnUpdate += Game_OnTick;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

        }


        private static void Game_OnUpdate(EventArgs args)
        {


            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
            //   if (Orbwalker.ActiveMode.Equals(OrbwalkerMode.LastHit))
            //      {
            //    Lasthit();
            //     }
            Activator();
            Heal();
            HealSelf();
            Run();
            AHarras();




        }

        private static void Game_OnTick(EventArgs args)
        {

            if (ComboMenu["FlashR"].GetValue<MenuKeyBind>().Active)
            {
                FlashR();
            }

            if (ComboMenu["UltA"].GetValue<MenuKeyBind>().Active)
            {
                UltA();
            }


        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Physical);
            if (DrawingsMenu["DQ"].GetValue<MenuBool>().Enabled && _Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _Q.Range, Color.Black, 1);
            }
            if (DrawingsMenu["DW"].GetValue<MenuBool>().Enabled && _W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _W.Range, Color.Black, 1);
            }
            if (DrawingsMenu["DE"].GetValue<MenuBool>().Enabled && _E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _E.Range, Color.Black, 1);
            }
            if (DrawingsMenu["DR"].GetValue<MenuBool>().Enabled && _R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _R.Range, Color.Black, 1);
            }

        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);

            if (target == null)
                return;
            if (ComboMenu["UseQ"].GetValue<MenuBool>().Enabled)
            {
                if (!target.IsValidTarget(_Q.Range) && _Q.IsReady())
                    return;
                {
                    _Q.Cast();
                }



            }
            if (ComboMenu["UseE"].GetValue<MenuBool>().Enabled)
            {
                if (!target.IsValidTarget(_E.Range) && _E.IsReady())
                    return;

                {

                    _E.Cast(target);


                }
            }
            if (ComboMenu["UseR"].GetValue<MenuBool>().Enabled)
            {
                var Rpred = _R.GetPrediction(target);

                _R.Cast(target.Position);
                if (Rpred.Hitchance >= HitChance.VeryHigh && target.IsValidTarget(_R.Range))
                    if (!target.IsValidTarget(_R.Range) && _R.IsReady())
                        return;
                {




                }
            }





        }

        public static void Heal()
        {
            foreach (var allies in GameObjects.AllyHeroes.Where(y => y.HealthPercent < ComboMenu["HPW"].GetValue<MenuSlider>().Value && ComboMenu["UseW"].GetValue<MenuBool>().Enabled))
            {
                _W.Cast(allies);
            }


        }

        public static void HealSelf()
        {
            foreach (var HealSelf in GameObjects.AllyHeroes.Where(y => y.HealthPercent < ComboMenu["HPWS"].GetValue<MenuSlider>().Value && ComboMenu["UseW"].GetValue<MenuBool>().Enabled))
            {
                _W.Cast(HealSelf);
            }


        }
        public static void Run()
        {
            foreach (var Run in GameObjects.AllyHeroes.Where(y => _Player.HealthPercent < ComboMenu["HPE"].GetValue<MenuSlider>().Value && ComboMenu["UseEA"].GetValue<MenuBool>().Enabled))
            {
                _W.Cast(Run);
            }


        }

        public static void AHarras()
        {

            //       foreach (var HQ in EntityManager.Heroes.Enemies.Where(y => y.ManaPercent > AHarrasMenu["AHQM"].GetValue<MenuSlider>().Value && AHarrasMenu["AHQ"].GetValue<MenuBool>().Enabled))
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);

            if (target == null)
                return;
            {
                if (_Player.ManaPercent > AHarrasMenu["AHQM"].GetValue<MenuSlider>().Value && AHarrasMenu["AHQ"].GetValue<MenuBool>().Enabled)

                {
                    _Q.Cast(target);
                }
            }






        }


        public static void UltA()
        {

            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                var target = TargetSelector.GetTarget(_R.Range, DamageType.Magical);
                if (target.IsValidTarget(_R.Range))
                    if (!target.IsValidTarget(_R.Range))
                        return;
                {
                    if (target.IsValidTarget() && _R.IsReady() && ComboMenu["UseUA"].GetValue<MenuBool>().Enabled)
                    {
                        _R.Cast(target);
                    }
                }
            }
        }

        public static void Activator()
        {
            var target = TargetSelector.GetTarget(_Ignite.Range, DamageType.True);
            if (target == null)
                return;
            if (ActivatorMenu["IGNI"].GetValue<MenuBool>().Enabled && _Ignite.IsReady() && target.IsValidTarget())

            {
                if (target.Health + target.PhysicalShield <
                    _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                {
                    _Ignite.Cast(target);
                }
            }
        }
        /* credits for wladi0*/
        public static void FlashR()
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
            var target = TargetSelector.GetTarget(_FlashR.Range, DamageType.Magical);
            if (target.IsValidTarget(_FlashR.Range))
                if (!target.IsValidTarget(_FlashR.Range))
                    return;
            {
                var Flashh = ObjectManager.Player.Position.Extend(target.Position, _Flash.Range);

                if (_Flash.IsReady() && target.IsValidTarget() && _R.IsReady())
                {
                    _Flash.Cast(Flashh.ToVector2());

                    var Rpred = _R.GetPrediction(target);

                    _R.Cast(target.Position);

                }
            }


        }
    }
}
