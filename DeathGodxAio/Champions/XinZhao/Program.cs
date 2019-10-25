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

namespace XinZhao7
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
        public static Spell Ignite;
        public static Font thm;
        public static Item Hydra;
        public static Item Tiamat;
        public static Item Titanic;
        public static Item Botrk;
        public static Item Bil;
        public const float YOff = 10;
        public const float XOff = 0;
        public const float Width = 107;
        public const float Thick = 9;

     
       public static void XinZhaoOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("XinZhao")) return;
            Game.Print("Doctor's Xinzhao Loaded! PORTEX by DEATHGODx", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 500);
            W.SetSkillshot(0.25f, 60f, 1400f, true, true, SkillshotType.Circle);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 15, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Item(ItemId.Titanic_Hydra, ObjectManager.Player.GetRealAutoAttackRange());
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            var MenuXinzhao = new Menu("Doctor's Xinzhao", "Xinzhao", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSlider("DisE", "Use [E] If Enemy Distance >", 250, 0, 600));
            ComboMenu.Add(new MenuKeyBind("CTurret", "Dont Use [E] UnderTurret", System.Windows.Forms.Keys.T, KeyBindType.Toggle)).Permashow();
            ComboMenu.Add(new MenuSeparator("Items Settings", "Items Settings"));
            ComboMenu.Add(new MenuBool("hydra", "Use [Hydra] Reset AA"));
            ComboMenu.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            ComboMenu.Add(new MenuSlider("ihp", "My HP Use BOTRK", 50));
            ComboMenu.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK", 50));
            MenuXinzhao.Add(ComboMenu);
            Ulti = new Menu("Ultimate Settings", "Ulti");
            Ulti.Add(new MenuSeparator("Ultimate Enemies In Count", "Ultimate Enemies In Count"));
            Ulti.Add(new MenuBool("ultiR", "Use [R] Enemies In Range"));
            Ulti.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 2, 1, 5));
            Ulti.Add(new MenuSeparator("Ultimate My HP", "Ultimate My HP"));
            Ulti.Add(new MenuBool("ultiR2", "Use [R] If My HP"));
            Ulti.Add(new MenuSlider("MauR", "My HP Use [R]", 40));
            MenuXinzhao.Add(Ulti);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass", false));
            HarassMenu.Add(new MenuSlider("ManaHR", "Mana For Harass", 40));
            MenuXinzhao.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear", false));
            LaneClearMenu.Add(new MenuBool("WLC", "Use [W] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana For LaneClear", 50));
            MenuXinzhao.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("ManaJC", "Mana For JungleClear", 30));
            MenuXinzhao.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal", false));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuXinzhao.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6" }) { Index = 0 });
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawR", "R Range"));
            Misc.Add(new MenuBool("DrawE", "E Range"));
            Misc.Add(new MenuBool("Damage", "Damage Indicator [R]"));
            Misc.Add(new MenuBool("DrawTR", "Draw Text Under Turret"));
            Misc.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            Misc.Add(new MenuBool("inter", "Use [R] Interupt"));
            MenuXinzhao.Add(Misc);
            MenuXinzhao.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnTick;
            Orbwalker.OnAction += ResetAttack;
            Interrupter.OnInterrupterSpell += Interupt;
            Drawing.OnEndScene += Damage;
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

        private static void Game_OnTick(EventArgs args)
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

        private static void Damage(EventArgs args)
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValid && e.IsHPBarRendered && e.Health + e.AllShield > 10))
            {
                var damage = RDamage(enemy);

                if (Misc["Damage"].GetValue<MenuBool>().Enabled && R.IsReady())
                {
                    var dmgPer = (enemy.Health + enemy.AllShield - damage > 0 ? enemy.Health + enemy.AllShield - damage : 0) / enemy.Health + enemy.AllShield;
                    var currentHPPer = enemy.Health + enemy.AllShield / enemy.Health + enemy.AllShield;
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width), (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1, (int)enemy.HPBarPosition.Y + YOff);
                    //EloBuddy.SDK.Rendering.Line.DrawLine(System.Drawing.Color.Orange, Thick, initPoint, endPoint);
                }
            }
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical,
                (float)(new[] { 0, 75, 175, 275 }[Program.R.Level] + 1.0f * _Player.FlatPhysicalDamageMod));
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var disE = ComboMenu["DisE"].GetValue<MenuSlider>().Value;
            var item = ComboMenu["BOTRK"].GetValue<MenuBool>().Enabled;
            var Minhp = ComboMenu["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = ComboMenu["ihpp"].GetValue<MenuSlider>().Value;
            var turret = ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active;
            if (target != null)
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && (disE <= target.Distance(ObjectManager.Player) || target.IsDashing()))
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
                if (useQ && Q.IsReady() && target.IsValidTarget(250) && !target.IsDead && !target.IsDead)
                {
                    Q.Cast();
                }
                if (useW && W.IsReady() && target.IsValidTarget(800) && !target.IsDead && !target.IsDead)
                {
                    W.Cast(target);
                }
                if (item && Bil.IsReady && Bil.IsOwned() && target.IsValidTarget(450))
                {
                    Bil.Cast(target);
                }
                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(450)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }
            }
        }

        private static void Ultimate()
        {
            var useR = Ulti["ultiR"].GetValue<MenuBool>().Enabled;
            var minR = Ulti["MinR"].GetValue<MenuSlider>().Value;
            var useR2 = Ulti["ultiR2"].GetValue<MenuBool>().Enabled;
            var mauR = Ulti["MauR"].GetValue<MenuSlider>().Value;
            if (useR && !ObjectManager.Player.InShop() && _Player.Position.CountEnemyHeroesInRange(R.Range) >= minR)
            {
                R.Cast();
            }

            if (useR2 && _Player.HealthPercent <= mauR && _Player.Position.CountEnemyHeroesInRange(R.Range) >= 1 && !ObjectManager.Player.InShop())
            {
                R.Cast();
            }
        }

        private static void ResetAttack(object target, OrbwalkerActionArgs args)
        {
            var targetm = TargetSelector.GetTarget(1000, DamageType.Physical);
            var useriu = ComboMenu["hydra"].GetValue<MenuBool>().Enabled;
            if (target != null)
            {
                if (useriu && Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
                {
                    if (Hydra.IsOwned() && Hydra.IsReady && targetm.IsValidTarget(325))
                    {
                        Hydra.Cast();
                    }

                    if (Tiamat.IsOwned() && Tiamat.IsReady && targetm.IsValidTarget(325))
                    {
                        Tiamat.Cast();
                    }

                    if (Titanic.IsOwned() && targetm.IsValidTarget(325) && Titanic.IsReady)
                    {
                        Titanic.Cast();
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useW = LaneClearMenu["WLC"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["ManaLC"].GetValue<MenuSlider>().Value;
            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (ObjectManager.Player.ManaPercent < mana) return;
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(250) && minions.Count() >= 3)
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && minion.IsValidTarget(800))
                {
                    W.Cast(minion);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["ManaHR"].GetValue<MenuSlider>().Value;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(250) && !target.IsDead && !target.IsDead)
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && target.IsValidTarget(800) && !target.IsDead && !target.IsDead)
                {
                    W.Cast();
                }
            }
        }

        public static void JungleClear()
        {

            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["ManaJC"].GetValue<MenuSlider>().Value;
            var jungleMonsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(250));
            if (jungleMonsters != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useQ && Q.IsReady() && jungleMonsters.IsValidTarget(325))
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && jungleMonsters.IsValidTarget(800))
                {
                    W.Cast(jungleMonsters);
                }

                if (useE && E.IsReady() && jungleMonsters.IsValidTarget(E.Range))
                {
                    E.Cast(jungleMonsters);
                }
            }
        }

        public static void Interupt(object sender, InterruptSpellArgs i)
        {
            var Inter = Misc["inter"].GetValue<MenuBool>().Enabled;
            if (!(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && R.IsReady() && i.DangerLevel == DangerLevel.High && R.IsInRange(i.Sender))
            {
                R.Cast();
            }
        }

        public static void Flee()
        {
            var Enemies = GameObjects.EnemyHeroes.FirstOrDefault(e => e.IsValidTarget(E.Range));
            var minions = GameObjects.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(E.Range));
            if (Enemies != null && E.IsReady())
            {
                if (Enemies.IsValidTarget(250))
                {
                    E.Cast(Enemies);
                }
            }

            if (minions != null && E.IsReady())
            {
                if (minions.IsValidTarget(250))
                {
                    E.Cast(minions);
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
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(R.Range) && !hero.HasBuff("FioraW") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.HasBuff("SpellShield") && !hero.HasBuff("NocturneShield") && !hero.IsDead && !hero.IsDead))
            {
                if (KsE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (target.Health + target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.E))
                    {
                        E.Cast(target);
                    }
                }

                if (KsR && R.IsReady() && target.IsValidTarget(R.Range))
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
