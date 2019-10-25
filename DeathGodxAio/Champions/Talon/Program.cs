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

namespace Talon7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, Misc, KillStealMenu, Items;
        public static Item Botrk;
        public static Item Hydra;
        public static Item Tiamat;
        public static Item Bil;
        public static Item Youmuu;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell R;
        public static Spell Ignite;

       public static void TalonOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Talon")) return;
            Game.Print("Doctor's Talon PORTED By DEATHGODX!", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 500);
            W = new Spell(SpellSlot.W, 750);
            W.SetSkillshot(1, 2300, 80, false, false, SkillshotType.Cone);
            R = new Spell(SpellSlot.R);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Bil = new Item(3144, 475f);
            Youmuu = new Item(3142, 10);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var MenuRyze = new Menu("Talon", "Talon", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("combo settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q]"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W]"));
            ComboMenu.Add(new MenuBool("ComboR", "Always Use [R] On Combo"));
            ComboMenu.Add(new MenuBool("riu", "Use [Hydra] Reset AA"));
            ComboMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            ComboMenu.Add(new MenuBool("rcount", "Use [R] Aoe"));
            ComboMenu.Add(new MenuSlider("cou", "Min Enemies Around Use [R] Aoe", 2, 1, 5));
            MenuRyze.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q]"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W]"));
            HarassMenu.Add(new MenuSlider("ManaW", "Min Mana Harass", 40));
            MenuRyze.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("LaneW", "Use [W]"));
            LaneClearMenu.Add(new MenuSlider("MinW", "Hit Minions LaneClear", 3, 1, 6));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Min Mana LaneClear", 60));
            MenuRyze.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("MnJungle", "Min Mana JungleClear [Q]", 20));
            MenuRyze.Add(JungleClearMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("AntiGap Settings", "AntiGap Settings"));
            Misc.Add(new MenuBool("AntiGap", "Use [W] AntiGapcloser"));
            Misc.Add(new MenuBool("Rstun", "Use [W] AUTO", false));
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawQ", "[Q] Range"));
            Misc.Add(new MenuBool("DrawW", "[W] Range"));
            MenuRyze.Add(Misc);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsW", "Use [W] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuRyze.Add(KillStealMenu);
            Items = new Menu("Items Settings", "Items");
            Items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Items.Add(new MenuBool("you", "Use [Youmuu]"));
            Items.Add(new MenuBool("BOTRK", "Use [BOTRK]"));
            Items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuRyze.Add(Items);
            MenuRyze.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }
            if (Misc["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || MenuGUI.IsChatOpen || ObjectManager.Player.IsWindingUp)
            {
                return;
            }
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    return;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
            KillSteal();
            RStun();
            AutoR();
            Item();
        }
        public static bool RActive
        {
            get { return ObjectManager.Player.HasBuff("TalonRHaste"); }
        }
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Misc["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Position.Distance(_Player) < 300)
            {
                W.Cast(sender);
            }
        }
        public static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && _Player.Distance(target) > 125)
                {
                    Q.Cast(target);
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

                if (useR && R.IsReady() && target.IsValidTarget(425))
                {
                    R.Cast();
                }
            }
        }
        public static void AutoR()
        {
            var useR = ComboMenu["rcount"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["cou"].GetValue<MenuSlider>().Value;

            if (useR && R.IsReady() && _Player.Position.CountEnemyHeroesInRange(450) >= MinR)
            {
                R.Cast();
            }
        }
        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["MnJungle"].GetValue<MenuSlider>().Value;
            var jungleMonsters = GameObjects.Jungle.Where(j => j.IsValidTarget(W.Range)).FirstOrDefault(j => j.IsValidTarget(W.Range));
            if (ObjectManager.Player.ManaPercent < mana)
            {
                return;
            }
            if (jungleMonsters != null)
            {
                if (useQ && Q.IsReady() && jungleMonsters.IsValidTarget(Q.Range))
                {
                    Q.Cast(jungleMonsters);
                }

                if (useW && W.IsReady() && jungleMonsters.IsValidTarget(W.Range))
                {
                    W.Cast(jungleMonsters);
                }
            }
        }
        public static void LaneClear()
        {
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var useW = LaneClearMenu["LaneW"].GetValue<MenuBool>().Enabled;
            var minW = LaneClearMenu["MinW"].GetValue<MenuSlider>().Value;
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(W.Range) && e.IsMinion())
                            .Cast<AIBaseClient>().ToList();
            var qFarmLocation = W.GetLineFarmLocation(minions, W.Width);
            if (qFarmLocation.Position.IsValid())

                //var hitminion = GameObjects.Minions.(minions, W.Width, (int)W.Range);

                if (ObjectManager.Player.ManaPercent <= mana)
                {
                    return;
                }

            foreach (var minion in minions)
            {
                if (useW && W.IsReady() && minion.IsValidTarget(W.Range) && qFarmLocation.MinionsHit >= minW)
                {
                    W.Cast(qFarmLocation.Position);
                }
            }
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

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaW"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (ObjectManager.Player.ManaPercent <= mana)
            {
                return;
            }

            if (target != null)
            {
                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && _Player.Distance(target) > 125)
                {
                    Q.Cast(target);
                }
            }
        }

        public static void RStun()
        {
            var Rstun = Misc["Rstun"].GetValue<MenuBool>().Enabled;
            if (Rstun && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target != null)
                {
                    if (target.IsValid || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                    {
                        W.Cast(target.Position);
                    }
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(250, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useriu = ComboMenu["riu"].GetValue<MenuBool>().Enabled;
            var HasQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var ManaW = HarassMenu["ManaW"].GetValue<MenuSlider>().Value;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useQ && Q.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Combo && target.IsValidTarget(150))
                {
                    Q.Cast(target);
                    Orbwalker.ResetAutoAttackTimer();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
                if (HasQ && Q.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Harass && target.IsValidTarget(150) && ObjectManager.Player.ManaPercent > ManaW)
                {
                    Q.Cast(target);
                    Orbwalker.ResetAutoAttackTimer();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if ((useriu) && (Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass))
                {
                    if (Hydra.IsInRange(ObjectManager.Player) && Hydra.IsReady && target.IsValidTarget(250))
                    {
                        Hydra.Cast();
                    }

                    if (Tiamat.IsInRange(ObjectManager.Player) && Tiamat.IsReady && target.IsValidTarget(250))
                    {
                        Tiamat.Cast();
                    }
                }
            }
        }

        public static double QDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                    (float)(new[] { 0, 80, 120, 140, 160, 180 }[Q.Level] + 1.1f * _Player.FlatPhysicalDamageMod));

        }

        public static double WDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 60, 90, 120, 150, 180 }[W.Level] + 0.6f * _Player.FlatPhysicalDamageMod));
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 80, 120, 160 }[R.Level] + 0.8f * _Player.FlatPhysicalDamageMod));
        }

        public static void KillSteal()
        {
            if (ObjectManager.Player.HasBuff("TalonEHop"))
            {

            }
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsW = KillStealMenu["KsW"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(W.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady())
                {
                    if (target != null)
                    {
                        if (_Player.Distance(target) > 150)
                        {
                            if (target.Health + target.AllShield <= QDamage(target))
                            {
                                Q.Cast(target);
                            }
                        }
                        else
                        {
                            if (target.Health + target.AllShield <= QDamage(target) * 1.5f)
                            {
                                Q.Cast(target);
                            }
                        }
                    }
                }
                if (KsW && W.IsReady())
                {
                    if (target != null)
                    {
                        if (target.Health + target.AllShield <= WDamage(target))/*try*/
                        {
                            W.Cast(target);
                        }
                    }
                }
                if (KsR && R.IsReady() && target.IsValidTarget(500))
                {
                    if (target != null)
                    {
                        if (target.Health + target.AllShield <= RDamage(target))
                        {
                            R.Cast();
                        }
                    }
                }
                if (Ignite != null && KillStealMenu["ign"].GetValue<MenuBool>().Enabled && Ignite.IsReady())
                {
                    if (target.Health + target.AllShield < _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
