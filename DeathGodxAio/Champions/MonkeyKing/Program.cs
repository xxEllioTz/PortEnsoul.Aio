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

namespace Doctor_s_WuKong
{
    static class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, Ulti, LaneClearMenu, JungleClearMenu, KillStealMenu, Misc;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Font thm;
        public static Item Hydra;
        public static Item Tiamat;
        public static Spell Ignite;

  

       public static void MonkeyKingOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("MonkeyKing")) return;
            Game.Print("Doctor's Wukong Loaded! PORTED By DeathGODx", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 450);
            Q.SetSkillshot(0, 2000, 900, false, false, SkillshotType.Circle);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 375);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 15, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            var MenuWuk = new Menu("Doctor's Wukong", "Wukong", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSlider("DisE", "Use [E] If Enemy Distance >", 250, 0, 650));
            ComboMenu.Add(new MenuKeyBind("CTurret", "Don't Use [E] UnderTurret", System.Windows.Forms.Keys.T, KeyBindType.Toggle)).Permashow();
            ComboMenu.Add(new MenuSeparator("Items Settings", "Items Settings"));
            ComboMenu.Add(new MenuBool("hydra", "Use [Hydra] Reset AA"));
            MenuWuk.Add(ComboMenu);
            Ulti = new Menu("Ultimate Settings", "Ulti");
            Ulti.Add(new MenuSeparator("Ultimate Enemies In Count", "Ultimate Enemies In Count"));
            Ulti.Add(new MenuBool("ultiR", "Use [R] Aoe"));
            Ulti.Add(new MenuSlider("MinR", "Min Enemies Use [R] Aoe", 2, 1, 5));
            Ulti.Add(new MenuBool("follow", "Auto Move To Target While [R]", false));
            Ulti.Add(new MenuSeparator("Ultimate My HP", "Ultimate My HP"));
            Ulti.Add(new MenuBool("ultiR2", "Use [R] If My HP <"));
            Ulti.Add(new MenuSlider("MauR", "My HP Use [R]", 40));
            Ulti.Add(new MenuBool("wulti", "Use [W] If My HP <"));
            Ulti.Add(new MenuSlider("MauW", "My HP Use [W]", 40));
            MenuWuk.Add(Ulti);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass", false));
            HarassMenu.Add(new MenuSlider("ManaHR", "Mana For Harass", 40));
            MenuWuk.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear", false));
            LaneClearMenu.Add(new MenuBool("ELC", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana For LaneClear", 50));
            MenuWuk.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("ManaJC", "Mana For JungleClear", 30));
            MenuWuk.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal", false));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal", false));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuWuk.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            ComboMenu.Add(new MenuList("skin.Id", "Skin Mode:", new[] { "Default", "1", "2", "3", "4", "5" }) { Index = 0 });
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawR", "[R] Range"));
            Misc.Add(new MenuBool("DrawE", "[E] Range"));
            Misc.Add(new MenuBool("DrawTR", "Status UnderTurret"));
            Misc.Add(new MenuSeparator("Interrupt/Anti Gap Settings", "Interrupt/Anti Gap Settings"));
            Misc.Add(new MenuBool("inter", "Use [R] Interupt"));
            Misc.Add(new MenuBool("AntiGap", "Use [W] Anti Gapcloser"));
            MenuWuk.Add(Misc);
            MenuWuk.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
            Interrupter.OnInterrupterSpell += Interupt;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Black, 1);
            }
            if (Misc["DrawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Black, 1);
            }
            if (Misc["DrawTR"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active)
                {
                    DrawFont(thm, "Use E Under Turret : Disable", (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.White);
                }
                else
                {
                    DrawFont(thm, "Use E Under Turret : Enable", (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.Red);
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
            Ultimate();

            /*if (_Player.SkinId != Misc["skin.Id"].GetValue<MenuList>().Index)
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

        public static bool QPassive
        {
            get { return ObjectManager.Player.HasBuff("monkeykingdoubleattack"); }
        }

        public static bool RActive
        {
            get { return ObjectManager.Player.HasBuff("MonkeyKingSpinToWin"); }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var disE = ComboMenu["DisE"].GetValue<MenuSlider>().Value;
            var turret = ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active;
            if (target != null)
            {
                if (useE && E.IsReady() && !RActive && target.IsValidTarget(E.Range) && disE <= target.Distance(ObjectManager.Player))
                {
                    if (turret)
                    {
                        if (!target.Position.UnderTuret())
                        {
                            E.Cast(target);
                        }
                    }
                    else
                    {
                        E.Cast(target);
                    }
                }
                if (useQ && Q.IsReady() && !RActive && target.IsValidTarget(375))
                {
                    Q.Cast();
                }
                if (Q.IsReady() == false && useW && W.IsReady() && !RActive && target.IsValidTarget(370))
                {
                    W.Cast();
                }

            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Misc["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) < 300)
            {
                W.Cast(sender);
            }
        }

        private static void Ultimate()
        {
            var target = TargetSelector.GetTarget(R.Range + 175, DamageType.Physical);
            var useR = Ulti["ultiR"].GetValue<MenuBool>().Enabled;
            var minR = Ulti["MinR"].GetValue<MenuSlider>().Value;
            var useR2 = Ulti["ultiR2"].GetValue<MenuBool>().Enabled;
            var mauR = Ulti["MauR"].GetValue<MenuSlider>().Value;
            var auto = Ulti["follow"].GetValue<MenuBool>().Enabled;
            var autow = Ulti["wulti"].GetValue<MenuBool>().Enabled;
            var mauW = Ulti["MauW"].GetValue<MenuSlider>().Value;


            if (target != null)
            {
                if (useR && R.IsReady() && _Player.Position.CountEnemyHeroesInRange(R.Range) >= minR && !RActive)
                {
                    R.Cast();
                }

                if (useR2 && R.IsReady() && !RActive && _Player.HealthPercent <= mauR && _Player.Position.CountEnemyHeroesInRange(R.Range) >= 1 && !ObjectManager.Player.InShop())
                {
                    R.Cast();
                }

                if (autow && W.IsReady() && !RActive && _Player.HealthPercent <= mauW && _Player.Position.CountEnemyHeroesInRange(R.Range) >= 1 && !ObjectManager.Player.InShop())
                {
                    W.Cast();
                }

                if (auto && target.IsValidTarget() && RActive)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(300, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useriu = ComboMenu["hydra"].GetValue<MenuBool>().Enabled;
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useQ && Q.IsReady() && !RActive && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttackTimer();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if ((useriu && !RActive) && (Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass))
                {
                    if (Hydra.IsInRange(ObjectManager.Player) && Hydra.IsReady && target.IsValidTarget(250))
                    {
                        Hydra.Cast();
                    }

                    if (Tiamat.IsInRange(ObjectManager.Player) && Tiamat.IsReady && target.IsValidTarget(250))
                    {
                        Tiamat.Cast();
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["ELC"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent < mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(300)
                && ObjectManager.Player.Distance(minion.Position) <= 300
                && ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) + ObjectManager.Player.GetAutoAttackDamage(minion)
                >= minion.Health + minion.AllShield)
                {
                    Q.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range))
                {
                    E.Cast(minion);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaHR"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useQ && Q.IsReady() && !RActive && target.IsValidTarget(300) && _Player.Distance(target) > 175)
                {
                    Q.Cast();
                }

                if (useE && E.IsReady() && !RActive && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["ManaJC"].GetValue<MenuSlider>().Value;
            var jungleMonsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(625));
            if (jungleMonsters != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useE && E.IsReady() && jungleMonsters.IsValidTarget(E.Range))
                {
                    E.Cast(jungleMonsters);
                }

                if (useQ && Q.IsReady() && jungleMonsters.IsValidTarget(300))
                {
                    Q.Cast();
                }
            }
        }

        public static void Interupt(AIBaseClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = Misc["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && R.IsReady() && !RActive && i.DangerLevel == DangerLevel.High && R.IsInRange(sender))
            {
                R.Cast();
            }
        }

        public static void Flee()
        {
            if (E.IsReady())
            {
                var CursorPos = Game.CursorPos;
                AIBaseClient fl = GameObjects.Minions.FirstOrDefault(w => w.Distance(CursorPos) <= 250 && E.IsInRange(w));
                if (fl != default(AIBaseClient))
                {
                    E.Cast(fl);
                }
                else
                {
                    fl = GameObjects.EnemyHeroes.FirstOrDefault(w => w.Distance(CursorPos) <= 250 && E.IsInRange(w));
                    if (fl != default(AIBaseClient))
                    {
                        E.Cast(fl);
                    }
                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float jx, float jy, ColorBGRA jc)
        {
            vFont.DrawText(null, vText, (int)jx, (int)jy, jc);
        }

        public static bool UnderTuret(this Vector3 position)
        {
            return GameObjects.EnemyTurrets.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(position) < 950);
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuff("BlitzcrankManaBarrierCD") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsE && E.IsReady() && !RActive && target.IsValidTarget(E.Range))
                {
                    if (target.Health + target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) + ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        E.Cast(target);
                    }
                }

                if (KsQ && Q.IsReady() && !RActive && target.IsValidTarget(300))
                {
                    if (target.Health + target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast();
                    }
                }

                if (KsR && R.IsReady() && !RActive && target.IsValidTarget(R.Range))
                {
                    if (target.Health + target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        R.Cast();
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
