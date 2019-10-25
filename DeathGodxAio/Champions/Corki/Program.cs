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

namespace Corki7
{
    static class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Item Botrk;
        public static Item Bil;
        public static Spell Ignite;
        public static Menu Menu, SpellMenu, HarassMenu, ClearMenu, KillstealMenu, JungleMenu, items, Misc;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        // Menu

        public static void CorkiOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Corki")) return;
            Game.Print("Doctor's Corki Loaded! PORTED By DEATHGODX", Color.Orange);
            Q = new Spell(SpellSlot.Q, 825);
            W = new Spell(SpellSlot.W, 800);
            W.SetSkillshot(300, 1000, 250, false, false, SkillshotType.Line);
            E = new Spell(SpellSlot.E, 660);
            R = new Spell(SpellSlot.R, 1200);
            R.SetSkillshot(200, 1950, 40, false, false, SkillshotType.Line);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var MenuCorki = new Menu("Doctor's Corki", "Corki", true);
            SpellMenu = new Menu("Combo Settings", "Combo");
            SpellMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            SpellMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            SpellMenu.Add(new MenuList("QMode", "Q Mode:", new[] { "Fast [Q]", "[Q] After Attack" }) { Index = 0 });
            SpellMenu.Add(new MenuBool("ComboR", "Use [R] Combo"));
            SpellMenu.Add(new MenuList("RMode", "R Mode:", new[] { "Fast [R]", "[R] After Attack" }) { Index = 0 });
            SpellMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            MenuCorki.Add(SpellMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassR", "Use [R] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E Harass]"));
            HarassMenu.Add(new MenuSlider("manaHarass", "Min Mana Harass", 50, 0, 100));
            HarassMenu.Add(new MenuSlider("RocketHarass", "Save Rockets [R]", 3, 0, 6));
            MenuCorki.Add(HarassMenu);
            ClearMenu = new Menu("LaneClear Settings", "LaneClear");
            ClearMenu.Add(new MenuSeparator("Laneclear Settings", "Laneclear Settings"));
            ClearMenu.Add(new MenuBool("ClearQ", "Use [Q] LaneClear", false));
            ClearMenu.Add(new MenuBool("ClearR", "Use [R] LaneClear", false));
            ClearMenu.Add(new MenuBool("ClearE", "Use [E] LaneClear", false));
            ClearMenu.Add(new MenuSlider("manaClear", "Min Mana LaneClear", 65, 0, 100));
            ClearMenu.Add(new MenuSlider("RocketClear", "Save Rockets [R]", 3, 0, 6));
            MenuCorki.Add(ClearMenu);
            JungleMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleMenu.Add(new MenuBool("JungleQ", "Use [Q] JungleClear"));
            JungleMenu.Add(new MenuBool("JungleR", "Use [R] JungleClear"));
            JungleMenu.Add(new MenuBool("JungleE", "Use [E] JungleClear"));
            JungleMenu.Add(new MenuSlider("manaJung", "Min Mana JungleClear", 30, 0, 100));
            JungleMenu.Add(new MenuSlider("RocketJung", "Save Rockets [R]", 3, 0, 6));
            MenuCorki.Add(JungleMenu);
            KillstealMenu = new Menu("KillSteal Settings", "KS");
            KillstealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillstealMenu.Add(new MenuBool("RKs", "Use [R] KillSteal"));
            KillstealMenu.Add(new MenuBool("QKs", "Use [Q] KillSteal"));
            KillstealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuCorki.Add(KillstealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Drawings Settings", "Drawings Settings"));
            Misc.Add(new MenuBool("Draw_Disabled", "Disabled Drawings", false));
            Misc.Add(new MenuBool("drawQ", "Range [Q]"));
            Misc.Add(new MenuBool("drawW", "Range [W]", false));
            Misc.Add(new MenuBool("drawE", "Range [E]"));
            Misc.Add(new MenuBool("drawR", "Range [R]"));
            MenuCorki.Add(Misc);
            items = new Menu("Items Settings", "Items");
            items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            items.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuCorki.Add(items);
            MenuCorki.Attach();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnAction += ResetAttack;
        }

        // Game OnTick

        private static void Game_OnUpdate(EventArgs args)
        {
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
                LaneClear();
                JungleClear();
            }

