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

namespace Tryndamere
{
    static class Program
    {
        private static AIHeroClient User = ObjectManager.Player;
        public static Menu Menu, ComboMenu, HarassMenu, Ulti, LaneClearMenu, JungleClearMenu, KillStealMenu, Misc;
        private static AIHeroClient fly { get { return ObjectManager.Player; } }
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
        public static Font thn;
        public static Item Hydra;
        public static Item Tiamat;
        public static Item Botrk;
        public static Item Bil;
        public const float YOff = 10;
        public const float XOff = 0;
        public const float Width = 107;
        public const float Thick = 9;

     
       public static void TryndamereOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Tryndamere")) return;
            Game.Print("Doctor's Tryndamere Loaded!", Color.Orange);
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 660);
            W.SetSkillshot(250, 700, 92, false, false, SkillshotType.Line);
            R = new Spell(SpellSlot.R, 500);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 15, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            thn = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 22, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            var MenuTryndamare = new Menu("Tryndamare", "Tryndamare", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboW2", "Only Use [W] If [E] Is CoolDown", false));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuSlider("DisE", "Use [E] If Enemy Distance >", 300, 0, 600));
            ComboMenu.Add(new MenuKeyBind("CTurret", "Don't Use [E] UnderTurret", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            ComboMenu.Add(new MenuSeparator("Items Settings", "Items Settings"));
            ComboMenu.Add(new MenuBool("hydra", "Use [Hydra] Reset AA"));
            ComboMenu.Add(new MenuBool("BOTRK", "Use [Botrk]"));
            ComboMenu.Add(new MenuSlider("ihp", "My HP Use BOTRK", 50));
            ComboMenu.Add(new MenuSlider("ihpp", "Enemy HP Use BOTRK", 50));
            MenuTryndamare.Add(ComboMenu);
            Ulti = new Menu("Q/R Settings", "Ulti");
            Ulti.Add(new MenuSeparator("Use [R] Low Hp", "Use [R] Low Hp"));
            Ulti.Add(new MenuBool("ultiR", "Use [R] No Die"));
            Ulti.Add(new MenuSlider("MauR", "My HP Use [R] <=", 15));
            Ulti.Add(new MenuSeparator("Use [Q] Low Hp", "Use [Q] Low Hp"));
            Ulti.Add(new MenuBool("Q", "Use [Q]"));
            Ulti.Add(new MenuBool("Q2", "Only Use [Q] If [R] Is CoolDown"));
            Ulti.Add(new MenuSlider("QHp", "My HP Use [Q] <=", 30));
            Ulti.Add(new MenuSlider("Ps", "Min Fury Use [Q]", 5));
            MenuTryndamare.Add(Ulti);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassW", "Use [W] Harass"));
            HarassMenu.Add(new MenuBool("HarassW2", "Only Use [W] If [E] Is CoolDown"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuSlider("DistanceE", "Use [E] If Enemy Distance >", 300, 0, 600));
            HarassMenu.Add(new MenuBool("HTurret", "Don't Use [E] UnderTurret"));
            MenuTryndamare.Add(HarassMenu);
            LaneClearMenu = new Menu("LaneClear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Lane Clear Settings", "Lane Clear Settings"));
            LaneClearMenu.Add(new MenuBool("E", "Use [E] LaneClear", false));
            LaneClearMenu.Add(new MenuSlider("mine", "Min Hit Minions Use [E]", 2, 1, 6));
            MenuTryndamare.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            MenuTryndamare.Add(JungleClearMenu);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal", false));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuTryndamare.Add(KillStealMenu);
            Misc = new Menu("Misc Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Settings", "Skin Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuSlider("skin.Id", "Skin Mode", 0, 0, 8));
            Misc.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Misc.Add(new MenuBool("DrawE", "E Range"));
            Misc.Add(new MenuBool("DrawW", "W Range"));
            Misc.Add(new MenuBool("Damage", "Damage Indicator"));
            Misc.Add(new MenuBool("DrawTR", "Draw Text Under Turret"));
            Misc.Add(new MenuBool("DrawTime", "Draw Time [R]"));
            ObjectManager.Player.SetSkin(Misc["skin.Id"].GetValue<MenuSlider>().Value);
            Misc["skin.Id"].GetValue<MenuSlider>().DisplayName = ObjectManager.Player.SkinName;
            MenuTryndamare.Add(Misc);
            MenuTryndamare.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += ResetAttack;
            Drawing.OnEndScene += Damage;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Red, 1);
            }
            if (Misc["DrawW"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Red, 1);
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

            if (Misc["DrawTime"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ObjectManager.Player.HasBuff("Undying Rage"))
                {
                    DrawFont(thn, "Undying Rage : " + RTime(ObjectManager.Player), (float)(ft[0] - 125), (float)(ft[1] + 100), SharpDX.Color.Orange);
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


        }

        public static int SkinId()
        {
            return Misc["skin.Id"].GetValue<MenuSlider>().Value;
        }
        public static bool checkSkin()
        {
            return Misc["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        private static void Damage(EventArgs args)
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValid && e.IsHPBarRendered && e.Health + e.AllShield > 10))
            {
                var damage = ObjectManager.Player.GetAutoAttackDamage(enemy) * 2;

                if (Misc["Damage"].GetValue<MenuBool>().Enabled)
                {
                    var dmgPer = (enemy.Health + enemy.AllShield - damage > 0 ? enemy.Health + enemy.AllShield - damage : 0) / enemy.Health + enemy.AllShield;
                    var currentHPPer = enemy.Health + enemy.AllShield / enemy.Health + enemy.AllShield;
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width), (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1, (int)enemy.HPBarPosition.Y + YOff);
                    //EloBuddy.SDK.Rendering.Line.DrawLine(System.Drawing.Color.Orange, Thick, initPoint, endPoint);
                }
            }
        }

        public static float RTime(AIBaseClient target)
        {
            if (target.HasBuff("Undying Rage"))
            {
                return Math.Max(0, target.GetBuff("Undying Rage").EndTime) - Game.Time;
            }
            return 0;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useW2 = ComboMenu["ComboW2"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var disE = ComboMenu["DisE"].GetValue<MenuSlider>().Value;
            var item = ComboMenu["BOTRK"].GetValue<MenuBool>().Enabled;
            var Minhp = ComboMenu["ihp"].GetValue<MenuSlider>().Value;
            var Minhpp = ComboMenu["ihpp"].GetValue<MenuSlider>().Value;
            var turret = ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active;
            if (target != null)
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && (disE <= target.Distance(ObjectManager.Player) || ObjectManager.Player.HealthPercent <= 20))
                {
                    if (turret)
                    {
                        if (!target.Position.UnderTuret())
                        {
                            E.Cast(target.Position);
                        }
                    }
                    else
                    {
                        E.Cast(target.Position);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (_Player.Distance(target) <= target.GetRealAutoAttackRange() && ObjectManager.Player.HealthPercent <= 60)
                    {
                        W.Cast();
                    }

                    if (useW2)
                    {
                        if (!target.IsFacing(ObjectManager.Player) && _Player.Distance(target) >= 325 && !E.IsReady())
                        {
                            W.Cast();
                        }
                    }
                    else
                    {
                        if (!target.IsFacing(ObjectManager.Player) && _Player.Distance(target) >= 325)
                        {
                            W.Cast();
                        }
                    }
                }

                if (item && Bil.IsReady && Bil.IsOwned() && target.IsValidTarget(475))
                {
                    Bil.Cast(target);
                }

                if ((item && Botrk.IsReady && Botrk.IsOwned() && target.IsValidTarget(475)) && (ObjectManager.Player.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }
            }
        }

        private static void Ultimate()
        {
            var useQ = Ulti["Q"].GetValue<MenuBool>().Enabled;
            var useQ2 = Ulti["Q2"].GetValue<MenuBool>().Enabled;
            var mauQ = Ulti["QHp"].GetValue<MenuSlider>().Value;
            var useR = Ulti["ultiR"].GetValue<MenuBool>().Enabled;
            var mauR = Ulti["MauR"].GetValue<MenuSlider>().Value;
            var passive = Ulti["Ps"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(E.Range) && !e.IsDead))
            {
                if (!ObjectManager.Player.HasBuff("JudicatorIntervention") && !ObjectManager.Player.HasBuff("kindredrnodeathbuff"))
                {
                    if (useR && R.IsReady() && !ObjectManager.Player.InShop() && (target.IsValidTarget(E.Range) || _Player.IsUnderEnemyTurret()))
                    {
                        if (ObjectManager.Player.HealthPercent <= mauR)
                        {
                            R.Cast();
                        }

                        if (target.GetAutoAttackDamage(_Player) >= _Player.Health)
                        {
                            R.Cast();
                        }

                        /*if (ObjectManager.Player.HasBuff("ZedR"))
                        {
                            Core.DelayAction(() => R.Cast(), 500);
                        }*/
                    }
                }
                if (useQ && Q.IsReady() && RTime(ObjectManager.Player) <= 1 && ObjectManager.Player.Mana >= passive)
                {
                    if (useQ2)
                    {
                        if (!R.IsReady() && (ObjectManager.Player.HealthPercent <= mauQ || ObjectManager.Player.HasBuff("ZedR")))
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        if ((ObjectManager.Player.HealthPercent <= mauQ || ObjectManager.Player.HasBuff("ZedR")))
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        public static void ResetAttack(object e, OrbwalkerActionArgs args)
        {
            if (!(e is AIHeroClient)) return;
            var target = TargetSelector.GetTarget(300, DamageType.Physical);
            var champ = (AIHeroClient)e;
            var useriu = ComboMenu["hydra"].GetValue<MenuBool>().Enabled;
            if (champ == null || champ.Type != GameObjectType.AIHeroClient || !champ.IsValid) return;
            if (target != null)
            {
                if (useriu && (Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass))
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
            var useE = LaneClearMenu["E"].GetValue<MenuBool>().Enabled;
            var minE = LaneClearMenu["mine"].GetValue<MenuSlider>().Value;
            var minionsx = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(W.Range) && e.IsMinion())
                .Cast<AIBaseClient>().ToList();
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(E.Range));
            var ECanCast = W.GetLineFarmLocation(minionsx, W.Width);
            if (ECanCast.Position.IsValid())
                foreach (var minion in minions)
                {
                    if (useE && E.IsReady() && minion.IsValidTarget(E.Range) && ECanCast.MinionsHit >= minE)
                    {
                        E.Cast(ECanCast.Position);
                    }
                }
        }

        private static void Harass()
        {
            var useW = HarassMenu["HarassW"].GetValue<MenuBool>().Enabled;
            var useW2 = HarassMenu["HarassW2"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            var disE = HarassMenu["DistanceE"].GetValue<MenuSlider>().Value;
            var turret = HarassMenu["HTurret"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (target != null)
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && (disE <= target.Distance(ObjectManager.Player) || ObjectManager.Player.HealthPercent <= 20))
                {
                    if (turret)
                    {
                        if (!target.Position.UnderTuret())
                        {
                            E.Cast(target.Position);
                        }
                    }
                    else
                    {
                        E.Cast(target.Position);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (_Player.Distance(target) <= target.GetRealAutoAttackRange() && ObjectManager.Player.HealthPercent <= 60)
                    {
                        W.Cast();
                    }

                    if (useW2)
                    {
                        if (!target.IsFacing(ObjectManager.Player) && _Player.Distance(target) >= 325 && !E.IsReady())
                        {
                            W.Cast();
                        }
                    }
                    else
                    {
                        if (!target.IsFacing(ObjectManager.Player) && _Player.Distance(target) >= 325)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        public static void JungleClear()
        {
            var useE = JungleClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(E.Range));
            if (monster != null)
            {
                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.Cast(monster.Position);
                }
            }
        }

        public static void Flee()
        {
            E.Cast(fly.Position.Extend(Game.CursorPos, E.Range).ToVector2());
            /*if (E.IsReady())
            {
                var cursorPos = Game.CursorPos;
                var castPos = ObjectManager.Player.Position.Distance(cursorPos) <= E.Range ? cursorPos : ObjectManager.Player.Position.Extend(cursorPos, E.Range).To3D();
                E.Cast(castPos);
            }*/
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
            foreach (var target in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(R.Range) && !hero.HasBuff("FioraW") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.HasBuff("SpellShield") && !hero.HasBuff("NocturneShield") && !hero.IsDead && !hero.IsDead))
            {
                if (KsE && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (target.Health <= ObjectManager.Player.GetSpellDamage(target, SpellSlot.E))
                    {
                        E.Cast(target.Position);
                    }
                }
                if (Ignite != null && KillStealMenu["ign"].GetValue<MenuBool>().Enabled && Ignite.IsReady())
                {
                    if (target.Health + target.AllShield < _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
