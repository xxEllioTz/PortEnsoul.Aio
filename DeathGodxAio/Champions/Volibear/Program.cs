using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using SPrediction;
using  System.Drawing;
namespace VoliPower
{
    class Program
    {
        private const string ChampionName = "Volibear";
        public static AIHeroClient Player;
        public static Menu Menu { get; set; }
        public static Menu comboMenu, laneclearing, fleeMenu, misc, drawingMenu;
        private static Spell _q, _w, _e, _r;
        static Items.Item _botrk, _cutlass;


        //credits Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
    

        public static void VolibearGame_OnLoad()
        {
            #region main
            {
                Player = ObjectManager.Player;

                if (Player.CharacterName != ChampionName)
                {
                    return;
                }

                _q = new Spell(SpellSlot.Q, 600);
                _w = new Spell(SpellSlot.W, 405);
                _e = new Spell(SpellSlot.E, 400);
                _r = new Spell(SpellSlot.R, 125);

                _botrk = new Items.Item(3153, 450f);
                _cutlass = new Items.Item(3144, 450f);

            }
            #endregion

            #region content menu

            Menu = new Menu("Teddy Bear - ThunderBuddy", "teddy.bear", true);

            comboMenu = new Menu("Combo", "teddy.bear.combo");
            comboMenu.Add(new MenuBool("teddy.bear.combo.useq", "Use Q", true));
            comboMenu.Add(new MenuBool("teddy.bear.combo.usew", "Use W", true));
            comboMenu.Add(new MenuBool("teddy.bear.combo.usee", "Use E", true));
            comboMenu.Add(new MenuBool("teddy.bear.combo.user", "Use R", true));
            Menu.Add(comboMenu);
            laneclearing = new Menu("Lane clear", "teddy.bear.laneclearing");
            laneclearing.Add(new MenuBool("teddy.bear.laneclearing.useQ", "Use Q", true));
            laneclearing.Add(new MenuBool("teddy.bear.laneclearing.useW", "Use W", true));
            laneclearing.Add(new MenuBool("teddy.bear.laneclearing.useE", "Use E", true));
            Menu.Add(laneclearing);

            fleeMenu = new Menu("Flee", "teddy.bear.flee");
            fleeMenu.Add(new MenuBool("teddy.bear.flee.useQ", "Use Q", true));
            fleeMenu.Add(new MenuBool("teddy.bear.flee.useE", "Use E", true));
            Menu.Add(fleeMenu);

            misc = new Menu("Misc", "teddy.bear.misc");
            misc.Add(new MenuBool("teddy.bear.misc.skW", "safe kill with W", true));
            Menu.Add(misc);
            drawingMenu = new Menu("Drawing", "teddy.bear.drawing");
            drawingMenu.Add(new MenuBool("DrawQ", "Draw Q range", true));
            drawingMenu.Add(new MenuBool("DrawW", "Draw W range", true));
            drawingMenu.Add(new MenuBool("DrawE", "Draw E range", true));
            drawingMenu.Add(new MenuBool("DrawR", "Draw R range", true));
            drawingMenu.Add(new MenuBool("DrawHP", "Draw HP Indicator", true));
            Menu.Add(drawingMenu);
            Menu.Attach();
            #endregion

            Interrupter.OnInterrupterSpell += Interrupter_OnPossibleToInterrupt;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Game.Print("<font color='#881df2'>TeddyBear - Loaded.");

        }


        public static bool getMenuBoolItem(Menu m, string item)
        {
            return m[item].GetValue<MenuBool>().Enabled;
        }

        public static int getMenuSliderItem(Menu m, string item)
        {
            return m[item].GetValue<MenuSlider>().Value;
        }

        public static bool getMenuKeyBindItem(Menu m, string item)
        {
            return m[item].GetValue<MenuKeyBind>().Active;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].GetValue<MenuList>().Index;
        }

        static void Interrupter_OnPossibleToInterrupt(AIBaseClient unit, Interrupter.InterruptSpellArgs args)
        {
            if (args.DangerLevel >= Interrupter.DangerLevel.High && unit.Distance(Player.Position) <= _q.Range)
            {
                _q.Cast(unit);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (getMenuBoolItem(drawingMenu, "DrawHP"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x =>
                    x.IsValidTarget() && x.IsVisibleOnScreen))
                {
                    var dmg = GameObjects.Player.GetSpellDamage(target, SpellSlot.W) + GameObjects.Player.GetSpellDamage(target, SpellSlot.Q) + GameObjects.Player.GetSpellDamage(target, SpellSlot.E) + GameObjects.Player.GetAutoAttackDamage(target);
                    if (dmg > 0)
                    {
                        var barPos = target.HPBarPosition;
                        var xPos = barPos.X - 45;
                        var yPos = barPos.Y - 19;
                        if (target.CharacterName == "Annie")
                        {
                            yPos += 2;
                        }

                        var remainHealth = target.Health - dmg;
                        var x1 = xPos + (target.Health / target.MaxHealth * 104);
                        var x2 = (float)(xPos + ((remainHealth > 0 ? remainHealth : 0) / target.MaxHealth * 103.4));
                        Drawing.DrawLine(x1, yPos, x2, yPos, 11, Color.FromArgb(255, 147, 0));
                    }
                }
            }
        }

