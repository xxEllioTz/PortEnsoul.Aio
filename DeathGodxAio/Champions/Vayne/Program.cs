#region

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
using static EnsoulSharp.SDK.Items;
using SPrediction;
#endregion

namespace hi_im_gosu
{
    public class Vayne
    {
   
        public static Spell E;
        public static Spell Q;
        public static Spell R;

        private static Menu menu;

        private static Dictionary<string, SpellSlot> spellData;

        private static AIHeroClient tar;
        public const string ChampName = "Vayne";
        public static AIHeroClient Player;
        private static Menu qmenu;
        private static Menu emenu;

        /* Asuna VayneHunter Copypasta */
        private static readonly Vector2 MidPos = new Vector2(6707.485f, 8802.744f);

        private static readonly Vector2 DragPos = new Vector2(11514, 4462);

        private static float LastMoveC;

        private static void TumbleHandler()
        {
            if (!getMenuKeyBindItem(menu, "walltumble"))
                return;

            if (Player.Distance(MidPos) >= Player.Distance(DragPos))
            {
                if (Player.Position.X < 12000 || Player.Position.X > 12070 || Player.Position.Y < 4800 ||
                Player.Position.Y > 4872)
                {
                    MoveToLimited(new Vector2(12050, 4827).ToVector3());
                }
                else
                {
                    MoveToLimited(new Vector2(12050, 4827).ToVector3());
                    Q.Cast(DragPos, true);
                }
            }
            else
            {
                if (Player.Position.X < 6908 || Player.Position.X > 6978 || Player.Position.Y < 8917 ||
                Player.Position.Y > 8989)
                {
                    MoveToLimited(new Vector2(6958, 8944).ToVector3());
                }
                else
                {
                    MoveToLimited(new Vector2(6958, 8944).ToVector3());
                    Q.Cast(MidPos, true);
                }
            }
        }

        private static void MoveToLimited(Vector3 where)
        {
            if (Environment.TickCount - LastMoveC < 80)
            {
                return;
            }

            LastMoveC = Environment.TickCount;
            Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }

        /* End Asuna VayneHunter Copypasta */

