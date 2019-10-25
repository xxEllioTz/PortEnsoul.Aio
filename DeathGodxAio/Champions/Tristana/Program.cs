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

namespace Tristana
{
    static class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Font Thm;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Menu Menu, SpellMenu, JungleMenu, HarassMenu, LaneMenu, Misc;


        // Menu

        public static void TristanaOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Tristana")) return;
            Game.Print("Doctor's Tristana Loaded! PORTED by DEATHGODx", Color.Orange);
            uint level = (uint)ObjectManager.Player.Level;
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            //W.SetSkillshot(450, int.MaxValue, 180, false, false, SkillshotType.Circle);
            E = new Spell(SpellSlot.E, 550 + level * 7);
            R = new Spell(SpellSlot.R, 550 + level * 7);
            Thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 32, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            var MenuTris = new Menu("Tristana", "Tristana", true);
            SpellMenu = new Menu("Combo Settings", "Combo");
            SpellMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            SpellMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            SpellMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            SpellMenu.Add(new MenuSeparator("Combo [E] On", "Combo [E] On"));
            foreach (var target in GameObjects.EnemyHeroes)
            {
                SpellMenu.Add(new MenuBool("useECombo" + target.CharacterName, "" + target.CharacterName));
            }
            SpellMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            SpellMenu.Add(new MenuBool("ERKs", "KillSteal [ER]"));
            SpellMenu.Add(new MenuBool("RKs", "Automatic [R] KillSteal"));
            SpellMenu.Add(new MenuKeyBind("RKb", "Semi Manual [R] KillSteal", System.Windows.Forms.Keys.U, KeyBindType.Toggle));
            SpellMenu.Add(new MenuSeparator("[W] KillSteal Settings", "[W] KillSteal Settings"));
            SpellMenu.Add(new MenuBool("WKs", "Use [W] KillSteal", false));
            SpellMenu.Add(new MenuBool("CTurret", "Dont Use [W] KillSteal Under Turet"));
            SpellMenu.Add(new MenuSlider("Attack", "Use [W] KillSteal If Can Kill Enemy With x Attack", 2, 1, 6));
            SpellMenu.Add(new MenuSlider("MinW", "Use [W] KillSteal If Enemies Around Target <=", 2, 1, 5));
            SpellMenu.Add(new MenuSeparator("Always Use [W] KillSteal If MenuSlider Enemies Around = 5", "Always Use [W] KillSteal If MenuSlider Enemies Around = 5"));
            MenuTris.Add(SpellMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass", false));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSeparator("Use [E] On", "Use [E] On"));
            foreach (var target in GameObjects.EnemyHeroes)
            {
                HarassMenu.Add(new MenuBool("HarassE" + target.CharacterName, "" + target.CharacterName));
            }
            HarassMenu.Add(new MenuSlider("manaHarass", "Min Mana For Harass", 50, 0, 100));
            MenuTris.Add(HarassMenu);
            LaneMenu = new Menu("Laneclear Settings", "Clear");
            LaneMenu.Add(new MenuSeparator("Laneclear Settings", "Laneclear Settings"));
            LaneMenu.Add(new MenuBool("ClearQ", "Use [Q] Laneclear", false));
            LaneMenu.Add(new MenuBool("ClearE", "Use [E] Laneclear", false));
            LaneMenu.Add(new MenuSlider("manaFarm", "Min Mana For LaneClear", 50, 0, 100));
            MenuTris.Add(LaneMenu);
            JungleMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleMenu.Add(new MenuBool("jungleQ", "Use [Q] JungleClear"));
            JungleMenu.Add(new MenuBool("jungleE", "Use [E] JungleClear"));
            JungleMenu.Add(new MenuBool("jungleW", "Use [W] JungleClear", false));
            JungleMenu.Add(new MenuSlider("manaJung", "Min Mana For JungleClear", 50, 0, 100));
            MenuTris.Add(JungleMenu);
            Misc = new Menu("Misc Settings", "Draw");
            Misc.Add(new MenuSeparator("Anti Gapcloser", "Anti Gapcloser"));
            Misc.Add(new MenuBool("antiGap", "Anti Gapcloser", false));
            Misc.Add(new MenuBool("antiRengar", "Anti Rengar"));
            Misc.Add(new MenuBool("antiKZ", "Anti Kha'Zix"));
            Misc.Add(new MenuBool("inter", "Use [R] Interupt", false));
            Misc.Add(new MenuSeparator("Drawings Settings", "Drawings Settings"));
            Misc.Add(new MenuBool("Draw_Disabled", "Disabled Drawings", false));
            Misc.Add(new MenuBool("DrawE", "Draw Attack Range"));
            Misc.Add(new MenuBool("DrawW", "Draw [W]", false));
            Misc.Add(new MenuBool("Notifications", "Alerter Can Kill With [R]"));
            MenuTris.Add(Misc);
            MenuTris.Attach();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterrupterSpell += Interupt;
            GameObject.OnCreate += GameObject_OnCreate;

        }

