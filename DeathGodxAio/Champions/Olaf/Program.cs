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

namespace Olaf7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, LastHitMenu, JungleClearMenu, Misc, KillStealMenu, Skin, Drawings;
        public static Item Botrk;
        public static Item Bil;
        public static Item Hydra;
        public static Item Tiamat;
        public static Item Titanic;
        public static GameObject Axe { get; set; }
        public static float Time { get; set; }
        public static float End { get; set; }
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

      
       public static void OlafOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Olaf")) return;
            Game.Print("Doctor's Olaf Loaded! PORTED by DEATHGODx", Color.Orange);
            Q = new Spell(SpellSlot.Q, 1000);//, SkillShotType.Linear, 250, 1550, 75);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 450);
            Bil = new Item(3144, 475f);
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Item(ItemId.Titanic_Hydra, ObjectManager.Player.GetRealAutoAttackRange());
            var MenuOlaf = new Menu("Olaf", "Olaf", true);
            MenuOlaf.Add(new MenuSeparator("Doctor7", "Doctor7"));
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q]"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W]"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E]"));
            ComboMenu.Add(new MenuSeparator("Items Settings", "Items Settings"));
            ComboMenu.Add(new MenuBool("item", "Use [BOTRK]"));
            ComboMenu.Add(new MenuBool("hyd", "Use [Hydra] Reset AA"));
            ComboMenu.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            ComboMenu.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuOlaf.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q]"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W]", false));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E]"));
            HarassMenu.Add(new MenuSlider("ManaQ", "Min Mana For Harass", 40));
            MenuOlaf.Add(HarassMenu);
            Misc = new Menu("Ultimate Settings", "Misc");
            Misc.Add(new MenuBool("Ulti", "Use Ultimate"));
            Misc.Add(new MenuSeparator("Use [R] On", "Use [R] On"));
            Misc.Add(new MenuBool("stun", "Stuns"));
            Misc.Add(new MenuBool("rot", "Root"));
            Misc.Add(new MenuBool("knockup", "Knock Ups"));
            Misc.Add(new MenuBool("tunt", "Taunt"));
            Misc.Add(new MenuBool("charm", "Charm", false));
            Misc.Add(new MenuBool("snare", "Snare"));
            Misc.Add(new MenuBool("sleep", "Sleep", false));
            Misc.Add(new MenuBool("blind", "Blinds", false));
            Misc.Add(new MenuBool("disarm", "Disarm", false));
            Misc.Add(new MenuBool("fear", "Fear", false));
            Misc.Add(new MenuBool("silence", "Silence", false));
            Misc.Add(new MenuBool("frenzy", "Frenzy", false));
            Misc.Add(new MenuBool("supperss", "Supperss", false));
            Misc.Add(new MenuBool("slow", "Slows", false));
            Misc.Add(new MenuBool("poison", "Poisons", false));
            Misc.Add(new MenuBool("knockback", "Knock Backs", false));
            Misc.Add(new MenuBool("nearsight", "NearSight", false));
            Misc.Add(new MenuBool("poly", "Polymorph", false));
            Misc.Add(new MenuSeparator("Ultimate Setting", "Ultimate Setting"));
            Misc.Add(new MenuSlider("healulti", "Min Health Use [R]", 60));
            Misc.Add(new MenuSlider("Rulti", "Min Enemies Around Use [R]", 1, 1, 5));
            Misc.Add(new MenuSeparator("Ultimate Delay", "Ultimate Delay"));
            Misc.Add(new MenuSlider("delay", "Humanizer Delay", 0, 0, 1000));
            MenuOlaf.Add(Misc);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("LaneClear Settings", "LaneClear Settings"));
            LaneClearMenu.Add(new MenuBool("ClearQ", "Use [Q]"));
            LaneClearMenu.Add(new MenuBool("CantLC", "Only [Q] If Orbwalker Cant Killable Minion", false));
            LaneClearMenu.Add(new MenuSlider("minQ", "Min Hit Minions Use [Q]", 3, 1, 6));
            LaneClearMenu.Add(new MenuBool("ClearE", "Use [E]"));
            LaneClearMenu.Add(new MenuBool("ClearW", "Use [W]", false));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Min Mana For LaneClear", 60));
            MenuOlaf.Add(LaneClearMenu);
            LastHitMenu = new Menu("LastHit Settings", "LastHit");
            LastHitMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LastHitMenu.Add(new MenuBool("LastQ", "Use [Q] LastHit", false));
            LastHitMenu.Add(new MenuBool("LhAA", "Only [Q] If Orbwalker Cant Killable Minion"));
            LastHitMenu.Add(new MenuBool("LastE", "Use [E] LastHit"));
            LastHitMenu.Add(new MenuSlider("LhMana", "Min Mana For LastHit", 60));
            MenuOlaf.Add(LastHitMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q]"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W]"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E]"));
            JungleClearMenu.Add(new MenuSlider("MnJungle", "Min Mana JungleClear", 30));
            MenuOlaf.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsIgnite", "Use [Ignite] KillSteal"));
            MenuOlaf.Add(KillStealMenu);
            Skin = new Menu("Skin Changer", "SkinChanger");
            Skin.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Skin.Add(new MenuBool("checkSkin", "Use Skin Changer"));
            Skin.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6" }) { Index = 0 });
            MenuOlaf.Add(Skin);
            Drawings = new Menu("Misc Settings", "Draw");
            Drawings.Add(new MenuSeparator("Misc Setting", "Misc Setting"));
            Drawings.Add(new MenuBool("QStun", "Use [Q] If Enemy Has CC", false));
            Drawings.Add(new MenuBool("AntiGap", "Use [Q] Anti Gapcloser"));
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "Q Range"));
            Drawings.Add(new MenuBool("DrawE", "E Range", false));
            Drawings.Add(new MenuBool("Axe", "Axe Draw"));
            MenuOlaf.Add(Drawings);
            MenuOlaf.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Orbwalker.OnAction += Orbwalker_CantLasthit;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Drawings["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Drawings["Axe"].GetValue<MenuBool>().Enabled && Axe != null)
            {
                Render.Circle.DrawCircle(Axe.Position, 600, Color.Orange, 6);
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
            RStun();
            Ult();

            /* if (_Player.SkinId != Skin["skin.Id"].GetValue<MenuList>().Index)
             {
                 if (checkSkin())
                 {
                     Player.SetSkinId(SkinId());
                 }
             }*/
        }

        public static int SkinId()
        {
            return Skin["skin.Id"].GetValue<MenuList>().Index;
        }

        public static bool checkSkin()
        {
            return Skin["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var item = ComboMenu["item"].GetValue<MenuBool>().Enabled;
            var Minhp = ComboMenu["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = ComboMenu["ihpp"].GetValue<MenuSlider>().Value;
            if (target != null)
            {
                var pos = Q.GetPrediction(target).CastPosition.Extend(ObjectManager.Player.Position, -80);
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (_Player.Distance(target) > 375)
                    {
                        Q.Cast(pos.ToVector2());
                    }
                    else
                    {
                        Q.Cast(target.Position);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(E.Range) && E.IsReady() == false)
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) )
                {
                    E.Cast(target);
                }

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
            var target = TargetSelector.GetTarget(325, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useriu = ComboMenu["hyd"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
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

                if ((useE && E.IsReady()) && Orbwalker.ActiveMode == OrbwalkerMode.Combo && target.IsValidTarget(325) && _Player.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange(target))
                {
                    E.Cast(target);
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Drawings["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) < 300 && !sender.CharacterName.ToLower().Contains("MasterYi"))
            {
                Q.Cast(sender);
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["MnJungle"].GetValue<MenuSlider>().Value;
            var jungle = GameObjects.Jungle.Where(j => j.IsValidTarget(Q.Range)).OrderByDescending(a => a.MaxHealth).FirstOrDefault();

            if (jungle != null)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(jungle) && ObjectManager.Player.ManaPercent >= mana)
                {
                    Q.Cast(jungle.Position);
                }

                if (useW && W.IsReady() && jungle.IsValidTarget(325) && ObjectManager.Player.ManaPercent >= mana)
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && E.IsInRange(jungle))
                {
                    E.Cast(jungle);
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["ClearQ"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["ClearW"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["ClearE"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var MinQ = LaneClearMenu["minQ"].GetValue<MenuSlider>().Value;
            var minionE = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range) && e.IsMinion())
                           .Cast<AIBaseClient>().ToList();
            var quang = W.GetLineFarmLocation(minionE, W.Width);
            if (quang.Position.IsValid())
                foreach (var minion in minionE)
                {
                    if (useW && W.IsReady() && ObjectManager.Player.ManaPercent >= mana && minion.IsValidTarget(E.Range))
                    {
                        W.Cast();
                    }

                    if (useE && E.IsReady() && !minion.IsValidTarget(_Player.AttackRange) && minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E))
                    {
                        E.Cast(minion);
                    }

                    if (useQ && Q.IsReady() && ObjectManager.Player.ManaPercent >= mana && minion.IsValidTarget(600))
                    {
                        Q.Cast(minion.Position);
                    }
                }
        }

        private static void Orbwalker_CantLasthit(object target, OrbwalkerActionArgs args)
        {
            var useCant = LaneClearMenu["CantLC"].GetValue<MenuBool>().Enabled;
            var laneQMN = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var useAA = LastHitMenu["LhAA"].GetValue<MenuBool>().Enabled;
            var LhM = LastHitMenu["LhMana"].GetValue<MenuSlider>().Value;
            var unit = (useCant && Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && ObjectManager.Player.ManaPercent >= laneQMN)
            || (useAA && Orbwalker.ActiveMode == OrbwalkerMode.LastHit && ObjectManager.Player.ManaPercent >= LhM);
            var targe = TargetSelector.GetTarget(Q.Range);
            if (target == null)
            {
                return;
            }

            if (unit && Q.IsReady() && targe.IsValidTarget(Q.Range))
            {
                /*if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) >= HealthPrediction.Health.GetPrediction(target, Q.CastDelay))
                {
                    Q.Cast(target);
                }*/
            }
        }

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target.Position);
                }
            }
        }

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var ManaQ = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                var pos = Q.GetPrediction(target).CastPosition.Extend(ObjectManager.Player.Position, -80);
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && ObjectManager.Player.ManaPercent >= ManaQ)
                {
                    if (_Player.Distance(target) > 375)
                    {
                        Q.Cast(pos);
                    }
                    else
                    {
                        Q.Cast(target.Position);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(325) && ObjectManager.Player.ManaPercent >= ManaQ)
                {
                    W.Cast();
                }

                if (E.IsReady() && useE && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }
        }

        public static void LastHit()
        {
            var useE = LastHitMenu["LastE"].GetValue<MenuBool>().Enabled;
            var useQ = LastHitMenu["LastQ"].GetValue<MenuBool>().Enabled;
            var LhM = LastHitMenu["LhMana"].GetValue<MenuSlider>().Value;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (minion != null)
            {
                if (useQ && Q.IsReady() && ObjectManager.Player.ManaPercent > LhM && minion.IsValidTarget(Q.Range) && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health + minion.AllShield)
                {
                    Q.Cast(minion);
                }

                if (useE && E.IsReady() && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E) >= minion.Health + minion.AllShield)
                {
                    E.Cast(minion);
                }
            }
        }

        public static void RStun()
        {
            var Rstun = Drawings["QStun"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Rstun && Q.IsReady())
            {
                if (target != null)
                {
                    if (target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                    {
                        Q.Cast(target.Position);
                    }
                }
            }
        }

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("olaf_base_q_axe"))
            {
                Axe = sender;
                Time = Game.Time;
                End = Game.Time + 8;
            }
        }

        public static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("olaf_base_q_axe"))
            {
                Axe = null;
            }
        }

        public static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(Q.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady())
                {
                    var pos = Q.GetPrediction(target).CastPosition.Extend(ObjectManager.Player.Position, -80);
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(pos);/*pos.To3DWorld()*/
                    }
                }

                if (KsE && E.IsReady())
                {
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.E))
                    {
                        E.Cast(target);
                    }
                }

                if (Ignite != null && KillStealMenu["KsIgnite"].GetValue<MenuBool>().Enabled && Ignite.IsReady())
                {
                    if (target.Health <= _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }

        private static void Ult()
        {
            var ulti = Misc["Ulti"].GetValue<MenuBool>().Enabled;
            var heal = Misc["healulti"].GetValue<MenuSlider>().Value;
            var minR = Misc["Rulti"].GetValue<MenuSlider>().Value;
            var cc = (Misc["silence"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Silence))
            || (Misc["snare"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Snare))
            || (Misc["supperss"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Suppression))
            || (Misc["sleep"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Sleep))
            || (Misc["poly"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Polymorph))
            || (Misc["frenzy"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Frenzy))
            || (Misc["disarm"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Disarm))
            || (Misc["nearsight"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.NearSight))
            || (Misc["knockback"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Knockback))
            || (Misc["knockup"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Knockup))
            || (Misc["slow"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Slow))
            || (Misc["poison"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Poison))
            || (Misc["blind"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Blind))
            || (Misc["charm"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Charm))
            || (Misc["tunt"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
            || (Misc["stun"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Stun))
            || (Misc["rot"].GetValue<MenuBool>().Enabled)
            || (Misc["fear"].GetValue<MenuBool>().Enabled && ObjectManager.Player.HasBuffOfType(BuffType.Fear));
            if (R.IsReady() && ulti && cc && _Player.Position.CountEnemyHeroesInRange(800) >= minR && ObjectManager.Player.HealthPercent <= heal)
            {
                R.Cast();
            }
        }
    }
}
