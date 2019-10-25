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

namespace Ekko
{
    static class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Font Thm;
        public static GameObject EkkoREmitter { get; set; }
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Menu Menu, ComboMenu, JungleClearMenu, HarassMenu, Ulti, LaneClearMenu, Misc, KillSteals;

  
        // Menu

        public static void EkkoOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Ekko")) return;
            Game.Print("Victorious Ekko Loaded! PORT By DEATHGODX", Color.White);
            EkkoREmitter = ObjectManager.Get<EffectEmitter>().FirstOrDefault(x => x.Name.Equals("Ekko_Base_R_TrailEnd.troy"));
            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(250, 2200, 60, false, false, SkillshotType.Line);
            W = new Spell(SpellSlot.W, 1600);
            W.SetSkillshot(1500, 500, 650, false, false, SkillshotType.Circle);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, 375);
            Thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 16, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            var MenuEkko = new Menu("Ekko", "Ekko", true);
            MenuEkko.Add(new MenuSeparator("Mercedes7", "Mercedes7"));
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("CQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("CW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("CW2", "Use [W] No Prediction", false));
            ComboMenu.Add(new MenuBool("CE", "Use [E] Combo"));
            ComboMenu.Add(new MenuList("EMode", "Combo Mode:", new[] { "E To Target", "E To Mouse" }) { Index = 0 });
            ComboMenu.Add(new MenuKeyBind("CTurret", "Don't Use [E] UnderTurret", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            MenuEkko.Add(ComboMenu);
            Ulti = new Menu("Ulti Settings", "Ulti");
            Ulti.Add(new MenuSeparator("Ulti Settings", "Ulti Settings"));
            Ulti.Add(new MenuBool("RKs", "Use [R] Ks"));
            Ulti.Add(new MenuBool("RAoe", "Use [R] Aoe"));
            Ulti.Add(new MenuSlider("MinR", "Min Hit Enemies Use [R] Aoe", 3, 1, 5));
            Ulti.Add(new MenuBool("REscape", "Use [R] Low Hp"));
            Ulti.Add(new MenuSlider("RHp", "Below MyHp Use [R]", 20));
            MenuEkko.Add(Ulti);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HW", "Use [W] Harass"));
            HarassMenu.Add(new MenuBool("HW2", "Use [W] No Prediction", false));
            HarassMenu.Add(new MenuBool("HE", "Use [E] Harass"));
            HarassMenu.Add(new MenuBool("HTurret", "Don't [E] Under Turret"));
            HarassMenu.Add(new MenuSlider("MinE", "Limit Enemies Around Target Use [E] Harass", 5, 1, 5));
            HarassMenu.Add(new MenuSlider("HM", "Mana Harass", 50, 0, 100));
            MenuEkko.Add(HarassMenu);
            LaneClearMenu = new Menu("Laneclear Settings", "Clear");
            LaneClearMenu.Add(new MenuSeparator("Laneclear Settings", "Laneclear Settings"));
            LaneClearMenu.Add(new MenuBool("LQ", "Use [Q] Laneclear"));
            LaneClearMenu.Add(new MenuSlider("MinQ", "Min Hit Minions Use [Q] LaneClear", 3, 1, 6));
            LaneClearMenu.Add(new MenuBool("LE", "Use [E] Laneclear", false));
            LaneClearMenu.Add(new MenuSlider("LM", "Mana LaneClear", 60, 0, 100));
            MenuEkko.Add(LaneClearMenu);
            JungleClearMenu = new Menu("JungleClear Settings", "JungleClear");
            JungleClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            JungleClearMenu.Add(new MenuBool("JQ", "Use [Q] JungleClear"));
            JungleClearMenu.Add(new MenuBool("JW", "Use [W] JungleClear"));
            JungleClearMenu.Add(new MenuBool("JE", "Use [E] JungleClear"));
            JungleClearMenu.Add(new MenuSlider("JM", "Mana JungleClear", 20, 0, 100));
            MenuEkko.Add(JungleClearMenu);
            KillSteals = new Menu("KillSteal Settings", "KillSteal");
            KillSteals.Add(new MenuBool("QKs", "Use [Q] Ks"));
            KillSteals.Add(new MenuBool("EKs", "Use [E] Ks"));
            MenuEkko.Add(KillSteals);
            Misc = new Menu("Misc Settings", "Draw");
            Misc.Add(new MenuSeparator("Anti Gapcloser", "Anti Gapcloser"));
            Misc.Add(new MenuBool("antiGap", "Use [Q] Anti Gapcloser"));
            Misc.Add(new MenuBool("inter", "Use [W] Interupt", false));
            Misc.Add(new MenuBool("Qcc", "Use [Q] Immobile"));
            Misc.Add(new MenuBool("QPassive", "Auto [Q] Enemies With 2 Stacks"));
            Misc.Add(new MenuSeparator("Drawings Settings", "Drawings Settings"));
            Misc.Add(new MenuBool("Draw_Disabled", "Disabled Drawings", false));
            Misc.Add(new MenuBool("DrawE", "Draw [E]"));
            Misc.Add(new MenuBool("DrawQ", "Draw [Q]"));
            Misc.Add(new MenuBool("DrawW", "Draw [W]", false));
            Misc.Add(new MenuBool("DrawR", "Draw [R]"));
            Misc.Add(new MenuBool("DrawTR", "Status UnderTuret"));
            Misc.Add(new MenuSeparator("Skins Settings", "Skins Settings"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer", false));
            Misc.Add(new MenuSlider("skin.Id", "Skin Mode (F5)", 0, 0, 3));
            MenuEkko.Add(Misc);
            MenuEkko.Attach();
            ObjectManager.Player.SetSkin(Misc["skin.Id"].GetValue<MenuSlider>().Value);
            Misc["skin.Id"].GetValue<MenuSlider>().DisplayName = ObjectManager.Player.SkinName;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterrupterSpell += Interupt;
            //AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            GameObject.OnCreate += Game_On_Create;
            GameObject.OnDelete += Game_On_Delete;

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.None)
            {
                Flee();
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
            Ultimate();
            Miscs();

            if (EkkoREmitter == null && R.IsReady())
            {
                EkkoREmitter = ObjectManager.Get<EffectEmitter>().FirstOrDefault(x => x.Name.Equals("Ekko_Base_R_TrailEnd.troy"));
            }
        }

        // Drawings

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;

            if (Misc["Draw_Disabled"].GetValue<MenuBool>().Enabled) return;

            if (Misc["DrawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Misc["DrawW"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1);
            }

            if (Misc["DrawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Misc["DrawR"].GetValue<MenuBool>().Enabled && EkkoREmitter != null)
            {
                Render.Circle.DrawCircle(EkkoREmitter.Position, 375, Color.Yellow, 1);
            }

            if (Misc["DrawTR"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active)
                {
                    DrawFont(Thm, "Use E Under Turret : Disable", (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.White);
                }
                else
                {
                    DrawFont(Thm, "Use E Under Turret : Enable", (float)(ft[0] - 70), (float)(ft[1] + 50), SharpDX.Color.Red);
                }
            }
        }

        // Flee Mode

        private static void Flee()
        {
            /*if (E.IsReady())
            {
                var cursorPos = Game.CursorPosPressed;
                var castPos = ObjectManager.Player.Position.Distance(cursorPos) <= E.Range ? cursorPos : ObjectManager.Player.Position.Extend(cursorPos, E.Range).To3D();
                Player.CastSpell(SpellSlot.E,Game.CursorPos);
            }*/
        }

        // Skin Changer

        public static int SkinId()
        {
            return Misc["skin.Id"].GetValue<MenuSlider>().Value;
        }

        public static bool checkSkin()
        {
            return Misc["checkSkin"].GetValue<MenuBool>().Enabled;
        }

        //Damage

        public static double QDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 60, 75, 90, 105, 120 }[Q.Level] + 0.2f * _Player.FlatMagicDamageMod));
        }

        public static double EDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 40, 65, 90, 115, 140 }[E.Level] + 0.4f * _Player.FlatMagicDamageMod));
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 150, 300, 450 }[R.Level] + 1.5f * _Player.FlatMagicDamageMod));
        }

        // Create + Delete

        public static void Game_On_Create(GameObject sender, EventArgs args)
        {
            var Emitter = sender as EffectEmitter;
            if (Emitter != null)
            {
                if (Emitter.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    EkkoREmitter = Emitter;
                }
            }
        }

        public static void Game_On_Delete(GameObject sender, EventArgs args)
        {
            var Emitter = sender as EffectEmitter;
            if (Emitter != null)
            {
                if (Emitter.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    EkkoREmitter = null;
                }
            }
        }

        // Ultimate

        private static void Ultimate()
        {
            var Minr = Ulti["MinR"].GetValue<MenuSlider>().Value;
            var enemies = GameObjects.EnemyHeroes.Where(e => e.IsValidTarget() && !e.IsDead).ToArray();
            var useRAoe = Ulti["RAoe"].GetValue<MenuBool>().Enabled;

            if (useRAoe && R.IsReady())
            {
                if (EkkoREmitter != null)
                {
                    if (enemies != null)
                    {
                        int RCal = enemies.Where(e => e.Distance(EkkoREmitter.Position) <= R.Range).Count(); ;
                        if (RCal >= Minr)
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        // Interrupt

        public static void Interupt(AIHeroClient sender, Interrupter.InterruptSpellArgs i)
        {
            var Inter = Misc["inter"].GetValue<MenuBool>().Enabled;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Inter && W.IsReady() && i.DangerLevel == DangerLevel.High && _Player.Distance(sender) <= W.Range)
            {
                W.Cast(sender);
            }
        }

        //Harass Mode

        private static void Harass()
        {
            var useQ = HarassMenu["HQ"].GetValue<MenuBool>().Enabled;
            var useW = HarassMenu["HW"].GetValue<MenuBool>().Enabled;
            var useW2 = HarassMenu["HW2"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HE"].GetValue<MenuBool>().Enabled;
            var mana = HarassMenu["HM"].GetValue<MenuSlider>().Value;
            var minE = HarassMenu["MinE"].GetValue<MenuSlider>().Value;
            var turret = HarassMenu["HTurret"].GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (ObjectManager.Player.ManaPercent <= mana)
            {
                return;
            }
            if (target != null)
            {
                if (useQ && Q.CanCast(target) && Q.IsReady())
                {
                    var Qpred = Q.GetPrediction(target);
                    if (Qpred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(Qpred.CastPosition);
                    }
                }

                if (useW && W.CanCast(target) && W.IsReady())
                {
                    if (useW2)
                    {
                        W.Cast(target.Position);
                    }
                    else
                    {
                        var Wpred = W.GetPrediction(target);
                        if (Wpred.Hitchance >= HitChance.High)
                        {
                            W.Cast(Wpred.CastPosition);
                        }
                    }
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range + 375) && target.Position.CountEnemyHeroesInRange(R.Range) <= minE)
                {
                    if (turret)
                    {
                        if (!UnderTuret(target))
                        {
                            if (E.Cast( target.Position))
                            {
                               // Orbwalker.ResetAutoAttackTimer();
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (E.Cast(target.Position))
                        {
                            // Orbwalker.ResetAutoAttackTimer();
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            return;
                        }
                    }
                }
            }
        }

        private static bool UnderTuret(AIBaseClient target)
        {
            var tower = ObjectManager.Get<AITurretClient>().FirstOrDefault(turret => turret != null && turret.Distance(target) <= 775 && turret.IsValid && turret.Health > 0 && !turret.IsAlly);
            return tower != null;
        }

        //Combo Mode

        private static void Combo()
        {
            var useQ = ComboMenu["CQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["CW"].GetValue<MenuBool>().Enabled;
            var useW2 = ComboMenu["CW2"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["CE"].GetValue<MenuBool>().Enabled;
            var turret = ComboMenu["CTurret"].GetValue<MenuKeyBind>().Active;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (target != null)
            {
                if (useQ && Q.CanCast(target) && Q.IsReady())
                {
                    var Qpred = Q.GetPrediction(target);
                    if (Qpred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(Qpred.CastPosition);
                    }
                }

                if (useW && W.CanCast(target) && W.IsReady())
                {
                    if (useW2)
                    {
                        W.Cast(target.Position);
                    }
                    else
                    {
                        var Wpred = W.GetPrediction(target);
                        if (Wpred.Hitchance >= HitChance.High)
                        {
                            W.Cast(Wpred.CastPosition);
                        }
                    }
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range + 375) && (target.Distance(_Player.Position) >= 175 || ObjectManager.Player.HealthPercent <= 20))
                {
                    if (ComboMenu["EMode"].GetValue<MenuList>().Index == 0)
                    {
                        if (turret)
                        {
                            if (!UnderTuret(target))
                            {
                                if (E.Cast(target.Position))
                                {
                                    // Orbwalker.ResetAutoAttackTimer();
                                    //Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target), 500);
                                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                    return;
                                }

                            }
                        }
                        else
                        {
                            if (E.Cast(target.Position))
                            {
                                // Orbwalker.ResetAutoAttackTimer();
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                return;
                            }
                        }

                        if (ComboMenu["EMode"].GetValue<MenuList>().Index == 1)
                        {
                            if (turret)
                            {
                                if (!UnderTuret(target))
                                {
                                    if (E.Cast(Game.CursorPos)) ;
                                    {
                                        // Orbwalker.ResetAutoAttackTimer();
                                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (E.Cast(Game.CursorPos))
                            {
                                // Orbwalker.ResetAutoAttackTimer();
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                return;
                            }
                        }
                    }
                }
            }
        }

        //LaneClear Mode

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["LQ"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["LE"].GetValue<MenuBool>().Enabled;
            var mana = LaneClearMenu["LM"].GetValue<MenuSlider>().Value;
            var Minq = LaneClearMenu["MinQ"].GetValue<MenuSlider>().Value;
            var minionQ = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range) && e.IsMinion())
                            .Cast<AIBaseClient>().ToList();
            var quang = W.GetLineFarmLocation(minionQ, Q.Width);
            if (quang.Position.IsValid())
                if (ObjectManager.Player.ManaPercent <= mana)
                {
                    return;
                }

            foreach (var minion in minionQ)
            {
                if (useQ && Q.CanCast(minion) && quang.MinionsHit >= Minq && Q.IsReady())
                {
                    Q.Cast(quang.Position);
                }

                if (useE && E.IsReady() && E.CanCast(minion) && EDamage(minion) + ObjectManager.Player.GetAutoAttackDamage(minion) >= minion.Health)
                {
                    if (minion.Distance(_Player.Position) > ObjectManager.Player.GetRealAutoAttackRange(minion))
                    {
                        if (E.Cast(minion.Position))
                        {
                           // Orbwalker.ResetAutoAttackTimer();
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                            return;
                        }
                    }
                }
            }
        }

        // JungleClear Mode

        private static void JungleClear()
        {
            var monster = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            var useQ = JungleClearMenu["JQ"].GetValue<MenuBool>().Enabled;
            var useW = JungleClearMenu["JW"].GetValue<MenuBool>().Enabled;
            var useE = JungleClearMenu["JE"].GetValue<MenuBool>().Enabled;
            var mana = JungleClearMenu["JM"].GetValue<MenuSlider>().Value;

            if (ObjectManager.Player.ManaPercent <= mana)
            {
                return;
            }

            if (monster != null)
            {
                if (useQ && Q.IsReady())
                {
                    Q.Cast(monster.Position);
                }

                if (useW && monster.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(monster.Position);
                }

                if (useE  && E.IsReady())
                {
                    //Player.CastSpell(SpellSlot.E, monster.Position);
                    //E.CanCast(monster);
                    //if (Player.CastSpell(SpellSlot.E, monster.Position))

                    // Orbwalker.ResetAutoAttackTimer();
                    // Player.IssueOrder(GameObjectOrder.AttackUnit, monster);


                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        // KillSteal

        private static void KillSteal()
        {
            var Enemies = GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsDead);
            var useQ = KillSteals["QKs"].GetValue<MenuBool>().Enabled;
            var useE = KillSteals["EKs"].GetValue<MenuBool>().Enabled;
            var useR = Ulti["RKs"].GetValue<MenuBool>().Enabled;
            foreach (var target in Enemies)
            {
                if (useQ && Q.CanCast(target) && Q.IsReady())
                {
                    if (QDamage(target) >= target.Health)
                    {
                        var Qpred = Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(Qpred.CastPosition);
                        }
                    }
                }

                if (useE && E.IsReady() && target.Distance(_Player.Position) <= E.Range + 375)
                {
                    if (EDamage(target) >= target.Health)
                    {
                        if (E.Cast(target.Position))
                        {
                           // Orbwalker.ResetAutoAttackTimer();
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            return;
                        }
                    }
                }

                if (useR && R.IsReady() && EkkoREmitter != null)
                {
                    if (target.Health <= RDamage(target) && target.Distance(EkkoREmitter.Position) <= R.Range)
                    {
                        R.Cast();
                    }
                }
            }
        }

        // OnProcessSpellCast

        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var useREs = Ulti["REscape"].GetValue<MenuBool>().Enabled;
            var Hp = Ulti["RHp"].GetValue<MenuSlider>().Value;

            if (!sender.IsEnemy && (!(sender is AIHeroClient) || !(sender is AITurretClient)))
            {
                return;
            }

            if (!sender.IsValidTarget(Q.Range) && Q.IsReady())
            {
                return;
            }

            if (useREs && R.IsReady())
            {
                if (ObjectManager.Player.HealthPercent <= Hp)
                {
                    R.Cast();
                }

                if (sender.BaseAttackDamage >= ObjectManager.Player.Health + ObjectManager.Player.AllShield || sender.BaseAbilityDamage >= ObjectManager.Player.Health + ObjectManager.Player.AllShield)
                {
                    R.Cast();
                }

                if (sender.GetAutoAttackDamage(ObjectManager.Player) >= ObjectManager.Player.Health + ObjectManager.Player.AllShield)
                {
                    R.Cast();
                }
            }

            if (args.SData.Name == "ZedR")
            {
                if (R.IsReady())
                {
                    //Core.DelayAction(() => R.Cast(), 2000 - Game.Ping - 200);
                    R.Cast(Game.Ping == 200);
                }
            }
        }

        // Misc

        public static void Miscs()
        {
            var useQ = Misc["Qcc"].GetValue<MenuBool>().Enabled;
            var Passive = Misc["QPassive"].GetValue<MenuBool>().Enabled;
            var Enemies = GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && e.GetBuffCount("EkkoStacks") == 2 && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsDead);

            foreach (var target in Enemies)
            {
                if (Passive && Q.CanCast(target) && Q.IsReady())
                {
                    var Qpred = Q.GetPrediction(target);
                    if (Qpred.Hitchance >= HitChance.Medium)
                    {
                        Q.Cast(Qpred.CastPosition);
                    }
                }
            }

            var targetQ = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (useQ && Q.CanCast(targetQ) && Q.IsReady())
            {
                if (targetQ != null)
                {
                    if (targetQ.HasBuffOfType(BuffType.Stun) || targetQ.HasBuffOfType(BuffType.Snare) || targetQ.HasBuffOfType(BuffType.Knockup))
                    {
                        Q.Cast(targetQ.Position);
                    }
                }
            }
        }

        // AntiGap

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Misc["antiGap"].GetValue<MenuBool>().Enabled && sender.IsEnemy && sender.Distance(_Player.Position) <= 325 && Q.IsReady())
            {
                Q.Cast(sender);
            }
        }
    }
}