        public static void VayneGame_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.CharacterName != ChampName) return;
            spellData = new Dictionary<string, SpellSlot>();
            var Menuvayne = new Menu("Gosu Vayne", "Gosu Vayne",true);
            menu = new Menu("Gosu", "Gosu");
            menu.Add(new MenuKeyBind("aaqaa", "GameClose", System.Windows.Forms.Keys.X, KeyBindType.Press));
            menu.Add(new MenuKeyBind("walltumble", "Wall Tumble", System.Windows.Forms.Keys.U, KeyBindType.Press));
            menu.Add(new MenuBool("useR", "Use R Combo"));
            menu.Add(new MenuSlider("enemys", "If Enemys Around >=", 2, 1, 5));
            Menuvayne.Add(menu);
            qmenu = new Menu("Tumble", "Tumble");
            qmenu.Add(new MenuBool("UseQC", "Use Q Combo"));
            qmenu.Add(new MenuBool("hq", "Use Q Harass"));
            qmenu.Add(new MenuBool("restrictq", "Restrict Q usage?"));
            qmenu.Add(new MenuBool("UseQJ", "Use Q Farm"));
            qmenu.Add(new MenuSlider("Junglemana", "Minimum Mana to Use Q Farm", 60, 1, 100));
            Menuvayne.Add(qmenu);
            emenu = new Menu("Condemn", "Condemn");
            emenu.Add(new MenuBool("UseEC", "Use E Combo"));
            emenu.Add(new MenuBool("he", "Use E Harass"));
            emenu.Add(new MenuKeyBind("UseET", "Use E (Toggle)", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            emenu.Add(new MenuBool("Int_E", "Use E To Interrupt"));
            emenu.Add(new MenuBool("Gap_E", "Use E To Gabcloser"));
            emenu.Add(new MenuSlider("PushDistance", "E Push Distance", 425, 425, 300));
            emenu.Add(new MenuKeyBind("UseEaa", "Use E after auto", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
            Menuvayne.Add(emenu);
            Menuvayne.Attach();
            Q = new Spell(SpellSlot.Q, 0f);
            R = new Spell(SpellSlot.R, float.MaxValue);
            E = new Spell(SpellSlot.E, float.MaxValue);

            E.SetTargetted(0.25f, 2200f);
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnAction += Orbwalking_AfterAttack;
            Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnInterrupterSpell += Interrupter2_OnInterruptableTarget;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs gapcloser)
        {
            if (E.IsReady() && sender.IsValidTarget(200) && getMenuBoolItem(emenu, "Gap_E") && sender.IsEnemy)
            {
                E.Cast(sender);
            }
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

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit,Interrupter.InterruptSpellArgs args)
        {
            if (E.IsReady() && unit.IsValidTarget(550) && getMenuBoolItem(emenu, "Int_E") && unit.IsEnemy)
            {
                E.Cast(unit);
            }
        }


        public static void Orbwalking_AfterAttack(Object targeti ,OrbwalkerActionArgs args)
        {
            var target = TargetSelector.GetTarget(600);
            if ((Orbwalker.ActiveMode == OrbwalkerMode.LaneClear || Orbwalker.ActiveMode == OrbwalkerMode.LaneClear) && 100 * (Player.Mana / Player.MaxMana) > getMenuSliderItem(qmenu, "Junglemana"))
            {
                var mob = MinionManager.GetMinions(Player.Position, E.Range).FirstOrDefault();
                var Minions = MinionManager.GetMinions(Player.Position.Extend(Game.CursorPos, Q.Range), Player.AttackRange);
                var useQ = getMenuBoolItem(qmenu, "UseQJ");
                int countMinions = 0;

                foreach (var minions in Minions.Where(minion => minion.Health < Player.GetAutoAttackDamage(minion) || minion.Health < Q.GetDamage(minion) + Player.GetAutoAttackDamage(minion) || minion.Health < Q.GetDamage(minion)))
                {
                    countMinions++;
                }

                if (countMinions >= 2 && useQ && Q.IsReady() && Minions != null)
                    Q.Cast(Player.Position.Extend(Game.CursorPos, Q.Range / 2));

                if (useQ && Q.IsReady() && Player.InAutoAttackRange() && mob != null)
                {
                    Q.Cast(Game.CursorPos);
                }
            }

            if (!(target is AIHeroClient)) return;

            tar = (AIHeroClient)target;

            if (getMenuKeyBindItem(menu, "aaqaa"))
            {
                if (Q.IsReady())
                {
                    Q.Cast(Game.CursorPos);
                }

                Orbwalker.Move(Game.CursorPos);
            }

            if (getMenuKeyBindItem(emenu, "UseEaa"))
            {
                E.Cast((AIBaseClient)target);
                emenu["UseEaa"].GetValue<MenuKeyBind>().Active = !getMenuBoolItem(emenu, "UseEaa");
            }

            if (Q.IsReady() && ((Orbwalker.ActiveMode == OrbwalkerMode.Combo && getMenuBoolItem(qmenu, "UseQC")) || (Orbwalker.ActiveMode == OrbwalkerMode.Harass && getMenuBoolItem(qmenu, "hq"))))
            {
                if (getMenuBoolItem(qmenu, "restrictq"))
                {
                    var after = ObjectManager.Player.Position + Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                    var disafter = Vector3.DistanceSquared(after, tar.Position);
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Q.Cast(Game.CursorPos);
                    }

                    if (Vector3.DistanceSquared(tar.Position, ObjectManager.Player.Position) > 630 * 630 && disafter < 630 * 630)
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
                else
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        public static Vector3 Normalize(Vector3 A)
        {
            double distance = Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Vector3(new Vector2((float)(A.X / distance)), (float)(A.Y / distance));
        }

        public static List<Vector2> Points = new List<Vector2>();

        public static bool threeSixty(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || ObjectManager.Player.IsDashing())
                return false;
            var prediction = E.GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3> { pos.ToVector3() } : new List<Vector3> { unit.Position, unit.Position, prediction.CastPosition, prediction.UnitPosition };
            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < getMenuSliderItem(emenu, "PushDistance"); i += (int)unit.BoundingRadius) // 420 = push distance
                {
                    var cPos = ObjectManager.Player.Position.Extend(position, ObjectManager.Player.Distance(position) + i);
                    Points.Add(cPos.ToVector2());
                    if (NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if (wallsFound / predictionsList.Count >= 33 / 100f)
            {
                return true;
            }

            return false;
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (getMenuBoolItem(menu, "useR") && R.IsReady() && ObjectManager.Player.CountEnemyHeroesInRange(1000) >= getMenuSliderItem(menu, "enemys") && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                R.Cast();
            }

            if (getMenuKeyBindItem(menu, "walltumble"))
            {
                TumbleHandler();
            }

            if (getMenuKeyBindItem(menu, "aaqaa"))
            {
                Orbwalker.Move(Game.CursorPos);
            }

            if (!E.IsReady()) return;
            if ((Orbwalker.ActiveMode == OrbwalkerMode.Combo && getMenuBoolItem(emenu, "UseEC")) || (Orbwalker.ActiveMode == OrbwalkerMode.Harass && getMenuBoolItem(emenu, "he")) || getMenuKeyBindItem(emenu, "UseET"))
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity) && threeSixty(x)))
                {
                    if (Player.Distance(enemy) < 450)
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }
    }
}