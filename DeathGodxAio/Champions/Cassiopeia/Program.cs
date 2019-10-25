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
using SPrediction;


namespace Cassiopeia_Du_Couteau_2
{
    static class Program
    {
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        // public static ColorBGRA Red { get; private set; }
        //   public static ColorBGRA Cyan { get; private set; }
        public static Spell _Q;
        public static Spell _W;
        public static Spell _E;
        public static Spell _R;
        public static Spell _Ignite;
        public static Item Seraph;
        public static Item Zhonia;

        private static Menu StartMenu, ComboMenu, LastHitM, DebugC, DrawingsMenu, JungleMenu, ClearMenu, UtilityMenu, RSet, ESet, WSet, QSet, otheroptions;

      
        public static void CassiopeiaLoading_OnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Cassiopeia"))
            {
                return;
            }
            Game.Print("Cassiopeia Du Couteau by Horizon - Loaded PORTED By DEATHGODX", System.Drawing.Color.Crimson);
            _Q = new Spell(SpellSlot.Q, 750);//, SkillShotType.Circular, 400, int.MaxValue, 130);
            _W = new Spell(SpellSlot.W, 800);//, SkillShotType.Circular, 250, 250, 160);
            _E = new Spell(SpellSlot.E, 700);
            _R = new Spell(SpellSlot.R, 800);//, SkillShotType.Cone, 250, 250, 80);
            Zhonia = new Item((int)ItemId.Zhonyas_Hourglass, 450);
            Seraph = new Item((int)ItemId.Seraphs_Embrace, 450);
            _Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            StartMenu = new Menu("Cassiopeia", "Cassiopeia", true);
            ComboMenu = new Menu("General/Combo Settings", "General/Combo Settings");
            ClearMenu = new Menu("Clearing Menu", "Clearing Menu");
            JungleMenu = new Menu("JClearing Menu", "JClearing Menu");
            DrawingsMenu = new Menu("Drawing Settings", "Drawing Settings");
            DebugC = new Menu("Debug", "Debug");
            ComboMenu.Add(new MenuSeparator("Cassiopeia Du Couteau by Horizon", "Cassiopeia Du Couteau by Horizon"));
            ComboMenu.Add(new MenuList("PredHit", "Prediction", new[] { "HitChance :: High", "HitChance :: Medium", "HitChance :: Low" }) { Index = 0 });
            ComboMenu.Add(new MenuBool("DrawStatus", "Draw Current Orbwalker mode ? [BETA]"));
            ComboMenu.Add(new MenuSeparator("If you wanna use drawing orbwalker mode you need 16:9 resoultion,", "If you wanna use drawing orbwalker mode you need 16:9 resoultion,"));
            ComboMenu.Add(new MenuSeparator("In future i will add customizable position.", "In future i will add customizable position."));
            ComboMenu.Add(new MenuBool("DisableAA", "Disable AA in Combo for faster Kite", false));
            QSet = new Menu("Q Spell Settings", "Q Spell Settings");
            QSet.Add(new MenuBool("UseQ", "Use [Q]"));
            QSet.Add(new MenuBool("UseQH", "Use [Q] in Harass"));
            QSet.Add(new MenuBool("UseS", "Use [Q] Mana Saver?", false));
            QSet.Add(new MenuBool("UseQI", "Use always [Q] if enemy is immobile?"));
            QSet.Add(new MenuBool("UseQ2", "Try to hit =< 2 champions if can ?"));
            QSet.Add(new MenuBool("UseQPok", "Use always [Q] if enemy is killable by Poison?"));
            QSet.Add(new MenuBool("QComboDash", "Always use [Q] on Dash end position?"));
            ComboMenu.Add(QSet);
            WSet = new Menu("W Spell Settings", "W Spell Settings");
            WSet.Add(new MenuBool("UseW", "Use [W]"));
            WSet.Add(new MenuBool("UseWH", "Use [W] in Harass", false));
            WSet.Add(new MenuBool("UseW2", "Try to hit =< 2 champions if can ?"));
            ComboMenu.Add(WSet);
            ESet = (new Menu("E Spell Settings", "E Spell Settings"));
            ESet.Add(new MenuBool("UseE", "Use [E]"));
            ESet.Add(new MenuBool("UseEH", "Use [E] in Harass"));
            ESet.Add(new MenuBool("UseES", "Use [E] casting speedup ? (animation cancel)"));
            ESet.Add(new MenuBool("UseEK", "Use [E] always if enemy is killable?"));
            ComboMenu.Add(ESet);
            RSet = new Menu("R Spell Settings", "R Spell Settings");
            RSet.Add(new MenuBool("UseR", "Use [R]"));
            RSet.Add(new MenuBool("UseRFace", "Use [R] only on facing enemy ?"));
            RSet.Add(new MenuBool("RGapClose", "Try use [R] for Gapclosing enemy ?", false));
            RSet.Add(new MenuBool("Rint", "Try use [R] for interrupt enemy ?"));
            RSet.Add(new MenuBool("UseRG", "Use [R] use minimum enemys for R ?"));
            RSet.Add(new MenuSlider("UseRGs", "Minimum people for R", 1, 1, 5));
            ComboMenu.Add(RSet);
            otheroptions = (new Menu("Other options", "Other options"));
            otheroptions.Add(new MenuBool("Ignite", "Use Summoner Ignite if target is killable ?"));
            otheroptions.Add(new MenuBool("Zhonya", "Use Zhonya if dangerous ?"));
            otheroptions.Add(new MenuSlider("ZhonyaHP", "Zhonya Health for use?  %", 25));
            otheroptions.Add(new MenuBool("Seraph", "Use Seraph"));
            otheroptions.Add(new MenuSlider("SeraphHP", "Seraph's Health for use? %", 35));
            otheroptions.Add(new MenuSlider("SeraphMana", "Minimum Mana for Seraph's use? %", 40));
            ComboMenu.Add(otheroptions);
            //     ClearMenu.Add("EMode", new MenuList("Clear E mode", 0, "Always", "Poisoned"));
            StartMenu.Add(ComboMenu);
            ClearMenu.Add(new MenuBool("UseQCL", "Use [Q] in clear ?"));
            ClearMenu.Add(new MenuBool("UseWCL", "Use [W] in clear ?", false));
            ClearMenu.Add(new MenuBool("UseECL", "Use [E] in clear ?"));
            ClearMenu.Add(new MenuBool("UseQLH", "Use [Q] in LastHit ?", false));
            ClearMenu.Add(new MenuBool("UseWLH", "Use [W] in LastHit ?", false));
            ClearMenu.Add(new MenuBool("UseELH", "Use [E] in LastHit ?"));
            ClearMenu.Add(new MenuSlider("ClearMana", "Minimum mana for clear %", 50));
            StartMenu.Add(ClearMenu);
            JungleMenu.Add(new MenuBool("UseJQCL", "Use [Q] in Jclear ?"));
            JungleMenu.Add(new MenuBool("UseJWCL", "Use [W] in Jclear ?", false));
            JungleMenu.Add(new MenuBool("UseJECL", "Use [E] in Jclear ?"));
            JungleMenu.Add(new MenuBool("UseJQLH", "Use [Q] in JLastHit ?", false));
            JungleMenu.Add(new MenuBool("UseJWLH", "Use [W] in JLastHit ?", false));
            JungleMenu.Add(new MenuBool("UseJELH", "Use [E] in JLastHit ?"));
            JungleMenu.Add(new MenuSlider("ClearManaJ", "Minimum mana J for clear %", 50));
            StartMenu.Add(JungleMenu);
            DrawingsMenu.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            DrawingsMenu.Add(new MenuSeparator("Tick for enable/disable spell drawings", "Tick for enable/disable spell drawings"));
            DrawingsMenu.Add(new MenuBool("DQ", "Draw [Q] range"));
            DrawingsMenu.Add(new MenuBool("QPred", "Draw [Q] Prediction"));
            DrawingsMenu.Add(new MenuBool("DW", "Draw [W] range"));
            DrawingsMenu.Add(new MenuBool("DE", "Draw [E] range"));
            DrawingsMenu.Add(new MenuBool("DR", "Draw [R] range"));
            StartMenu.Add(DrawingsMenu);
            DebugC.Add(new MenuBool("Debug", "Debug Console+Chat", false));
            DebugC.Add(new MenuBool("DrawStatus1", "Debug Curret Orbawlker mode"));
            StartMenu.Add(DebugC);
            StartMenu.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterrupterSpell += Interruptererer;
            //skin.OnValueChange += Skin_OnValueChange;
            Dash.OnDash += Dash_OnDash;
            //Player.SetSkinId(ComboMenu["SkinMisc"].GetValue<MenuSlider>().Value);
        }
        public static bool PoisonWillExpire(this AIBaseClient target, float time)
        {
            var buff = target.Buffs.OrderByDescending(x => x.EndTime).FirstOrDefault(x => x.Type == BuffType.Poison && x.IsActive && x.IsValid);
            return buff == null || time > (buff.EndTime - Game.Time) * 1000f;
        }
        private static bool Immobile(AIBaseClient unit)
        {
            return unit.HasBuffOfType(BuffType.Charm) || unit.HasBuffOfType(BuffType.Stun) ||
                   unit.HasBuffOfType(BuffType.Knockup) || unit.HasBuffOfType(BuffType.Snare) ||
                   unit.HasBuffOfType(BuffType.Taunt) || unit.HasBuffOfType(BuffType.Suppression) ||
                   unit.HasBuffOfType(BuffType.Polymorph);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || MenuGUI.IsChatOpen || ObjectManager.Player.IsWindingUp)
            {
                return;
            }
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LastHit:
                    LastHit();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
            Ignite();
            ImmobileQ();
            KillSteal();
            Zhonya();
            SeraphsEmbrace();
        }
        public static void LastHit()
        {
            var MHR = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= _Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (MHR != null)
            {



                if (ClearMenu["UseQLH"].GetValue<MenuBool>().Enabled && _Q.IsReady() && ObjectManager.Player.ManaPercent > ClearMenu["ClearMana"].GetValue<MenuSlider>().Value && MHR.IsValidTarget(_Q.Range) &&
                    ObjectManager.Player.GetSpellDamage(MHR, SpellSlot.Q) >= MHR.Health)

                {
                    _Q.Cast(MHR);
                }


                if (ClearMenu["UseWLH"].GetValue<MenuBool>().Enabled && _W.IsReady() && ObjectManager.Player.GetSpellDamage(MHR, SpellSlot.W) >= MHR.Health &&
                    ObjectManager.Player.ManaPercent > ClearMenu["ClearMana"].GetValue<MenuSlider>().Value)
                {
                    _W.Cast(MHR.Position);
                }




            }
        }

        public static void JungleClear()
        {
            var MHR = GameObjects.Jungle.Where(a => a.Distance(ObjectManager.Player) <= _Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (MHR != null)
            {
                if (_Q.IsReady() && _Player.ManaPercent > JungleMenu["ClearManaJ"].GetValue<MenuSlider>().Value && JungleMenu["UseJQCL"].GetValue<MenuBool>().Enabled && MHR.IsValidTarget(_Q.Range))
                {
                    _Q.Cast(MHR);
                }
            }

            if (_W.IsReady() && _Q.IsReady() == false && _Player.ManaPercent > JungleMenu["ClearManaJ"].GetValue<MenuSlider>().Value && JungleMenu["UseJWCL"].GetValue<MenuBool>().Enabled && MHR.IsValidTarget(_W.Range))
            {
                _W.Cast(MHR.Position);
            }
            if (_E.IsReady() && _Player.ManaPercent > JungleMenu["ClearManaJ"].GetValue<MenuSlider>().Value && JungleMenu["UseJECL"].GetValue<MenuBool>().Enabled && MHR.IsValidTarget(_E.Range))
            {
                _E.Cast(MHR);
            }
        }

        public static void LaneClear()

        {
            if (_Q.IsReady() && ClearMenu["UseQCL"].GetValue<MenuBool>().Enabled)
            {
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(_Q.Range))
                {

                    if (minion.IsValidTarget(_Q.Range) && minion != null && ClearMenu["UseQCL"].GetValue<MenuBool>().Enabled)
                    {
                        _Q.CastOnUnit(minion);
                    }
                }
            }
            var MHR = GameObjects.EnemyMinions.Where(a => a.Distance(ObjectManager.Player) <= _Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (MHR != null)

                if (ClearMenu["UseWCL"].GetValue<MenuBool>().Enabled)
                {
                    if (_W.IsReady())
                    {
                        _W.Cast(MHR.Position);
                    }

                }

            if (ClearMenu["UseECL"].GetValue<MenuBool>().Enabled && _E.IsReady() && ObjectManager.Player.ManaPercent > ClearMenu["ClearMana"].GetValue<MenuSlider>().Value && MHR.IsValidTarget(_E.Range))

            {
                _E.Cast(MHR);
            }
            foreach (var minion in GetEnemyLaneMinionsTargetsInRange(_E.Range))
            {

                if (minion.Health <= GameObjects.Player.GetSpellDamage(minion, SpellSlot.E))
                {
                    if (ClearMenu["UseELH"].GetValue<MenuBool>().Enabled)
                    {
                        if (minion.Distance(GameObjects.Player) > 250)
                        {
                            _E.CastOnUnit(minion);
                        }
                    }
                    if (ClearMenu["UseELH"].GetValue<MenuBool>().Enabled)
                    {
                        _E.CastOnUnit(minion);
                    }

                }

            }

        }

        public static List<AIMinionClient> GetGenericJungleMinionsTargets()
        {
            return GetGenericJungleMinionsTargetsInRange(float.MaxValue);
        }

        public static List<AIMinionClient> GetGenericJungleMinionsTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(m => !GameObjects.JungleSmall.Contains(m) && m.IsValidTarget(range)).ToList();
        }

        public static List<AIMinionClient> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }

        public static List<AIMinionClient> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range)).ToList();
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            if (target == null) return;
            if (ESet["UseEH"].GetValue<MenuBool>().Enabled)
            {
                if (!target.IsValidTarget(_E.Range) && !_E.IsReady())
                    return;
                {
                    if (_E.IsReady() && ESet["UseES"].GetValue<MenuBool>().Enabled)
                    {

                        _E.Cast(target);
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                        {
                            Game.Print("Casting E with speedup");
                            Console.WriteLine(Game.Time + "Casting E with Speedup");
                        }

                    }
                    if (_E.IsReady() && !ESet["UseES"].GetValue<MenuBool>().Enabled)
                    {
                        _E.Cast(target);
                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                        {
                            Game.Print("Casting E with normal");
                            Console.WriteLine(Game.Time + "Casting E with Speedup");
                        }
                    }
                }
            }
            if (WSet["UseWH"].GetValue<MenuBool>().Enabled)
            {
                if (!_W.IsReady() && _Player.Distance(target) >= 500) return;
                {

                    var Wpred = _W.GetPrediction(target);
                    if (Wpred.Hitchance >= HitChance.High && target.IsValidTarget(_W.Range))
                    {
                        if (WSet["UseW2"].GetValue<MenuBool>().Enabled)
                        {
                            var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_W.Range));
                            if (Enemys != null)
                            {
                                if (Enemys.Count() >= 2)
                                {
                                    _W.Cast(target.Position);
                                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                    {

                                        Game.Print("Casting W Found more than >= 2 People ");
                                        Console.WriteLine("Casting W Found more than >= 2 People");
                                    }
                                }
                                else if (Enemys.Count() >= 1)
                                {
                                    _W.Cast(target.Position);
                                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                    {

                                        Game.Print("Casting W Found more than >= 1 People ");
                                        Console.WriteLine("Casting W Found more than >= 1 People");
                                    }
                                }
                            }
                        }
                    }

                }
            }
            if (!WSet["UseW2"].GetValue<MenuBool>().Enabled)
            {
                _W.Cast(target.Position);
                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                {

                    Game.Print("Casting W ");
                    Console.WriteLine("Casting W");
                }
            }

            if (QSet["UseQH"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
            {
                if (_Q.IsReady())
                {
                    var canHitMoreThanOneTarget =
                      GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                      .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 1);
                    if (canHitMoreThanOneTarget != null)
                    {
                        var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                        //var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count;
                        var Qpred = _Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                        {
                            _Q.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {

                                Game.Print("FOUND 1 PEOPLE FOR Q ");
                                Console.WriteLine("FOUND 1 PEOPLE FOR Q");
                            }
                        }
                    }
                }
            }
            if (QSet["UseQH"].GetValue<MenuBool>().Enabled)
            {

                if (!target.IsValidTarget(_Q.Range))
                    return;
                {
                    if (_Q.IsReady() && QSet["UseS"].GetValue<MenuBool>().Enabled)
                    {
                        var Qpred = _Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                        {
                            if (!target.PoisonWillExpire(250))
                                return;
                            {
                                _Q.Cast(target.Position);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {

                                    Game.Print("Casting Q with HIGH pred saver ");
                                    Console.WriteLine("Casting Q with HIGH pred saver ");
                                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                                }
                            }
                        }

                    }

                }
            }
        }
        private static void Zhonya()
        {
            var Zhonya = otheroptions["Zhonya"].GetValue<MenuBool>().Enabled;
            var ZhonyaHP = otheroptions["ZhonyaHP"].GetValue<MenuSlider>().Value;
            if (!Zhonya || !Zhonia.IsReady || !Zhonia.IsOwned()) return;
            if (_Player.HealthPercent <= ZhonyaHP && _Player.CountEnemyHeroesInRange(500) >= 1)
            {
                Zhonia.Cast();
            }
        }
        private static void SeraphsEmbrace()
        {
            if (Seraph.IsReady && Seraph.IsOwned())
            {
                var embrace = otheroptions["Seraph"].GetValue<MenuBool>().Enabled;
                var shealth = otheroptions["SeraphHP"].GetValue<MenuSlider>().Value;
                var smana = otheroptions["SeraphMana"].GetValue<MenuSlider>().Value;
                if (!embrace || !Zhonia.IsReady || !Zhonia.IsOwned()) return;
                if (_Player.HealthPercent <= shealth && _Player.ManaPercent >= smana && _Player.CountEnemyHeroesInRange(500) >= 1)
                {
                    Seraph.Cast();
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            var Combo = Orbwalker.ActiveMode == OrbwalkerMode.Combo;
            var LastHit = Orbwalker.ActiveMode == OrbwalkerMode.LastHit;
            var LaneClear = Orbwalker.ActiveMode == OrbwalkerMode.LaneClear;
            var Harass = Orbwalker.ActiveMode == OrbwalkerMode.Harass;

            if (DrawingsMenu["DQ"].GetValue<MenuBool>().Enabled && _Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _Q.Range, Color.Lime, 1);
            }
            if (DrawingsMenu["DE"].GetValue<MenuBool>().Enabled && _E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _E.Range, Color.Lime, 1);
            }
            if (DrawingsMenu["DR"].GetValue<MenuBool>().Enabled && _R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _R.Range, Color.Lime, 1);
            }
            if (DrawingsMenu["QPred"].GetValue<MenuBool>().Enabled && _Q.IsReady())
            {
                if (target == null)
                    return;
                Drawing.DrawCircle(_Q.GetPrediction(target).CastPosition, _Q.Width, System.Drawing.Color.Violet);

            }
            if (ComboMenu["DrawStatus"].GetValue<MenuBool>().Enabled)

            {
                if (Harass && !Combo && LaneClear && LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.93f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                }
                if (Harass && Combo && !LaneClear && LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.93f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                }
                if (Harass && Combo && LaneClear && !LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.93f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }
                if (Harass && Combo && LaneClear && LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.93f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.95f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }
                if (Harass && !Combo && !LaneClear && LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                }
                if (Harass && Combo && !LaneClear && !LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                }

                if (Harass && LaneClear && !Combo && !LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }
                if (Harass && !LaneClear && !Combo && !LastHit)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Harass ]");
                }
                if (LaneClear && LastHit && !Combo && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }
                if (LaneClear && Combo && !LastHit && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }
                if (LaneClear && LastHit && Combo && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.93f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }
                if (LaneClear && !LastHit && !Combo && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: LaneClear ]");
                }

                if (LastHit && Combo && !LaneClear && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.91f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                }
                if (LastHit && !Combo && !LaneClear && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: LastHit ]");
                }

                if (Combo && !LastHit && !LaneClear && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: Combo ]");
                }

                else if (!Combo && !LastHit && !LaneClear && !Harass)
                {
                    Drawing.DrawText(Drawing.Width * 0.72f, Drawing.Height * 0.89f, System.Drawing.Color.White, "[ Orbwalker Mode: None ]");
                }
            }
        }
        private static void Combo()
        {

            var HighP = ComboMenu["PredHit"].GetValue<MenuList>().Index == 0;
            var MediumP = ComboMenu["PredHit"].GetValue<MenuList>().Index == 1;
            var LowP = ComboMenu["PredHit"].GetValue<MenuList>().Index == 2;
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            var targetQ2 = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (ComboMenu["DisableAA"].GetValue<MenuBool>().Enabled && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Orbwalker.AttackState = true;
            }
            if (!ComboMenu["DisableAA"].GetValue<MenuBool>().Enabled && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Orbwalker.AttackState = false;
            }
            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
            {
                Orbwalker.AttackState = false;
            }
            if (HighP)
            {
                if (ESet["UseE"].GetValue<MenuBool>().Enabled)
                {
                    if (!target.IsValidTarget(_E.Range) && !_E.IsReady())
                        return;
                    {
                        if (_E.IsReady() && ESet["UseES"].GetValue<MenuBool>().Enabled)
                        {

                            _E.Cast(target);
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {
                                Game.Print("Casting E with speedup");
                                Console.WriteLine(Game.Time + "Casting E with Speedup");
                            }

                        }
                        if (_E.IsReady() && !ESet["UseES"].GetValue<MenuBool>().Enabled)
                        {
                            _E.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {
                                Game.Print("Casting E with normal");
                                Console.WriteLine(Game.Time + "Casting E with Speedup");
                            }
                        }
                    }
                }

                if (WSet["UseW"].GetValue<MenuBool>().Enabled)
                {
                    if (!_W.IsReady() && _Player.Distance(target) >= 500) return;
                    {

                        var Wpred = _W.GetPrediction(target);
                        if (Wpred.Hitchance >= HitChance.High && target.IsValidTarget(_W.Range))
                        {
                            if (WSet["UseW2"].GetValue<MenuBool>().Enabled)
                            {
                                var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_W.Range));
                                if (Enemys != null)
                                {
                                    if (Enemys.Count() >= 2)
                                    {
                                        _W.Cast(target.Position);
                                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                        {

                                            Game.Print("Casting W Found more than >= 2 People ");
                                            Console.WriteLine("Casting W Found more than >= 2 People");
                                        }
                                    }
                                    else if (Enemys.Count() >= 1)
                                    {
                                        _W.Cast(target.Position);
                                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                        {

                                            Game.Print("Casting W Found more than >= 1 People ");
                                            Console.WriteLine("Casting W Found more than >= 1 People");
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                if (!WSet["UseW2"].GetValue<MenuBool>().Enabled)
                {
                    _W.Cast(target.Position);
                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                    {

                        Game.Print("Casting W ");
                        Console.WriteLine("Casting W");
                    }
                }

                if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 2);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                            // var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count;

                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                            {
                                _Q.Cast(target);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {

                                    Game.Print("FOUND MORE THAN 2 PEOPLE FOR Q ");
                                    Console.WriteLine("FOUND MORE THAN 2 PEOPLE FOR Q");
                                }
                            }
                        }
                    }

                }

                if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 1);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                            //var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count;
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                            {
                                _Q.Cast(target);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {

                                    Game.Print("FOUND 1 PEOPLE FOR Q ");
                                    Console.WriteLine("FOUND 1 PEOPLE FOR Q");
                                }
                            }
                        }
                    }
                }
                if (QSet["UseQ"].GetValue<MenuBool>().Enabled)
                {

                    if (!target.IsValidTarget(_Q.Range))
                        return;
                    {
                        if (_Q.IsReady() && QSet["UseS"].GetValue<MenuBool>().Enabled)
                        {
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                            {
                                if (!target.PoisonWillExpire(250))
                                    return;
                                {
                                    _Q.Cast(target.Position);
                                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                    {

                                        Game.Print("Casting Q with HIGH pred saver ");
                                        Console.WriteLine("Casting Q with HIGH pred saver ");
                                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                                    }
                                }
                            }

                        }

                    }
                }

                if (!QSet["UseS"].GetValue<MenuBool>().Enabled && QSet["UseQ"].GetValue<MenuBool>().Enabled && !QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {

                        var Qpred = _Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.High && target.IsValidTarget(_Q.Range))
                        {
                            _Q.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {

                                Game.Print("Casting Q with HIGH pred ");
                                Console.WriteLine("Casting Q with HIGH pred ");
                                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                            }
                        }
                    }



                }

                if (RSet["UseR"].GetValue<MenuBool>().Enabled && RSet["UseRG"].GetValue<MenuBool>().Enabled && _R.IsReady())
                {
                    var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_R.Range - 25));
                    if (Enemys != null)
                    {
                        if (Enemys.Count() >= RSet["UseRGs"].GetValue<MenuSlider>().Value && target.IsFacing(_Player) && RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target);
                        }
                        if (Enemys.Count() >= RSet["UseRGs"].GetValue<MenuSlider>().Value && !RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target);
                        }
                    }

                }

                if (RSet["UseR"].GetValue<MenuBool>().Enabled && _R.IsReady())
                {
                    if (!_R.IsReady()) return;
                    {
                        if (target.IsFacing(_Player) && target.IsValidTarget(_R.Range) && RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target.Position);
                        }
                    }
                    if (target.IsValidTarget(_R.Range) && !RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                        _R.Cast(target.Position);
                    }

                }
            }

            if (MediumP)
            {
                if (ESet["UseE"].GetValue<MenuBool>().Enabled)
                {
                    if (!target.IsValidTarget(_E.Range) && !_E.IsReady())
                        return;
                    {
                        if (_E.IsReady() && ESet["UseES"].GetValue<MenuBool>().Enabled)
                        {

                            _E.Cast(target);
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {
                                Game.Print("Casting E with speedup");
                                Console.WriteLine(Game.Time + "Casting E with Speedup");
                            }

                        }
                        if (_E.IsReady() && !ESet["UseES"].GetValue<MenuBool>().Enabled)
                        {
                            _E.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {
                                Game.Print("Casting E with normal");
                                Console.WriteLine(Game.Time + "Casting E with Speedup");
                            }
                        }
                    }
                }

                if (WSet["UseW"].GetValue<MenuBool>().Enabled)
                {
                    if (!_W.IsReady() && _Player.Distance(target) >= 500) return;
                    {

                        var Wpred = _W.GetPrediction(target);
                        if (Wpred.Hitchance >= HitChance.Medium && target.IsValidTarget(_W.Range))
                        {
                            if (WSet["UseW2"].GetValue<MenuBool>().Enabled)
                            {
                                var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_W.Range));
                                if (Enemys != null)
                                {
                                    if (Enemys.Count() >= 2)
                                    {
                                        _W.Cast(target.Position);
                                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                        {

                                            Game.Print("Casting W Found more than >= 2 People ");
                                            Console.WriteLine("Casting W Found more than >= 2 People");
                                        }
                                    }
                                    else if (Enemys.Count() >= 1)
                                    {
                                        _W.Cast(target.Position);
                                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                        {


                                            Game.Print("Casting W Found more than >= 1 People ");
                                            Console.WriteLine("Casting W Found more than >= 1 People");
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                if (!WSet["UseW2"].GetValue<MenuBool>().Enabled)
                {
                    _W.Cast(target.Position);
                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                    {

                        Game.Print("Casting W ");
                        Console.WriteLine("Casting W");
                    }
                }

                if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 2);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                            //var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count;

                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.Medium && target.IsValidTarget(_Q.Range))
                            {
                                _Q.Cast(target);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {

                                    Game.Print("FOUND MORE THAN 2 PEOPLE FOR Q ");
                                    Console.WriteLine("FOUND MORE THAN 2 PEOPLE FOR Q");
                                }
                            }
                        }
                    }

                }

                if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 1);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                            //var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.Medium && target.IsValidTarget(_Q.Range))
                            {
                                _Q.Cast(target);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {

                                    Game.Print("FOUND 1 PEOPLE FOR Q ");
                                    Console.WriteLine("FOUND 1 PEOPLE FOR Q");
                                }
                            }
                        }

                    }

                }
                if (QSet["UseQ"].GetValue<MenuBool>().Enabled)
                {

                    if (!target.IsValidTarget(_Q.Range))
                        return;
                    {
                        if (_Q.IsReady() && QSet["UseS"].GetValue<MenuBool>().Enabled)
                        {
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.Medium && target.IsValidTarget(_Q.Range))
                            {
                                if (!target.PoisonWillExpire(250))
                                    return;
                                {
                                    _Q.Cast(target.Position);
                                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                    {

                                        Game.Print("Casting Q with HIGH pred saver ");
                                        Console.WriteLine("Casting Q with HIGH pred saver ");
                                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                                    }
                                }

                            }


                        }

                    }
                }

                if (!QSet["UseS"].GetValue<MenuBool>().Enabled && QSet["UseQ"].GetValue<MenuBool>().Enabled && !QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {

                        var Qpred = _Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.Medium && target.IsValidTarget(_Q.Range))
                        {
                            _Q.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {

                                Game.Print("Casting Q with HIGH pred ");
                                Console.WriteLine("Casting Q with HIGH pred ");
                                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                            }
                        }

                    }



                }

                if (RSet["UseR"].GetValue<MenuBool>().Enabled && RSet["UseRG"].GetValue<MenuBool>().Enabled && _R.IsReady())
                {
                    var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_R.Range - 25));
                    if (Enemys != null)
                    {
                        if (Enemys.Count() >= RSet["UseRGs"].GetValue<MenuSlider>().Value && target.IsFacing(_Player) && RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target);
                        }
                        if (Enemys.Count() >= RSet["UseRGs"].GetValue<MenuSlider>().Value && !RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target);
                        }
                    }


                }

                if (RSet["UseR"].GetValue<MenuBool>().Enabled && _R.IsReady())
                {
                    if (!_R.IsReady()) return;
                    {
                        if (target.IsFacing(_Player) && target.IsValidTarget(_R.Range) && RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target.Position);
                        }
                    }
                    if (target.IsValidTarget(_R.Range) && !RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                        _R.Cast(target.Position);
                    }

                }
            }
            if (LowP)
            {
                if (ESet["UseE"].GetValue<MenuBool>().Enabled)
                {
                    if (!target.IsValidTarget(_E.Range) && !_E.IsReady())
                        return;
                    {
                        if (_E.IsReady() && ESet["UseES"].GetValue<MenuBool>().Enabled)
                        {

                            _E.Cast(target);
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {
                                Game.Print("Casting E with speedup");
                                Console.WriteLine(Game.Time + "Casting E with Speedup");
                            }


                        }
                        if (_E.IsReady() && !ESet["UseES"].GetValue<MenuBool>().Enabled)
                        {
                            _E.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {
                                Game.Print("Casting E with normal");
                                Console.WriteLine(Game.Time + "Casting E with Speedup");
                            }
                        }
                    }
                }

                if (WSet["UseW"].GetValue<MenuBool>().Enabled)
                {
                    if (!_W.IsReady() && _Player.Distance(target) >= 500) return;
                    {

                        var Wpred = _W.GetPrediction(target);
                        if (Wpred.Hitchance >= HitChance.Low && target.IsValidTarget(_W.Range))
                        {
                            if (WSet["UseW2"].GetValue<MenuBool>().Enabled)
                            {
                                var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_W.Range));
                                if (Enemys != null)
                                {
                                    if (Enemys.Count() >= 2)
                                    {
                                        _W.Cast(target.Position);
                                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                        {

                                            Game.Print("Casting W Found more than >= 2 People ");
                                            Console.WriteLine("Casting W Found more than >= 2 People");
                                        }
                                    }
                                    else if (Enemys.Count() >= 1)
                                    {
                                        _W.Cast(target.Position);
                                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                        {

                                            Game.Print("Casting W Found more than >= 1 People ");
                                            Console.WriteLine("Casting W Found more than >= 1 People");
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
                if (!WSet["UseW2"].GetValue<MenuBool>().Enabled)
                {
                    _W.Cast(target.Position);
                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                    {

                        Game.Print("Casting W ");
                        Console.WriteLine("Casting W");
                    }
                }

                if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 2);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                            //var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count;
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.Low && target.IsValidTarget(_Q.Range))
                            {
                                _Q.Cast(target);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {
                                    //radi is gay
                                    Game.Print("FOUND MORE THAN 2 PEOPLE FOR Q ");
                                    Console.WriteLine("FOUND MORE THAN 2 PEOPLE FOR Q");
                                }
                            }
                        }
                    }

                }

                if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          GameObjects.EnemyHeroes.OrderByDescending(x => x.CountEnemyHeroesInRange(_Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(_Q.Range) && x.CountEnemyHeroesInRange(_Q.Width) >= 1);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = GameObjects.EnemyHeroes.Find(x => x.IsValidTarget() && x.IsValidTarget(_Q.Width));
                            //var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.Position) / getAllTargets.Count;
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.Low && target.IsValidTarget(_Q.Range))
                            {
                                _Q.Cast(target);
                                if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                {

                                    Game.Print("FOUND 1 PEOPLE FOR Q ");
                                    Console.WriteLine("FOUND 1 PEOPLE FOR Q");
                                }
                            }
                        }
                    }
                }
                if (QSet["UseQ"].GetValue<MenuBool>().Enabled)
                {

                    if (!target.IsValidTarget(_Q.Range))
                        return;
                    {
                        if (_Q.IsReady() && QSet["UseS"].GetValue<MenuBool>().Enabled)
                        {
                            var Qpred = _Q.GetPrediction(target);
                            if (Qpred.Hitchance >= HitChance.Low && target.IsValidTarget(_Q.Range))
                            {
                                if (!target.PoisonWillExpire(250))
                                    return;
                                {
                                    _Q.Cast(target.Position);
                                    if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                                    {

                                        Game.Print("Casting Q with HIGH pred saver ");
                                        Console.WriteLine("Casting Q with HIGH pred saver ");
                                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                                    }
                                }
                            }

                        }

                    }
                }

                if (!QSet["UseS"].GetValue<MenuBool>().Enabled && QSet["UseQ"].GetValue<MenuBool>().Enabled && !QSet["UseQ2"].GetValue<MenuBool>().Enabled)
                {
                    if (_Q.IsReady())
                    {

                        var Qpred = _Q.GetPrediction(target);
                        if (Qpred.Hitchance >= HitChance.Low && target.IsValidTarget(_Q.Range))
                        {
                            _Q.Cast(target);
                            if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                            {

                                Game.Print("Casting Q with HIGH pred ");
                                Console.WriteLine("Casting Q with HIGH pred ");
                                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                            }
                        }
                    }



                }

                if (RSet["UseR"].GetValue<MenuBool>().Enabled && RSet["UseRG"].GetValue<MenuBool>().Enabled && _R.IsReady())
                {
                    var Enemys = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(_R.Range - 25));
                    if (Enemys != null)
                    {
                        if (Enemys.Count() >= RSet["UseRGs"].GetValue<MenuSlider>().Value && target.IsFacing(_Player) && RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target);
                        }
                        if (Enemys.Count() >= RSet["UseRGs"].GetValue<MenuSlider>().Value && !RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target);
                        }
                    }

                }

                if (RSet["UseR"].GetValue<MenuBool>().Enabled && _R.IsReady())
                {
                    if (!_R.IsReady()) return;
                    {
                        if (target.IsFacing(_Player) && target.IsValidTarget(_R.Range) && RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            _R.Cast(target.Position);
                        }
                    }
                    if (target.IsValidTarget(_R.Range) && !RSet["UseRFace"].GetValue<MenuBool>().Enabled)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                        _R.Cast(target.Position);
                    }

                }
            }
        }
        private static void Interruptererer(AIBaseClient sender, Interrupter.InterruptSpellArgs args)
        {
            var RintTarget = TargetSelector.GetTarget(_R.Range, DamageType.Magical);
            if (RintTarget == null) return;
            if (_R.IsReady() && sender.IsValidTarget(_R.Range) && ComboMenu["Rint"].GetValue<MenuBool>().Enabled)
                _R.Cast(RintTarget);

        }
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs e)
        {
            if (!ComboMenu["RGapClose"].GetValue<MenuBool>().Enabled) return;
            if (sender.IsEnemy)
                _R.Cast(sender);
        }
        public static void Ignite()
        {
            var target = TargetSelector.GetTarget(_Ignite.Range, DamageType.True);
            if (target == null)
            {
                return;
            }
            if (otheroptions["Ignite"].GetValue<MenuBool>().Enabled && !_Ignite.IsReady() && target.IsValidTarget()) return;

            {
                if (target.Health + target.PhysicalShield <
                    _Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                {
                    _Ignite.Cast(target);
                }
            }
        }
        private static void Dash_OnDash(AIBaseClient sender, Dash.DashArgs e)
        {
            if (!QSet["QComboDash"].GetValue<MenuBool>().Enabled) return;
            if (!sender.IsEnemy) return;
            if (!_Q.IsReady()) return;
            if (e.EndPos.IsValid())
                _Q.Cast(e.EndPos);
        }
        /* private static void Skin_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
         {
             Player.SetSkinId(sender.CurrentValue);
         }*/
        public static void KillSteal()
        {
            var targetQ = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            var targetE = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
            if (targetQ == null)
            {
                return;
            }
            if (targetE == null)
            {
                return;
            }
            if (QSet["UseQPok"].GetValue<MenuBool>().Enabled)
            {
                var Qpred = _Q.GetPrediction(targetQ);
                if (Qpred.Hitchance >= HitChance.High && targetQ.IsValidTarget(_Q.Range))
                {
                    if (targetQ.Health + targetQ.PhysicalShield < _Player.GetSpellDamage(targetQ, SpellSlot.Q))
                    {
                        if (!targetQ.IsValidTarget(_Q.Range) && !_Q.IsReady()) return;
                        {
                            _Q.Cast(targetQ);
                        }
                    }
                }
            }

            if (ESet["UseEK"].GetValue<MenuBool>().Enabled)
            {
                if (targetE.Health + targetE.PhysicalShield < _Player.GetSpellDamage(targetE, SpellSlot.E))
                {
                    if (!targetE.IsValidTarget(_E.Range) && !_E.IsReady()) return;
                    {
                        {
                            _E.Cast(targetE);
                        }
                    }
                }

            }
        }
        private static void ImmobileQ()
        {
            var target = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (QSet["UseQ"].GetValue<MenuBool>().Enabled && QSet["UseQI"].GetValue<MenuBool>().Enabled)

            {
                if (_Q.IsReady())
                {

                    var Qpred = _Q.GetPrediction(target);
                    if (Qpred.Hitchance >= HitChance.Immobile && target.IsValidTarget(_Q.Range))
                    {
                        _Q.Cast(target);
                        if (DebugC["Debug"].GetValue<MenuBool>().Enabled)
                        {

                            Game.Print("Casting Q for immobile enemy");
                            Console.WriteLine("Casting Q for immobile enemy ");
                        }
                    }
                }
            }

        }

    }

}