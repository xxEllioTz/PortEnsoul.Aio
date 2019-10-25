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

namespace Twitch
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, Misc, KillStealMenu, Items;
        public static Item Botrk;
        public static Item Bil;
        public static Item Youmuu;
        public static readonly int[] SDamage = { 0, 15, 20, 25, 30, 35 };
        public static readonly int[] BDamage = { 0, 20, 35, 50, 65, 80 };
        public const float YOff = 10;
        public const float XOff = 0;
        public const float Width = 107;
        public const float Thick = 9;
        private static Font thm;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;

   
       public static void TwitchOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Twitch")) return;
            Game.Print("Doctor's Twitch Loaded! PORTED by DeathGODX", Color.Orange);
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            //W.SetSkillshot(250, 1550, 275, false,false, SkillshotType.Circle);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R);
            thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 22, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            Youmuu = new Item(3142, 10);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            var MenuTw = new Menu("Doctor's Twitch", "Twitch", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Spell [Q]"));
            ComboMenu.Add(new MenuBool("ComboW", "Spell [W]"));
            ComboMenu.Add(new MenuSeparator("Combo [E] Settings", "Combo [E] Settings"));
            ComboMenu.Add(new MenuBool("ComboE", "Spell [E]", false));
            ComboMenu.Add(new MenuSlider("MinEC", "Min Stacks Use [E]", 6, 0, 6));
            ComboMenu.Add(new MenuSeparator("Combo [E] On", "Combo [E] On"));
            
            foreach (var target in GameObjects.EnemyHeroes)
            {
                ComboMenu.Add(new MenuBool("combo" + target.CharacterName, "" + target.CharacterName));
            }
            
            ComboMenu.Add(new MenuBool("ComboR", "Spell [R]"));
            ComboMenu.Add(new MenuSlider("MinR", "Min Enemies Use [R]", 3, 0, 5));
            MenuTw.Add(ComboMenu);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W]", false));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q]", false));
            HarassMenu.Add(new MenuSlider("HminQ", "Min Enemies Use [Q]", 2, 1, 5));
            HarassMenu.Add(new MenuSeparator("Harass [E] Settings", "Harass [E] Settings"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E]", false));
            HarassMenu.Add(new MenuSlider("HminE", "Min Stacks Use [E]", 5, 0, 6));
            HarassMenu.Add(new MenuSeparator("Harass [E] On", "Harass [E] On"));
            foreach (var target in GameObjects.EnemyHeroes)
            {
                HarassMenu.Add(new MenuBool("haras" + target.CharacterName, "" + target.CharacterName));
            }
            HarassMenu.Add(new MenuSlider("ManaQ", "Min Mana For Harass", 40));
            MenuTw.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("LaneClear Settings", "LaneClear Settings"));
            LaneClearMenu.Add(new MenuSeparator("[E] Settings", "[E] Settings"));
            LaneClearMenu.Add(new MenuBool("E", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuBool("ELH", "Only Use [E] If Orbwalker Cant Killable Minion", false));
            LaneClearMenu.Add(new MenuSlider("Minm", "Min Minions HasBuff Use [E] LaneClear", 3, 0, 6));
            LaneClearMenu.Add(new MenuSlider("MinS", "Min Stacks Use [E] LaneClear", 3, 1, 6));
            LaneClearMenu.Add(new MenuSeparator("[W] Settings", "[W] Settings"));
            LaneClearMenu.Add(new MenuBool("W", "Use [W] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("minW", "Min Hit Minions Use [W] LaneClear", 3, 1, 6));
            LaneClearMenu.Add(new MenuSeparator("Mana Settings", "Mana Settings"));
            LaneClearMenu.Add(new MenuSlider("M", "Min Mana For LaneClear", 40));
            MenuTw.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("Q", "Use [Q] JungleClear", false));
            JungleClearMenu.Add(new MenuBool("W", "Use [W] JungleClear", false));
            JungleClearMenu.Add(new MenuBool("E", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("M", "Min Mana For JungleClear", 30));
            MenuTw.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            MenuTw.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Misc Settings", "Misc Settings"));
            Misc.Add(new MenuBool("AntiGap", "Use [W] AntiGapcloser"));
            Misc.Add(new MenuBool("FleeQ", "Use [Q] Flee"));
            Misc.Add(new MenuBool("FleeW", "Use [W] Flee"));
            Misc.Add(new MenuSeparator("Use [E] Enemy Out Range", "Use [E] Enemy Out Range"));
            Misc.Add(new MenuBool("E", "Use [E] If Enemy Escape", false));
            Misc.Add(new MenuSlider("ES", "Min Stacks Use [E]", 6, 1, 6));
            Misc.Add(new MenuSeparator("Draw Settings", "Draw Settings"));
            Misc.Add(new MenuBool("DrawW", "[W] Range"));
            Misc.Add(new MenuBool("DrawE", "[E] Range"));
            Misc.Add(new MenuBool("DrawT", "Draw [Q] Time"));
            Misc.Add(new MenuBool("Damage", "Damage Indicator"));
            MenuTw.Add(Misc);
            Items = new Menu("Items Settings", "Items");
            Items.Add(new MenuSeparator("Items Settings", "Items Settings"));
            Items.Add(new MenuBool("you", "Use [Youmuu]"));
            Items.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            Items.Add(new MenuSlider("ihp", "My HP Use BOTRK <=", 50));
            Items.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50));
            MenuTw.Add(Items);
            MenuTw.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Damage;
            Game.OnUpdate += Game_OnUpdate;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Orbwalker.OnAction += Orbwalker_CantLasthit;
            Orbwalker.OnAction += ResetAttack;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawW"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }

            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Misc["DrawT"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ObjectManager.Player.HasBuff("TwitchHideInShadows"))
                {
                    DrawFont(thm, "Q Stealthed : " + QTime(ObjectManager.Player), (float)(ft[0] - 100), (float)(ft[1] + 50), SharpDX.Color.GreenYellow);
                }
            }

            if (Misc["DrawT"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ObjectManager.Player.HasBuff("TwitchFullAutomatic"))
                {
                    DrawFont(thm, "R Time : " + RTime(ObjectManager.Player), (float)(ft[0] - 70), (float)(ft[1] + 100), SharpDX.Color.Red);
                }
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
            KillSteal();
            Item();
            Escape();
        }

        public static float QTime(AIBaseClient target)
        {
            if (target.HasBuff("TwitchHideInShadows"))
            {
                return Math.Max(0, target.GetBuff("TwitchHideInShadows").EndTime) - Game.Time;
            }
            return 0;
        }

        public static float RTime(AIBaseClient target)
        {
            if (target.HasBuff("TwitchFullAutomatic"))
            {
                return Math.Max(0, target.GetBuff("TwitchFullAutomatic").EndTime) - Game.Time;
            }
            return 0;
        }

        private static bool QCasting
        {
            get { return ObjectManager.Player.HasBuff("TwitchHideInShadows"); }
        }

        public static void Combo()
        {
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            var MinE = ComboMenu["MinEC"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(W.Range))
                {
                    Q.Cast();
                }

                if (useW && W.IsReady() && !QCasting && target.IsValidTarget(W.Range) && _Player.Distance(target) > ObjectManager.Player.GetRealAutoAttackRange(target))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        W.Cast(pred.CastPosition);
                    }
                }

                if (useE && E.IsReady() && E.IsInRange(target) && target.HasBuff("twitchdeadlyvenom"))
                {
                    if (ComboMenu["combo" + target.CharacterName].GetValue<MenuBool>().Enabled && Stack(target) >= MinE)
                    {
                        E.Cast();
                    }
                }

                if (useR && R.IsReady() && _Player.Position.CountEnemyHeroesInRange(E.Range) >= MinR)
                {
                    R.Cast();
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useW && W.IsReady() && target.IsValidTarget(W.Range) && _Player.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    var Pred = W.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        W.Cast(Pred.CastPosition);
                    }
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["Q"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["W"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["E"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["M"].GetValue<MenuSlider>().Value;
            var monsters = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(R.Range));
            if (ObjectManager.Player.ManaPercent < mana)
            {
                return;
            }

            if (monsters != null)
            {
                if (useW && W.CanCast(monsters) && W.IsInRange(monsters) && Stack(monsters) <= 4)
                {
                    W.Cast(monsters);
                }
                if (useE && E.IsReady() && E.IsInRange(monsters) && monsters.HasBuff("twitchdeadlyvenom") && monsters.Health + monsters.AllShield <= EDamage(monsters))
                {
                    E.Cast();
                }

                if (useQ && Q.IsReady() && W.IsInRange(monsters))
                {
                    Q.Cast();
                }
            }
        }

        public static void Item()
        {
            var item = Items["BOTRK"].GetValue<MenuBool>().Enabled;
            var yous = Items["you"].GetValue<MenuBool>().Enabled;
            var Minhp = Items["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = Items["ihpp"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(900) && !e.IsDead))
            {
                if (item && Bil.IsReady && Bil.IsOwned() && Bil.IsInRange(target))
                {
                    Bil.Cast(target);
                }

                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }

                if (yous && Youmuu.IsReady && Youmuu.IsOwned() && _Player.Distance(target) <= ObjectManager.Player.GetRealAutoAttackRange() && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (_Player.HasBuff("TwitchFullAutomatic"))
                    {
                        Youmuu.Cast();
                    }
                    else
                    {
                        if (_Player.Distance(target) <= 550)
                        {
                            Youmuu.Cast();
                        }
                    }
                }
            }
        }

        private static void Orbwalker_CantLasthit(object targeti, OrbwalkerActionArgs args)
        {
            var minions = GameObjects.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            var mana = LaneClearMenu["M"].GetValue<MenuSlider>().Value;
            var useE = LaneClearMenu["ELH"].GetValue<MenuBool>().Enabled;
            var unit = (useE && Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && ObjectManager.Player.ManaPercent >= mana);
            if (minions == null) return;
            if (unit && E.IsReady() && E.IsInRange(minions) && minions.HasBuff("twitchdeadlyvenom"))
            {
                if (EDamage(minions) >= minions.Health + minions.AllShield)
                {
                    E.Cast();
                }
            }
        }

        private static void Flee()
        {
            var useQ = Misc["FleeQ"].GetValue<MenuBool>().Enabled;
            var useW = Misc["FleeW"].GetValue<MenuBool>().Enabled;
            if (useQ && Q.IsReady())
            {
                Q.Cast();
            }

            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(W.Range) && !e.IsDead))
            {
                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    var Pred = W.GetPrediction(target);
                    if (Pred.Hitchance >= HitChance.High)
                    {
                        W.Cast(Pred.CastPosition);
                    }
                }

            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (Misc["AntiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player) <= 300)
            {
                W.Cast(sender);
            }
        }

        public static void LaneClear()
        {
            var mana = LaneClearMenu["M"].GetValue<MenuSlider>().Value;
            var useW = LaneClearMenu["W"].GetValue<MenuBool>().Enabled;
            var MinW = LaneClearMenu["minW"].GetValue<MenuSlider>().Value;
            var useE = LaneClearMenu["E"].GetValue<MenuBool>().Enabled;
            var minm = LaneClearMenu["Minm"].GetValue<MenuSlider>().Value;
            var MinE2 = LaneClearMenu["MinS"].GetValue<MenuSlider>().Value;
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(E.Range) && e.IsMinion())
                    .Cast<AIBaseClient>().ToList();
            if (ObjectManager.Player.ManaPercent < mana) return;
            if (minions != null)
            {
                foreach (var minion in minions)
                {
                    if (useW && W.IsReady())
                    {
                        W.Cast(minion);
                    }
                }
                if (useE && E.IsReady())
                {
                    int ECal = minions.Where(e => e.Distance(_Player.Position) < (E.Range) && Stack(e) >= MinE2).Count(); ;
                    if (ECal >= minm)
                    {
                        E.Cast();
                    }
                }
            }
        }

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var Mana = HarassMenu["ManaQ"].GetValue<MenuSlider>().Value;
            var MinQ = HarassMenu["HminQ"].GetValue<MenuSlider>().Value;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var MinE = HarassMenu["HminE"].GetValue<MenuSlider>().Value;
            if (ObjectManager.Player.ManaPercent <= Mana)
            {
                return;
            }

            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (_Player.Position.CountEnemyHeroesInRange(900) >= MinQ)
                    {
                        Q.Cast();
                    }
                }

                if (useW && W.IsReady() && !QCasting && target.IsValidTarget(W.Range))
                {
                        W.CastOnUnit(target);
                }

                if (useE && E.IsReady() && E.IsInRange(target) && target.HasBuff("twitchdeadlyvenom"))
                {
                    if (HarassMenu["haras" + target.CharacterName].GetValue<MenuBool>().Enabled && Stack(target) >= MinE)
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private static void Damage(EventArgs args)
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValid && e.IsHPBarRendered && e.Health + e.AllShield > 10))
            {
                var damage = EDamage(enemy);
                if (Misc["Damage"].GetValue<MenuBool>().Enabled && E.IsReady() && enemy.HasBuff("twitchdeadlyvenom"))
                {
                    var dmgPer = (enemy.Health + enemy.AllShield - damage > 0 ? enemy.Health + enemy.AllShield - damage : 0) / (enemy.MaxHealth + enemy.AllShield + enemy.PhysicalShield + enemy.MagicalShield);
                    var currentHPPer = enemy.Health + enemy.AllShield / enemy.Health + enemy.AllShield;
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width), (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1, (int)enemy.HPBarPosition.Y + YOff);
                   // EloBuddy.SDK.Rendering.Line.DrawLine(System.Drawing.Color.Orange, Thick, initPoint, endPoint);
                }
            }
        }
        public static double EDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Physical, SDamage[E.Level] * Stack(target) + (0.25f * _Player.FlatPhysicalDamageMod + 0.2f * _Player.FlatMagicDamageMod + BDamage[E.Level]));
        }


        public static float StackTimeDamage(AIBaseClient target)
        {
            float dmg = 0;
            if (!target.HasBuff("twitchdeadlyvenom")) return 0;

            if (ObjectManager.Player.Level < 5)
            {
                dmg = 2;
            }
            if (ObjectManager.Player.Level < 9)
            {
                dmg = 3;
            }
            if (ObjectManager.Player.Level < 13)
            {
                dmg = 4;
            }
            if (ObjectManager.Player.Level < 17)
            {
                dmg = 5;
            }
            if (ObjectManager.Player.Level == 18)
            {
                dmg = 6;
            }
            return dmg * Stack(target) * StackTime(target) - target.HPRegenRate * StackTime(target);
        }

        private static int Stack(AIBaseClient target)
        {
            var Ec = 0;
            for (var t = 1; t < 7; t++)
            {
                if (ObjectManager.Get<EffectEmitter>().Any(s => s.Position.Distance(target.Position) <= 175 && s.Name == "twitch_poison_counter_0" + t + ".troy"))
                {
                    Ec = t;
                }
            }
            return Ec;
        }

        public static float StackTime(AIBaseClient target)
        {
            if (target.HasBuff("twitchdeadlyvenom"))
            {
                return Math.Max(0, target.GetBuff("twitchdeadlyvenom").EndTime) - Game.Time;
            }
            return 0;
        }

        public static void KillSteal()
        {
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(E.Range) && hero.HasBuff("twitchdeadlyvenom") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage")))
            {
                if (KsE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (EDamage(target) + StackTimeDamage(target) >= target.Health + target.AllShield || target.HealthPercent <= 10)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.E);
                    }
                }
            }
        }

        public static void Escape()
        {
            var Eranh = Misc["E"].GetValue<MenuBool>().Enabled;
            var Eranhs = Misc["ES"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuff("FioraW") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.HasBuff("SpellShield") && !hero.HasBuff("NocturneShield") && hero.HasBuff("twitchdeadlyvenom") && !hero.IsDead && !hero.IsDead))
            {
                if (Eranh && E.IsReady())
                {
                    if (Stack(target) >= Eranhs && _Player.Position.Distance(target) >= 1050)
                    {
                        E.Cast();
                    }
                }

                if (E.IsReady() && ObjectManager.Player.HealthPercent <= 15)
                {
                    E.Cast();
                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float jx, float jy, ColorBGRA jc)
        {
            vFont.DrawText(null, vText, (int)jx, (int)jy, jc);
        }
    }
}
