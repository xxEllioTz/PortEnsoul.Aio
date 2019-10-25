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
namespace T7_Nidalee
{
    class Program
    {
        public static AIHeroClient myhero { get { return ObjectManager.Player; } }
        private static Menu menu, combo, harass, laneclear, jungleclear, misc, draw, pred, flee;
        public static bool Q1Ready = true, Q2Ready = true, W1Ready = true, W2Ready = true, E2Ready = true;
        static readonly string ChampionName = "Nidalee";
        static readonly string Version = "1.0";
        static readonly string Date = "7/8/16";
        private static Spell ignt { get; set; }
        private static Spell smite { get; set; }
        public static Item Potion { get; private set; }
        public static Item Biscuit { get; private set; }
        public static Item RPotion { get; private set; }

        public static void NidaleeOnLoad()
        {
            if (ObjectManager.Player.CharacterName != ChampionName) { return; }

            Game.Print("<font color='#0040FF'>T7</font><font color='#FF0505'> " + ChampionName + "</font> : Loaded! DEATHGODxPORTED(v" + Version + ")");
            Game.Print("<font color='#04B404'>By </font><font color='#3737E6'>Toyota</font><font color='#D61EBE'>7</font><font color='#FF0000'> <3 </font>");

            Drawing.OnDraw += OnDraw;
            //AIBaseClient.OnLevelUp += OnLvlUp;
            AIBaseClient.OnProcessSpellCast += OnProcess;
            Game.OnUpdate += OnTick;

            Potion = new Item((int)ItemId.Health_Potion, 400);
            Biscuit = new Item((int)ItemId.Total_Biscuit_of_Rejuvenation, 400);
            RPotion = new Item((int)ItemId.Refillable_Potion, 400);
            ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
            DatMenu();
            ignt = new Spell(myhero.GetSpellSlot("summonerdot"), 600);
            smite = new Spell(myhero.GetSpellSlot("summonersmite"), 500);
        }

