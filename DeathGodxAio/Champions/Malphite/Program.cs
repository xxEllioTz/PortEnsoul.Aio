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

namespace Malphite
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, JungleClearMenu, LaneClearMenu, KillStealMenu, Drawings;
        public static Font Thm;
        public static Font Thn;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

  

       public static void MalphiteOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Malphite")) return;
            Game.Print("Doctor's Malphite Loaded! Ported by DEATGODx", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 250);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 1000);
            //, SkillShotType.Circular, 250, 700, 270);
            Thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 32, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Thn = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 20, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Ignite = new Spell(_Player.GetSpellSlot("summonerdot"), 600);
            var Menumalp = new Menu("Malphite", "Malphite", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSlider("DisQ", "Use [Q] If Enemy Distance >", 10, 0, 650));
            ComboMenu.Add(new MenuSeparator("[Q] Distance < 125 = Always [Q]", "[Q] Distance < 125 = Always [Q]"));
            ComboMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            ComboMenu.Add(new MenuKeyBind("ComboFQ", "Use [R] Selected Target", System.Windows.Forms.Keys.T, KeyBindType.Press)).Permashow();
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Aoe"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Hit Enemies Use [R] Aoe", 3, 1, 5));
            ComboMenu.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            ComboMenu.Add(new MenuBool("inter", "Use [R] Interrupt", false));
            Menumalp.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSlider("DisQ2", "Use [Q] If Enemy Distance >", 350, 0, 650));
            HarassMenu.Add(new MenuSeparator("[Q] Distance < 125 = Always [Q]", "[Q] Distance < 125 = Always [Q]"));
            HarassMenu.Add(new MenuSlider("ManaQ", "Mana Harass", 40));
            Menumalp.Add(HarassMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("JungleMana", "Mana JungleClear", 20));
            Menumalp.Add(JungleClearMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("LaneClearQ", "Use [Q] LaneClear"));
            LaneClearMenu.Add(new MenuBool("LaneClearW", "Use [W] LaneClear"));
            LaneClearMenu.Add(new MenuBool("LaneClearE", "Use [E] LaneClear"));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana LaneClear", 50));
            LaneClearMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LaneClearMenu.Add(new MenuBool("LastHitQ", "Use [Q] LastHit"));
            LaneClearMenu.Add(new MenuSlider("ManaLH", "Mana LastHit", 50));
            Menumalp.Add(LaneClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            KillStealMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuSlider("minKsR", "Min [R] Distance KillSteal", 100, 1, 1000));
            KillStealMenu.Add(new MenuKeyBind("RKb", "[R] Semi Manual Key", System.Windows.Forms.Keys.Y, KeyBindType.Toggle)).Permashow();
            KillStealMenu.Add(new MenuSeparator("Recommended Distance 600", "Recommended Distance 600"));
            Menumalp.Add(KillStealMenu);
            Drawings = new Menu("Draw Settings", "Draw");
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "[Q] Range"));
            Drawings.Add(new MenuBool("DrawE", "[E] Range"));
            Drawings.Add(new MenuBool("DrawR", "[R] Range"));
            Drawings.Add(new MenuBool("DrawRhit", "[R] Draw Hit"));
            Drawings.Add(new MenuBool("Notifications", "Alerter Can Killable [R]"));
            Drawings.Add(new MenuBool("Draw_Disabled", "Disabled Drawings"));
            Menumalp.Add(Drawings);
            Menumalp.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterrupterSpell += Interupt;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;

            if (Drawings["Draw_Disabled"].GetValue<MenuBool>().Enabled && Q.IsReady()) return;

            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Drawings["DrawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Drawings["DrawR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
            }

            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (Drawings["Notifications"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (target != null && target.IsValidTarget(R.Range) && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health + target.PhysicalShield)
                {
                    DrawFont(Thm, "R Can Killable " + target.CharacterName, (float)(ft[0] - 140), (float)(ft[1] + 80), SharpDX.Color.Red);
                }
            }

            if (Drawings["DrawRhit"].GetValue<MenuBool>().Enabled && target != null && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var RPred = R.GetPrediction(target);
                var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
                if (RPred.CastPosition.CountEnemyHeroesInRange(250) >= MinR)
                {
                    Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                    DrawFont(Thm, "[R] Can Hit " + RPred.CastPosition.CountEnemyHeroesInRange(250), (float)(ft[0] - 90), (float)(ft[1] + 20), SharpDX.Color.Orange);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
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
            }
            KillSteal();
            RSelect();

            if (ComboMenu["ComboFQ"].GetValue<MenuKeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void Combo()
        {
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var disQ = ComboMenu["DisQ"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && disQ < target.Distance(ObjectManager.Player))
                {
                    Q.Cast(target);
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast();
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var RPred = R.GetPrediction(target);
                    if (RPred.CastPosition.CountEnemyHeroesInRange(250) >= MinR && RPred.Hitchance >= HitChance.High)
                    {
                        R.Cast(RPred.CastPosition);
                    }
                }
            }
        }

        private static void RSelect()
        {
            var targetF = TargetSelector.SelectedTarget;
            var useFQ = ComboMenu["ComboFQ"].GetValue<MenuKeyBind>().Active;

            if (targetF == null)
            {
                return;
            }

            if (useFQ && R.IsReady())
            {
                if (targetF.IsValidTarget(R.Range))
                {
                    R.Cast(targetF.Position);
                }
            }
        }

        private static void LaneClear()
        {
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var useQ = LaneClearMenu["LaneClearQ"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["LaneClearW"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["LaneClearE"].GetValue<MenuBool>().Enabled;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (minion != null)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && minion.Health < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }

                if (useW && W.IsReady() && minion.IsValidTarget(W.Range) && minion.IsValidTarget(Q.Range))
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range) && minion.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
            }
        }

        private static void LastHit()
        {
            var mana = LaneClearMenu["ManaLH"].GetValue<MenuSlider>().Value;
            var useQ = LaneClearMenu["LastHitQ"].GetValue<MenuBool>().Enabled;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (ObjectManager.Player.ManaPercent < mana) return;

            if (minion != null)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) && _Player.Distance(minion) > 175)
                {
                    Q.Cast(minion);
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["JungleMana"].GetValue<MenuSlider>().Value;
            var monters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(R.Range));
            if (ObjectManager.Player.ManaPercent <= mana) return;

            if (monters != null)
            {
                if (useQ && Q.IsReady() && monters.IsValidTarget(Q.Range))
                {
                    Q.Cast(monters);
                }

                if (useE && E.IsReady() && monters.IsValidTarget(E.Range))
                {
                    E.Cast();
                }

                if (useW && W.IsReady() && monters.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        private static void Harass()
        {
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var ManaQ = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var disQ = HarassMenu["DisQ2"].GetValue<MenuSlider>().Value;
            if (ObjectManager.Player.ManaPercent < ManaQ) return;

            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && disQ <= target.Distance(ObjectManager.Player))
                {
                    Q.Cast(target);
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
            }
        }

        private static void Flee()
        {

        }

        public static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = ComboMenu["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && R.IsReady() && i.DangerLevel == Interrupter.DangerLevel.High && R.IsInRange(sender))
            {
                R.Cast(sender.Position);
            }
        }

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            var minKsR = KillStealMenu["minKsR"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(R.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health + target.PhysicalShield <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(target);
                    }
                }

                if (KsE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (target.Health + target.PhysicalShield <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.E))
                    {
                        E.Cast();
                    }
                }

                if (KsR && R.IsReady())
                {
                    if (target.Health + target.PhysicalShield <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) && !target.IsValidTarget(minKsR))
                    {
                        R.Cast(target);
                    }
                }

                if (R.IsReady() && KillStealMenu["RKb"].GetValue<MenuKeyBind>().Active)
                {
                    if (target.Health + target.PhysicalShield <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(pred.CastPosition);
                        }
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
