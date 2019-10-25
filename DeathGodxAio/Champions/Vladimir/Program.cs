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

namespace Vladimir
{
    class Program
    {
        public static Menu Menu, ComboMenu, Evade, HarassMenu, LaneClearMenu, Misc, KillStealMenu, Drawings;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Font Thn;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite;


        public static void VladimirOnLoadingComplete()
        {
            if (!_Player.CharacterName.Contains("Vladimir")) return;
            Game.Print("Doctor's Vladimir Loaded! PORTED by DeathGODX", Color.Orange);
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 700);
            R.SetSkillshot(250, 1200, 150, false, false, SkillshotType.Circle);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Thn = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 15, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            var MenuVlad = new Menu("Doctor's Vladimir", "Vladimir", true);
            ComboMenu = new Menu("Combo Settings", "Combo");
            ComboMenu.Add(new MenuSeparator("Combo Settings", "Combo Settings"));
            ComboMenu.Add(new MenuBool("ComboQ", "Use [Q] Combo"));
            ComboMenu.Add(new MenuBool("ComboW", "Use [W] Combo"));
            ComboMenu.Add(new MenuBool("ComboE", "Use [E] Combo"));
            ComboMenu.Add(new MenuBool("ComboR", "Use [R] Combo"));
            ComboMenu.Add(new MenuSeparator("Use [R] Aoe", "Use [R] Aoe"));
            ComboMenu.Add(new MenuBool("ComboR2", "Use [R] Aoe"));
            ComboMenu.Add(new MenuSlider("MinR", "Use [R] Aoe Enemies >=", 2, 1, 5));
            ComboMenu.Add(new MenuSeparator("Auto [Q-W] Low HP", "Auto [Q-W] Low HP"));
            ComboMenu.Add(new MenuBool("Wtoggle", "Auto [W] Low MyHp"));
            ComboMenu.Add(new MenuSlider("minHealth", "Use [W] My Hp <", 20));
            ComboMenu.Add(new MenuBool("AutoQ", "Auto [Q] Low MyHp On Enemies", false));
            ComboMenu.Add(new MenuBool("AutoQm", "Auto [Q] Low MyHp On Minions", false));
            ComboMenu.Add(new MenuSlider("healthQ", "Auto [Q] MyHp <", 30));
            ComboMenu.Add(new MenuSeparator("Use [W] Dodge Spell", "Use [W] Dodge Spell"));
            ComboMenu.Add(new MenuBool("dodge", "Use [W] Dodge"));
            ComboMenu.Add(new MenuBool("antiGap", "Use [W] Anti Gap"));
            ComboMenu.Add(new MenuSlider("healthgap", "Use [W] AntiGap My Hp <", 50));
            MenuVlad.Add(ComboMenu);
            Evade = new Menu("Spell Dodge Settings", "Evade");
            Evade.Add(new MenuSeparator("Dodge Settings", "Dodge Settings"));
            foreach (var enemies in GameObjects.EnemyHeroes.Where(a => a.Team != ObjectManager.Player.Team))
            {
                Evade.Add(new MenuSeparator(enemies.CharacterName, enemies.CharacterName));
                {
                    foreach (var spell in enemies.Spellbook.Spells.Where(a => a.Slot == SpellSlot.Q || a.Slot == SpellSlot.W || a.Slot == SpellSlot.E || a.Slot == SpellSlot.R))
                    {
                        if (spell.Slot == SpellSlot.Q)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                        else if (spell.Slot == SpellSlot.W)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                        else if (spell.Slot == SpellSlot.E)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                        else if (spell.Slot == SpellSlot.R)
                        {
                            Evade.Add(new MenuBool(spell.SData.Name, enemies.CharacterName + " : " + spell.Slot.ToString() + " : " + spell.Name, false));
                        }
                    }
                }
            }
            MenuVlad.Add(Evade);
            HarassMenu = new Menu("Harass Settings", "Harass");
            HarassMenu.Add(new MenuSeparator("Harass Settings", "Harass Settings"));
            HarassMenu.Add(new MenuBool("HarassQ", "Use [Q] Harass"));
            HarassMenu.Add(new MenuBool("HarassE", "Use [E] Harass"));
            HarassMenu.Add(new MenuKeyBind("Autoqh", "Auto [Q] Harass", System.Windows.Forms.Keys.T, KeyBindType.Toggle)).Permashow();
            MenuVlad.Add(HarassMenu);
            LaneClearMenu = new Menu("Clear Settings", "LaneClear");
            LaneClearMenu.Add(new MenuSeparator("Clear Settings", "Clear Settings"));
            LaneClearMenu.Add(new MenuBool("QLC", "Use [Q] LaneClear"));
            LaneClearMenu.Add(new MenuBool("ELC", "Use [E] LaneClear"));
            LaneClearMenu.Add(new MenuSlider("minE", "Min Hit Minions Use [E]", 3, 1, 6));
            LaneClearMenu.Add(new MenuSeparator("LastHit Settings", "LastHit Settings"));
            LaneClearMenu.Add(new MenuBool("QLH", "Use [Q] LastHit"));
            LaneClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            LaneClearMenu.Add(new MenuBool("QJungle", "Use [Q] JungleClear"));
            LaneClearMenu.Add(new MenuBool("EJungle", "Use [E] JungleClear"));
            MenuVlad.Add(LaneClearMenu);
            Misc = new Menu("Skin Settings", "Misc");
            Misc.Add(new MenuSeparator("Skin Changer", "Skin Changer"));
            Misc.Add(new MenuBool("checkSkin", "Use Skin Changer"));
            Misc.Add(new MenuList("skin.Id", "Skin Mode", new[] { "Default", "1", "2", "3", "4", "5", "6", "7", "8" }) { Index = 0 });
            MenuVlad.Add(Misc);
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal");
            KillStealMenu.Add(new MenuSeparator("KillSteal Settings", "KillSteal Settings"));
            KillStealMenu.Add(new MenuBool("KsQ", "Use [Q] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsE", "Use [E] KillSteal"));
            KillStealMenu.Add(new MenuBool("KsR", "Use [R] KillSteal"));
            KillStealMenu.Add(new MenuBool("ign", "Use [Ignite] KillSteal"));
            MenuVlad.Add(KillStealMenu);
            Drawings = new Menu("Drawings Settings", "Draw");
            Drawings.Add(new MenuSeparator("Drawing Settings", "Drawing Settings"));
            Drawings.Add(new MenuBool("DrawQ", "[Q] Range"));
            Drawings.Add(new MenuBool("DrawE", "[E] Range"));
            Drawings.Add(new MenuBool("DrawR", "[R] Range"));
            Drawings.Add(new MenuBool("DrawAT", "Draw Auto Harass"));
            MenuVlad.Add(Drawings);
            MenuVlad.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;

