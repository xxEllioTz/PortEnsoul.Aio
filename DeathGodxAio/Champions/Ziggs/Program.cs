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

namespace Ziggs7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, Misc;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell Q2;
        public static Spell Q3;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;
        public const float YOff = 10;
        public const float XOff = 0;
        public const float Width = 107;
        public const float Thick = 9;

    
       public static void ZiggsOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Ziggs")) return;
            Game.Print("Doctor's Ziggs Loaded! PORTED By DEATHGODx", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(300, 1700, 130, false, false, SkillshotType.Circle);
            Q2 = new Spell(SpellSlot.Q, 1125);
            Q2.SetSkillshot(300 + Q.Delay, 1700, 130, false, false, SkillshotType.Line);
            Q3 = new Spell(SpellSlot.Q, 1400);
            Q3.SetSkillshot(300 + Q.Delay, 1700, 130, false, false, SkillshotType.Line);
            W = new Spell(SpellSlot.W, 1000);
            W.SetSkillshot(250, 1750, 275, false, false, SkillshotType.Circle);
            E = new Spell(SpellSlot.E, 900);
            E.SetSkillshot(500, 1600, 90, false, false,SkillshotType.Circle);
            R = new Spell(SpellSlot.R, 5000);
            R.SetSkillshot(1000, 2800, 500, false, false,SkillshotType.Circle);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var MenuZiggs = new Menu("Doctor's Ziggs", "Ziggs", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo", false));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSeparator("Ultimate Enemies In Count", "Ultimate Enemies In Count"));
            ComboMenu.Add(new MenuBool("ultiR", "Use [R] Enemies In Range", false));
            ComboMenu.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 2, 1, 5));
            MenuZiggs.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass", false));
            HarassMenu.Add(new MenuSlider("ManaHR", "Mana For Harass", 40));
            MenuZiggs.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear"));
            LaneClearMenu.Add(new MenuBool("ELC", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("ManaLC", "Mana For LaneClear", 60));
            MenuZiggs.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("WJungle", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("ManaJC", "Mana For JungleClear", 30));
            MenuZiggs.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal", false));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsW", "Use [W] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            KillStealMenu.Add(new MenuSeparator("Ultimate KillSteal", "Ultimate KillSteal"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuSlider("minKsR", "Min [R] Distance KillSteal", 100, 1, 5000));
            KillStealMenu.Add(new MenuKeyBind("RKb", "[R] Semi Manual", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            MenuZiggs.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5" }) { Index = 0 });
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawR", "[R] Range"));
            Misc.Add(new MenuBool("DrawQ", "[Q] Range"));
            Misc.Add(new MenuBool("DrawW", "[W] Range"));
            Misc.Add(new MenuBool("DrawE", "[E] Range"));
            Misc.Add(new MenuBool("Damage", "Damage Indicator [R]"));
            Misc.Add(new MenuBool("Notifications", "Notifications Can Kill R"));
            Misc.Add(new MenuSeparator("Interrupt Settings", "Interrupt Settings"));
            Misc.Add(new MenuBool("inter", "Use [W] Interupt"));
            Misc.Add(new MenuBool("AntiGapW", "Use [W] AntiGapcloser", false));
            MenuZiggs.Add(Misc);
            MenuZiggs.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnTick;
            Interrupter.OnInterrupterSpell += Interupt;
            Drawing.OnEndScene += Damage;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawR"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
            }

            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Misc["DrawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Misc["DrawW"].GetValue<MenuBool>().Enabled)
            {

                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }

            if (Misc["Notifications"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target.IsValidTarget(2300))
                {
                    if (Rborders(target) > target.Health + target.AllShield)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, Color.Orange, " Useeeeee RRRRRRRR Cannnnnnnn Killlllllllll: " + target.CharacterName);
                    }
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
            /*
            if (_Player.SkinId != Misc["skin.Id"].GetValue<MenuList>().Index)
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
                var damage = Rborders(enemy);
                if (Misc["Damage"].GetValue<MenuBool>().Enabled && R.IsReady())
                {
                    var dmgPer = (enemy.Health + enemy.AllShield - damage > 0 ? enemy.Health + enemy.AllShield - damage : 0) / enemy.Health + enemy.AllShield;
                    var currentHPPer = enemy.Health + enemy.AllShield / enemy.Health + enemy.AllShield;
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width), (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1, (int)enemy.HPBarPosition.Y + YOff);
                    //EloBuddy.SDK.Rendering.Line.DrawLine(System.Drawing.Color.Aqua, Thick, initPoint, endPoint);
                }
            }
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 200, 300, 400 }[Program.R.Level] + 0.7f * _Player.FlatMagicDamageMod));
        }

        public static double Rborders(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 170, 250, 350 }[Program.R.Level] + 0.6f * _Player.FlatMagicDamageMod));
        }

        public static void QCast(AIBaseClient target)
        {
            var QPred = Q.GetPrediction(target);
            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(QPred.CastPosition);
            }

            else if (target.IsValidTarget(Q2.Range) && Q.IsReady())
            {
                Q2.Cast(QPred.CastPosition);
            }
            else
            {
                if (target.IsValidTarget(Q3.Range) && Q.IsReady())
                {
                    Q3.Cast(QPred.CastPosition);
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Misc["AntiGapW"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) < 300)
            {
                W.Cast(sender);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q3.Range, DamageType.Magical);
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ultiR"].GetValue<MenuBool>().Enabled;
            var minR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q3.Range) && !target.IsDead && !target.IsDead)
                {
                    QCast(target);
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range - 100) && !target.IsDead && !target.IsDead)
                {
                    E.Cast(target);
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range - 100) && !target.IsDead && !target.IsDead)
                {
                    W.Cast(target.Position);
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.CastPosition.CountEnemyHeroesInRange(500) >= minR && pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(pred.CastPosition);
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
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q3.Range) && minions.Count() >= 3 && _Player.GetAutoAttackDamage(minion) < minion.Health + minion.AllShield)
                {
                    Q.Cast(minion);
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range) && minions.Count() >= 3)
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
            var target = TargetSelector.GetTarget(Q3.Range, DamageType.Magical);
            if (target != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsDead)
                {
                    E.Cast(target);
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q3.Range) && !target.IsDead && !target.IsDead)
                {
                    QCast(target);
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["WJungle"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["ManaJC"].GetValue<MenuSlider>().Value;
            var monster = GameObjects.Jungle.Where(j => j.IsValidTarget(Q.Range)).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            if (monster != null && ObjectManager.Player.ManaPercent >= mana)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.Cast(monster);
                }

                if (useW && W.IsReady() && monster.IsValidTarget(W.Range))
                {
                    W.Cast(monster);
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

            if (Inter && W.IsReady() && i.DangerLevel == DangerLevel.High && W.IsInRange(sender))
            {
                W.Cast(sender.Position);
            }
        }

        public static void Flee()
        {
            if (W.IsReady())
            {
                W.Cast(_Player.Position);
            }
        }

        private static void KillSteal()
        {
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            var KsW = KillStealMenu["KsW"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var minKsR = KillStealMenu["minKsR"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(Q3.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsDead))
            {
                if (KsE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.E))
                    {
                        E.Cast(target);
                    }
                }

                if (KsW && W.IsReady() && target.IsValidTarget(W.Range) && !Q.IsReady())
                {
                    if (target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.W))
                    {
                        W.Cast(target);
                    }
                }

                if (KsR && R.IsReady() && target.IsValidTarget(R.Range) && !target.IsValidTarget(minKsR))
                {
                    if (target.Health + target.AllShield < RDamage(target))
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.AoeTargetsHitCount >= 70)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }

                if (R.IsReady() && KillStealMenu["RKb"].GetValue<MenuKeyBind>().Active && target.IsValidTarget(R.Range))
                {
                    if (target.Health + target.AllShield < RDamage(target))
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.AoeTargetsHitCount >= 70)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }

                if (KsQ && Q.IsReady() && target.IsValidTarget(Q3.Range))
                {
                    if (target.Health + target.AllShield < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                    {
                        QCast(target);
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
