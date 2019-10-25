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

namespace Hecarim7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, Auto, JungleClearMenu, LaneClearMenu, KillStealMenu, Skin, Drawings;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Font Thm;
        public static Font thn;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

    
       public static void HecarimOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Hecarim")) return;
            Game.Print("Doctor's Hecarim Loaded! Ported by DEATHGODx", Color.Orange);
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 1000);//, SkillShotType.Linear, 250, 800, 200);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 20, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            thn = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 22, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            var MenuHeca = new Menu("Hecarim", "Hecarim", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSeparator("Ultimate Aoe Settings", "Ultimate Aoe Settings"));
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Aoe"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 3, 0, 5));
            ComboMenu.Add(new MenuSeparator("Ultimate Selected Target Settings", "Ultimate Selected Target Settings"));
            ComboMenu.Add(new MenuKeyBind("ComboSL", "Use [R] On Selected Target", System.Windows.Forms.Keys.T, KeyBindType.Press));
            ComboMenu.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            ComboMenu.Add(new MenuBool("inter", "Use [R] Interrupt"));
            MenuHeca.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuSlider("ManaQ", "Min Mana Harass [Q]", 40));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass", false));
            HarassMenu.Add(new MenuSlider("ManaW", "Min Mana Harass [W]", 40));
            MenuHeca.Add(HarassMenu);
            Auto = new Menu("Auto Harass Settings", "Auto Harass");
            Auto.Add(new MenuSeparator("Auto Harass Settings", "Auto Harass Settings"));
            Auto.Add(new MenuBool("AutoQ", "Auto [Q]"));
            Auto.Add(new MenuSlider("ManaQ", "Min Mana Auto [Q]", 60));
            MenuHeca.Add(Auto);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("JungleMana", "Min Mana JungleClear", 20));
            MenuHeca.Add(JungleClearMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LaneClearMenu.Add(new MenuBool("LastQ", "Use [Q] LastHit"));
            LaneClearMenu.Add(new MenuSlider("LhMana", "Min Mana Lasthit [Q]", 60));
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("LastQLC", "Always [Q] LaneClear (Keep Passive Q)"));
            LaneClearMenu.Add(new MenuBool("CantLC", "Only [Q] Killable Minion", false));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Min Mana [Q] LaneClear", 50));
            LaneClearMenu.Add(new MenuBool("LastWLC", "Use [W] LaneClear"));
            LaneClearMenu.Add(new MenuSlider("ManaLCW", "Min Mana [W] LaneClear", 70));
            MenuHeca.Add(LaneClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsW", "Use [W] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            KillStealMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuSlider("minKsR", "Use [R] KillSteal If Enemy Distance >", 100, 1, 1000));
            KillStealMenu.Add(new MenuSeparator("Distance < 125 = Always ,Recommended Distance 500", "Distance < 125 = Always ,Recommended Distance 500"));
            MenuHeca.Add(KillStealMenu);
            Skin = new Menu("Skin Changer", "SkinChanger");
            Skin.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Skin.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5" }) { Index = 0 });
            MenuHeca.Add(Skin);
            Drawings = new Menu("Draw Settings", "Draw");
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "[Q] Range"));
            Drawings.Add(new MenuBool("DrawW", "[W] Range", false));
            Drawings.Add(new MenuBool("DrawR", "[R] Range"));
            Drawings.Add(new MenuBool("DrawT", "Draw [E] Time"));
            Drawings.Add(new MenuBool("DrawRhit", "[R] Draw Hit"));
            MenuHeca.Add(Drawings);
            MenuHeca.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterrupterSpell += Interupt;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Drawings["DrawW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }

            if (Drawings["DrawR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
            }

            if (Drawings["DrawT"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ObjectManager.Player.HasBuff("HecarimRamp"))
                {
                    DrawFont(thn, "E Time : " + ETime(ObjectManager.Player), (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.GreenYellow);
                }
            }

            var target = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
            if (Drawings["DrawRhit"].GetValue<MenuBool>().Enabled && target != null && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var RPred = R.GetPrediction(target);
                var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;

                if (RPred.CastPosition.CountEnemyHeroesInRange(250) >= MinR)
                {
                    Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                    DrawFont(Thm, "Ultimate Can Hit : " + RPred.CastPosition.CountEnemyHeroesInRange(250), (float)(ft[0] - 70), (float)(ft[1] + 20), SharpDX.Color.Orange);
                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
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

            if (ComboMenu["ComboSL"].GetValue<MenuKeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            KillSteal();
            AutoQ();
            RSelected();
        }

        public static int SkinId()
        {
            return Skin["skin.Id"].GetValue<MenuList>().Index;
        }

        public static bool checkSkin()
        {
            return Skin["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        public static float ETime(AIBaseClient target)
        {
            if (target.HasBuff("HecarimRamp"))
            {
                return Math.Max(0, target.GetBuff("HecarimRamp").EndTime) - Game.Time;
            }
            return 0;
        }

        private static void Combo()
        {
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.IsDead))
            {
                if (useE && E.IsReady() && target.IsValidTarget(700))
                {
                    E.Cast();
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var RPred = R.GetPrediction(target);

                    if (RPred.CastPosition.CountEnemyHeroesInRange(250) >= MinR && RPred.Hitchance >= HitChance.High)
                    {
                        R.Cast(RPred.CastPosition);
                    }
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
            }
        }

        private static void RSelected()
        {
            var targetS = TargetSelector.SelectedTarget;
            var useSL = ComboMenu["ComboSL"].GetValue<MenuKeyBind>().Active;
            if (targetS == null) return;

            if (useSL && R.IsReady() && targetS.IsValidTarget(R.Range))
            {
                var RPred = R.GetPrediction(targetS);
                if (RPred.Hitchance >= HitChance.High)
                {
                    R.Cast(RPred.CastPosition);
                }
            }
        }

        private static void LaneClear()
        {
            var laneQMN = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var laneWMN = LaneClearMenu["ManaLCW"].GetValue<MenuSlider>().Value;
            var useQLH = LaneClearMenu["LastQLC"].GetValue<MenuBool>().Enabled;
            var useQ = LaneClearMenu["CantLC"].GetValue<MenuBool>().Enabled;
            var useWLH = LaneClearMenu["LastWLC"].GetValue<MenuBool>().Enabled;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= W.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (minion != null)
            {
                if (useQLH && Q.IsReady() && minion.IsValidTarget(Q.Range) && ObjectManager.Player.ManaPercent >= laneQMN)
                {
                    Q.Cast();
                }

                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && minion.Health < QDamage(minion) && ObjectManager.Player.ManaPercent >= laneQMN)
                {
                    Q.Cast();
                }

                if (useWLH && W.IsReady() && minion.IsValidTarget(W.Range) && ObjectManager.Player.ManaPercent >= laneWMN)
                {
                    W.Cast();
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["JungleMana"].GetValue<MenuSlider>().Value;
            var jungleMonsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(R.Range));
            if (ObjectManager.Player.ManaPercent <= mana) return;
            if (jungleMonsters != null)
            {
                if (useQ && Q.IsReady() && Q.IsInRange(jungleMonsters))
                {
                    Q.Cast();
                }

                if (useE && E.IsReady() && jungleMonsters.IsValidTarget(800))
                {
                    E.Cast();
                }

                if (useW && W.IsReady() && jungleMonsters.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var ManaQ = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var ManaW = HarassMenu["ManaW"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(W.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && ObjectManager.Player.ManaPercent >= ManaQ)
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range) && ObjectManager.Player.ManaPercent >= ManaW)
                {
                    W.Cast();
                }
            }
        }

        public static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = ComboMenu["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && R.IsReady() && i.DangerLevel == DangerLevel.High && R.IsInRange(sender))
            {
                R.Cast(sender.Position);
            }
        }

        private static void AutoQ()
        {
            var useQ = Auto["AutoQ"].GetValue<MenuBool>().Enabled;
            var mana = Auto["ManaQ"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && !Tru(_Player.Position) && ObjectManager.Player.ManaPercent >= mana && target.IsValidTarget(Q.Range) && Orbwalker.ActiveMode != OrbwalkerMode.Combo && Orbwalker.ActiveMode != OrbwalkerMode.Harass)
                {
                    Q.Cast();
                }
            }
        }

        private static void Flee()
        {
            if (E.IsReady())
            {
                E.Cast();
            }
        }

        public static bool Tru(Vector3 position)
        {
            return ObjectManager.Get<AITurretClient>().Any(turret => turret.IsValidTarget(950) && turret.IsEnemy);
        }

        private static void LastHit()
        {
            var useQ = LaneClearMenu["LastQ"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["LhMana"].GetValue<MenuSlider>().Value;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (minion != null)
            {
                if (useQ && Q.IsReady() && ObjectManager.Player.ManaPercent >= mana && minion.IsValidTarget(Q.Range) && minion.Health < QDamage(minion))
                {
                    Q.Cast();
                }
            }
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 150, 250, 350 }[R.Level] + 1.0f * _Player.FlatMagicDamageMod));
        }

        public static double QDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 35, 60, 85, 110, 130 }[Q.Level] + 0.4f * _Player.FlatPhysicalDamageMod));
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsW = KillStealMenu["KsW"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            var minKsR = KillStealMenu["minKsR"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(R.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health + target.AllShield <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast();
                    }
                }

                if (KsW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (target.Health + target.AllShield <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.W))
                    {
                        W.Cast();
                    }
                }

                if (KsR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (target.Health + target.AllShield <= RDamage(target) && (!target.IsValidTarget(minKsR) || ObjectManager.Player.HealthPercent <= 25))
                    {
                        var RPred = R.GetPrediction(target);
                        if (RPred.Hitchance >= HitChance.High)
                        {
                            R.Cast(RPred.CastPosition);
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
