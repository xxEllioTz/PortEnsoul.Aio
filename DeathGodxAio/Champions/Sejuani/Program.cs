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
using SharpDX.Direct3D9;
using static EnsoulSharp.SDK.Interrupter;

namespace Sejuani7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, JungleClearMenu, LaneClearMenu, KillStealMenu, Skin, Drawings;
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
        public static Spell F;
        public static Spell Ignite;

      
       public static void SejuaniOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Sejuani")) return;
            Game.Print("Doctor's Sejuani Loaded! Ported by DeathGODx", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 850);// SkillShotType.Linear, 0, 1600, 70);
            Q.SetSkillshot(0, 1600, 70, false,false, SkillshotType.Line);
            W = new Spell(SpellSlot.W, 600);
           // W.SetSkillshot(0, 1600, 70, false, false, SkillshotType.Line);
            E = new Spell(SpellSlot.E, 590);
            R = new Spell(SpellSlot.R, 1200);// SkillShotType.Linear, );
            R.SetSkillshot(250, 1600, 110, false,false, SkillshotType.Line);
            F = new Spell(_Player.GetSpellSlot("summonerflash"), 425);//, SkillShotType.Linear, 0, int.MaxValue, 60);
            F.SetSkillshot(0, int.MaxValue, 60, false, false, SkillshotType.Line);
            //F.AllowedCollisionCount = int.MaxValue;
            Thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 32, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Thn = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 20, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Ignite = new Spell(_Player.GetSpellSlot("summonerdot"), 600);
            var MenuSeju = new Menu("Sejuani", "Sejuani", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuKeyBind("ComboFQ", "[Q] + [Flash] Target ", System.Windows.Forms.Keys.T, KeyBindType.Press));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuSlider("DisQ", "Use [Q] If Enemy Distance >", 10, 0, 650));
            ComboMenu.Add(new MenuSeparator("[Q] Distance < 125 = Always [Q]", "[Q] Distance < 125 = Always [Q]"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSlider("DisE", "Use [E] If Enemy Distance > ", 10, 0, 1000));
            ComboMenu.Add(new MenuSeparator("[E] Distance < 125 = Always [E]", "[E] Distance < 125 = Always [E]"));
            ComboMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Combo"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Hit Enemies Use [R]", 3, 0, 5));
            ComboMenu.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            ComboMenu.Add(new MenuBool("inter", "Use [R] Interrupt", false));
            ComboMenu.Add(new MenuBool("interQ", "Use [Q] Interrupt", false));
            MenuSeju.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            HarassMenu.Add(new MenuSlider("DisQ2", "Use [Q] If Enemy Distance >", 350, 0, 650));
            HarassMenu.Add(new MenuSeparator("[Q] Distance < 125 = Always [Q]", "[Q] Distance < 125 = Always [Q]"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSlider("DisE2", "Use [E] If Enemy Distance >", 350, 0, 1000));
            HarassMenu.Add(new MenuSeparator("[E] Distance < 125 = Always [E]", "[E] Distance < 125 = Always [E]"));
            HarassMenu.Add(new MenuSlider("ManaQ", "Min Mana Harass", 40));
            MenuSeju.Add(HarassMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("JungleMana", "Min Mana JungleClear", 20));
            MenuSeju.Add(JungleClearMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("LastQLC", "Always [Q] LaneClear"));
            LaneClearMenu.Add(new MenuBool("CantLC", "Only [Q] Killable Minion", false));
            LaneClearMenu.Add(new MenuBool("LastWLC", "Use [W] LaneClear"));
            LaneClearMenu.Add(new MenuBool("LaneE", "Use [E] LaneClear"));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Min Mana LaneClear", 50));
            MenuSeju.Add(LaneClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            KillStealMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuSlider("minKsR", "Min [R] Distance KillSteal", 100, 1, 1175));
            KillStealMenu.Add(new MenuKeyBind("RKb", "[R] Semi Manual Key", System.Windows.Forms.Keys.Y, KeyBindType.Toggle));
            KillStealMenu.Add(new MenuSeparator("Recommended Distance 600", "Recommended Distance 600"));
            MenuSeju.Add(KillStealMenu);
            Skin = new Menu("Skin Changer", "SkinChanger");
            Skin.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Skin.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6" }) {Index = 0 });
            MenuSeju.Add(Skin);
            Drawings = new Menu("Draw Settings", "Draw");
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "[Q] Range"));
            Drawings.Add(new MenuBool("DrawW", "[W] Range"));
            Drawings.Add(new MenuBool("DrawE", "[E] Range"));
            Drawings.Add(new MenuBool("DrawR", "[R] Range"));
            Drawings.Add(new MenuBool("DrawRhit", "[R] Draw Hit"));
            Drawings.Add(new MenuBool("Notifications", "Notifications Killable [R]"));
            Drawings.Add(new MenuBool("Draw_Disabled", "Disabled Drawings"));
            MenuSeju.Add(Drawings);
            MenuSeju.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterrupterSpell += Interupt;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;
            if (Drawings["Draw_Disabled"].GetValue<MenuBool>().Enabled)
                return;
            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }
            if (Drawings["DrawW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
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
                if (target != null && target.IsValidTarget(R.Range) && RDamage(target) > target.Health + target.PhysicalShield)
                {
                    DrawFont(Thm, "R Can Killable " + target.CharacterName, (float)(ft[0] - 140), (float)(ft[1] + 80), SharpDX.Color.Red);
                }
            }
            if (Drawings["DrawRhit"].GetValue<MenuBool>().Enabled && target != null && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var RPred = R.GetPrediction(target);
                var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
                if (RPred.CastPosition.CountEnemyHeroesInRange(400) >= MinR)
                {
                    Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                    DrawFont(Thm, "[R] Can Hit " + RPred.CastPosition.CountEnemyHeroesInRange(400), (float)(ft[0] - 90), (float)(ft[1] + 20), SharpDX.Color.Orange);
                }
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
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
            KillSteal();
            FlashQ();
            if (ComboMenu["ComboFQ"].GetValue<MenuKeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos); //hata verirse Cursoru sil
            }
        }

        public static int SkinId()
        {
            return Skin["skin.Id"].GetValue<MenuList>().Index;
        }
        public static bool checkSkin()
        {
            return Skin["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        private static void Combo()
        {
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var disE = ComboMenu["DisE"].GetValue<MenuSlider>().Value;
            var disQ = ComboMenu["DisQ"].GetValue<MenuSlider>().Value;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && disQ <= target.Distance(ObjectManager.Player))
                {
                    Q.Cast(target);
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && disE <= target.Distance(ObjectManager.Player))
                {
                    E.CastOnUnit(target);
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var RPred = R.GetPrediction(target);
                    if (RPred.CastPosition.CountEnemyHeroesInRange(400) >= MinR && RPred.Hitchance >= HitChance.High)
                    {
                        R.Cast(RPred.CastPosition);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target.Position);
                }
            }
        }


        private static void FlashQ()
        {
            var targetF = TargetSelector.SelectedTarget;
            var useFQ = ComboMenu["ComboFQ"].GetValue<MenuKeyBind>().Active;
            if (targetF != null)
            {
                if (useFQ && Q.IsReady() && F.IsReady() && targetF.IsValidTarget(1000))
                {
                   Q.Cast(targetF.Position);
                }
                if (ObjectManager.Player.HasBuff("SejuaniArcticAssault") && targetF.IsValidTarget(425))
                {
                    ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.GetSpellSlot("summonerflash"));
                }
            }
        }


        private static void LaneClear()
        {
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var useQLH = LaneClearMenu["LastQLC"].GetValue<MenuBool>().Enabled;
            var useQ = LaneClearMenu["CantLC"].GetValue<MenuBool>().Enabled;
            var useWLH = LaneClearMenu["LastWLC"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["LaneE"].GetValue<MenuBool>().Enabled;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= E.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (minion != null)
            {
                if (useQLH && Q.IsReady() && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast(minion);
                }

                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && minion.Health < QDamage(minion))
                {
                    Q.Cast(minion);
                }

                if (useWLH && W.IsReady() && minion.IsValidTarget(W.Range))
                {
                    W.Cast(minion);
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(minion);
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
                    E.CastOnUnit(monters);
                }

                if (useW && W.IsReady() && monters.IsValidTarget(W.Range))
                {
                    W.Cast(monters.Position);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var disE = HarassMenu["DisE2"].GetValue<MenuSlider>().Value;
            var disQ = HarassMenu["DisQ2"].GetValue<MenuSlider>().Value;
            var ManaQ = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            if (ObjectManager.Player.ManaPercent < ManaQ) return;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && disQ <= target.Distance(ObjectManager.Player))
                {
                    Q.Cast(target);
                }
                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target.Position);
                }
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && disE <= target.Distance(ObjectManager.Player))
                {
                    E.CastOnUnit(target);
                }
            }
        }

        private static void Flee()
        {

        }

        public static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = ComboMenu["inter"].GetValue<MenuBool>().Enabled;
            var InterQ = ComboMenu["interQ"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }
            if (Inter && R.IsReady() && i.DangerLevel == DangerLevel.High && R.IsInRange(sender))
            {
                R.Cast(sender.Position);
            }
            if (InterQ && Q.IsReady() && i.DangerLevel ==  DangerLevel.High && Q.IsInRange(sender))
            {
                Q.Cast(sender.Position);
            }
        }

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 150, 250, 350 }[R.Level] + 0.8f * _Player.FlatMagicDamageMod));
        }

        public static double QDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 80, 125, 170, 215, 260 }[Q.Level] + 0.4f * _Player.FlatMagicDamageMod));
        }

        public static double EDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 80, 105, 130, 155, 180 }[E.Level] + 0.5f * _Player.FlatMagicDamageMod));
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
                    if (target.Health + target.PhysicalShield <= QDamage(target))
                    {
                        Q.Cast(target);
                    }
                }

                if (KsE && E.IsReady() && target.IsValidTarget(E.Range) && target.HasBuff("SejuaniFrost"))
                {
                    if (target.Health + target.PhysicalShield <= EDamage(target))
                    {
                        E.Cast();
                    }
                }

                if (KsR && R.IsReady())
                {
                    if (target.Health + target.PhysicalShield <= RDamage(target) && !target.IsValidTarget(minKsR))
                    {
                        R.Cast(target);
                    }
                }

                if (R.IsReady() && KillStealMenu["RKb"].GetValue<MenuKeyBind>().Active)
                {
                    if (target.Health + target.PhysicalShield <= RDamage(target))
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
