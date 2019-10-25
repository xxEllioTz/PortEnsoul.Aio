namespace ElRumble
{

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
    using SPrediction;

    internal enum Spells
    {
        Q,

        W,

        E,

        R,

        R1
    }

    internal static class Rumble
    {
        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 500) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 0) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 950) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 1700) },
                                                                 { Spells.R1, new Spell(SpellSlot.R, 800) }
                                                             };

        private static SpellSlot _ignite;

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

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

        #region Public Methods and Operators
     
        public static int CountEnemiesNearPosition(Vector3 pos, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(hero => hero.IsEnemy && !hero.IsDead && hero.IsValid && hero.Distance(pos) <= range);
        }

        public static float GetComboDamage(AIBaseClient enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, SummonerSpell.Ignite);
            }

            return damage;
        }

        public static void OnDraw(EventArgs args)
        {
            var drawQ = getMenuBoolItem(ElRumbleMenu.miscMenu, "ElRumble.Draw.Q");
            var drawE = getMenuBoolItem(ElRumbleMenu.miscMenu, "ElRumble.Draw.E");
            var drawR = getMenuBoolItem(ElRumbleMenu.miscMenu, "ElRumble.Draw.R");


            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }

                if (CountEnemiesNearPosition(Player.Position, spells[Spells.R].Range + 500) < 2)
                {
                    var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);

                    if (target == null)
                    {
                        return;
                    }

                    var vector1 = target.Position
                                  - Vector3.Normalize(target.Position - Player.Position) * 300;

                    spells[Spells.R1].UpdateSourcePosition(vector1, vector1);

                    var pred = spells[Spells.R1].GetPrediction(target, true);

                    var midpoint = (Player.Position + pred.UnitPosition) / 2;
                    var vector2 = midpoint - Vector3.Normalize(pred.UnitPosition - Player.Position) * 300;

                    if (Player.Distance(target.Position) < 400)
                    {
                        vector1 = midpoint + Vector3.Normalize(pred.UnitPosition - Player.Position) * 800;
                        if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, vector2))
                        {
                            var wts = Drawing.WorldToScreen(Player.Position);
                            Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + 1);

                            var wtsPlayer = Drawing.WorldToScreen(vector1);
                            var wtsPred = Drawing.WorldToScreen(vector2);

                            Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                            Render.Circle.DrawCircle(vector1, 50, Color.Aqua);
                            Render.Circle.DrawCircle(vector2, 50, Color.Yellow);
                            Render.Circle.DrawCircle(pred.UnitPosition, 50, Color.Red);
                        }
                    }
                    else if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, pred.CastPosition))
                    {
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            var wts = Drawing.WorldToScreen(Player.Position);
                            Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + 1);

                            var wtsPlayer = Drawing.WorldToScreen(vector1);
                            var wtsPred = Drawing.WorldToScreen(pred.CastPosition);

                            Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                            Render.Circle.DrawCircle(vector1, 50, Color.Aqua);
                            Render.Circle.DrawCircle(pred.CastPosition, 50, Color.Yellow);
                        }
                    }
                }
            }
        }

        public static void RumbleOnLoad()
        {
            if (ObjectManager.Player.CharacterName!= "Rumble")
            {
                return;
            }

            _ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.R].SetSkillshot(0.25f, 110, 2500, false, false, SkillshotType.Line);
            spells[Spells.R1].SetSkillshot(0.25f, 110, 2600, false, false, SkillshotType.Line);
            spells[Spells.E].SetSkillshot(0.45f, 90, 1200, true, true, SkillshotType.Line);

            ElRumbleMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        #endregion

        #region Methods

        //CREDITS TO XSALICE - Made a few changes to it
        private static void CastR()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var vector1 = target.Position - Vector3.Normalize(target.Position - Player.Position) * 300;

            spells[Spells.R1].UpdateSourcePosition(vector1, vector1);

            var pred = spells[Spells.R1].GetPrediction(target, true);

            if (Player.Distance(target.Position) < 400)
            {
                var midpoint = (Player.Position + pred.UnitPosition) / 2;

                vector1 = midpoint + Vector3.Normalize(pred.UnitPosition - Player.Position) * 800;
                var vector2 = midpoint - Vector3.Normalize(pred.UnitPosition - Player.Position) * 300;

                if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, vector2))
                {
                    CastR2(vector1, vector2);
                }
            }
            else if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, pred.CastPosition))
            {
                if (pred.Hitchance >= HitChance.High)
                {
                    CastR2(vector1, pred.CastPosition);
                }
            }
        }

        private static void CastR2(Vector3 start, Vector3 end)
        {
            if (!spells[Spells.R].IsReady())
            {
                return;
            }

            spells[Spells.R].Cast(start, end);
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite);
        }

        private static bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 25)
            {
                var pos = start.ToVector2().Extend(Player.Position.ToVector2(), -i);
                if (IsWall(pos))
                {
                    return true;
                }
            }
            return false;
        }

        //CREDITS TO XSALICE
        private static bool IsWall(Vector2 pos)
        {
            return (NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Wall
                    || NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Building);
        }

        private static void KeepHeat()
        {
            var useQ = getMenuBoolItem(ElRumbleMenu.heatMenu, "ElRumble.Heat.Q");
            var useW = getMenuBoolItem(ElRumbleMenu.heatMenu, "ElRumble.Heat.W");

            if (Player.Mana < 50)
            {
                if (useQ && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(Game.CursorPos);
                }

                if (useW && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast();
                }
            }
        }

        private static void OnClear()
        {
            var useQ = getMenuBoolItem(ElRumbleMenu.clearMenu, "ElRumble.LaneClear.Q");
            var useE = getMenuBoolItem(ElRumbleMenu.clearMenu, "ElRumble.LaneClear.E");

            var minions = MinionManager.GetMinions(Player.Position, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (minions.Count > 1)
                {
                    var farmLocation = spells[Spells.Q].GetCircularFarmLocation(minions);
                    spells[Spells.Q].Cast(farmLocation.Position);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(minions.FirstOrDefault());
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);

            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getMenuBoolItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.Q");
            var useW = getMenuBoolItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.W");
            var useE = getMenuBoolItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.E");
            var useR = getMenuBoolItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.R");
            var useI = getMenuBoolItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.Ignite");
            var countEnemies = getMenuSliderItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.Count.Enemies");

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.E].Cast(pred.CastPosition);
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }

            if (useR && spells[Spells.R].IsReady() && Player.CountEnemyHeroesInRange(spells[Spells.R].Range) >= countEnemies)
            {
                CastR();
            }

            if (useR && spells[Spells.R].IsReady() && spells[Spells.W].IsReady() == false)
            {
                if (target.Health < spells[Spells.R].GetDamage(target))
                {
                    CastR();
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getMenuBoolItem(ElRumbleMenu.harassMenu, "ElRumble.Harass.Q");
            var useE = getMenuBoolItem(ElRumbleMenu.harassMenu, "ElRumble.Harass.E");

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.E].Cast(target);
                }
            }
        }

        private static void OnJungleClear()
        {
            var useQ = getMenuBoolItem(ElRumbleMenu.clearMenu, "ElRumble.JungleClear.Q");
            var useE = getMenuBoolItem(ElRumbleMenu.clearMenu, "ElRumble.JungleClear.E");

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.Position,
                spells[Spells.Q].Range);

            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (minions.Count > 1)
                {
                    var farmLocation = spells[Spells.Q].GetCircularFarmLocation(minions);
                    spells[Spells.Q].Cast(farmLocation.Position);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(minions.FirstOrDefault());
            }
        }

        private static void OnLastHit()
        {
            var useE = getMenuBoolItem(ElRumbleMenu.clearMenu, "ElRumble.LastHit.E");
            if (useE && spells[Spells.E].IsReady())
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.E].Cast(minion);
                            return;
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                OnCombo();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                OnHarass();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                OnClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                OnJungleClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                OnLastHit();
            }

            var keepHeat = getMenuKeyBindItem(ElRumbleMenu.heatMenu, "ElRumble.KeepHeat.Activated");
            if (keepHeat)
            {
                KeepHeat();
            }

            if (getMenuKeyBindItem(ElRumbleMenu.miscMenu, "ElRumble.Misc.R") && spells[Spells.R].IsReady())
            {
                CastR();
            }
        }

        #endregion
    }
}