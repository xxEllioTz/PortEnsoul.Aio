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
namespace Trundle7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, Items, Misc;
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
        public static Item Botrk;
        public static Item Bil;

   
       public static void TrundleOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Trundle")) return;
            Game.Print("Doctor's Trundle Loaded! Ported By DeathGodx", Color.Orange);
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            W.SetSkillshot(0, 2000, 900, false,false, SkillshotType.Circle);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 650);

            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Item(ItemId.Titanic_Hydra, ObjectManager.Player.GetRealAutoAttackRange());
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            var MenuTrundle = new Menu("Doctor's Trundle", "Trundle", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuList("comboMode", "Combo Mode:", new[] { "Always [Q]", "Only [Q] Reset AA" }) {Index = 0 });
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuSeparator("Combo [E] Settings", "Combo [E] Settings"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSlider("ekc", "Min Distance Use [E]", 300, 1, 1000));
            ComboMenu.Add(new MenuSeparator("Ultimate Health Settings", "Ultimate Health Settings"));
            ComboMenu.Add(new MenuBool("ultiR", "Use [R] My Health"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Health Use [R]", 60));
            ComboMenu.Add(new MenuSeparator("Use [R] On", "Use [R] On"));
            
            foreach (var target in GameObjects.EnemyHeroes)
            {
                ComboMenu.Add(new MenuBool("useRCombo" + target.CharacterName, "" + target.CharacterName));
            }
            MenuTrundle.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass", false));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSlider("MHR", "Min Mana Harass", 40));
            MenuTrundle.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear"));
            LaneClearMenu.Add(new MenuBool("WLC", "Use [W] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("MLC", "Min Mana LaneClear", 60));
            LaneClearMenu.Add(new MenuSeparator("Lasthit Settings", "Lasthit Settings"));
            LaneClearMenu.Add(new MenuBool("LHQ", "Use [Q] LastHit"));
            LaneClearMenu.Add(new MenuSlider("MLH", "Min Mana Lasthit", 60));
            MenuTrundle.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("MJC", "Min Mana JungleClear", 30));
            MenuTrundle.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuTrundle.Add(KillStealMenu);
            Items = new Menu("Items Settings", "Items");
            Items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Items.Add(new MenuBool("hydra", "Use [Hydra] Reset AA"));
            Items.Add(new MenuBool("titanic", "Use [Titanic]"));
            Items.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            Items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuTrundle.Add(Items);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4" }) { Index = 0 });
            Misc.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            Misc.Add(new MenuBool("inter", "Use [E] Interupt"));
            Misc.Add(new MenuBool("AntiGap", "Use [E] Anti Gapcloser"));
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawW", "W Range"));
            Misc.Add(new MenuBool("DrawE", "E Range", false));
            Misc.Add(new MenuBool("DrawR", "R Range", false));
            MenuTrundle.Add(Misc);
            MenuTrundle.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterrupterSpell += Interupt;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawW"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }
            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }
            if (Misc["DrawR"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
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
            KillSteal();
            Item();
            /*if (_Player.SkinId != Misc["skin.Id"].GetValue<MenuList>().Index)
            {
                if (checkSkin())
                {
                    Player.SetSkinId(SkinId());
                }
            }*/
        }

        public static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = Misc["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }
            if (Inter && E.IsReady() && i.DangerLevel == DangerLevel.Medium && E.IsInRange(sender))
            {
                E.Cast(sender.Position);
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Misc["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) < 300)
            {
                E.Cast(sender.Position);
            }
        }

        public static int SkinId()
        {
            return Misc["skin.Id"].GetValue<MenuList>().Index;
        }

        public static bool checkSkin()
        {
            return Misc["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var E2dis = ComboMenu["ekc"].GetValue<MenuSlider>().Value;
            var useR = ComboMenu["ultiR"].GetValue<MenuBool>().Enabled;
            var minR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            if (target != null)
            {
                if (ComboMenu["comboMode"].GetValue<MenuList>().Index == 0)
                {
                    if (Q.IsReady() && target.IsValidTarget(375))
                    {
                        Q.Cast();
                    }
                }

                if (useW && W.IsReady() && W.IsInRange(target))
                {
                    W.Cast(target.Position);
                }

                var pos = E.GetPrediction(target).CastPosition.Extend(ObjectManager.Player.Position, -100);

                if (useE && E.IsReady() && E.IsInRange(target) && E2dis <= _Player.Position.Distance(target))
                {
                    E.Cast(pos.ToVector2());
                }

                if (useR && ObjectManager.Player.HealthPercent <= minR && target.IsValidTarget(R.Range))
                {
                    if (ComboMenu["useRCombo" + target.CharacterName].GetValue<MenuBool>().Enabled)
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        public static void Item()
        {
            var item = Items["BOTRK"].GetValue<MenuBool>().Enabled;
            var Minhp = Items["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = Items["ihpp"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(475, DamageType.Physical);
            if (target != null)
            {
                if (item && Bil.IsReady && Bil.IsOwned() && target.IsValidTarget(475))
                {
                    Bil.Cast(target);
                }

                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(300, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useriu = Items["hydra"].GetValue<MenuBool>().Enabled;
            var HasQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var Hmana = HarassMenu["MHR"].GetValue<MenuSlider>().Value;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (ComboMenu["comboMode"].GetValue<MenuList>().Index == 1)
                {
                    if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && Q.IsReady())
                    {
                        Q.Cast();
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }

                if (HasQ && Q.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Harass && ObjectManager.Player.ManaPercent >= Hmana)
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttackTimer();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
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
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["WLC"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["MLC"].GetValue<MenuSlider>().Value;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent < mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(275)
                && ObjectManager.Player.Distance(minion.Position) <= 225f
                && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    Q.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }

                if (useW && W.IsReady() && minion.IsValidTarget(W.Range) && minions.Count() >= 3)
                {
                    W.Cast(_Player.Position);
                }
            }
        }

        public static void LastHit()
        {
            var useQ = LaneClearMenu["LHQ"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["MLH"].GetValue<MenuSlider>().Value;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent < mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(275)
                && ObjectManager.Player.Distance(minion.Position) <= 225f
                && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    Q.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }
            }
        }

        private static void Harass()
        {
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["MHR"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(700, DamageType.Physical);
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (target != null)
            {
                if (useW && W.IsReady() && W.IsInRange(target))
                {
                    W.Cast(target.Position);
                }

                var pos = E.GetPrediction(target).CastPosition.Extend(ObjectManager.Player.Position, -100);
                if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(pos.ToVector2());
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["MJC"].GetValue<MenuSlider>().Value;
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(W.Range));
            if (ObjectManager.Player.ManaPercent <= mana)
            {
                return;
            }

            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(300))
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && W.IsInRange(monster.Position))
                {
                    W.Cast(monster);
                }
            }
        }

        public static void Flee()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target != null)
            {
                var pos = E.GetPrediction(target).CastPosition.Extend(ObjectManager.Player.Position, 100);

                if (E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(pos.ToVector2());
                }
            }

            if (W.IsReady())
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, ObjectManager.Player);
            }
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(475) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(250))
                {
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast();
                    }
                }

                if (Ignite != null && KillStealMenu["ign"].GetValue<MenuBool>().Enabled && Ignite.IsReady())
                {
                    if (target.Health <= _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
