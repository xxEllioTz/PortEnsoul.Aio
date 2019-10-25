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

namespace Graves7
{
    internal static class Program
    {
        private static Menu Menu, ComboMenu, HarassMenu, ClearMenu, JungleMenu, Drawings, KillStealMenu, Items, Misc;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Item Botrk;
        public static Item Bil;
        public static Item Youmuu;
        public static Font Thm;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static MenuSlider ComboMode;

    
        public static void GravesLoading_OnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Graves")) return;
            Game.Print("Doctor's Graves Loaded! PORTED by DeathGODX", Color.Orange);
            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(250, 2000, 60, false, false, SkillshotType.Line);
            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(250, 1650, 150, false, false, SkillshotType.Circle);
            E = new Spell(SpellSlot.E, 425);
            E.SetSkillshot(250, 1650, 150, false, false, SkillshotType.Line);
            R = new Spell(SpellSlot.R, 1500);
            R.SetSkillshot(250, 2100, 100, false, false, SkillshotType.Circle);
            Thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 32, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 450);
            Bil = new Item(3144, 475f);
            Youmuu = new Item(3142, 10);
            var MenuGraves = new Menu("Doctor's Graves7", "Graves7", true);
            ComboMenu = new Menu("Combo Settings", "ComboMenu");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMode = ComboMenu.Add(new MenuSlider("comboMode", "Min Stack Use [E] Reload", 1, 0, 1));
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Aoe In Combo", false));
            ComboMenu.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 2, 1, 5));
            MenuGraves.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "HarassMenu");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuSeparator("Harass [Q] On", "Harass [Q] On"));
            foreach (var Selector in GameObjects.EnemyHeroes)
            {
                HarassMenu.Add(new MenuBool("haras" + Selector.CharacterName, "" + Selector.CharacterName));
            }
            HarassMenu.Add(new MenuSlider("HarassMana", "Min Mana Harass [Q]", 50));
            HarassMenu.Add(new MenuSeparator("Spells Settings", "Spells Settings"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W]", false));
            HarassMenu.Add(new MenuSlider("ManaW", "Mana Harass [W]", 40));
            HarassMenu.Add(new MenuBool("HarassAA", "Use [E] Reset AA", false));
            HarassMenu.Add(new MenuSlider("ManaHarass", "Mana [E] Harass", 50));
            MenuGraves.Add(HarassMenu);
            ClearMenu = new Menu("Laneclear Settings", "LaneClearMenu");
            ClearMenu.Add(new MenuSeparator("Laneclear Settings", "Laneclear Settings"));
            ClearMenu.Add(new MenuBool("QClear", "Use [Q]"));
            ClearMenu.Add(new MenuSlider("minQ", "Min Hit Minion [Q]", 3, 1, 6));
            ClearMenu.Add(new MenuSlider("ClearMana", "Min Mana For [Q] LaneClear", 70));
            ClearMenu.Add(new MenuBool("LaneAA", "Use [E] Reset AA", false));
            ClearMenu.Add(new MenuSlider("ELane", "Min Mana For [E] LaneClear", 70));
            MenuGraves.Add(ClearMenu);
            JungleMenu = new Menu("JungleClear Settings", "JungleMenu");
            JungleMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleMenu.Add(new MenuBool("QJungleClear", "Use [Q]"));
            JungleMenu.Add(new MenuSlider("JungleMana", "Min Mana For [Q] JungleClear", 30));
            JungleMenu.Add(new MenuBool("WJungleClear", "Use [W]"));
            JungleMenu.Add(new MenuSlider("JungleManaW", "Min Mana For [W] JungleClear", 50));
            JungleMenu.Add(new MenuBool("JungleAA", "Use [E]"));
            JungleMenu.Add(new MenuSlider("EJung", "Min Mana For [E] JungleClear", 30));
            MenuGraves.Add(JungleMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillStealMenu");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsW", "Use [W] KillSteal"));
            KillStealMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuSlider("minKsR", "Min [R] Range KillSteal", 100, 1, 1500));
            KillStealMenu.Add(new MenuKeyBind("RKb", "[R] Semi Manual Key", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            MenuGraves.Add(KillStealMenu);
            Items = new Menu("Items Settings", "Items");
            Items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Items.Add(new MenuBool("you", "Use [Youmuu]"));
            Items.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            Items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuGraves.Add(Items);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Misc Settings", "Misc Settings"));
            Misc.Add(new MenuBool("AntiGap", "Use [E] AntiGap"));
            Misc.Add(new MenuBool("AntiGapW", "Use [W] AntiGap"));
            Misc.Add(new MenuBool("QStun", "Use [Q] Immoblie"));
            Misc.Add(new MenuSeparator("Skin Changer", "Skin Changer"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6", "7", "8" }) { Index = 0 });
            MenuGraves.Add(Misc);
            Drawings = new Menu("Drawings Settings", "DrawingMenu");
            Drawings.Add(new MenuSeparator("Drawings", "Drawings"));
            Drawings.Add(new MenuBool("DrawQ", "[Q] Range"));
            Drawings.Add(new MenuBool("DrawW", "[W] Range", false));
            Drawings.Add(new MenuBool("DrawE", "[E] Range", false));
            Drawings.Add(new MenuBool("DrawR", "[R] Range"));
            Drawings.Add(new MenuBool("Draw_Disabled", "Disabled Drawings"));
            Drawings.Add(new MenuBool("Notifications", "Alerter Can Kill [R]"));
            MenuGraves.Add(Drawings);
            MenuGraves.Attach();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnAction += ResetAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }
            KillSteal();
            //QStun();
            Item();
        }

        public static int SkinId()
        {
            return Misc["skin.Id"].GetValue<MenuList>().Index;
        }

        public static bool checkSkin()
        {
            return Misc["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Drawings["Draw_Disabled"].GetValue<MenuBool>().Enabled) return;

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

            if (Drawings["Notifications"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (target.IsValidTarget(R.Range) && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health + target.PhysicalShield)
                {
                    DrawFont(Thm, "R Can Killable " + target.CharacterName, (float)(ft[0] - 140), (float)(ft[1] + 80), SharpDX.Color.Red);
                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void Item()
        {
            var item = Items["BOTRK"].GetValue<MenuBool>().Enabled;
            var yous = Items["you"].GetValue<MenuBool>().Enabled;
            var Minhp = Items["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = Items["ihpp"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(550) && !e.IsDead))
            {
                if (item && Bil.IsReady && Bil.IsOwned() && Bil.IsInRange(target))
                {
                    Bil.Cast(target);
                }
                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }

                if (yous && Youmuu.IsReady && Youmuu.IsOwned() && target.IsValidTarget(530) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    Youmuu.Cast();
                }
            }
        }
        // Thanks MarioGK has allowed me to use some his logic
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Misc["AntiGap"].GetValue<MenuBool>().Enabled && E.IsReady() && sender.IsEnemy && sender.IsVisible && sender.IsValidTarget(E.Range))
            {
                E.Cast((sender.Position));
            }

            if (Misc["AntiGapW"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) <= 300)
            {
                W.Cast(sender);
            }
        }

        private static void Flee()
        {
        }

        private static void ResetAttack(object target, OrbwalkerActionArgs args)
        {
            var useJ = JungleMenu["JungleAA"].GetValue<MenuBool>().Enabled;
            var manaJ = JungleMenu["EJung"].GetValue<MenuSlider>().Value;
            var useL = ClearMenu["LaneAA"].GetValue<MenuBool>().Enabled;
            var mana = ClearMenu["ELane"].GetValue<MenuSlider>().Value;

            if (useJ && E.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && ObjectManager.Player.ManaPercent >= manaJ)
            {
                E.Cast( Game.CursorPos);
                Orbwalker.ResetAutoAttackTimer();
            }

            if (useL && E.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && ObjectManager.Player.ManaPercent >= mana)
            {
                E.Cast(Game.CursorPos);
                Orbwalker.ResetAutoAttackTimer();
            }

            if (HarassMenu["HarassAA"].GetValue<MenuBool>().Enabled && E.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Harass && ObjectManager.Player.ManaPercent >= HarassMenu["ManaHarass"].GetValue<MenuSlider>().Value)
            {
                E.Cast( Game.CursorPos);
                Orbwalker.ResetAutoAttackTimer();
            }
        }

        public static void Combo()
        {
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;

            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.IsDead && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

                if (useE && E.IsReady())
                {
                    if (ComboMode.Value == 1 && !ObjectManager.Player.HasBuff("GravesBasicAttackAmmo2") && _Player.Position.CountEnemyHeroesInRange(R.Range) >= 1)
                    {
                        E.Cast( Game.CursorPos);
                    }
                    //Olmazsa CursorPosCenteri düzelt Sil)
                    if (ComboMode.Value == 0 && !ObjectManager.Player.HasBuff("GravesBasicAttackAmmo2") && !ObjectManager.Player.HasBuff("GravesBasicAttackAmmo1") && _Player.Position.CountEnemyHeroesInRange(R.Range) >= 1)
                    {
                        E.Cast( Game.CursorPos);
                    }
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.CastPosition.CountEnemyHeroesInRange(R.Range) >= MinR && pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var ManaW = HarassMenu["ManaW"].GetValue<MenuSlider>().Value;
            if (ObjectManager.Player.ManaPercent < ManaW) return;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(W.Range) && !e.IsDead && !e.IsDead))
            {
                if (useQ && Q.IsReady() && HarassMenu["haras" + target.CharacterName].GetValue<MenuBool>().Enabled)
                {
                    var predQ = Q.GetPrediction(target);
                    if (predQ.Hitchance >= HitChance.Medium)
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = ClearMenu["QClear"].GetValue<MenuBool>().Enabled;
            var ManaQ = ClearMenu["ClearMana"].GetValue<MenuSlider>().Value;
            var MinQ = ClearMenu["minQ"].GetValue<MenuSlider>().Value;
            var minionQ = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range) && e.IsMinion())
                .Cast<AIBaseClient>().ToList();
            var quang = W.GetLineFarmLocation(minionQ, Q.Width);

            foreach (var minion in minionQ)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && ObjectManager.Player.ManaPercent > ManaQ && quang.MinionsHit >= MinQ)
                {
                    Q.Cast(quang.Position);
                }
            }
        }

        private static void JungleClear()
        {
            var useQ = JungleMenu["QJungleClear"].GetValue<MenuBool>().Enabled;
            var useW = JungleMenu["WJungleClear"].GetValue<MenuBool>().Enabled;
            var jungMana = JungleMenu["JungleMana"].GetValue<MenuSlider>().Value;
            var jungManaW = JungleMenu["JungleManaW"].GetValue<MenuSlider>().Value;
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (monster == null && _Player.ManaPercent < jungMana) return;

            if (Q.IsReady() && useQ && monster.Distance(_Player) <= Q.Range)
            {
                Q.Cast(monster);
            }

            if (W.IsReady() && useW && monster.Distance(_Player) <= W.Range)
            {
                W.Cast(monster);
            }
        }

        public static void QStun()
        {
            var Qstun = Misc["Qstun"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                if (Qstun && Q.IsReady())
                {
                    if (target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                    {
                        Q.Cast(target.Position);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsW = KillStealMenu["KsW"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.HasBuff("BlitzcrankManaBarrierCD") && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health + target.PhysicalShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        var Qpred = Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(Qpred.CastPosition);
                        }
                    }
                }

                if (KsW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (target.Health + target.PhysicalShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.W))
                    {
                        var Wpred = W.GetPrediction(target);
                        if (Wpred.Hitchance >= HitChance.VeryHigh)
                        {
                            W.Cast(Wpred.CastPosition);
                        }
                    }
                }

                var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
                var minKsR = KillStealMenu["minKsR"].GetValue<MenuSlider>().Value;

                if (KsR && R.IsReady())
                {
                    if (target.Health + target.PhysicalShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) && target.IsValidTarget(R.Range) && !target.IsValidTarget(minKsR))
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }

                if (R.IsReady() && KillStealMenu["RKb"].GetValue<MenuKeyBind>().Active)
                {
                    if (target.Health + target.PhysicalShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.Hitchance>= HitChance.VeryHigh)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
