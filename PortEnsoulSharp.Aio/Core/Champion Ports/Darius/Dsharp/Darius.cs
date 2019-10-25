using System;
using System.Globalization;
using System.Linq;
using Darius.Orb;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using SharpDX;
using Color = System.Drawing.Color;
namespace Darius
{
    internal static class Darius
    {
        public const string ChampName = "Darius";

        public static int passiveCounter;
        public static Menu QMenu { get; private set; }

        public static Menu WMenu { get; private set; }

        public static Menu EMenu { get; private set; }

        public static Menu RMenu { get; private set; }

        public static Menu ManaMenu { get; private set; }

        public static Menu ItemsMenu { get; private set; }

        public static Menu KillStealMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        private static Menu menuIni;
        public static Spell Q { get; private set; }


        public static Spell W { get; private set; }

        public static Spell E { get; private set; }

        public static Spell R { get; private set; }

        public static HpBarIndicator Hpi = new HpBarIndicator();

        public static Spell Ignite;

        public static void Execute()
        {
            menuIni = new Menu("Darius", "Darius#", true);
            menuIni.Add(new MenuSeparator("note", "Darius The Dank Memes Master!"));
            menuIni.Add(new MenuSeparator("setting", "Global Settings"));
            //menuIni.Add("Items", new CheckBox("Use Items?"));
            menuIni.Add(new MenuBool("Combo", "Use Combo?"));
            menuIni.Add(new MenuBool("Harass", "Use Harass?"));
            menuIni.Add(new MenuBool("Clear", "Use Clear?"));
            menuIni.Add(new MenuBool("Drawings", "Use Drawings?"));
            menuIni.Add(new MenuBool("KillSteal", "Use KillSteal?"));

            QMenu = menuIni.Add(new Menu("qset", "Q Settings"));
            QMenu.Add(new MenuSeparator("qset1", "Q Settings"));
            QMenu.Add(new MenuBool("Combo", "Q Combo"));
            QMenu.Add(new MenuBool("Harass", "Q Harass"));
            QMenu.Add(new MenuSeparator("qset2", "Q LaneClear Settings"));
            QMenu.Add(new MenuBool("Clear", "Q LaneClear"));
            QMenu.Add(new MenuSlider("Qlc", "Q On Hit Minions >=", 3, 1, 10));
            QMenu.Add(new MenuSeparator("extset", "Extra Settings"));
            QMenu.Add(new MenuBool("QE", "Always Q Before E", false));
            QMenu.Add(new MenuBool("Stick", "Stick to Target while Casting Q"));
            QMenu.Add(new MenuBool("QAA", "Use Q if AA is in Cooldown", false));
            QMenu.Add(new MenuBool("range", "Dont Cast Q when Enemy in AA range", false));
            QMenu.Add(new MenuBool("Flee", "Q On Flee (Ignores Stick to target)"));
            QMenu.Add(new MenuSlider("QFlee", "Cast Q flee When HP is below %", 90));
            QMenu.Add(new MenuBool("Qaoe", "Auto Q AoE"));
            QMenu.Add(new MenuSlider("Qhit", "Q Aoe Hit >=", 3, 1, 5));

            WMenu = menuIni.Add(new Menu("wset", "W Settings"));
            WMenu.Add(new MenuSeparator("w", "W Settings"));
            WMenu.Add(new MenuBool("Combo", "W Combo"));
            WMenu.Add(new MenuBool("Harass", "W Harass"));
            WMenu.Add(new MenuBool("Clear", "W LaneClear"));
            WMenu.Add(new MenuSeparator("w2", "Extra Settings"));
            WMenu.Add(new MenuBool("AAr", "W AA Reset"));

            EMenu = menuIni.Add(new Menu("eset", "E Settings"));
            EMenu.Add(new MenuSeparator("e1", "E Settings"));
            EMenu.Add(new MenuBool("Combo", "E Combo"));
            EMenu.Add(new MenuBool("Harass", "E Harass"));
            EMenu.Add(new MenuSeparator("e2", "Extra Settings"));
            EMenu.Add(new MenuBool("Interrupt", "E To Interrupt"));

            RMenu = menuIni.Add(new Menu("rset", "R Settings"));
            RMenu.Add(new MenuSeparator("r1", "R Settings"));
            RMenu.Add(new MenuBool("Combo", "R Combo Finisher"));
            RMenu.Add(new MenuBool("stack", "Use R On Stacks", false));
            RMenu.Add(new MenuSlider("count", "Cast R On Stacks >=", 5, 0, 5));
            RMenu.Add(new MenuBool("SaveR", "Dont Ult if target killable with AA", false));
            RMenu.Add(new MenuSlider("SR", "Dont Use Ult if target can be kill With X AA", 1, 0, 6));
            RMenu.Add(new MenuKeyBind("semiR", "Semi-Auto R", System.Windows.Forms.Keys.T, KeyBindType.Press));

            KillStealMenu = menuIni.Add(new Menu("ksset", "KillSteal"));
            KillStealMenu.Add(new MenuSeparator("k1", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("Rks", "R KillSteal"));
            if (ObjectManager.Player.Spellbook.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerDot")) != null)
            {
                KillStealMenu.Add(new MenuBool("IGP", "Ignite + Passive Kill"));
                KillStealMenu.Add(new MenuBool("IG", "Ignite Only", false));
                KillStealMenu.Add(new MenuSeparator("k2", "Iginte + Passive takes in account Max Ignite + Passive dmg"));
                Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            }

            ManaMenu = menuIni.Add(new Menu("manaset", "Mana Manager"));
            ManaMenu.Add(new MenuSeparator("hmana", "Harass"));
            ManaMenu.Add(new MenuSlider("harassmana", "Harass Mana %", 75));
            ManaMenu.Add(new MenuSeparator("lclear", "Lane Clear"));
            ManaMenu.Add(new MenuSlider("lanemana", "Lane Clear Mana %", 60));

            DrawMenu = menuIni.Add(new Menu("drawing", "Drawings"));
            DrawMenu.Add(new MenuSeparator("d1", "Drawing Settings"));
            DrawMenu.Add(new MenuBool("Q", "Draw Q"));
            DrawMenu.Add(new MenuBool("W", "Draw W"));
            DrawMenu.Add(new MenuBool("E", "Draw E"));
            DrawMenu.Add(new MenuBool("R", "Draw R"));
            DrawMenu.Add(new MenuSeparator("d2", "Ultimate Drawings"));
            DrawMenu.Add(new MenuBool("DrawD", "Draw R Damage"));
            DrawMenu.Add(new MenuBool("Killable", "Draw Killable"));
            DrawMenu.Add(new MenuBool("Stacks", "Draw Passive Stacks"));
            DrawMenu.Add(new MenuSlider("PPx", "Passive Stacks Position X", 100, 0, 150));
            DrawMenu.Add(new MenuSlider("PPy", "Passive Stacks Position Y", 100, 0, 150));
            DrawMenu.Add(new MenuBool("RHealth", "Draw After R health"));
            DrawMenu.Add(new MenuSlider("RHx", "After R health Position", 135, 0, 150));
            menuIni.Attach();

            Q = new Spell(SpellSlot.Q, 400f);
            W = new Spell(SpellSlot.W, 300f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R, 475f);

            E.SetSkillshot(0.25f, 80f, 1000f, false, SkillshotType.Cone);
            R.SetTargetted(0.5f, 1000f);

            //AIBaseClient.OnBuffGain += AIBaseClient_OnBuffGain;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Orbwalker.OnAction += OnAfterAttack;
            //Orbwalk.OnPostAttack += Orbwalk_OnPostAttack;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Interrupter.OnInterrupterSpell += Interrupter_OnInterrupterSpell;
        }

        private static void OnAfterAttack(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type != OrbwalkerType.AfterAttack)
            {
                return;
            }
            var target = TargetSelector.GetTarget(W.Range, DamageType.True);
            var Wcombo = WMenu["Combo"].GetValue<MenuBool>().Enabled;
            if (args.Target == null || args.Target.IsDead || !args.Target.IsValidTarget() ||
                Orbwalker.ActiveMode == OrbwalkerMode.None || args.Target.Type != GameObjectType.AIHeroClient)
            {
                return;
            }
            if (target != null)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (Wcombo)
                    {
                        if (WMenu["AAr"].GetValue<MenuBool>().Enabled)
                        {

                            if (W.Cast())
                            {
                                Orbwalk.ResetAutoAttack();
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            }
                        }
                    }
                    if (QMenu["Combo"].GetValue<MenuBool>().Enabled && (QMenu["QAA"].GetValue<MenuBool>().Enabled && Q.IsReady()) && !W.IsReady())
                    {
                        Q.Cast();
                    }
                }
            }

        }