            KillSteal();
            Item();
        }

        // Combo Mode

        private static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useQ = SpellMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useR = SpellMenu["ComboR"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (SpellMenu["QMode"].GetValue<MenuList>().Index == 1)
                {
                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && _Player.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                    {
                        var Pred = Q.GetPrediction(target);
                        if (Pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(Pred.CastPosition);
                        }
                    }
                }

                if (SpellMenu["RMode"].GetValue<MenuList>().Index == 1)
                {
                    if (useR && R.IsReady() && R.Ammo >= 1 && target.IsValidTarget(R.Range) && _Player.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                    {
                        var RPred = R.GetPrediction(target);
                        if (RPred.Hitchance >= HitChance.High)
                        {
                            R.Cast(RPred.CastPosition);
                        }
                    }
                }

            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var useQ = SpellMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useR = SpellMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var useE = SpellMenu["ComboE"].GetValue<MenuBool>().Enabled;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !Orbwalker.CanAttack())
                {
                    if (SpellMenu["QMode"].GetValue<MenuList>().Index == 0)
                    {
                        var Pred = Q.GetPrediction(target);
                        if (Pred.Hitchance >= HitChance.Medium)
                        {
                            Q.Cast(Pred.CastPosition);
                        }
                    }
                    else
                    {
                        if (_Player.Distance(target) > ObjectManager.Player.GetRealAutoAttackRange(target))
                        {
                            var Pred = Q.GetPrediction(target);
                            if (Pred.Hitchance >= HitChance.Medium)
                            {
                                Q.Cast(Pred.CastPosition);
                            }
                        }
                    }
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range) && R.Ammo >= 1)
                {
                    if (SpellMenu["RMode"].GetValue<MenuList>().Index == 0)
                    {
                        var RPred = R.GetPrediction(target);
                        if (RPred.Hitchance >= HitChance.Medium)
                        {
                            R.Cast(RPred.CastPosition);
                        }
                    }
                    else
                    {
                        if (_Player.Distance(target) > ObjectManager.Player.GetRealAutoAttackRange(target))
                        {
                            var RPred = R.GetPrediction(target);
                            if (RPred.Hitchance >= HitChance.Medium)
                            {
                                R.Cast(RPred.CastPosition);
                            }
                        }
                    }
                }
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }
                if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }
        }

        // Harass Mode

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var mana = HarassMenu["manaHarass"].GetValue<MenuSlider>().Value;
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useR = HarassMenu["HarassR"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var Rocket = HarassMenu["RocketHarass"].GetValue<MenuSlider>().Value;

            if (ObjectManager.Player.ManaPercent < mana)
            {
                return;
            }

            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !Orbwalker.CanAttack())
                {
                    var Pred = Q.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range) && R.Ammo > Rocket)
                {
                    var Pred = R.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(Pred.CastPosition);
                    }
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }
        }

        // LaneClear

        private static void LaneClear()
        {
            var mana = ClearMenu["manaClear"].GetValue<MenuSlider>().Value;
            var useQ = ClearMenu["ClearQ"].GetValue<MenuBool>().Enabled;
            var useR = ClearMenu["ClearR"].GetValue<MenuBool>().Enabled;
            var useE = ClearMenu["ClearE"].GetValue<MenuBool>().Enabled;
            var Rocket = ClearMenu["RocketClear"].GetValue<MenuSlider>().Value;
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(R.Range) && e.IsMinion())
                .Cast<AIBaseClient>().ToList();
            var QCal = Q.GetLineFarmLocation(minions, Q.Width);
            if (QCal.Position.IsValid())
                if (ObjectManager.Player.ManaPercent < mana)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (useR && R.IsReady() && minion.IsValidTarget(R.Range) && R.Ammo > Rocket)
                {
                    R.Cast(minion.Position);
                }

                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && QCal.MinionsHit >= 2 && !Orbwalker.CanAttack())
                {
                    Q.Cast(QCal.Position);
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range) && !Orbwalker.CanAttack())
                {
                    E.Cast(minion.Position);
                }
            }
        }

        // KillSteal

        private static void KillSteal()
        {
            var useQ = KillstealMenu["QKs"].GetValue<MenuBool>().Enabled;
            var useR = KillstealMenu["RKs"].GetValue<MenuBool>().Enabled;
            var Ignites = KillstealMenu["ign"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead ))
            {
                if (useR && R.IsReady() && target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.R))
                {
                    var RPred = R.GetPrediction(target);
                    if (RPred.Hitchance >= HitChance.Medium)
                    {
                        R.Cast(RPred.CastPosition);
                    }
                }

                if (useQ && Q.IsReady() && target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                {
                    var Pred = Q.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.Medium)
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }

                if (Ignite != null && Ignites && Ignite.IsReady())
                {
                    if (target.Health < _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }

        // Flee

        private static void Flee()
        {
            /*
            if (W.IsReady())
            {
                var cursorPos = Game.CursorPos;
                var castPos = ObjectManager.Player.Position.Distance(cursorPos) <= W.Range ? cursorPos : ObjectManager.Player.Position.Extend(cursorPos, W.Range).To3D();
                W.Cast(castPos);
            }*/
        }

        // JungleClear

        private static void JungleClear()
        {
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(R.Range));
            var useQ = JungleMenu["JungleQ"].GetValue<MenuBool>().Enabled;
            var useR = JungleMenu["JungleR"].GetValue<MenuBool>().Enabled;
            var useE = JungleMenu["JungleE"].GetValue<MenuBool>().Enabled;
            var mana = JungleMenu["manaJung"].GetValue<MenuSlider>().Value;
            var Rocket = JungleMenu["RocketJung"].GetValue<MenuSlider>().Value;

            if (ObjectManager.Player.ManaPercent < mana)
            {
                return;
            }

            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }

                if (useR && R.IsReady() && monster.IsValidTarget(R.Range) && R.Ammo > Rocket)
                {
                    R.Cast(monster);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.Cast(monster);
                }
            }
        }

        // Use Items

        public static void Item()
        {
            var item = items["BOTRK"].GetValue<MenuBool>().Enabled;
            var Minhp = items["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = items["ihpp"].GetValue<MenuSlider>().Value;
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
            }
        }

        // Drawings

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;

            if (Misc["Draw_Disabled"].GetValue<MenuBool>().Enabled) return;

            if (Misc["drawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Misc["drawW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }

            if (Misc["drawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Misc["drawR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
            }
        }
    }
}
