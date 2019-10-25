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
using static EnsoulSharp.SDK.Interrupter;

namespace Yi
{
    class Program
    {
        public static Menu Menu, ComboMenu, Evade, HarassMenu, LaneClearMenu, Items, KillStealMenu, Drawings;
        public static Item Botrk;
        public static Item Bil;
        public static Item Youmuu;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

        public static void MasterYiOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("MasterYi")) return;
            Game.Print("Doctor's Yi Loaded! Ported By DEATHGODX", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            Youmuu = new Item(3142, 10);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var MenuCorki = new Menu("Doctor's Yi", "Yi", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Count Enemies Around"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 2, 1, 5));
            ComboMenu.Add(new MenuSeparator("Use [W] Low HP", "Use [W] Low HP"));
            ComboMenu.Add(new MenuBool("WLowHp", "Use [W] Low Hp"));
            ComboMenu.Add(new MenuSlider("minHealth", "Use [W] My Hp <=", 25));
            ComboMenu.Add(new MenuSeparator("Use [Q] Dodge Spell", "Use [Q] Dodge Spell"));
            ComboMenu.Add(new MenuBool("dodge", "Use [Q] Dodge"));
            ComboMenu.Add(new MenuBool("antiGap", "Use [Q] Anti Gap"));
            ComboMenu.Add(new MenuSlider("delay", "Use [Q] Dodge Delay", 1, 1, 1000));
            MenuCorki.Add(ComboMenu);
            MenuCorki.Attach();
            Evade = new Menu("Spell Dodge Settings", "Evade");
            Evade.Add(new MenuSeparator("Dodge Settings", "Dodge Settings"));
            foreach (var enemies in GameObjects.EnemyHeroes.Where(a => a.Team != ObjectManager.Player.Team))
            {
                Evade.Add(new MenuSeparator(enemies.CharacterName, enemies.CharacterName));
                {
                    foreach (var spell in enemies.Spellbook.Spells.Where(a => a.Slot == SpellSlot.Q || a.Slot == SpellSlot.W || a.Slot == SpellSlot.E || a.Slot == SpellSlot.R))
                    {
                        if (spell.Slot == SpellSlot.Q)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                        else if (spell.Slot == SpellSlot.W)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                        else if (spell.Slot == SpellSlot.E)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                        else if (spell.Slot == SpellSlot.R)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                    }
                }
            }

            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSlider("ManaQ", "Mana Harass", 40));
            MenuCorki.Add(HarassMenu);
            LaneClearMenu = new Menu("Clear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Clear Settings", "Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear"));
            LaneClearMenu.Add(new MenuSlider("mine", "Min x Minions Killable Use Q", 3, 1, 4));
            LaneClearMenu.Add(new MenuBool("ELC", "Use [E] LaneClear"));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana LaneClear", 50));
            LaneClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            LaneClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            LaneClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            LaneClearMenu.Add(new MenuSlider("MnJungle", "Mana JungleClear", 30));
            MenuCorki.Add(LaneClearMenu);
            Items = new Menu("Items Settings", "Items");
            Items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Items.Add(new MenuBool("you", "Use [Youmuu]"));
            Items.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            Items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuCorki.Add(Items);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuCorki.Add(KillStealMenu);
            Drawings = new Menu("Draw Settings", "Draw");
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "[Q] Range"));
            MenuCorki.Add(Drawings);
            MenuCorki.Add(Evade);
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Orbwalker.OnAction += ResetAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;
            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("Meditate"))
            {
                Orbwalker.MovementState = false;
                Orbwalker.AttackState = false;
            }
            else
            {
                if (ObjectManager.Player.HasBuff("Meditate") == false && W.IsReady() == false)
                {
                    Orbwalker.MovementState = true;
                    Orbwalker.AttackState = true;
                }
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }

            KillSteal();
            Item();
        }

        public static void Item()
        {
            var item = Items["BOTRK"].GetValue<MenuBool>().Enabled;
            var yous = Items["you"].GetValue<MenuBool>().Enabled;
            var Minhp = Items["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = Items["ihpp"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(475) && !e.IsDead))
            {
                if (item && Bil.IsReady && Bil.IsOwned() && Bil.IsInRange(target))
                {
                    Bil.Cast(target);
                }
                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }
                if (yous && Youmuu.IsReady && Youmuu.IsOwned() && _Player.Distance(target) < 325 && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    Youmuu.Cast();
                }
            }
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (ComboMenu["antiGap"].GetValue<MenuBool>().Enabled && Q.IsReady() && sender.Distance(_Player) < 325)
            {
                Q.Cast(sender);
            }
        }

        public static void Combo()
        {
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && (_Player.Distance(target) > 250 || target.IsDashing() || ObjectManager.Player.HealthPercent <= 20))
                {
                    Q.Cast(target);
                }

                if (useE && E.IsReady() && _Player.Distance(target) <= 250)
                {
                    E.Cast();
                }

                if (useR && R.IsReady() && (_Player.Position.CountEnemyHeroesInRange(675) >= MinR || ObjectManager.Player.HealthPercent <= 30))
                {
                    R.Cast();
                }

                if (useW && ObjectManager.Player.HasBuff("Meditate"))
                {
                    if (_Player.Distance(target) <= ObjectManager.Player.GetRealAutoAttackRange(target))
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                    else
                    {
                        if (useQ && Q.IsReady())
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }
        }


        public static void JungleClear()
        {
            var useQ = LaneClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["MnJungle"].GetValue<MenuSlider>().Value;
            var monster = GameObjects.Jungle.Where(j => j.IsValidTarget(Q.Range)).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (monster != null)
            {
                if (useQ && Q.IsReady() && _Player.ManaPercent > mana && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(225))
                {
                    E.Cast();
                }
            }
        }

        public static void LaneClear()
        {
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["ELC"].GetValue<MenuBool>().Enabled;
            var minE = LaneClearMenu["mine"].GetValue<MenuSlider>().Value;
            var minion = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range) && e.IsMinion())
                            .Cast<AIBaseClient>().ToList();

            foreach (var minions in minion)
            {
                if (useQ && Q.IsReady() && ObjectManager.Player.ManaPercent >= mana)
                {
                    int ECal = minion.Where(e => e.Distance(_Player.Position) <= Q.Range && ObjectManager.Player.GetSpellDamage(e, SpellSlot.Q) >= e.Health).Count(); ;
                    if (ECal >= minE)
                    {
                        Q.Cast(minions);
                    }
                }

                if (useE && E.IsReady() && minions.IsValidTarget(200))
                {
                    E.Cast();
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(300, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var useQC = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useW && W.IsReady() && ObjectManager.Player.Distance(target) <= ObjectManager.Player.GetRealAutoAttackRange() - 50 && _Player.ManaPercent > mana && Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    W.Cast();
                    Orbwalker.CanAttack();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if (useQC && W.IsReady() && ObjectManager.Player.Distance(target) <= ObjectManager.Player.GetRealAutoAttackRange() - 50 && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    W.Cast();
                    Orbwalker.CanAttack();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }
        }

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && _Player.ManaPercent > mana && (target.IsDashing() || _Player.Distance(target) > 250))
                {
                    Q.Cast(target);
                }
                if (useE && E.IsReady() && _Player.Distance(target) < 275)
                {
                    E.Cast();
                }
            }
        }

        public static void Flee()
        {
            var Enemies = GameObjects.EnemyHeroes.FirstOrDefault(e => e.IsValidTarget(Q.Range));
            var minions = GameObjects.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            var monster = GameObjects.Jungle.FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (Enemies != null && Q.IsReady())
            {
                if (Enemies.InAutoAttackRange(250))
                {
                    Q.Cast(Enemies);
                }
            }

            else if (minions != null && Q.IsReady())
            {
                if (minions.InAutoAttackRange(250))
                {
                    Q.Cast(minions);
                }
            }

            else if (monster != null && Q.IsReady())
            {
                if (monster.IsValidTarget(250))
                {
                    Q.Cast(monster);
                }
            }
        }
        public static void QEvade()
        {
            var Enemies = GameObjects.EnemyHeroes.FirstOrDefault(e => e.IsValidTarget(Q.Range));
            var minions = GameObjects.AllyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            if (Q.IsReady())
            {
                if (Enemies != null)
                {
                    Q.Cast(Enemies);
                    Q.CastOnUnit(Enemies);
                }
                if (minions != null)
                {
                    Q.Cast(minions);
                    Q.CastOnUnit(minions);
                }
            }
        }
        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var targetz = TargetSelector.GetTarget(W.Range);
            var useW = ComboMenu["WLowHp"].GetValue<MenuBool>().Enabled;
            var MinHealth = ComboMenu["minHealth"].GetValue<MenuSlider>().Value;
            if (useW && !_Player.IsRecalling() && targetz.IsValidTarget(425))
            {
                if (ObjectManager.Player.HealthPercent <= MinHealth)
                {
                    W.Cast();
                }
            }

            if ((args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E ||
                 args.Slot == SpellSlot.R) && sender.IsEnemy && Q.IsReady() && _Player.Distance(sender) <= args.SData.CastRange && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                if (args.SData.TargetingType == SpellDataTargetType.Unit || args.SData.TargetingType == SpellDataTargetType.SelfAndUnit || args.SData.TargetingType == SpellDataTargetType.Self)
                {
                    if ((args.Target.NetworkId == ObjectManager.Player.NetworkId && args.Time < 1.5 ||
                         args.End.Distance(ObjectManager.Player.Position) <= ObjectManager.Player.BoundingRadius * 3) &&
                        Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        QEvade();
                    }
                }
                else if (args.SData.TargetingType == SpellDataTargetType.LocationAoe)
                {
                    var castvector =
                        new Geometry.Circle(args.End, args.SData.CastRadius).IsInside(
                            ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        QEvade();
                    }
                }

                else if (args.SData.TargetingType == SpellDataTargetType.Cone)
                {
                    var castvector =
                        new Geometry.Arc(args.Start, args.End, args.SData.CastConeAngle, args.SData.CastRange)
                            .IsInside(ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        QEvade();
                    }
                }

                else if (args.SData.TargetingType == SpellDataTargetType.SelfAoe)
                {
                    var castvector =
                        new Geometry.Circle(sender.Position, args.SData.CastRadius).IsInside(
                            ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        QEvade();
                    }
                }
                else
                {
                    var castvector =
                        new Geometry.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(
                            ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        QEvade();
                    }
                }

                if (args.SData.Name == "yasuoq3w")
                {
                    QEvade();
                }

                if (args.SData.Name == "ZedR")
                {
                    if (Q.IsReady())
                    {
                        QEvade();
                    }
                    else
                    {
                        if (W.IsReady())
                        {
                            W.Cast();
                        }
                    }
                }

                if (args.SData.Name == "KarthusFallenOne")
                {
                    QEvade();
                }

                if (args.SData.Name == "SoulShackles")
                {
                    QEvade();
                }

                if (args.SData.Name == "AbsoluteZero")
                {
                    QEvade();
                }

                if (args.SData.Name == "NocturneUnspeakableHorror")
                {
                    QEvade();
                }
            }
        }

        public static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(target);
                    }
                }

                if (Ignite != null && KillStealMenu["ign"].GetValue<MenuBool>().Enabled && Ignite.IsReady())
                {
                    if (target.Health < _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