        private static void Interrupter_OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (!EMenu.GetValue<MenuBool>("Interrupt").Enabled || sender == null)
            {
                return;
            }

            var pred = E.GetPrediction(sender);
            if (E.IsReady() && sender.IsKillable(E.Range) && sender.IsEnemy && !sender.IsDead)
            {
                E.Cast(pred.CastPosition);
            }
        }




        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen || ObjectManager.Player.IsRecalling())
            {
                return;
            }
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();

                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    Clear();
                    break;
            }
            if (menuIni["KillSteal"].GetValue<MenuBool>().Enabled)
            {
                KillSteal();
            }
            if (QMenu["Flee"].GetValue<MenuBool>().Enabled)
            {
                Flee();
            }
            if (QMenu["Qaoe"].GetValue<MenuBool>().Enabled
                && ObjectManager.Player.CountEnemyHeroesInRange(Q.Range) >= QMenu["Qhit"].GetValue<MenuSlider>().Value)
            {
                Q.Cast();
            }
        }


        private static void Clear()
        {
            var lanemana = ManaMenu["lanemana"].GetValue<MenuSlider>().Value;
            var Qclear = QMenu["Clear"].GetValue<MenuBool>().Enabled && Q.IsReady();
            var Wclear = WMenu["Clear"].GetValue<MenuBool>().Enabled && W.IsReady();

            var allMinions = GameObjects.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (ObjectManager.Player.ManaPercent >= lanemana)
            {
                if (allMinions == null)
                {
                    return;
                }

                foreach (var minion in allMinions)
                {
                    if (Qclear)
                    {


                        if (allMinions.Any())
                        {
                            if (allMinions.Count > QMenu["Qlc"].GetValue<MenuSlider>().Value)
                            {
                                {
                                    Q.Cast();
                                }
                            }
                        }

                    }

                    if (Wclear)
                    {
                        if (minion.InAutoAttackRange() && minion.IsKillable() && W.IsReady()
                           && ObjectManager.Player.Distance(minion.Position) <= 225f && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W) + ObjectManager.Player.GetAutoAttackDamage(minion)
                            >= minion.Health)
                        {
                            W.Cast();
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            var harassmana = ManaMenu["harassmana"].GetValue<MenuSlider>().Value;
            if (ObjectManager.Player.ManaPercent >= harassmana)
            {
                QCast();
                WCast();
                ECast();
            }
        }

        private static void Combo()
        {
            QCast();
            WCast();
            ECast();
            RCast();
        }

        private static void RCast()
        {
            var SaveR = RMenu["SaveR"].GetValue<MenuBool>().Enabled;
            var SR = RMenu["SR"].GetValue<MenuSlider>().Value;
            var buffcount = RMenu["count"].GetValue<MenuSlider>().Value;
            var rt = TargetSelector.GetTarget(R.Range, DamageType.True);
            var Rcombo = RMenu["Combo"].GetValue<MenuBool>().Enabled && R.IsReady();
            var Rstack = RMenu["stack"].GetValue<MenuBool>().Enabled && R.IsReady();
            var SemiR = RMenu["semiR"].GetValue<MenuKeyBind>().Active && R.IsReady();
            var SemiRtarget =
                GameObjects.EnemyHeroes.Where(x => x.IsKillable(R.Range) && x != null)
                    .OrderByDescending(x => RDmg(x, passiveCounter))
                    .FirstOrDefault();
            if (rt != null)
            {



                if (!rt.HasBuff("kindredrnodeathbuff") && !rt.HasBuff("JudicatorIntervention") && !rt.HasBuff("ChronoShift")
                                    && !rt.HasBuff("UndyingRage") && !rt.IsInvulnerable
                                    && !rt.HasBuff("AatroxPassiveActivate") && !rt.HasBuff("rebirthcooldown"))
                {
                    if (SaveR && ObjectManager.Player.GetAutoAttackDamage(rt) * SR > rt.Health
                        && rt.IsKillable(ObjectManager.Player.GetRealAutoAttackRange()))
                    {
                        return;
                    }

                    if (SemiR)
                    {
                        R.Cast(SemiRtarget);
                    }

                    if (Rcombo)
                    {
                        if (rt.IsKillable(R.Range) && RDmg(rt, passiveCounter) + PassiveDmg(rt, 1) > rt.Health)
                        {
                            R.Cast(rt);
                        }
                    }

                    if (Rstack)
                    {
                        if (rt.IsKillable(R.Range) && rt.GetBuffCount("DariusHemo") >= buffcount)
                        {
                            R.Cast(rt);
                        }
                    }
                }
            }
        }

        private static void ECast()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var Qcombo = QMenu["Combo"].GetValue<MenuBool>().Enabled && Q.IsReady();
            var QE = QMenu["QE"].GetValue<MenuBool>().Enabled;
            var Ecombo = EMenu["Combo"].GetValue<MenuBool>().Enabled && E.IsReady();
            var Eharass = EMenu["Harass"].GetValue<MenuBool>().Enabled && E.IsReady();
            if (target != null)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (Ecombo)
                    {
                        if (QE && Q.IsReady())
                        {
                            return;
                        }

                        if (target.IsKillable(E.Range))
                        {
                            if (Q.IsReady() && target.IsKillable(Q.Range) && Qcombo
                                || !Q.IsReady() && target.IsValidTarget(ObjectManager.Player.GetRealAutoAttackRange()))
                            {
                                return;
                            }

                            var pred = E.GetPrediction(target);
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    if (Eharass)
                    {
                        if (target.IsKillable(E.Range)
                            && ((Q.IsReady() && !target.IsKillable(Q.Range)) || (!Q.IsReady() && target.InAutoAttackRange())))
                        {
                            var pred = E.GetPrediction(target);
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void WCast()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var Wcombo = WMenu["Combo"].GetValue<MenuBool>().Enabled && W.IsReady();
            var Wharass = WMenu["Harass"].GetValue<MenuBool>().Enabled && W.IsReady();
            if (target != null)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (Wcombo)
                    {
                        if (!WMenu["AAr"].GetValue<MenuBool>().Enabled)
                        {
                            if (target.InAutoAttackRange() && W.IsReady())
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                W.Cast();
                            }
                            if (ObjectManager.Player.HasBuff("DariusNoxianTacticsONH"))
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            }
                        }
                    }
                }
                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    if (Wharass)
                    {
                        if (target.IsKillable(W.Range))
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void QCast()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var Qrange = QMenu["range"].GetValue<MenuBool>().Enabled;
            var Qcombo = QMenu["Combo"].GetValue<MenuBool>().Enabled && Q.IsReady();
            var Qharass = QMenu["Harass"].GetValue<MenuBool>().Enabled && Q.IsReady();
            if (target != null)
            {
                if (QMenu["QAA"].GetValue<MenuBool>().Enabled && Q.IsReady() && target.InAutoAttackRange()
    && ObjectManager.Player.CanAttack)
                {
                    return;
                }
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (Qcombo && target.IsKillable(Q.Range))
                    {
                        if (Qrange)
                        {
                            if (!target.InAutoAttackRange() && Q.Range <= 400)
                            {

                                Q.Cast();
                            }
                        }
                        if (!Qrange)
                        {
                            if (Q.Range <= 400)
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                                Q.Cast();
                            }
                        }

                        if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) > target.Health)
                        {
                            Q.Cast();
                        }

                    }
                }
                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    if (Qharass && target.IsKillable(Q.Range))
                    {

                        if (!target.InAutoAttackRange())
                        {

                            Q.Cast();
                        }

                    }
                }
                if (QMenu["Stick"].GetValue<MenuBool>().Enabled)
                {
                    if (target.HasBuff("RumbleDangerZone") && target.IsKillable(Q.Range) && !target.IsUnderEnemyTurret())
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                    }
                }

            }
        }

        private static void Flee()
        {
            var hp = QMenu["QFlee"].GetValue<MenuSlider>().Value;
            var Qflee = QMenu["Flee"].GetValue<MenuBool>().Enabled && Q.IsReady();
            if (Qflee && ObjectManager.Player.HealthPercent < hp)
            {
                if (ObjectManager.Player.CountEnemyHeroesInRange(Q.Range) >= 1)
                {
                    Q.Cast();
                }
            }
        }
        private static void KillSteal()
        {
            var SaveR = RMenu["SaveR"].GetValue<MenuBool>().Enabled;
            var SR = RMenu["SR"].GetValue<MenuSlider>().Value;
            var Rks = KillStealMenu["Rks"].GetValue<MenuBool>().Enabled && R.IsReady();
            var target =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(
                        enemy =>
                        enemy.IsEnemy && enemy.IsKillable(1000) && !enemy.IsDead && !enemy.HasBuff("kindredrnodeathbuff")
                        && !enemy.HasBuff("JudicatorIntervention") && !enemy.HasBuff("ChronoShift") && !enemy.HasBuff("UndyingRage")
                        && !enemy.IsInvulnerable && !enemy.HasBuff("AatroxPassiveActivate")
                        && !enemy.HasBuff("rebirthcooldown"));
            if (target != null)
            {
                if (Rks)
                {
                    if (SaveR && ObjectManager.Player.GetAutoAttackDamage(target) * SR > target.Health
                        && target.IsKillable(ObjectManager.Player.GetRealAutoAttackRange()))
                    {
                        return;
                    }
                    var pred = E.GetPrediction(target);
                    passiveCounter = target.GetBuffCount("DariusHemo") <= 0 ? 0 : target.GetBuffCount("DariusHemo");
                    if (RDmg(target, passiveCounter) + PassiveDmg(target, 1) >= target.Health)
                    {
                        if (target.IsKillable(R.Range))
                        {
                            R.Cast(target);
                        }

                        if (!target.IsKillable(R.Range) && target.IsKillable(E.Range)
                            && ObjectManager.Player.Mana >= (R.Mana + E.Mana))
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                    if (target.IsKillable(R.Range) && RDmg(target, passiveCounter) + PassiveDmg(target, 1) > target.Health)
                    {
                        if (!target.IsKillable(R.Range) && target.IsKillable(E.Range)
                            && ObjectManager.Player.Mana >= (R.Mana + E.Mana))
                        {
                            E.Cast(pred.CastPosition);
                        }

                        R.Cast(target);
                    }
                }
                if (Rks && target.IsKillable(R.Range))
                {
                    return;
                }
                if (ObjectManager.Player.Spellbook.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerDot")) != null)
                {
                    var IG = KillStealMenu["IG"].GetValue<MenuBool>().Enabled;
                    var IGP = KillStealMenu["IGP"].GetValue<MenuBool>().Enabled;
                    if (IGP && target.IsKillable(Ignite.Range)
                        && ObjectManager.Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite)
                        + PassiveDmg(target, target.GetBuffCount("DariusHemo")) > target.Health + target.HPRegenRate)
                    {
                        if (PassiveDmg(target, target.GetBuffCount("DariusHemo")) < target.Health + (target.HPRegenRate * 4)
                           && !target.IsKillable(ObjectManager.Player.GetRealAutoAttackRange()))
                        {
                            Ignite.Cast(target);
                        }

                        if (PassiveDmg(target, target.GetBuffCount("DariusHemo")) < target.Health)
                        {
                            if (target.Health > ObjectManager.Player.Health)
                            {
                                Ignite.Cast(target);
                            }
                        }
                    }
                    if (IG && target.IsKillable(Ignite.Range)
                        && ObjectManager.Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite)
                        > target.Health + (target.HPRegenRate * 4) && !target.IsKillable(ObjectManager.Player.GetRealAutoAttackRange()))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }

        public static double PassiveDmg(AIBaseClient unit, int stackcount)
        {
            if (stackcount < 1)
            {
                stackcount = 1;
            }

            return ObjectManager.Player.CalculateDamage(
                unit,
                DamageType.Physical,
                (9 + ObjectManager.Player.Level) + (float)(0.3 * ObjectManager.Player.FlatPhysicalDamageMod)) * stackcount;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!menuIni.GetValue<MenuBool>("Drawings").Enabled)
            {
                return;
            }
            if (DrawMenu.GetValue<MenuBool>("Q").Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Color.LightYellow : Color.DarkBlue);
            }
            if (DrawMenu.GetValue<MenuBool>("W").Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ? Color.LightYellow : Color.DarkRed);
            }

            if (DrawMenu.GetValue<MenuBool>("E").Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ? Color.LightYellow : Color.DarkRed);
            }

            if (DrawMenu.GetValue<MenuBool>("R").Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, R.IsReady() ? Color.LightYellow : Color.DarkRed);
            }
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.SData.Name.ToLower().Contains("itemtiamatcleave") && sender.IsMe)
            {
                Orbwalk.ResetAutoAttack();
            }
        }

        public static float RDmg(AIBaseClient unit, int stackcount)
        {
            var bonus = stackcount * (new[] { 20, 20, 40, 60 }[R.Level] + (0.15 * ObjectManager.Player.FlatPhysicalDamageMod));

            return
                (float)
                (bonus
                 + ObjectManager.Player.CalculateDamage(
                     unit,
                     DamageType.True,
                     new[] { 100, 100, 200, 300 }[R.Level] + (float)(0.75 * ObjectManager.Player.FlatPhysicalDamageMod)));
        }

        private static void OnEndScene(EventArgs args)
        {
            if (menuIni["Drawings"].GetValue<MenuBool>().Enabled)
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsKillable() && e.IsHPBarRendered))
                {
                    if (DrawMenu["DrawD"].GetValue<MenuBool>().Enabled)
                    {
                        Hpi.unit = enemy;
                        Hpi.drawDmg((int)RDmg(enemy, passiveCounter), Color.Goldenrod);
                    }
                    var hpPos = enemy.HPBarPosition;
                    if (DrawMenu["Killable"].GetValue<MenuBool>().Enabled && enemy.IsVisible)
                    {
                        if (RDmg(enemy, passiveCounter) > enemy.Health)
                        {
                            Drawing.DrawText(hpPos.X + 63f, hpPos.Y - 20f, System.Drawing.Color.FromArgb(255, 255, 255), "DUNK = DEAD");
                        }
                    }
                    if (DrawMenu["Stacks"].GetValue<MenuBool>().Enabled)
                    {
                        if (enemy.GetBuffCount("DariusHemo") > 0)
                        {
                            var endTime = Math.Max(0, enemy.GetBuff("DariusHemo").EndTime - Game.Time);
                            Drawing.DrawText(Drawing.WorldToScreen(enemy.Position) - new Vector2(DrawMenu["PPx"].GetValue<MenuSlider>().Value, DrawMenu["PPy"].GetValue<MenuSlider>().Value),
                                System.Drawing.Color.FromArgb(255, 106, 255),
                                enemy.GetBuffCount("DariusHemo") + " Stacks " + $"{endTime:0.00}");
                        }
                    }
                    if (DrawMenu["RHealth"].GetValue<MenuBool>().Enabled)
                    {
                        Drawing.DrawText(
                            hpPos.X + DrawMenu["RHx"].GetValue<MenuSlider>().Value,
                            hpPos.Y - 20f,
                            System.Drawing.Color.FromArgb(255, 106, 106),
                            Convert.ToString((int)(enemy.Health - RDmg(enemy, passiveCounter)), CultureInfo.CurrentCulture));
                    }
                }

            }
        }

        public static bool IsKillable(this AIBaseClient target, float range)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention")     && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.HasBuff("bansheesveil") && !target.IsDead
                    && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range);
        }

        public static bool IsKillable(this AIBaseClient target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") 
                   && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.HasBuff("bansheesveil") && !target.IsDead
                   && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        public static bool IsKillable(this AIHeroClient target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") 
                   && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable  && !target.HasBuff("bansheesveil") && !target.IsDead
                   && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        public static bool IsKillable(this AIHeroClient target, float range)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") 
                   && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.HaveImmovableBuff() && !target.IsInvulnerable  && !target.HasBuff("bansheesveil") && !target.IsDead
                   && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range);
        }
    }
}