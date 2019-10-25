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
// Rdamage to SpellSlotR
namespace KogMaw
{
    internal class Program
    {
        public static Item Botrk;
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, Misc, Items;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

   

        public static void KogMawOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("KogMaw")) return;
            Game.Print("Doctor's KogMaw Loaded!", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, (uint)ObjectManager.Player.GetRealAutoAttackRange());
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 900 + 300 * (uint)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level);
            Q.SetSkillshot(0.25f, 50f, 2000f, true, false, SkillshotType.Line);
            E.SetSkillshot(0.25f, 120f, 1400f, false, false, SkillshotType.Line);
            R.SetSkillshot(1.2f, 120f, float.MaxValue, false, false, SkillshotType.Circle);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var MenuKog = new Menu("Doctor's KogMaw", "KogMaw", true);
            MenuKog.Add(new MenuSeparator("Ideas Haxory", "Ideas Haxory"));
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            ComboMenu.Add(new MenuBool("ultiR", "Use [R] Combo"));
            ComboMenu.Add(new MenuList("RMode", "Ultimate Mode:", new[] { "Always", "HP =< 50%", "HP =< 25%" }) { Index = 0 });
            ComboMenu.Add(new MenuSlider("MinR", "Max Stacks [R] Combo", 5, 1, 10));
            ComboMenu.Add(new MenuSlider("ManaR", "Mana [R] Combo", 30));
            MenuKog.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            HarassMenu.Add(new MenuBool("HRR", "Use [R] Harass"));
            HarassMenu.Add(new MenuSlider("MinRHR", "Max Stacks [R] Harass", 2, 1, 10));
            HarassMenu.Add(new MenuSlider("ManaHR", "Mana Harass", 50));
            MenuKog.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear", false));
            LaneClearMenu.Add(new MenuBool("ELC", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("minE", "Min Hit Minion Use [E]", 3, 1, 6));
            LaneClearMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            LaneClearMenu.Add(new MenuBool("RLC", "Use [R] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("MinRLC", "Max Stacks [R] LaneClear", 1, 1, 10));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana LaneClear", 50));
            LaneClearMenu.Add(new MenuSeparator("Lasthit Settings", "Lasthit Settings"));
            LaneClearMenu.Add(new MenuBool("QLH", "Use [Q] Lasthit", false));
            LaneClearMenu.Add(new MenuSlider("ManaLH", "Mana Lasthit", 70));
            MenuKog.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            JungleClearMenu.Add(new MenuBool("RJungle", "Use [R] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("MinRJC", "Max Stacks [R] JungleClear", 1, 1, 10));
            JungleClearMenu.Add(new MenuSlider("ManaJC", "Mana JungleClear", 30));
            MenuKog.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuKog.Add(KillStealMenu);
            Items = new Menu("Items Settings", "Items");
            Items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Items.Add(new MenuBool("you", "Use [Youmuu]"));
            Items.Add(new MenuBool("BOTRK", "Use [BOTRK]"));
            Items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuKog.Add(Items);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Misc Settings", "Misc Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6", "7", "8" }) { Index = 0 });
            Misc.Add(new MenuBool("AntiGap", "Use [E] AntiGap"));
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawQ", "[Q] Range", false));
            Misc.Add(new MenuBool("DrawW", "[W] Range"));
            Misc.Add(new MenuBool("DrawE", "[E] Range", false));
            Misc.Add(new MenuBool("DrawR", "[R] Range"));
            MenuKog.Add(Misc);
            MenuKog.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
        }
        public static void Item()
        {
            var item = Items["BOTRK"].GetValue<MenuBool>().Enabled;
            var yous = Items["you"].GetValue<MenuBool>().Enabled;
            var Minhp = Items["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = Items["ihpp"].GetValue<MenuSlider>().Value;

            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(475) && !e.IsDead))
            {
                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
            }
            if (Misc["DrawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }
            if (Misc["DrawW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }
            if (Misc["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
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

        private static void Game_OnUpdate(EventArgs args)
        {
            R = new Spell(SpellSlot.R, 900 + 300 * (uint)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level);
            R.SetSkillshot(1200, 2200, 120, false, false, SkillshotType.Circle);
            W = new Spell(SpellSlot.W, (uint)ObjectManager.Player.GetRealAutoAttackRange());

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
                Ultimate();
            }
            Item();
            KillSteal();
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    var Pred = E.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        //E.Cast(Pred.CastPosition);
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var Pred = Q.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && Q.IsReady() == false && W.IsReady() == false)
                {
                    var Pred = E.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        //E.Cast(Pred.CastPosition);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(610 + 20 * (uint)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level))
                {
                    W.Cast();
                }
            }
        }

        private static void Ultimate()
        {
            var Rlimit = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            var useR = ComboMenu["ultiR"].GetValue<MenuBool>().Enabled;
            var mana = ComboMenu["ManaR"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(R.Range);

                if (useR && R.IsReady() && ObjectManager.Player.GetBuffCount("kogmawlivingartillerycost") < Rlimit && ObjectManager.Player.ManaPercent > mana)
                {
                    if (ComboMenu["RMode"].GetValue<MenuList>().Index == 0)
                    {
                        var Rpred = R.GetPrediction(target);
                        if (Rpred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(Rpred.CastPosition);
                        }
                    }

                    if (R.IsReady() && ComboMenu["RMode"].GetValue<MenuList>().Index == 1 && target.HealthPercent <= 50)
                    {
                        var Rpred = R.GetPrediction(target);
                        if (Rpred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(Rpred.CastPosition);
                        }
                    }

                    if (R.IsReady() && ComboMenu["RMode"].GetValue<MenuList>().Index == 2 && target.HealthPercent <= 25)
                    {
                        var Rpred = R.GetPrediction(target);
                        if (Rpred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(Rpred.CastPosition);
                        }
                    }
                }
            }
        

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["ELC"].GetValue<MenuBool>().Enabled;
            var useR = LaneClearMenu["RLC"].GetValue<MenuBool>().Enabled;
            var Rlimit = LaneClearMenu["MinRLC"].GetValue<MenuSlider>().Value;
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var MinE = LaneClearMenu["minE"].GetValue<MenuSlider>().Value;
            var minionE = GameObjects.EnemyMinions.Where((e => e.IsValidTarget(E.Range)));
            if (ObjectManager.Player.ManaPercent < mana) return;
            foreach (var minions in minionE)
            {
                if (useQ && Q.IsReady() && minions.IsValidTarget(Q.Range) && ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q) > minions.Health + minions.AllShield && _Player.Distance(minions) > 500)
                {
                    Q.Cast(minions);
                }

                if (useR && R.IsReady() && minions.IsValidTarget(R.Range) && ObjectManager.Player.GetBuffCount("kogmawlivingartillerycost") <= Rlimit)
                {
                    R.Cast(minions);
                }

                if (useE && E.IsReady() && minions.IsValidTarget(E.Range) && Q.IsReady() == false)
                {
                    E.Cast(minions.Position);
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (E.IsReady() && Misc["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) <= 375)
            {
                //E.Cast(sender);
            }
        }

        private static void LastHit()
        {
            var useQ = LaneClearMenu["QLH"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["ManaLH"].GetValue<MenuSlider>().Value;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent < mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health + minion.AllShield)
                {
                    Q.Cast(minion);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaHR"].GetValue<MenuSlider>().Value;
            var Rlimit = HarassMenu["MinRHR"].GetValue<MenuSlider>().Value;
            var useR = HarassMenu["HRR"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (ObjectManager.Player.ManaPercent <= mana) return;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var Pred = Q.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(610 + 20 * (uint)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level))
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    var Pred = E.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        //E.Cast(Pred.CastPosition);
                    }
                }
                //(ObjectManager.Player.ManaPercent <= mana
                if (R.IsReady() && useR && ObjectManager.Player.GetBuffCount("kogmawlivingartillerycost") < Rlimit)
                {
                    var Rpred = R.GetPrediction(target);
                    if (Rpred.Hitchance >= HitChance.Medium)
                    {
                        R.Cast(Rpred.CastPosition);
                    }
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var Rlimit = JungleClearMenu["MinRJC"].GetValue<MenuSlider>().Value;
            var useR = JungleClearMenu["RJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["ManaJC"].GetValue<MenuSlider>().Value;
            var monster = GameObjects.Jungle.Where(j => j.IsValidTarget(Q.Range)).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            if (ObjectManager.Player.ManaPercent <= mana) return;
            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(E.Range) && Q.IsReady() == false && W.IsReady() == false)
                {
                    E.Cast(monster.Position);
                }

                if (useW && W.IsReady() && monster.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (useR && R.IsReady() && ObjectManager.Player.GetBuffCount("kogmawlivingartillerycost") < Rlimit)
                {
                    R.Cast(monster);
                }
            }
        }

        public static void Flee()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target != null)
            {
                if (E.IsReady() && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsDead)
                {
                    E.Cast(target);
                }
            }
        }

        public static double RDamege(AIBaseClient target)
        {
            float RDamege = 0;
            if (target.HealthPercent > 50)
            {
                RDamege = new[] { 0, 70, 110, 150 }[R.Level] + 0.65f * _Player.FlatPhysicalDamageMod + 0.25f * _Player.FlatMagicDamageMod;
            }
            else if (target.HealthPercent < 50)
            {
                RDamege = new[] { 0, 140, 220, 300 }[R.Level] + 1.3f * _Player.FlatPhysicalDamageMod + 0.5f * _Player.FlatMagicDamageMod;
            }
            else if (target.HealthPercent < 25)
            {
                RDamege = new[] { 0, 210, 330, 450 }[R.Level] + 1.95f * _Player.FlatPhysicalDamageMod + 0.75f * _Player.FlatMagicDamageMod;
            }
            return _Player.CalculateDamage(target, DamageType.Magical, RDamege);
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(R.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(target);
                    }
                }

                if (KsR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var Rpred = R.GetPrediction(target);
                    if (target.Health < _Player.GetSpellDamage(target, SpellSlot.R) && Rpred.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(Rpred.CastPosition);
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