        private static int CalcDamage(AIBaseClient target)
        {
            var aa = Player.GetAutoAttackDamage(target) * (1 + Player.Crit);
            var damage = aa;

            if (_r.IsReady()) // rdamage
            {
                damage += _r.GetDamage(target);
            }

            if (_q.IsReady()) // qdamage
            {

                damage += _q.GetDamage(target);
            }

            if (_e.IsReady()) // edamage
            {

                damage += _e.GetDamage(target);
            }

            return (int)damage;
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = getMenuBoolItem(drawingMenu, "DrawQ");
            var menuItem2 = getMenuBoolItem(drawingMenu, "DrawE");
            var menuItem3 = getMenuBoolItem(drawingMenu, "DrawW");
            var menuItem4 = getMenuBoolItem(drawingMenu, "DrawR");

            if (menuItem1 && _q.IsReady()) Render.Circle.DrawCircle(Player.Position, _q.Range, Color.SpringGreen);
            if (menuItem2 && _e.IsReady()) Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Crimson);
            if (menuItem3 && _w.IsReady()) Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Aqua);
            if (menuItem4 && _r.IsReady()) Render.Circle.DrawCircle(Player.Position, _r.Range, Color.Firebrick);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            /* if (Orbwalker.ActiveMode.HasFlag(OrbwalkerMode.Flee))
             {
                 Flee();
             }*/

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                Laneclear();
            }

        }

        private static void Flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos, false);
            _q.Cast();
        }

        private static void Laneclear() // jungle clear ^^
        {
            bool vQ = _q.IsReady() && getMenuBoolItem(laneclearing, "teddy.bear.laneclearing.useQ");
            bool vW = _w.IsReady() && getMenuBoolItem(laneclearing, "teddy.bear.laneclearing.useW");
            bool vE = _e.IsReady() && getMenuBoolItem(laneclearing, "teddy.bear.laneclearing.useE");

            var minionBase = MinionManager.GetMinions(_e.Range);
            var jungleBase = MinionManager.GetMinions(_w.Range, MinionManager.MinionTypes.All, MinionManager.MinionTeam.Neutral, MinionManager.MinionOrderTypes.MaxHealth);

            #region Q-Cast Jungle
            if (vQ)
            {
                foreach (var junglemob in jungleBase.Where(x => x.HealthPercent >= 25))
                {
                    if (_q.IsReady())
                    {
                        _q.Cast(junglemob);
                    }
                }
            }
            #endregion

            #region W-Cast Jungle
            if (vW)
            {
                foreach (var junglemob in jungleBase.Where(x => x.HealthPercent >= 25))
                {
                    if (_w.IsReady())
                    {
                        _w.CastOnUnit(junglemob);
                    }
                }
            }
            #endregion

            #region E-Cast Jungle
            if (vE)
            {

                if (jungleBase.Count >= 1 && _e.IsReady())
                {
                    _e.Cast();
                }

                if (minionBase.Count >= 3 && _e.IsReady())
                {
                    _e.Cast();
                }


            }
            #endregion
        }

        private static void Combo()
        {
            bool vQ = _q.IsReady() && getMenuBoolItem(comboMenu, "teddy.bear.combo.useq");
            bool vW = _w.IsReady() && getMenuBoolItem(comboMenu, "teddy.bear.combo.usew");
            bool vE = _e.IsReady() && getMenuBoolItem(comboMenu, "teddy.bear.combo.usee");
            bool vR = _r.IsReady() && getMenuBoolItem(comboMenu, "teddy.bear.combo.user"); ;
            bool useskW = getMenuBoolItem(misc, "teddy.bear.misc.skW");

            AIHeroClient tsQ = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            AIHeroClient tsR = TargetSelector.GetTarget(_r.Range, DamageType.Magical);

            #region Q-Cast
            if (vQ)
            {
                if (tsQ.Distance(Player.Position) >= 2500 && tsQ.Direction == Player.Direction && tsQ.MoveSpeed > Player.MoveSpeed &&
                    tsQ.MoveSpeed < Player.MoveSpeed * 1.3)
                {
                    _q.Cast(tsQ);
                }
                if (tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
                else if (Player.CountAllyHeroesInRange(500) >= 1 && tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
                else if (tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
            }
            #endregion

            #region W-Cast
            if (vW && useskW)
            {
                if (tsQ.IsValidTarget(300))
                {
                    _w.CastOnUnit(tsQ);
                }
            }
            else if (vW)
            {
                if (tsQ.IsValidTarget(300))
                {
                    _w.CastOnUnit(tsQ);
                }
            }
            #endregion

            #region E-Cast
            if (vE)
            {
                if (tsQ.IsValidTarget(_e.Range) && tsQ.Distance(Player.Position) <= _w.Range)
                {
                    _e.Cast();
                }
            }
            #endregion

            #region R-Cast
            if (vR)
            {
                if (tsR.IsValidTarget(Player.AttackRange) && tsR.HealthPercent > 25)
                {
                    _r.Cast();
                }
            }
            #endregion

        }
    }
}
