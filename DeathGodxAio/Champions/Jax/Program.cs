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

namespace Jax
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, Autos, HarassMenu, LaneClearMenu, JungleClearMenu, Misc, KillStealMenu, Drawings;
        public static Item Botrk;
        public static Item Bil;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

      

       public static void JaxOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Jax")) return;
            Game.Print("Doctor's Jax Loaded! PORTED by DEATHGODx", Color.White);
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var Menujax = new Menu("Jax", "Jax", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuList("comboMode", "Combo Mode:", new[] { "E => Q", "Q => E" }) { Index = 0 });
            ComboMenu.Add(new MenuBool("ComboQ", "Combo [Q]"));
            ComboMenu.Add(new MenuBool("ComboW", "Combo [W]"));
            ComboMenu.Add(new MenuBool("ComboE", "Combo [E]"));
            ComboMenu.Add(new MenuBool("ComboR", "Combo [R]"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 2, 1, 5));
            Menujax.Add(ComboMenu);
            Autos = new Menu("Auto E/R Settings", "Autos");
            Autos.Add(new MenuSeparator("Automatic Settings", "Automatic Settings"));
            Autos.Add(new MenuBool("AutoE", "Auto [E] Enemies In Range"));
            Autos.Add(new MenuSlider("minE", "Min Enemies Auto [E]", 2, 1, 5));
            Autos.Add(new MenuBool("AutoR", "Auto [R] If My HP =<"));
            Autos.Add(new MenuSlider("mauR", "My HP Auto [R]", 50));
            Menujax.Add(Autos);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Harass [Q]", false));
            HarassMenu.Add(new MenuBool("HarassW", "Harass [W]"));
            HarassMenu.Add(new MenuBool("HarassE", "Harass [E]"));
            HarassMenu.Add(new MenuSlider("ManaQ", "Min Mana For Harass", 30));
            Menujax.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("LaneClear Settings", "LaneClear Settings"));
            LaneClearMenu.Add(new MenuBool("LCQ", "Lane Clear [Q]", false));
            LaneClearMenu.Add(new MenuBool("LCW", "Lane Clear [W]"));
            LaneClearMenu.Add(new MenuBool("LCE", "Lane Clear [E]", false));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Min Mana LaneClear [Q]", 60));
            LaneClearMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LaneClearMenu.Add(new MenuBool("LHQ", "Lane Clear [Q]", false));
            LaneClearMenu.Add(new MenuBool("LHW", "Lane Clear [W]"));
            LaneClearMenu.Add(new MenuSlider("ManaLH", "Min Mana LaneClear [Q]", 60));
            Menujax.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Spell [Q]"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Spell [W]"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Spell [E]"));
            JungleClearMenu.Add(new MenuSlider("MnJungle", "Min Mana For JungleClear", 30));
            Menujax.Add(JungleClearMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("AntiGap Settings", "AntiGap Settings"));
            Misc.Add(new MenuBool("antiGap", "Use [E] AntiGapcloser"));
            Misc.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Misc.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            Misc.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Misc.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            Menujax.Add(Misc);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "[Q] KillSteal", false));
            KillStealMenu.Add(new MenuBool("ign", "[Ignite] KillSteal"));
            Menujax.Add(KillStealMenu);
            Drawings = new Menu("Draw Settings", "Draw");
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "Q Range"));
            Drawings.Add(new MenuBool("DrawE", "E Range", false));
            Menujax.Add(Drawings);
            Menujax.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.White, 1);
            }

            if (Drawings["DrawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.White, 1);
            }
        }

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
                JungleClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }
            KillSteal();
            Item();
        }

        public static bool ECasting
        {
            get { return ObjectManager.Player.HasBuff("JaxCounterStrike"); }
        }

        public static void Item()
        {
            var item = Misc["BOTRK"].GetValue<MenuBool>().Enabled;
            var Minhp = Misc["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = Misc["ihpp"].GetValue<MenuSlider>().Value;
            var useR = Autos["AutoR"].GetValue<MenuBool>().Enabled;
            var MinR = Autos["mauR"].GetValue<MenuSlider>().Value;
            var useE = Autos["AutoE"].GetValue<MenuBool>().Enabled;
            var MinE = Autos["minE"].GetValue<MenuSlider>().Value;
            foreach (var targeti in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(475) && !e.IsDead))
            {
                if (item && Bil.IsReady && Bil.IsOwned() && Bil.IsInRange(targeti))
                {
                    Bil.Cast(targeti);
                }

                if ((item && Botrk.IsReady && Botrk.IsOwned() && targeti.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || targeti.HealthPercent < Minhpp))
                {
                    Botrk.Cast(targeti);
                }
            }

            if (useR && R.IsReady() && _Player.Position.CountEnemyHeroesInRange(Q.Range) >= 1 && ObjectManager.Player.HealthPercent <= MinR)
            {
                R.Cast();
            }

            if (useE && E.IsReady() && _Player.Position.CountEnemyHeroesInRange(E.Range) >= MinE)
            {
                E.Cast();
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            if (ComboMenu["comboMode"].GetValue<MenuList>().Index == 0)
            {
                if (target != null)
                {
                    if (useE && E.IsReady())
                    {
                        if (!ECasting && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsValidTarget(E.Range))
                        {
                            E.Cast();
                        }

                        if (!ECasting && target.IsValidTarget(E.Range))
                        {
                            E.Cast();
                        }

                        if (ECasting && target.IsValidTarget(E.Range))
                        {
                            E.Cast();
                        }
                    }

                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && (ObjectManager.Player.GetRealAutoAttackRange() < target.Distance(ObjectManager.Player) || ObjectManager.Player.HealthPercent <= 25))
                    {
                        if (ECasting)
                        {
                            Q.Cast(target);
                        }
                        else
                        {
                            if (!E.IsReady())
                            {
                                Q.Cast(target);
                            }
                        }
                    }
                }
                var targetz = TargetSelector.GetTarget(300, DamageType.Physical);
                var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
                var HasW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
                var mana = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
                if (target != null)
                {
                    if (useW && W.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                    {
                        W.Cast();
                        //Orbwalker.ResetAutoAttackTimer();
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targetz);
                    }

                    if (HasW && W.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Harass && _Player.ManaPercent >= mana)
                    {
                        W.Cast();
                        //Orbwalker.ResetAutoAttackTimer();
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targetz);
                    }
                }
            }

            if (ComboMenu["comboMode"].GetValue<MenuList>().Index == 1)
            {
                if (target != null)
                {
                    if (useQ && Q.IsReady() && ObjectManager.Player.GetRealAutoAttackRange() < target.Distance(ObjectManager.Player))
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }
                    }

                    if (useE && E.IsReady())
                    {
                        if (!ECasting && target.IsValidTarget(E.Range))
                        {
                            E.Cast();
                        }

                        if (ECasting && target.IsValidTarget(E.Range))
                        {
                            E.Cast();
                        }
                    }
                }
            }

            if (useR && R.IsReady())
            {
                if (_Player.Position.CountEnemyHeroesInRange(Q.Range) >= MinR)
                {
                    R.Cast();
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            
        }

        public static void JungleClear()
        {
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["MnJungle"].GetValue<MenuSlider>().Value;
            var jungleMonsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (jungleMonsters != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(jungleMonsters))
                {
                    Q.Cast(jungleMonsters);
                }

                if (useW && W.IsReady() && jungleMonsters.IsValidTarget(275) && jungleMonsters.InAutoAttackRange() && ObjectManager.Player.Distance(jungleMonsters.Position) <= 225f)
                {
                    W.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, jungleMonsters);
                }

                if (useE && E.IsReady())
                {
                    if (!ECasting)
                    {
                        E.Cast();
                    }

                    else if (ECasting && jungleMonsters.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }
                }
            }
        }

        public static void LaneClear()
        {
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var useW = LaneClearMenu["LCW"].GetValue<MenuBool>().Enabled;
            var useQ = LaneClearMenu["LCQ"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["LCE"].GetValue<MenuBool>().Enabled;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent <= mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(minion) && _Player.GetRealAutoAttackRange() <= minion.Distance(ObjectManager.Player) && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health + minion.AllShield)
                {
                    Q.Cast(minion);
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
                if (useW && W.IsReady() && minion.IsValidTarget(275) && minion.InAutoAttackRange()
                && ObjectManager.Player.Distance(minion.Position) <= 225f
                && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W) + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    W.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }
            }
        }

        public static void LastHit()
        {
            var mana = LaneClearMenu["ManaLH"].GetValue<MenuSlider>().Value;
            var useQ = LaneClearMenu["LHQ"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["LHW"].GetValue<MenuBool>().Enabled;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent <= mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(minion) && ObjectManager.Player.GetRealAutoAttackRange() <= minion.Distance(ObjectManager.Player) && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health + minion.AllShield)
                {
                    Q.Cast(minion);
                }

                if (useW && W.IsReady() && minion.IsValidTarget(275) && minion.InAutoAttackRange()
                && ObjectManager.Player.Distance(minion.Position) <= 225f
                && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W) + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    W.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }
            }
        }

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (target != null)
            {
                if (useE && E.IsReady())
                {
                    if (useQ && !ECasting && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }

                    if (!ECasting && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }

                    if (ECasting && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && (ObjectManager.Player.GetRealAutoAttackRange() < target.Distance(ObjectManager.Player) || ObjectManager.Player.HealthPercent <= 25))
                {
                    if (ECasting)
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        public static void Flee()
        {
            if (Q.IsReady())
            {
                var CursorPos = Game.CursorPos;
                AIBaseClient JumpPlace = GameObjects.EnemyHeroes.FirstOrDefault(w => w.Distance(CursorPos) <= 250 && Q.IsInRange(w));
                if (JumpPlace != default(AIBaseClient))
                {
                    Q.Cast(JumpPlace);
                }
                else
                {
                    JumpPlace = GameObjects.AllyMinions.FirstOrDefault(w => w.Distance(CursorPos) <= 250 && Q.IsInRange(w));

                    if (JumpPlace != default(AIBaseClient))
                    {
                        Q.Cast(JumpPlace);
                    }
                    var Ward2 = ObjectManager.Get<AIBaseClient>().FirstOrDefault(a => a.IsAlly && a.Distance(CursorPos) < 300);
                    if (Ward2 != null)
                    {
                        Q.Cast(Ward2);
                    }
                    else if (JumpWard() != default(InventorySlot))
                    {
                        var Ward = JumpWard();
                        //CursorPos = _Player.Position.Extend(CursorPos, 600).ToVector2();
                        ObjectManager.Player.Spellbook.CastSpell(Ward.SpellSlot,CursorPos);
                        WardJump(CursorPos);
                    }
                }
            }
        }

        public static void WardJump(Vector3 cursorpos)
        {
            var jumpPos = Game.CursorPos;
            var Ward = ObjectManager.Get<AIBaseClient>().FirstOrDefault(a => a.IsAlly && a.Distance(cursorpos) < 300);
            if (Ward != null)
            {
                Q.Cast(Ward);
            }
        }

        public static ItemId[] WardIds = {ItemId.Warding_Totem, ItemId.Greater_Stealth_Totem_Trinket, ItemId.Greater_Vision_Totem_Trinket, ItemId.Sightstone, ItemId.Ruby_Sightstone, (ItemId) 2043, (ItemId)3340, (ItemId)2303,
                (ItemId) 2049, (ItemId) 2045};

        public static InventorySlot JumpWard()
        {
            return WardIds.Select(wardId => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == wardId)).FirstOrDefault(slot => slot != null && ObjectManager.Player.Spellbook.CanUseSpell(slot.SpellSlot) == SpellState.Ready);
        }

        public static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(Q.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health + target.PhysicalShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(target);
                    }
                }

                if (Ignite != null && KillStealMenu["ign"].GetValue<MenuBool>().Enabled && Ignite.IsReady())
                {
                    if (target.Health <= _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite) && (ObjectManager.Player.GetAutoAttackDamage(target) < target.Health || !target.IsValidTarget(ObjectManager.Player.GetRealAutoAttackRange())))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