        private static void OnTick(EventArgs args)
        {
            if (myhero.IsDead) return;

            

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo) Combo();

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && myhero.ManaPercent > MenuSlider(harass, "HMIN")) Harass();

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && myhero.ManaPercent > MenuSlider(laneclear, "LMIN")) Laneclear();

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && myhero.ManaPercent > MenuSlider(jungleclear, "JMIN")) Jungleclear();

            if (flee["floy"].GetValue<MenuKeyBind>().Active && check(misc, "W2FLEE") && myhero.Spellbook.GetSpell(SpellSlot.W).IsLearned)
            {
                if (IsCougar())
                {
                    DemSpells.W2.Cast(myhero.Position.Extend(Game.CursorPos, DemSpells.W2.Range - 1.0f).ToVector2());
                }
                else
                {
                    if (W2Ready && DemSpells.R.Cast())
                    {
                        DemSpells.W2.Cast(myhero.Position.Extend(Game.CursorPos, DemSpells.W2.Range - 1.0f).ToVector2());
                        return;
                    }
                }
            }

            Misc();
        }

        private static bool check(Menu submenu, string sig)
        {
            return submenu[sig].GetValue<MenuBool>().Enabled;
        }

        private static int MenuSlider(Menu submenu, string sig)
        {
            return submenu[sig].GetValue<MenuSlider>().Value;
        }

        private static int comb(Menu submenu, string sig)
        {
            return submenu[sig].GetValue<MenuList>().Index;
        }

        private static bool key(Menu submenu, string sig)
        {
            return submenu[sig].GetValue<MenuKeyBind>().Active;
        }

        private static void OnLvlUp(AIBaseClient sender, AIHeroClientLevelUpEventArgs args)
        {
            if (!sender.IsMe) return;

            var U = SpellSlot.Unknown;
            var Q = SpellSlot.Q;
            var W = SpellSlot.W;
            var E = SpellSlot.E;
            var R = SpellSlot.R;

            /*>>*/
            SpellSlot[] sequence1 = { U, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W, U };

            if (check(misc, "autolevel")) ObjectManager.Player.Spellbook.LevelSpell(sequence1[myhero.Level]);
        }

        /*  private static float ComboDamage(AIHeroClient target)
            {
                if (target != null)
                {
                    float TotalDamage = 0;

                    if (myhero.Spellbook.GetSpell(SpellSlot.Q).IsLearned && Q2Ready) { TotalDamage += Q2Damage(target); }  
                  
                    if (myhero.Spellbook.GetSpell(SpellSlot.Q).IsLearned && Q1Ready) { TotalDamage += Q1Damage(target); } 
 
                    if (myhero.Spellbook.GetSpell(SpellSlot.W).IsLearned && W2Ready) { TotalDamage += WDamage(target); }

                    if (myhero.Spellbook.GetSpell(SpellSlot.E).IsLearned && E2Ready) { TotalDamage += EDamage(target); }                              

                    return TotalDamage;
                }
                return 0;
            }*/

        private static double QDamage(AIHeroClient target)
        {
            int index = myhero.Spellbook.GetSpell(SpellSlot.Q).Level - 1;
            var dist = target.Distance(myhero.Position);
            var BaseDamage = new[] { 70, 85, 100, 115, 130 }[index];

            double QDamage = 0;

            if (dist <= 525)
            {
                QDamage = BaseDamage + (0.4f * myhero.FlatMagicDamageMod);
            }
            else if (dist > 525 && dist < 1300)
            {
                QDamage = ((((dist - 525) / 3.875f) / 100) * BaseDamage) + (0.4f * myhero.FlatMagicDamageMod);
            }
            else if (dist >= 1300)
            {
                QDamage = new[] { 210, 255, 300, 345, 390 }[index] + (1.2f * myhero.FlatMagicDamageMod);
            }

            return myhero.CalculateDamage(target, DamageType.Magical, QDamage);
        }

        private static bool IsCougar()
        {
            return myhero.GetRealAutoAttackRange() < 300;
        }

        private static bool IsHunted(AIHeroClient target)
        {
            return target.HasBuff("NidaleePassiveHunted");
        }

        private static void RLogic(AIHeroClient target)
        {
            if (IsCougar())
            {
                if (Q1Ready && (myhero.CountEnemyHeroesInRange(300) == 0) &&
                    (myhero.CountEnemyHeroesInRange(DemSpells.W2.Range) == 0) &&
                    (myhero.CountEnemyHeroesInRange(DemSpells.E2.Range + 100) == 0) && !myhero.IsFleeing)
                {
                    DemSpells.R.Cast();
                }
            }
            else if (!IsCougar())
            {
                if (((W2Ready && (IsHunted(target) ? myhero.CountEnemyHeroesInRange(DemSpells.W2E.Range) : myhero.CountEnemyHeroesInRange(DemSpells.W2.Range)) >= 1) ||
                                                  (Q2Ready && myhero.CountEnemyHeroesInRange(DemSpells.Q2.Range) >= 1) ||
                                                  (E2Ready && myhero.CountEnemyHeroesInRange(DemSpells.E2.Range) >= 1)))
                {
                    DemSpells.R.Cast();
                }
            }
            return;
        }

        private static void CastW2(AIHeroClient target)
        {
            var targeti = TargetSelector.GetTarget(700);
            if (target.HasBuff("NidaleePassiveHunted"))
            {

                DemSpells.W2E.Cast(targeti.Position);
            }
            if (DemSpells.W2.IsReady())
            {
                switch (IsHunted(targeti))
                {
                    case true:
                        if (target.IsValidTarget(DemSpells.W2E.Range))
                        {
                            var wpred = DemSpells.W2E.GetPrediction(targeti);

                            DemSpells.W2E.Cast(targeti.Position);
                        }
                        break;
                    case false:
                        if (targeti.IsValidTarget(DemSpells.W2E.Range))
                        {

                            DemSpells.W2.Cast(targeti.Position);
                        }
                        break;
                }
            }
            return;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1600, DamageType.Physical);

            if (target != null)
            {
                if (IsCougar())
                {
                    if (check(combo, "CQ2") && DemSpells.Q2.CanCast(target) && DemSpells.Q2.Cast())
                    {
                        return;
                    }

                    if (check(combo, "CW2") && DemSpells.W2.CanCast(target))
                    {
                        CastW2(target);
                    }

                    if (check(combo, "CE2") && DemSpells.E2.CanCast(target))
                    {
                        DemSpells.E2.Cast(target.Position);
                        return;
                    }
                }
                else if (!IsCougar())
                {
                    if (check(combo, "CQ1") && DemSpells.Q1.CanCast(target))
                    {
                        var qpred = DemSpells.Q1.GetPrediction(target);

                        DemSpells.Q1.Cast(qpred.CastPosition); return;
                    }

                    if (check(combo, "CW1") && DemSpells.W1.CanCast(target))
                    {
                        var wpred = DemSpells.W1.GetPrediction(target);

                        DemSpells.W1.Cast(wpred.CastPosition); return;
                    }
                }

                if (check(combo, "CR") && DemSpells.R.IsReady())
                {
                    RLogic(target);
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(1600, DamageType.Physical);

            if (target != null)
            {
                if (IsCougar())
                {
                    if (check(harass, "HQ2") && DemSpells.Q2.CanCast(target) && DemSpells.Q2.Cast())
                    {
                        return;
                    }

                    if (check(harass, "HW2") && DemSpells.W2.CanCast(target))
                    {
                        CastW2(target);
                    }

                    if (check(harass, "HE2") && DemSpells.E2.CanCast(target))
                    {
                        if (target.Health + target.AllShield <= DemSpells.E2.Range && DemSpells.E2.Cast(target.Position))
                        {
                            return;
                        }
                    }
                }
                else if (!IsCougar())
                {
                    if (check(harass, "HQ1") && DemSpells.Q1.CanCast(target))
                    {
                        var qpred = DemSpells.Q1.GetPrediction(target);

                        DemSpells.Q1.Cast(qpred.CastPosition); return;
                    }

                    if (check(harass, "HW1") && DemSpells.W1.CanCast(target))
                    {
                        var wpred = DemSpells.W1.GetPrediction(target);

                        DemSpells.W1.Cast(wpred.CastPosition); return;
                    }
                }

                if (check(harass, "HR") && DemSpells.R.IsReady())
                {
                    RLogic(target);
                }
            }
        }

        private static void Laneclear()
        {
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(DemSpells.Q1.Range) && e.IsMinion())
                            .Cast<AIBaseClient>().ToList();

            if (minions != null)
            {
                if (!IsCougar())
                {
                    if (check(laneclear, "LQ1") && DemSpells.Q1.IsReady())
                    {
                        foreach (var minion in minions.Where(x => x.IsValidTarget(DemSpells.Q1.Range) && x.Health > 40).OrderBy(x => x.Distance(myhero.Position)))
                        {
                            var qpred = DemSpells.Q1.GetPrediction(minion);

                            DemSpells.Q1.Cast(qpred.CastPosition);
                        }
                    }
                }
                else if (IsCougar())
                {
                    if (check(laneclear, "LQ2") && DemSpells.Q2.IsReady())
                    {
                        if (minions.Where(x => x.IsValidTarget(DemSpells.Q2.Range) && x.Health > 30).Count() >= MenuSlider(laneclear, "LQ2MIN")) DemSpells.Q2.Cast();
                    }

                    if (check(laneclear, "LW2") && DemSpells.W2.IsReady())
                    {
                        foreach (var minion in minions.Where(x => x.IsValidTarget(DemSpells.W2.Range)))
                            DemSpells.W2.Cast(minion);
                    }

                    if (check(laneclear, "LE2") && DemSpells.E2.IsReady())
                    {
                        foreach (var minion in minions.Where(x => x.IsValidTarget(DemSpells.E2.Range)))
                        {
                            var epred = DemSpells.E2.GetPrediction(minion);

                            DemSpells.E2.Cast(minion);
                        }
                    }
                }

                if (check(laneclear, "LAUTOR") && DemSpells.R.IsReady())
                {
                    if (IsCougar())
                    {
                        if (Q1Ready && (minions.Where(x => x.IsValidTarget(DemSpells.Q2.Range)).Count() == 0) &&
                            (minions.Where(x => x.IsValidTarget(DemSpells.W2.Range)).Count() == 0) &&
                            (minions.Where(x => x.IsValidTarget(DemSpells.E2.Range)).Count() == 0) && !myhero.IsFleeing)
                        {
                            DemSpells.R.Cast();
                        }
                    }
                    if (!IsCougar())
                    {
                        if (((W2Ready && minions.Where(x => x.IsValidTarget(DemSpells.W2.Range)).Count() >= 1) ||
                                         (Q2Ready && minions.Where(x => x.IsValidTarget(DemSpells.Q2.Range)).Count() >= 1) ||
                                         (E2Ready && minions.Where(x => x.IsValidTarget(DemSpells.E2.Range)).Count() >= 1)))
                        {
                            DemSpells.R.Cast();
                        }
                    }
                }
            }
        }

        private static void Jungleclear()
        {
            var jungleMonsters = GameObjects.Jungle.Where(j => j.IsValidTarget(1600)).FirstOrDefault(j => j.IsValidTarget(1600));
            var Monsters = GameObjects.Jungle.Where(e => e.IsValidTarget(1600) && e.IsMinion())
                           .Cast<AIBaseClient>().ToList();
            if (jungleMonsters.HasBuff("NidaleePassiveHunted"))
            {
            }
            if (Monsters != null)
            {
                if (!IsCougar())
                {
                    if (check(jungleclear, "JQ1") && DemSpells.Q1.IsReady())
                    {

                        DemSpells.Q1.Cast(jungleMonsters.Position);
                    }
                }

                else if (IsCougar())
                {
                    if (check(jungleclear, "JQ2") && DemSpells.Q2.IsReady())
                    {
                        if (Monsters.Where(x => x.IsValidTarget(DemSpells.Q2.Range) && x.Health > 30).Count() >= MenuSlider(jungleclear, "JQ2MIN")) ;
                        DemSpells.Q2.Cast();
                    }

                    if (check(jungleclear, "JW2") && DemSpells.W2.IsReady())
                    {
                        var pred = GameObjects.Jungle.Where(e => e.IsValidTarget(1600) && e.IsJungle());
                        var minions = GameObjects.Jungle.Where(e => e.IsValidTarget(1600) && e.IsJungle())
                           .Cast<AIBaseClient>().ToList();

                        var qFarmLocation = DemSpells.Q1.GetLineFarmLocation(minions, DemSpells.Q1.Width);
                        if (minions.Count >= MenuSlider(jungleclear, "JW2MIN")) DemSpells.W2.Cast(qFarmLocation.Position);
                    }

                    if (check(jungleclear, "JE2") && DemSpells.E2.IsReady())
                    {

                        DemSpells.E2.Cast(jungleMonsters);

                    }
                }


                if (DemSpells.R.IsReady())
                {
                    if (IsCougar())
                    {
                        if (Q1Ready && (Monsters.Where(x => x.IsValidTarget(DemSpells.Q2.Range)).Count() == 0) &&
                            (Monsters.Where(x => x.IsValidTarget(DemSpells.W2.Range)).Count() == 0) &&
                            (Monsters.Where(x => x.IsValidTarget(DemSpells.E2.Range)).Count() == 0) && !myhero.IsFleeing)
                        {
                            DemSpells.R.Cast();
                        }
                    }
                    if (!IsCougar())
                    {
                        if (((DemSpells.Q1.IsReady() == false)))
                        {
                            DemSpells.R.Cast();
                        }
                    }
                }
            }
        }

        private static void Misc()
        {
            var target = TargetSelector.GetTarget(1600, DamageType.Magical);

            if (target != null && target.IsValidTarget() && !target.IsInvulnerable)
            {
                if (check(misc, "ksQ") && !IsCougar() && DemSpells.Q1.IsReady() && target.IsValidTarget(DemSpells.Q1.Range) && QDamage(target) > target.Health)
                {
                    var qpred = DemSpells.Q1.GetPrediction(target);

                    DemSpells.Q1.Cast(qpred.CastPosition);
                }

                if (ignt != null && check(misc, "autoign") && ignt.IsReady() && target.IsValidTarget(ignt.Range) &&
                    myhero.GetSummonerSpellDamage(target, SummonerSpell.Ignite) > target.Health)
                {
                    ignt.Cast(target);
                }
            }

            if (key(misc, "EKEY") && !IsCougar() && DemSpells.E1.IsReady() && myhero.ManaPercent >= MenuSlider(misc, "EMINM") && !myhero.IsRecalling())
            {
                var ClosestAlly = GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(DemSpells.E1.Range) && x.HealthPercent <= MenuSlider(misc, "EMINA"))
                                                                     .OrderBy(x => x.Health)
                                                                     .FirstOrDefault();
                switch (comb(misc, "EMODE"))
                {
                    case 0:
                        if (myhero.HealthPercent <= MenuSlider(misc, "EMINH"))
                        {
                            DemSpells.E1.Cast(myhero);
                        }
                        break;
                    case 1:
                        if (ClosestAlly != null) DemSpells.E1.Cast(ClosestAlly.Position);
                        break;
                    case 2:
                        if (ClosestAlly != null)
                        {
                            switch (myhero.Health > ClosestAlly.Health)
                            {
                                case true:
                                    DemSpells.E1.Cast(ClosestAlly.Position);
                                    break;
                                case false:
                                    if (myhero.HealthPercent <= MenuSlider(misc, "EMINH") && myhero.CountEnemyHeroesInRange(1000) >= MenuSlider(misc, "EMINE"))
                                    {
                                        DemSpells.E1.Cast(myhero.Position);
                                    }
                                    break;
                            }
                        }
                        else goto case 0;
                        break;
                }
            }

            if (check(misc, "AUTOPOT") && (!myhero.HasBuff("RegenerationPotion") && !myhero.HasBuff("ItemMiniRegenPotion") && !myhero.HasBuff("ItemCrystalFlask")) &&
                myhero.HealthPercent <= MenuSlider(misc, "POTMIN"))
            {
                /* if (Item.HasItem(Potion.Id) && Item.CanUseItem(Potion.Id)) Potion.Cast();

                 else if (Item.HasItem(Biscuit.Id) && Item.CanUseItem(Biscuit.Id)) Biscuit.Cast();

                 else if (Item.HasItem(RPotion.Id) && Item.CanUseItem(RPotion.Id)) RPotion.Cast();*/
            }

            if (smite != null && smite.IsReady() && key(jungleclear, "SMITEKEY"))
            {
                var Monsters = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(500) && e.IsMinion())
                .Cast<AIBaseClient>().ToList();
                var qFarmLocation = DemSpells.W1.GetLineFarmLocation(Monsters, DemSpells.W1.Width);



                if (Monsters != null)
                {
                    foreach (var monster in Monsters)
                    {
                        var SmiteDamage = myhero.GetSummonerSpellDamage(monster, SummonerSpell.Smite);

                        if (smite.CanCast(monster) && monster.Health < SmiteDamage && smite.Cast(monster) == CastStates.SuccessfullyCasted) return;
                    }
                }
            }

        }

        private static void OnDraw(EventArgs args)
        {
            if (myhero.IsDead || check(draw, "nodraw")) return;

            if (check(draw, "drawQ") && myhero.Spellbook.GetSpell(SpellSlot.Q).Level > 0 && !myhero.IsDead && !check(draw, "nodraw"))
            {
                if (IsCougar())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, DemSpells.Q2.Range, Color.Gold, 1);
                }

                else if (!check(draw, "drawonlyrdy"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, DemSpells.Q1.Range, Color.Gold, 1);
                }

            }

            if (check(draw, "drawW") && myhero.Spellbook.GetSpell(SpellSlot.W).Level > 0 && !myhero.IsDead && !check(draw, "nodraw"))
            {
                if (IsCougar())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, DemSpells.W2.Range, Color.Gold, 1);
                }

                else if (!check(draw, "drawonlyrdy"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, DemSpells.W1.Range, Color.Gold, 1);
                }
            }

            if (check(draw, "drawE") && myhero.Spellbook.GetSpell(SpellSlot.E).Level > 0 && !myhero.IsDead && !check(draw, "nodraw"))
            {
                if (IsCougar())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, DemSpells.E2.Range, Color.Gold, 1);
                }

                else if (!check(draw, "drawonlyrdy"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, DemSpells.E1.Range, Color.Gold, 1);
                }
            }

            if (check(draw, "DRAWHEAL"))
            {
                Drawing.DrawText(Drawing.WorldToScreen(myhero.Position).X - 50,
                                 Drawing.WorldToScreen(myhero.Position).Y + 10,
                                 Color.White,
                                 "Auto Healing: ");
                Drawing.DrawText(Drawing.WorldToScreen(myhero.Position).X + 37,
                                 Drawing.WorldToScreen(myhero.Position).Y + 10,
                                 key(misc, "EKEY") ? Color.Green : Color.Red,
                                 key(misc, "EKEY") ? "ON" : "OFF");
            }

            /*   foreach (var enemy in EntityManager.Heroes.Enemies)
               {
                   if (check(draw, "drawkillable") && !check(draw, "nodraw") && enemy.IsVisible &&
                       enemy.IsHPBarRendered && !enemy.IsDead && ComboDamage(enemy) > enemy.Health)
                   {
                       Drawing.DrawText(Drawing.WorldToScreen(enemy.Position).X,
                                        Drawing.WorldToScreen(enemy.Position).Y - 30,
                                        Color.Green, "Killable With Combo");
                   }
                   else if (check(draw, "drawkillable") && !check(draw, "nodraw") && enemy.IsVisible &&
                            enemy.IsHPBarRendered && !enemy.IsDead && ignt != null
                            ComboDamage(enemy) + myhero.GetSummonerSpellDamage(enemy, SummonerSpell.Ignite) > enemy.Health)
                   {
                       Drawing.DrawText(Drawing.WorldToScreen(enemy.Position).X, Drawing.WorldToScreen(enemy.Position).Y - 30, Color.Green, "Combo + Ignite");
                   }
               }*/
        }

        private static void OnProcess(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (IsCougar())
                {
                    case false:
                        switch (args.Slot)
                        {
                            case SpellSlot.Q:
                                Q1Ready = false;
                                if (Q1Ready = true)
                                {
                                    myhero.Spellbook.GetSpell(SpellSlot.Q);
                                }
                                break;
                            case SpellSlot.W:
                                W1Ready = false;
                                if (W1Ready = true)
                                {
                                    myhero.Spellbook.GetSpell(SpellSlot.W);
                                }
                                break;
                        }
                        break;
                    case true:
                        switch (args.Slot)
                        {
                            case SpellSlot.Q:
                                Q2Ready = false;
                                if (Q2Ready = true)
                                {
                                    myhero.Spellbook.GetSpell(SpellSlot.Q);
                                }
                                break;
                            case SpellSlot.W:
                                W2Ready = false;
                                if (W2Ready = true)
                                {
                                    myhero.Spellbook.GetSpell(SpellSlot.W);
                                }
                                break;
                            case SpellSlot.E:
                                E2Ready = false;
                                if (E2Ready = true)
                                {
                                    myhero.Spellbook.GetSpell(SpellSlot.E);
                                }
                                break;
                        }
                        break;
                }
            }
        }

        public static void DatMenu()
        {
            menu = new Menu("T7 " + ChampionName, ChampionName.ToLower());
            combo = new Menu("Combo", "combo");
            harass = new Menu("Harass", "harass");
            laneclear = new Menu("Laneclear", "lclear");
            jungleclear = new Menu("Jungleclear", "jclear");
            draw = new Menu("Drawings", "draw");
            misc = new Menu("Misc", "misc");
            pred = new Menu("Prediction", "pred");
            flee = new Menu("Fleee", "Flee");
            var Menumalp = new Menu("Nidalee", "Nidalee", true);
            combo.Add(new MenuSeparator("Spells", "Spells"));
            combo.Add(new MenuSeparator("Human Form", "Human Form"));
            combo.Add(new MenuBool("CQ1", "Use Q"));
            combo.Add(new MenuBool("CW1", "Use W", false));
            combo.Add(new MenuSeparator("Cougar Form", "Cougar Form"));
            combo.Add(new MenuBool("CQ2", "Use Q"));
            combo.Add(new MenuBool("CW2", "Use W"));
            combo.Add(new MenuBool("CE2", "Use E"));
            combo.Add(new MenuBool("CR", "Auto Switch Forms (R)"));
            Menumalp.Add(combo);
            flee.Add(new MenuKeyBind("floy", "Flee", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            harass.Add(new MenuSeparator("Spells", "Spells"));
            harass.Add(new MenuSeparator("Human Form", "Human Form"));
            harass.Add(new MenuBool("HQ1", "Use Q", false));
            harass.Add(new MenuSeparator("Cougar Form", "Cougar Form"));
            harass.Add(new MenuBool("HQ2", "Use Q", false));
            harass.Add(new MenuBool("HW2", "Use W", false));
            harass.Add(new MenuBool("HE2", "Use E", false));
            harass.Add(new MenuBool("HR", "Auto Switch Forms (R)", false));
            harass.Add(new MenuSlider("HMIN", "Min Mana % To Harass", 50, 0, 100));
            Menumalp.Add(harass);
            laneclear.Add(new MenuSeparator("Spells", "Spells"));
            laneclear.Add(new MenuSeparator("Human Form", "Human Form"));
            laneclear.Add(new MenuBool("LQ1", "Use Q", false));
            laneclear.Add(new MenuList("LQ1MODE", "Human Q Mode", new[] { "Big Minions", "All Minions" }) { Index = 0 });
            laneclear.Add(new MenuSeparator("Cougar Form", "Cougar Form"));
            laneclear.Add(new MenuBool("LQ2", "Use Q", false));
            laneclear.Add(new MenuSlider("LQ2MIN", "Min Minions For Q", 1, 1, 4));
            laneclear.Add(new MenuBool("LW2", "Use W", false));
            laneclear.Add(new MenuSlider("LW2MIN", "Min Minions To Hit With W", 1, 1, 10));
            laneclear.Add(new MenuBool("LE2", "Use E", false));
            laneclear.Add(new MenuList("LE2MODE", "Cougar E Mode", new[] { "Big MInions", "All Minions" }) { Index = 0 });
            laneclear.Add(new MenuSeparator("R Usage", "R Usage"));
            laneclear.Add(new MenuBool("LAUTOR", "Auto Switch Forms (R)", false));
            laneclear.Add(new MenuSlider("LMIN", "Min Mana % To Laneclear", 50, 0, 100));
            Menumalp.Add(laneclear);
            jungleclear.Add(new MenuSeparator("Spells", "Spells"));
            jungleclear.Add(new MenuSeparator("Human Form", "Human Form"));
            jungleclear.Add(new MenuBool("JQ1", "Use Q", false));
            jungleclear.Add(new MenuList("JQ1MODE", "Human Q Mode", new[] { "Big Monsters", "All Monsters" }) { Index = 0 });
            jungleclear.Add(new MenuSeparator("Cougar Form", "Cougar Form"));
            jungleclear.Add(new MenuBool("JQ2", "Use Q", false));
            jungleclear.Add(new MenuSlider("JQ2MIN", "Min Monsters For Q", 1, 1, 4));
            jungleclear.Add(new MenuBool("JW2", "Use W", false));
            jungleclear.Add(new MenuSlider("JW2MIN", "Min Monsters To Hit With W", 1, 1, 4));
            jungleclear.Add(new MenuBool("JE2", "Use E", false));
            jungleclear.Add(new MenuList("JE2MODE", "Cougar E Mode", new[] { "Big Monsters", "All Monsters" }) { Index = 0 });
            jungleclear.Add(new MenuSeparator("R Usage", "R Usage"));
            jungleclear.Add(new MenuBool("JAUTOR", "Auto Switch Forms (R)", false));
            jungleclear.Add(new MenuSeparator("Smite", "Smite"));
            jungleclear.Add(new MenuKeyBind("SMITEKEY", "Auto-Smite Key", System.Windows.Forms.Keys.S, KeyBindType.Toggle));
            jungleclear.Add(new MenuSeparator("(Smite Will Target On Big Monsters Like Blue, Red, Dragon etc.)", "(Smite Will Target On Big Monsters Like Blue, Red, Dragon etc.)"));
            jungleclear.Add(new MenuSlider("JMIN", "Min Mana % To Jungleclear", 50, 0, 100));
            Menumalp.Add(jungleclear);
            draw.Add(new MenuBool("nodraw", "Disable All Drawings", false));
            draw.Add(new MenuBool("drawQ", "Draw Q Range"));
            draw.Add(new MenuBool("drawW", "Draw W Range"));
            draw.Add(new MenuBool("drawE", "Draw E Range"));
            draw.Add(new MenuBool("drawonlyrdy", "Draw Only Ready Spells", false));
            //draw.Add("drawkillable", "Draw Killable Enemies"));
            draw.Add(new MenuBool("DRAWHEAL", "Draw Auto Healing Status"));
            //draw.AddSeparator();
            //draw.Add("DRAWMODE", new MenuList("Which Spells To Draw?", 2, "Human Spells Only", "Cougar Spells Only", "Both Form Spells"));
            Menumalp.Add(draw);
            misc.Add(new MenuSeparator("Auto Healing (E)", "Auto Healing (E)"));
            misc.Add(new MenuKeyBind("EKEY", "Auto Heal Hotkey", System.Windows.Forms.Keys.H, KeyBindType.Toggle));
            misc.Add(new MenuList("EMODE", "Healing Mode", new[] { "Only Self", "Only Ally", "Self And Ally" }) { Index = 0 });
            misc.Add(new MenuSeparator("Self", "Self"));
            misc.Add(new MenuSlider("EMINH", "Min Self Health %", 25, 1, 100));
            misc.Add(new MenuSeparator("Ally", "Ally"));
            misc.Add(new MenuSlider("EMINA", "Min Ally Health %", 25, 1, 100));
            misc.Add(new MenuSeparator("Mana", "Mana"));
            misc.Add(new MenuSlider("EMINM", "Min Mana % To Auto Health", 50, 1, 100));
            misc.Add(new MenuSeparator("_____________________________________________________________________________", "_____________________________________________________________________________"));
            misc.Add(new MenuBool("W2FLEE", "Use Cougar W To Flee", false));
            misc.Add(new MenuSeparator("Killsteal", "Killsteal"));
            misc.Add(new MenuBool("ksQ", "Killsteal with Q", false));
            misc.Add(new MenuBool("autoign", "Auto Ignite If Killable"));
            misc.Add(new MenuSeparator("Auto Potion", "Auto Potion"));
            misc.Add(new MenuBool("AUTOPOT", "Activate Auto Potion"));
            misc.Add(new MenuSlider("POTMIN", "Min Health % To Active Potion", 25, 1, 100));
            misc.Add(new MenuSeparator("Auto Level Up Spells", "Auto Level Up Spells"));
            misc.Add(new MenuBool("autolevel", "Activate Auto Level Up Spells"));
            Menumalp.Add(misc);
            pred.Add(new MenuSeparator("Prediction", "Prediction"));
            pred.Add(new MenuSeparator("Q :", "Q :"));
            pred.Add(new MenuSlider("QPred", "Select % Hitchance", 90, 1, 100));
            pred.Add(new MenuSeparator("Human W :", "Human W :"));
            pred.Add(new MenuSlider("W1Pred", "Select % Hitchance", 90, 1, 100));
            pred.Add(new MenuSeparator("Cougar W :", "Cougar W :"));
            pred.Add(new MenuSlider("W2Pred", "Select % Hitchance", 90, 1, 100));
            pred.Add(new MenuSeparator("E :", "E :"));
            pred.Add(new MenuSlider("EPred", "Select % Hitchance", 90, 1, 100));
            Menumalp.Add(pred);
            Menumalp.Attach();
        }

    }

    public static class DemSpells
    {
        public static Spell Q1 { get; private set; }
        public static Spell Q2 { get; private set; }
        public static Spell W1 { get; private set; }
        public static Spell W2 { get; private set; }
        public static Spell W2E { get; private set; }
        public static Spell E1 { get; private set; }
        public static Spell E2 { get; private set; }
        public static Spell R { get; private set; }

        static DemSpells()
        {
            Q1 = new Spell(SpellSlot.Q, 1500);//, SkillShotType.Linear, 500, 1300, 80);
            Q2 = new Spell(SpellSlot.Q, 200);
            W1 = new Spell(SpellSlot.W, 900);//, SkillShotType.Circular, 500, 1450, 80);
            W2 = new Spell(SpellSlot.W, 375);//, SkillShotType.Circular, 500, int.MaxValue, 210);
            W2E = new Spell(SpellSlot.W, 750);//, SkillShotType.Circular, 500, int.MaxValue, 210);
            E1 = new Spell(SpellSlot.E, 600);
            E2 = new Spell(SpellSlot.E, 300);//, SkillShotType.Cone, 500, int.MaxValue,(int)(15.00 * Math.PI / 180.00));
            R = new Spell(SpellSlot.R);
        }
    }
}