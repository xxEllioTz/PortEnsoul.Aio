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

namespace Kassadin7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, LastHitMenu, JungleClearMenu, KillStealMenu, Misc;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        //public static Item Seraph;
        public static Spell Ignite;
        public static Font thm;


       public static void KassadinOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Kassadin")) return;
            Game.Print("Doctor's Kassadin Loaded! PORT by DEATHGODX", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 700);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 15, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            //Seraph = new Item(3040, 3040);
            var MenuKas = new Menu("Doctor's Kassadin", "Kassadin", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Combo"));
            ComboMenu.Add(new MenuKeyBind("CTurret", "Don't Use [R] UnderTurret", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            ComboMenu.Add(new MenuSlider("CMinR", "Limit Enemies Around Use [R]", 2, 1, 5));
            ComboMenu.Add(new MenuSlider("Cihp", "MyHP Use [R] >", 20));
            MenuKas.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSlider("ManaHR", "Mana For Harass", 40));
            HarassMenu.Add(new MenuSeparator("Ultimate Settings", "Ultimate Settings"));
            HarassMenu.Add(new MenuBool("HarassR", "Use [R] Harass"));
            HarassMenu.Add(new MenuSlider("StackRH", "Use [R] Stacks Limit Harass", 5, 1, 5));
            HarassMenu.Add(new MenuSlider("MinR", "Limit Enemies Around Use [R]", 3, 1, 5));
            HarassMenu.Add(new MenuSlider("ihp", "MyHP Use [R] >", 30));
            MenuKas.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", ("Lane Clear Settings")));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear", false));
            LaneClearMenu.Add(new MenuBool("WLC", "Use [W] LaneClear"));
            LaneClearMenu.Add(new MenuBool("ELC", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("MinELC", "Min Hit Minions Use [E]", 2, 1, 3));
            LaneClearMenu.Add(new MenuBool("RLC", "Use [R] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("StackRL", "Use [R] Stacks Limit LaneClear", 1, 1, 5));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana For LaneClear", 50));
            MenuKas.Add(LaneClearMenu);
            LastHitMenu = new Menu("LastHit Settings", "LastHit");
            LastHitMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LastHitMenu.Add(new MenuBool("QLH", "Use [Q] LastHit"));
            LastHitMenu.Add(new MenuSlider("WLH", "Use [W] LastHit"));
            LastHitMenu.Add(new MenuSlider("ManaLH", "Mana For LaneClear", 50));
            MenuKas.Add(LastHitMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuBool("RJungle", "Use [R] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("StackRJ", "Use [R] Stacks Limit LaneClear", 3, 1, 5));
            JungleClearMenu.Add(new MenuSlider("ManaJC", "Mana For JungleClear", 30));
            MenuKas.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal", false));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuKas.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5" }) { Index = 0 });
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawR", "R Range", false));
            Misc.Add(new MenuBool("DrawQ", "Q Range"));
            Misc.Add(new MenuBool("DrawE", "E Range", false));
            Misc.Add(new MenuBool("DrawTR", "DrawText Status [R]"));
            Misc.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            Misc.Add(new MenuBool("inter", "Use [Q] Interupt"));
            Misc.Add(new MenuBool("AntiGap", "Use [E] Anti Gapcloser"));
            Misc.Add(new MenuSeparator("Seraph Settings", "Seraph Settings"));
            Misc.Add(new MenuBool("dts", "Use Seraph"));
            Misc.Add(new MenuSlider("Hp", "HP For Seraph", 30, 0, 100));
            MenuKas.Add(Misc);
            MenuKas.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterrupterSpell += Interupt;
            Orbwalker.OnAction += ResetAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawR"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Purple, 1);
            }

            if (Misc["DrawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Purple, 1);
            }

            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Purple, 1);
            }

            if (Misc["DrawTR"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active)
                {
                    DrawFont(thm, "Use R Under Turret : Disable", (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.White);
                }
                else
                {
                    DrawFont(thm, "Use R Under Turret : Enable", (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.Red);
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
            Dtc();

            /* if (_Player.SkinId != Misc["skin.Id"].GetValue<MenuList>().Index)
             {
                 if (checkSkin())
                 {
                     Player.SetSkinId(SkinId());
                 }
             }*/
        }

        public static int SkinId()
        {
            return Misc["skin.Id"].GetValue<MenuList>().Index;
        }

        public static bool checkSkin()
        {
            return Misc["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        public static bool EReady
        {
            get { return ObjectManager.Player.HasBuff("ForcePulseE"); }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var turret = ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active;
            var minR = ComboMenu["CMinR"].GetValue<MenuSlider>().Value;
            var Minhp = ComboMenu["Cihp"].GetValue<MenuSlider>().Value;
            if (target != null)
            {
                if (useE && E.IsReady() && target.IsValidTarget(Q.Range) && EReady)
                {
                    E.Cast(target);
                }
                if (W.IsReady() && target.IsValidTarget(W.Range) && useW)
                {
                    W.Cast(target);
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (_Player.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange(target) && !W.IsReady())
                    {
                        Q.Cast(target);
                    }
                    else if (_Player.Distance(target) > ObjectManager.Player.GetRealAutoAttackRange(target))
                    {
                        Q.Cast(target);
                    }
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range) && target.Position.CountEnemyHeroesInRange(R.Range) <= minR && _Player.HealthPercent >= Minhp)
                {
                    if (turret)
                    {
                        if (!UnderTuret(target))
                        {
                            R.Cast(target);
                        }
                    }
                    else
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(300, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var HasW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useW && W.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    W.Cast(target);
                }

                if (HasW && W.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    W.Cast(target);
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["WLC"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["ELC"].GetValue<MenuBool>().Enabled;
            var useR = LaneClearMenu["RLC"].GetValue<MenuBool>().Enabled;
            var MinE = LaneClearMenu["MinELC"].GetValue<MenuSlider>().Value;
            var minRs = LaneClearMenu["StackRL"].GetValue<MenuSlider>().Value;
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var minionQ = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(E.Range) && e.IsMinion())
                           .Cast<AIBaseClient>().ToList();
            var qFarmLocation = W.GetLineFarmLocation(minionQ, E.Width);
            if (qFarmLocation.Position.IsValid())
                foreach (var minions in minionQ)
                {
                    if (useW && W.IsReady() && minions.IsValidTarget(275) && minions.InAutoAttackRange()
                    && ObjectManager.Player.Distance(minions.Position) <= 225f
                    && ObjectManager.Player.GetSpellDamage(minions, SpellSlot.W) + ObjectManager.Player.GetAutoAttackDamage(minions)
                    >= minions.Health + minions.AllShield)
                    {
                        W.Cast();
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minions);
                    }


                    if (ObjectManager.Player.ManaPercent < mana) return;
                    if (useQ && Q.IsReady() && minions.IsValidTarget(Q.Range) && ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q) > minions.Health + minions.AllShield && (_Player.Distance(minions) > 225 || !W.IsReady()))
                    {
                        Q.Cast(minions);
                    }

                    if (useE && E.IsReady() && minions.IsValidTarget(E.Range) && qFarmLocation.MinionsHit >= MinE && EReady)
                    {
                        E.Cast(qFarmLocation.Position);
                    }

                    if (useR && R.IsReady() && minions.IsValidTarget(R.Range) && !UnderTuret(minions) && ObjectManager.Player.GetBuffCount("RiftWalk") < minRs)
                    {
                        R.Cast(minions);
                    }
                }
        }

        private static void LastHit()
        {
            var useQ = LastHitMenu["QLH"].GetValue<MenuBool>().Enabled;
            var useW = LastHitMenu["WLH"].GetValue<MenuBool>().Enabled;
            var mana = LastHitMenu["ManaLH"].GetValue<MenuSlider>().Value;
            var minion = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (minion != null)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health + minion.AllShield
                && ObjectManager.Player.ManaPercent >= mana && (_Player.Distance(minion) > 225 || !W.IsReady()))
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

        private static bool UnderTuret(AIBaseClient target)
        {
            var tower = ObjectManager.Get<AITurretClient>().FirstOrDefault(turret => turret != null && turret.Distance(target) <= 775 && turret.IsValid && turret.Health > 0 && !turret.IsAlly);
            return tower != null;
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var useR = HarassMenu["HarassR"].GetValue<MenuBool>().Enabled;
            var minRs = HarassMenu["StackRH"].GetValue<MenuSlider>().Value;
            var turret = ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active;
            var minR = HarassMenu["MinR"].GetValue<MenuSlider>().Value;
            var Minhp = HarassMenu["ihp"].GetValue<MenuSlider>().Value;
            var mana = HarassMenu["ManaHR"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (target != null)
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && EReady)
                {
                    E.Cast(target);
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsDead)
                {
                    if (_Player.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange(target) && !W.IsReady())
                    {
                        Q.Cast(target);
                    }

                    else if (_Player.Distance(target) > ObjectManager.Player.GetRealAutoAttackRange(target))
                    {
                        Q.Cast(target);
                    }
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range) && target.Position.CountEnemyHeroesInRange(R.Range) <= minR && _Player.HealthPercent >= Minhp && ObjectManager.Player.GetBuffCount("RiftWalk") < minRs)
                {
                    if (turret)
                    {
                        if (!UnderTuret(target))
                        {
                            R.Cast(target);
                        }
                    }
                    else
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var useR = JungleClearMenu["RJungle"].GetValue<MenuBool>().Enabled;
            var minRs = JungleClearMenu["StackRJ"].GetValue<MenuSlider>().Value;
            var mana = JungleClearMenu["ManaJC"].GetValue<MenuSlider>().Value;
            var jungleMonsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (jungleMonsters != null)
            {
                if (useQ && Q.IsReady() && jungleMonsters.IsValidTarget(Q.Range))
                {
                    Q.Cast(jungleMonsters);
                }

                if (useW && W.IsReady() && jungleMonsters.IsValidTarget(275) && jungleMonsters.InAutoAttackRange() && ObjectManager.Player.Distance(jungleMonsters.Position) <= 225f)
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && jungleMonsters.IsValidTarget(E.Range) && EReady)
                {
                    E.Cast(jungleMonsters);
                }

                if (useR && R.IsReady() && jungleMonsters.IsValidTarget(R.Range) && ObjectManager.Player.GetBuffCount("RiftWalk") < minRs)
                {
                    R.Cast(jungleMonsters);
                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float jx, float jy, ColorBGRA jc)
        {
            vFont.DrawText(null, vText, (int)jx, (int)jy, jc);
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Misc["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) < 300 && EReady)
            {
                E.Cast(sender);
            }
        }

        public static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = Misc["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && Q.IsReady() && i.DangerLevel == DangerLevel.High && Q.IsInRange(sender))
            {
                Q.Cast(sender);
            }
        }

        private static void Flee()
        {
            if (R.IsReady())
            {
                R.Cast();
            }
        }

        private static void Dtc()
        {
            if (!_Player.IsDead && Misc["dts"].GetValue<MenuBool>().Enabled)
            {
                if (_Player.HealthPercent <= Misc["Hp"].GetValue<MenuSlider>().Value && _Player.Position.CountEnemyHeroesInRange(R.Range) >= 1)
                {

                }
            }
        }

        private static void KillSteal()
        {
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(Q.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsE && E.IsReady() && target.IsValidTarget(E.Range) && EReady)
                {
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) && EReady)
                    {
                        E.Cast(target);
                    }
                }

                if (KsR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        R.Cast(target);
                    }
                }

                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(target);
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
