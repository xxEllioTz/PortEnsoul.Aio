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

namespace Renekton7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, Ulti, LaneClearMenu, JungleClearMenu, KillStealMenu, Misc;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;
        public static Item Hydra;
        public static Item Tiamat;
        public static Item Titanic;

    
       public static void RenektonOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Renekton")) return;
            Game.Print("Doctor's Renekton Loaded! PORTED by DEATHGODx", Color.Orange);
            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            //450 SkillShotType.Line
            R = new Spell(SpellSlot.R);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Item(ItemId.Titanic_Hydra, ObjectManager.Player.GetRealAutoAttackRange());
            var Menurenek = new Menu("Doctor's Renekton", "Renekton", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuSeparator("Combo [E] Settings", "Combo [E] Settings"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuBool("ComboE2", "Use [E2] Combo"));
            ComboMenu.Add(new MenuSlider("Edis", "Use [E2] If Enemy Distance >", 250, 0, 450));
            ComboMenu.Add(new MenuSeparator("Items Settings", "Items Settings"));
            ComboMenu.Add(new MenuBool("hydra", "Use [Hydra] Reset AA"));
            Menurenek.Add(ComboMenu);
            Ulti = new Menu("Ultimate Settings", "Ulti");
            Ulti.Add(new MenuSeparator("Ultimate Health Settings", "Ultimate Health Settings"));
            Ulti.Add(new MenuBool("ultiR", "Use [R] My Health"));
            Ulti.Add(new MenuSlider("MinR", "Min Health Use [R]", 50));
            Ulti.Add(new MenuSeparator("Ultimate Enemies Count", "Ultimate Enemies Count"));
            Ulti.Add(new MenuBool("ultiR2", "Use [R] Enemies In Range", false));
            Ulti.Add(new MenuSlider("MinE", "Min Enemies Use [R]", 3, 1, 5));
            Menurenek.Add(Ulti);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            Menurenek.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear"));
            LaneClearMenu.Add(new MenuBool("WLC", "Use [W] LaneClear", false));
            LaneClearMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LaneClearMenu.Add(new MenuBool("LHQ", "Use [Q] LastHit", false));
            LaneClearMenu.Add(new MenuBool("LHW", "Use [W] LastHit", false));
            Menurenek.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            Menurenek.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsW", "Use [W] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            Menurenek.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer"));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6", "7" }) { Index = 0 });
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawQ", "Q Range"));
            Misc.Add(new MenuBool("DrawE", "E Range", false));
            Misc.Add(new MenuBool("DrawE2", "Drawings Distance Use E2 If Distance Target >"));
            Menurenek.Add(Misc);
            Menurenek.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            //Orbwalker.OnAction += ResetAttack;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var E2dis = ComboMenu["Edis"].GetValue<MenuSlider>().Value;
            if (Misc["DrawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Misc["DrawE2"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E2dis, Color.Orange, 1);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
            Item();
            KillSteal();
            Ultimate();
        }

        public static int SkinId()
        {
            return Misc["skin.Id"].GetValue<MenuList>().Index;
        }

        public static bool checkSkin()
        {
            return Misc["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        public static bool PassiveW
        {
            get { return ObjectManager.Player.HasBuff("renektonpreexecute"); }
        }

        public static bool PassiveE
        {
            get { return ObjectManager.Player.HasBuff("RenekthonSliceAndDiceDelay"); }
        }

        public static bool Fury
        {
            get { return ObjectManager.Player.HasBuff("renektonrageready"); }
        }

        public static double QDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 60, 90, 120, 150, 190 }[Q.Level] + 0.8f * _Player.FlatPhysicalDamageMod));
        }

        public static double WDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 5, 15, 25, 35, 45 }[W.Level] + 0.75f * _Player.FlatPhysicalDamageMod));
        }

        public static double EDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 30, 60, 90, 120, 150 }[E.Level] + 0.9f * _Player.FlatPhysicalDamageMod));
        }

        public static float GetDamage(AIHeroClient target)
        {
            if (target != null)
            {
                float Damage = 0;
                /*
                                if (Q.IsReady()) { Damage+= QDamage(target); }
                                if (E.IsReady()) { Damage += EDamage(); }
                                if (W.IsReady()) { Damage += WDamage(); }

                                return Damage;*/
            }
            return 0;
        }

        private static void Combo()
        {
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useE2 = ComboMenu["ComboE2"].GetValue<MenuBool>().Enabled;
            var E2dis = ComboMenu["Edis"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && !PassiveW && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (!PassiveE && useE && E.IsReady() && target.IsValidTarget(E.Range) && (200 <= target.Distance(ObjectManager.Player) || !Q.IsReady() && !W.IsReady()))
                {
                    E.Cast(target.Position);
                }

                if (useE2 && E.IsReady() && target.IsValidTarget(E.Range) && PassiveE && E2dis <= target.Distance(ObjectManager.Player))
                {
                    E.Cast(target.Position);
                }
            }
        }

        private static void Ultimate()
        {
            var useR = Ulti["ultiR"].GetValue<MenuBool>().Enabled;
            var useR2 = Ulti["ultiR2"].GetValue<MenuBool>().Enabled;
            var minR = Ulti["MinR"].GetValue<MenuSlider>().Value;
            var minE = Ulti["MinE"].GetValue<MenuSlider>().Value;

            if (useR && _Player.HealthPercent <= minR && _Player.Position.CountEnemyHeroesInRange(500) >= 1 && !ObjectManager.Player.InShop())
            {
                R.Cast();
            }

            if (useR2 && !ObjectManager.Player.InShop() && _Player.Position.CountEnemyHeroesInRange(450) >= minE)
            {
                R.Cast();
            }
        }

        public static void Item()
        {
            var target = TargetSelector.GetTarget(300, DamageType.Physical);

            var useriu = ComboMenu["hydra"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var HasW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            if (target != null)
            {
                if (useW && W.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    W.Cast();
                    //Orbwalker.ResetAutoAttackTimer();
                    //Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if (HasW && W.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    W.Cast();
                    //Orbwalker.ResetAutoAttackTimer();
                    //Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if ((useriu) && (Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass))
                {
                    if (Hydra.IsOwned() && Hydra.IsReady && target.IsValidTarget(250))
                    {
                        Hydra.Cast();
                    }

                    if (Tiamat.IsOwned() && Tiamat.IsReady && target.IsValidTarget(250))
                    {
                        Tiamat.Cast();
                    }

                    if (Titanic.IsOwned() && Titanic.IsReady && target.IsValidTarget(250))
                    {
                        Titanic.Cast();
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["WLC"].GetValue<MenuBool>().Enabled;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && minion.IsValidTarget(275) && minion.InAutoAttackRange()
                && ObjectManager.Player.Distance(minion.Position) <= 225f
                && WDamage(minion) * 2 + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    W.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }
            }
        }

        public static void LastHit()
        {
            var useQ = LaneClearMenu["LHQ"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["LHW"].GetValue<MenuBool>().Enabled;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(minion) && QDamage(minion) >= minion.Health + minion.AllShield)
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && minion.IsValidTarget(275) && minion.InAutoAttackRange()
                && ObjectManager.Player.Distance(minion.Position) <= 225f
                && WDamage(minion) * 2 + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    W.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
            }
        }

        public static void JungleClear()
        {
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var jungleMonsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (jungleMonsters != null)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(jungleMonsters))
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && jungleMonsters.IsValidTarget(275) && jungleMonsters.InAutoAttackRange() && ObjectManager.Player.Distance(jungleMonsters.Position) <= 225f)
                {
                    W.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, jungleMonsters);
                }

                if (useE && E.IsReady())
                {
                    if (!PassiveE)
                    {
                        E.Cast(jungleMonsters.Position);
                    }

                    if (PassiveE && jungleMonsters.IsValidTarget(E.Range) && !Q.IsReady() && !W.IsReady())
                    {
                        E.Cast(jungleMonsters.Position);
                    }
                }
            }
        }
        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsW = KillStealMenu["KsW"].GetValue<MenuBool>().Enabled;
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (Fury)
                    {
                        if (target.Health + target.PhysicalShield <= QDamage(target) * 0.5)
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        if (target.Health + target.PhysicalShield <= QDamage(target))
                        {
                            Q.Cast();
                        }
                    }
                }

                if (KsW && W.IsReady() && target.IsValidTarget(250))
                {
                    if (Fury)
                    {
                        if (target.Health + target.PhysicalShield <= WDamage(target) * 3)
                        {
                            W.Cast();
                        }
                    }
                    else
                    {
                        if (target.Health + target.PhysicalShield <= WDamage(target) * 2)
                        {
                            W.Cast();
                        }
                    }
                }

                if (KsE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (target.Health + target.PhysicalShield <= EDamage(target))
                    {
                        E.Cast(target.Position);
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
