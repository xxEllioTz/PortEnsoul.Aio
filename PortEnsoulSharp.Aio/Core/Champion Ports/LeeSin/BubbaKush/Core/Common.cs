namespace xxEliot.Core
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using BubbaKushs;
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Prediction;
    using SharpDX;

    #endregion

    internal static class Common
    {
        #region Properties

        internal static bool CanFlash => Pro.Flash != SpellSlot.Unknown && Pro.Flash.IsReady();

        internal static bool CanIgnite => Pro.Ignite != SpellSlot.Unknown && Pro.Ignite.IsReady();

        internal static bool CanSmite => Pro.Smite != SpellSlot.Unknown && Pro.Smite.IsReady();

        internal static bool CantAttack
            =>
                (Orbwalker.GetTarget() == null || !Orbwalker.CanAttack())
                && Orbwalker.CanMove();

        private static int GetSmiteDmg
            =>
                new[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 }[
                    Pro.player.Level - 1];

        #endregion

        #region Methods
        private static AIHeroClient _selectedTargetObjAiHero;
        public static void SetTarget(AIHeroClient hero)
        {
            if (!hero.IsValidTarget(float.MaxValue, true))
                return;
            _selectedTargetObjAiHero = hero;
        }

        internal static bool CanHitCircle(this Spell spell, AIBaseClient unit)
        {
            return spell.IsInRange(spell.GetPredPosition(unit));
        }

        internal static bool CanLastHit(this Spell spell, AIBaseClient unit, double dmg, double subDmg = 0)
        {
            var hpPred = spell.GetHealthPrediction(unit);
            return hpPred > 0 && hpPred - subDmg < dmg;
        }

        internal static CastStates Casting(
            this Spell spell,
            AIBaseClient unit,
            bool aoe = false,
          CollisionObjects collisionable = CollisionObjects.Minions | CollisionObjects.YasuoWall)
        {
            if (!unit.IsValidTarget())
            {
                return CastStates.InvalidTarget;
            }
            if (!spell.IsReady())
            {
                return CastStates.NotReady;
            }
            if (spell.CastControl != null && !spell.CastControl())
            {
                return CastStates.FailedControl;
            }
            var pred = spell.GetPrediction(unit, aoe, -1, collisionable);
            if (pred.CollisionObjects.Count > 0)
            {
                return CastStates.Collision;
            }
            if (spell.RangeCheckFrom.DistanceSquared(pred.CastPosition) > spell.Range)
            {
                return CastStates.OutOfRange;
            }
            if (pred.Hitchance < spell.MinHitChance
                && (!pred.Input.Aoe || pred.Hitchance < HitChance.High || pred.AoeTargetsHitCount < 2))
            {
                return CastStates.LowHitChance;
            }
            if (!Pro.player.Spellbook.CastSpell(spell.Slot, pred.CastPosition))
            {
                return CastStates.NotCasted;
            }
            spell.LastCastAttemptT = Variables.TickCount;
            return CastStates.SuccessfullyCasted;
        }

        internal static CastStates CastingBestTarget(
            this Spell spell,
            bool aoe = false,
            CollisionObjects collisionable = CollisionObjects.Minions | CollisionObjects.YasuoWall)
        {
            return spell.Casting(spell.GetTarget(spell.Width / 2), aoe, collisionable);
        }

        internal static bool CastSpellSmite(this Spell spell, AIHeroClient target, bool smiteCol)
        {
            var pred1 = spell.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);
            if (pred1.Hitchance < spell.MinHitChance)
            {
                return false;
            }
            var pred2 = spell.GetPrediction(target, false, -1, CollisionObjects.Minions);
            return pred2.Hitchance == HitChance.Collision
                       ? smiteCol && CastSmiteKillCollision(pred2.CollisionObjects) && spell.Cast(pred1.CastPosition)
                       : pred2.Hitchance >= spell.MinHitChance && spell.Cast(pred2.CastPosition);
        }

        internal static void CastTiamatHydra()
        {
            if (Pro.Tiamat.IsReady && Pro.player.CountEnemyHeroesInRange(Pro.Tiamat.Range) > 0)
            {
                Pro.Tiamat.Cast();
            }
            if (Pro.Ravenous.IsReady && Pro.player.CountEnemyHeroesInRange(Pro.Ravenous.Range) > 0)
            {
                Pro.Ravenous.Cast();
            }
            if (Pro.Titanic.IsReady && Orbwalker.GetTarget() != null)
            {
                Pro.Titanic.Cast();
            }
        }

        internal static Vector3 GetPredPosition(this Spell spell, AIBaseClient unit, bool useRange = false)
        {
            var pos = SpellPrediction.GetPrediction(unit, spell.Delay, 1, spell.Speed).UnitPosition;
            return useRange && !spell.IsInRange(pos) ? unit.Position : pos;
        }

        internal static bool IsCasted(this CastStates state)
        {
            return state == CastStates.SuccessfullyCasted;
        }

        internal static bool IsWard(this AIMinionClient minion)
        {
            return minion.GetMinionType().HasFlag(MinionTypes.Ward) && minion.SkinName != "BlueTrinket";
        }

        internal static List<AIBaseClient> ListEnemies(bool includeClones = false)
        {
            var list = new List<AIBaseClient>();
            list.AddRange(GameObjects.EnemyHeroes);
            list.AddRange(ListMinions(includeClones));
            return list;
        }

        internal static List<AIMinionClient> ListMinions(bool includeClones = false)
        {
            var list = new List<AIMinionClient>();
            list.AddRange(GameObjects.Jungle);
            list.AddRange(GameObjects.EnemyMinions.Where(i => i.IsMinion() || i.IsPet(includeClones)));
            return list;
        }

        private static bool CastSmiteKillCollision(List<AIBaseClient> col)
        {
            if (col.Count > 1 || !CanSmite)
            {
                return false;
            }
            var obj = col.First();
            return obj.Health <= GetSmiteDmg && obj.DistanceToPlayer() < Pro.SmiteRange
                   && Pro.player.Spellbook.CastSpell(Pro.Smite, obj);
        }

        #endregion
    }
}