            if (Drawings["DrawQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Orange, 1);
            }

            if (Drawings["DrawE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange, 1);
            }

            if (Drawings["DrawR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Orange, 1);
            }

            if (Drawings["DrawAT"].GetValue<MenuBool>().Enabled)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (HarassMenu["Autoqh"].GetValue<MenuKeyBind>().Active)
                {
                    DrawFont(Thn, "Auto [Q] Harass : Enable", (float)(ft[0] - 60), (float)(ft[1] + 20), SharpDX.Color.Red);
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

            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }
            KillSteal();
            AutoQ();
            WLogic();
            /*
            if (_Player.SkinId != Misc["skin.Id"].GetValue<MenuList>().CurrentValue)
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

        public static bool EActive
        {
            get { return ObjectManager.Player.HasBuff("VladimirE"); }
        }

        public static bool Frenzy
        {
            get { return ObjectManager.Player.HasBuff("vladimirqfrenzy"); }
        }

        public static double QDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 80, 100, 120, 140, 160 }[Program.Q.Level] + 0.45f * _Player.FlatMagicDamageMod));
        }

        public static double EDamage(AIBaseClient target)
        {
            return ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
        }

        public static double RDamage(AIBaseClient target)
        {
            return _Player.CalculateDamage(target, DamageType.Magical,
                (float)(new[] { 0, 168, 280, 392 }[Program.R.Level] + 0.75f * _Player.FlatMagicDamageMod));
        }

        public static void AutoQ()
        {
            var AutoQH = HarassMenu["Autoqh"].GetValue<MenuKeyBind>().Active;
            var AutoQ = ComboMenu["AutoQ"].GetValue<MenuBool>().Enabled;
            var AutoQm = ComboMenu["AutoQm"].GetValue<MenuBool>().Enabled;
            var health = ComboMenu["healthQ"].GetValue<MenuSlider>().Value;
            var Enemies = GameObjects.EnemyHeroes.FirstOrDefault(e => e.IsValidTarget(Q.Range));
            var minions = GameObjects.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo
            && Orbwalker.ActiveMode != OrbwalkerMode.Harass
            && Orbwalker.ActiveMode != OrbwalkerMode.LaneClear
            && Orbwalker.ActiveMode != OrbwalkerMode.LastHit)
            {
                if (Q.IsReady() && _Player.HealthPercent <= health)
                {
                    if (AutoQ && Enemies != null && Enemies.IsValidTarget(Q.Range))
                    {
                        Q.Cast(Enemies);
                    }

                    if (AutoQm && minions != null && minions.IsValidTarget(Q.Range))
                    {
                        Q.Cast(minions);
                    }
                }

                if (Enemies != null)
                {
                    if (AutoQH && Q.IsReady() && Enemies.IsValidTarget(Q.Range))
                    {
                        Q.Cast(Enemies);
                    }
                }
            }
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            var AntiGap = ComboMenu["antiGap"].GetValue<MenuBool>().Enabled;
            var HealthGap = ComboMenu["healthgap"].GetValue<MenuSlider>().Value;
            if (AntiGap && W.IsReady() && sender.Distance(_Player) < 325)
            {
                if (_Player.HealthPercent <= HealthGap)
                {
                    W.Cast();
                }
            }
        }

        public static void Combo()
        {
            var useQ = ComboMenu["ComboQ"].GetValue<MenuBool>().Enabled;
            var useW = ComboMenu["ComboW"].GetValue<MenuBool>().Enabled;
            var useE = ComboMenu["ComboE"].GetValue<MenuBool>().Enabled;
            var useR = ComboMenu["ComboR"].GetValue<MenuBool>().Enabled;
            var useR2 = ComboMenu["ComboR2"].GetValue<MenuBool>().Enabled;
            var MinR = ComboMenu["MinR"].GetValue<MenuSlider>().Value;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.IsDead))
            {
                if (useR2 && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.CastPosition.CountEnemyHeroesInRange(350) >= MinR && pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(pred.CastPosition);
                    }
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !EActive)
                {
                    Q.Cast(target);
                }

                if (useW && W.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (EActive && _Player.Distance(target) <= 375)
                    {
                        W.Cast();
                    }
                    else if (target.IsValidTarget(325))
                    {
                        W.Cast();
                    }

                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && !EActive)
                {
                    E.Cast();
                }

                if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (QDamage(target) * 2 + RDamage(target) + EDamage(target) >= target.Health)
                    {
                        var pred1 = R.GetPrediction(target);
                        if (pred1.Hitchance >= HitChance.Medium)
                        {
                            R.Cast(pred1.CastPosition);
                        }
                    }
                }
            }
        }


        public static void JungleClear()
        {
            var useQ = LaneClearMenu["QJungle"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["EJungle"].GetValue<MenuBool>().Enabled;
            var monster = GameObjects.Jungle.Where(j => j.IsValidTarget(600)).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }

                if (useE && E.IsReady() && monster.IsValidTarget(E.Range) && !EActive)
                {
                    E.Cast();
                }
            }
        }

        public static void LastHit()
        {
            var useQ = LaneClearMenu["QLH"].GetValue<MenuBool>().Enabled;
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range));
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady())
                {
                    if (QDamage(minion) >= minion.Health + minion.AllShield)
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }

        public static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].GetValue<MenuBool>().Enabled;
            var useE = LaneClearMenu["ELC"].GetValue<MenuBool>().Enabled;
            var MinE = LaneClearMenu["minE"].GetValue<MenuSlider>().Value;
            var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range) && e.IsMinion())
                            .Cast<AIBaseClient>().ToList();
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady())
                {
                    if (QDamage(minion) >= minion.Health + minion.AllShield)
                    {
                        Q.Cast(minion);
                    }
                }

                if (useE && E.IsReady() && minion.IsValidTarget(E.Range) && !EActive)
                {
                    E.Cast();
                }
            }
        }

        public static void WLogic()
        {
            var useW = ComboMenu["Wtoggle"].GetValue<MenuBool>().Enabled;
            var MinHealth = ComboMenu["minHealth"].GetValue<MenuSlider>().Value;
            if (useW && W.IsReady() && !_Player.IsRecalling() && !_Player.InShop())
            {
                if (_Player.CountEnemyHeroesInRange(600) >= 1 && _Player.HealthPercent < MinHealth)
                {
                    W.Cast();
                }
            }
        }

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].GetValue<MenuBool>().Enabled;
            var useE = HarassMenu["HarassE"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.IsDead))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && !EActive)
                {
                    E.Cast();
                }
            }
        }

        public static void Flee()
        {
            var Enemies = GameObjects.EnemyHeroes.FirstOrDefault(e => e.IsValidTarget(Q.Range));
            if (Enemies != null && Q.IsReady())
            {
                if (Q.IsReady() && Q.IsInRange(Enemies))
                {
                    Q.Cast(Enemies);
                }
            }

            if (W.IsReady())
            {
                W.Cast();
            }
        }


        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if ((args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E ||
                 args.Slot == SpellSlot.R) && sender.IsEnemy && W.IsReady() && _Player.Distance(sender) <= args.SData.CastRange && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                if (args.SData.TargetingType == SpellDataTargetType.Unit || args.SData.TargetingType == SpellDataTargetType.SelfAndUnit || args.SData.TargetingType == SpellDataTargetType.Self)
                {
                    if ((args.Target.NetworkId == ObjectManager.Player.NetworkId && args.Time < 1.5 ||
                         args.End.Distance(ObjectManager.Player.Position) <= ObjectManager.Player.BoundingRadius * 3) &&
                        Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        W.Cast();
                    }
                }
                else if (args.SData.TargetingType == SpellDataTargetType.LocationAoe)
                {
                    var castvector =
                        new Geometry.Circle(args.End, args.SData.CastRadius).IsInside(
                            ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        W.Cast();
                    }
                }

                else if (args.SData.TargetingType == SpellDataTargetType.Cone)
                {
                    var castvector =
                        new Geometry.Arc(args.Start, args.End, args.SData.CastConeAngle, args.SData.CastRange)
                            .IsInside(ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        W.Cast();
                    }
                }

                else if (args.SData.TargetingType == SpellDataTargetType.SelfAoe)
                {
                    var castvector =
                        new Geometry.Circle(sender.Position, args.SData.CastRadius).IsInside(
                            ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var castvector =
                        new Geometry.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(
                            ObjectManager.Player.Position);

                    if (castvector && Evade[args.SData.Name].GetValue<MenuBool>().Enabled)
                    {
                        W.Cast();
                    }
                }

                if (args.SData.Name == "yasuoq3w")
                {
                    W.Cast();
                }

                if (args.SData.Name == "ZedR")
                {
                    W.Cast();
                }

                if (args.SData.Name == "KarthusFallenOne")
                {
                    W.Cast();
                }

                if (args.SData.Name == "SoulShackles")
                {
                    W.Cast();
                }

                if (args.SData.Name == "AbsoluteZero")
                {
                    W.Cast();
                }

                if (args.SData.Name == "NocturneUnspeakableHorror")
                {
                    W.Cast();
                }
            }
        }

        public static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].GetValue<MenuBool>().Enabled;
            var KsR = KillStealMenu["KsR"].GetValue<MenuBool>().Enabled;
            var KsE = KillStealMenu["KsE"].GetValue<MenuBool>().Enabled;
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R.Range) && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsDead))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.Health + target.Health + target.AllShield <= QDamage(target))
                    {
                        Q.Cast(target);
                    }
                }

                if (KsR && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (target.Health + target.Health + target.AllShield <= RDamage(target))
                    {
                        var pred1 = R.GetPrediction(target);
                        if (pred1.Hitchance >= HitChance.High)
                        {
                            R.Cast(pred1.CastPosition);
                        }
                    }
                }

                if (KsE && E.IsReady() && E.IsInRange(target))
                {
                    if (target.Health + target.Health + target.AllShield <= EDamage(target) && !EActive)
                    {
                        E.Cast();
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

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }
    }
}