        // Damage

        public static double EDamage(AIBaseClient target)
        {
            double Edamage = 0;
            if (target.HasBuff("tristanaecharge"))
            {
                Edamage += (float)(ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) * (target.GetBuffCount("tristanaecharge") * 0.30)) + ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }

            return Edamage;
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 300, 400, 500 }[R.Level] + 1.0f * _Player.FlatMagicDamageMod));
        }

        public static double GetDamage(AIHeroClient target)
        {
            if (target != null)
            {
                /*float Damage = 0;

                if (E.IsLearned) { Damage += EDamage(target); }
                if (R.IsLearned) { Damage += RDamage(target); }

                return Damage;*/
            }
            return 0;
        }

        // Flee Mode

        private static void Flee()
        {

        }

        // Interrupt

        private static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = Misc["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && R.IsReady() && i.DangerLevel == DangerLevel.Medium && R.IsInRange(sender))
            {
                R.Cast(sender);
            }
        }

        //Harass Mode

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["manaHarass"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && target.HealthPercent >= 10 && _Player.ManaPercent >= mana)
                {
                    if (HarassMenu["HarassE" + target.CharacterName].GetValue<MenuBool>().Enabled)
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        //Combo Mode

        private static void Combo()
        {
            var useQ = SpellMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useE = SpellMenu["ComboE"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && target.HealthPercent >= 10)
                {
                    if (SpellMenu["useECombo" + target.CharacterName].GetValue<MenuBool>().Enabled)
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        //LaneClear Mode

        private static void LaneClear()
        {
            var useQ = LaneMenu["ClearQ"].GetValue<MenuBool>().Enabled;
            var useE = LaneMenu["ClearE"].GetValue<MenuBool>().Enabled;
            var mana = LaneMenu["manaFarm"].GetValue<MenuSlider>().Value;
            foreach (var minion in GameObjects.EnemyMinions.Where(e => e.IsValidTarget(E.Range)))
            {
                if (useE && E.IsReady() && minion.HealthPercent >= 70 && minion.IsValidTarget(E.Range) && _Player.ManaPercent >= mana)
                {
                    E.Cast(minion);
                }

                if (useQ && Q.IsReady() && minion.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }
            }
        }

        // JungleClear Mode

        private static void JungleClear()
        {
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(E.Range));
            var useQ = JungleMenu["jungleQ"].GetValue<MenuBool>().Enabled;
            var useW = JungleMenu["jungleW"].GetValue<MenuBool>().Enabled;
            var useE = JungleMenu["jungleE"].GetValue<MenuBool>().Enabled;
            var mana = JungleMenu["manaJung"].GetValue<MenuSlider>().Value;
            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }

                if (_Player.ManaPercent < mana) return;

                if (useW && W.IsReady() && monster.IsValidTarget(W.Range))
                {
                    W.Cast(monster.Position);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.Cast(monster);
                }
            }
        }

        private static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        // Anti Rengar

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var rengar = GameObjects.EnemyHeroes.Find(e => e.CharacterName.Equals("Rengar"));
            var khazix = GameObjects.EnemyHeroes.Find(e => e.CharacterName.Equals("Khazix"));
            if (rengar != null)
            {
                if (sender.Name == ("Rengar_LeapSound.troy") && Misc["antiRengar"].GetValue<MenuBool>().Enabled && R.IsReady() && sender.Position.Distance(_Player) <= 300)
                {
                    R.Cast(rengar);
                }
            }

            if (khazix != null)
            {
                if (sender.Name == ("Khazix_Base_E_Tar.troy") && Misc["antiKZ"].GetValue<MenuBool>().Enabled && R.IsReady() && sender.Position.Distance(_Player) <= 300)
                {
                    R.Cast(khazix);
                }
            }
        }

        private static void Gapcloser_OnGapCloser(AIBaseClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Misc["antiGap"].GetValue<MenuBool>().Enabled && R.IsReady() && sender.Distance(_Player) <= 325)
            {
                R.Cast(sender);
            }
        }

        // KillSteal

        private static void KillSteal()
        {
            var target = GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(W.Range) && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsDead);
            var RKill = SpellMenu["RKs"].GetValue<MenuBool>().Enabled;
            var WKill = SpellMenu["WKs"].GetValue<MenuBool>().Enabled;
            var WAttack = SpellMenu["Attack"].GetValue<MenuSlider>().Value;
            var minW = SpellMenu["MinW"].GetValue<MenuSlider>().Value;
            foreach (var target2 in target)
            {
                if (R.IsReady() && target2.IsValidTarget(R.Range) && (RKill || SpellMenu["RKb"].GetValue<MenuKeyBind>().Active))
                {
                    if (target2.Health + target2.AllShield <= RDamage(target2))
                    {
                        R.Cast(target2);
                    }
                }

                if (WKill && W.IsReady() && target2.IsValidTarget(W.Range))
                {
                    if (target2.Health + target2.AllShield <= ObjectManager.Player.GetAutoAttackDamage(target2) * WAttack && ObjectManager.Player.HealthPercent >= 10 && target2.Position.CountEnemyHeroesInRange(600) <= minW)
                    {
                        var turret = SpellMenu["CTurret"].GetValue<MenuBool>().Enabled;
                        if (target2.HasBuff("tristanaecharge"))
                        {
                            if (target2.Health + target2.AllShield > EDamage(target2))
                            {
                                if (turret)
                                {
                                    if (!target2.Position.UnderTuret())
                                    {
                                        W.Cast(target2.Position);
                                    }
                                }
                                else
                                {
                                    W.Cast(target2.Position);
                                }
                            }
                        }
                        else
                        {
                            if (turret)
                            {
                                if (!target2.Position.UnderTuret())
                                {
                                    W.Cast(target2.Position);
                                }
                            }
                            else
                            {
                                W.Cast(target2.Position);
                            }
                        }
                    }
                }

                if (SpellMenu["ERKs"].GetValue<MenuBool>().Enabled && R.IsReady() && target2.IsValidTarget(R.Range) && target2.HasBuff("tristanaecharge"))
                {
                    if (target2.Health + target2.AllShield <= GetDamage(target2))
                    {
                        R.Cast(target2);
                    }
                }
            }
        }

        // Drawings

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;
            if (Misc["Draw_Disabled"].GetValue<MenuBool>().Enabled) return;
            if (Misc["DrawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }
            if (Misc["DrawW"].GetValue<MenuBool>().Enabled && W.IsReady() && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }
            if (Misc["Notifications"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (target.IsValidTarget(1000) && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health + target.PhysicalShield)
                {
                    DrawFont(Thm, "[R] Can Killable " + target.CharacterName, (float)(ft[0] - 140), (float)(ft[1] + 80), SharpDX.Color.Red);
                }
            }
        }

        // Under Turet

        private static bool UnderTuret(this Vector3 position)
        {
            return GameObjects.EnemyTurrets.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(position) < 950);
        }

        // Game Update

        private static void Game_OnUpdate(EventArgs args)
        {
            uint level = (uint)ObjectManager.Player.Level;
            E = new Spell(SpellSlot.E, 550 + level * 7);
            R = new Spell(SpellSlot.R, 550 + level * 7);

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

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }

            KillSteal();
        }
    }
}
