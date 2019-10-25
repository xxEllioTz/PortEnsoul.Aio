namespace BubbaKushs.Plu
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using BubbaKushsLeeSin;
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;
    using SharpDX;
    using xxEliot.Core;
    using Color = System.Drawing.Color;
    using Menu = EnsoulSharp.SDK.MenuUI.Menu;

    #endregion
    internal class LeeSin : Pro
    {
        #region Static Fields

        private static readonly List<string> SpecialPet = new List<string>
                                                              { "jarvanivstandard", "teemomushroom", "illaoiminion" };

        private static int cPassive;

        private static bool isDashingQ;

        private static int lastCast, lastBubbaKush;

        private static int lastW, lastR;

        private static AIBaseClient objQ;

        private static Vector3 posBubbaKushFlash, posBubbaKushJump;

        #endregion

        #region Constructors and Destructors

        public LeeSin()
        {
            Q = new Spell(SpellSlot.Q, 1200);
            Q.SetSkillshot(0.25f, 70f, 1800, true, SkillshotType.Line);
            Q2 = new Spell(Q.Slot, 1300);
            W = new Spell(SpellSlot.W, 700);
            W.SetTargetted(0, 2000);
            E = new Spell(SpellSlot.E, 425);
            E.SetTargetted(0.25f, float.MaxValue);
            E2 = new Spell(E.Slot, 575);
            R = new Spell(SpellSlot.R, 375);
            R.SetTargetted(0.25f, float.MaxValue);
            Q.DamageType = Q2.DamageType = W.DamageType = R.DamageType = DamageType.Physical;
            E.DamageType = DamageType.Magical;
            Q.MinHitChance = HitChance.VeryHigh;

            Ward.Init();
            Insec.Init();
            var kuMenu = MainMenu.Add(new Menu("KnockUp", "Auto Knock Up"));
            {
                kuMenu.KeyBind("R", "Keybind", Keys.L, KeyBindType.Toggle);
                kuMenu.Bool("RKill", "If Kill Enemy Behind");
                kuMenu.Slider("RCountA", "Or Hit Enemy Behind >=", 1, 1, 4);
            }
            var bkMenu = MainMenu.Add(new Menu("BubbaKush", "Bubba Kush"));
            {
                bkMenu.KeyBind("R", "Keybind", Keys.XButton2);
                bkMenu.List("RMode", "Mode", new[] { "Flash", "WardJump", "Both" });
                bkMenu.Bool("RKill", "Priority To Kill Enemy");
                bkMenu.Slider("RCountA", "Or Hit Enemy >=", 1, 1, 4);
            }
            var comboMenu = MainMenu.Add(new Menu("Combo", "Combo"));
            {
                comboMenu.Bool("Ignite", "Use Ignite");
                comboMenu.Bool("Item", "Use Item");
                comboMenu.Separator("Q Settings");
                comboMenu.Bool("Q", "Use Q");
                comboMenu.Bool("Q2", "Also Q2");
                comboMenu.Bool("Q2Obj", "Q2 Even Miss", false);
                comboMenu.Bool("QCol", "Smite Collision");
                comboMenu.Separator("W Settings");
                comboMenu.Bool("W", "Use W", false);
                comboMenu.Bool("W2", "Also W2", false);
                comboMenu.Separator("E Settings");
                comboMenu.Bool("E", "Use E");
                comboMenu.Bool("E2", "Also E2");
                comboMenu.Separator("Star Combo Settings");
                comboMenu.KeyBind("Star", "Star Combo", Keys.X);
                comboMenu.Bool("StarKill", "Auto Star Combo If Killable");
                comboMenu.Bool("StarKillWJ", "-> Ward Jump In Auto Star Combo", false);
            }
            var lcMenu = MainMenu.Add(new Menu("LaneClear", "Lane Clear"));
            {
                lcMenu.Bool("W", "Use W", false);
                lcMenu.Bool("E", "Use E");
                lcMenu.Separator("Q Settings");
                lcMenu.Bool("Q", "Use Q");
                lcMenu.Bool("QBig", "Only Q Big Mob In Jungle", false);
            }
            var lhMenu = MainMenu.Add(new Menu("LastHit", "Last Hit"));
            {
                lhMenu.Bool("Q", "Use Q1");
            }
            var ksMenu = MainMenu.Add(new Menu("KillSteal", "Kill Steal"));
            {
                ksMenu.Bool("E", "Use E");
                ksMenu.Bool("R", "Use R");
                ksMenu.Separator("Q Settings");
                ksMenu.Bool("Q", "Use Q");
                ksMenu.Bool("Q2", "Also Q2");
                if (GameObjects.EnemyHeroes.Any())
                {
                    ksMenu.Separator("Extra R Settings");
                    GameObjects.EnemyHeroes.ForEach(
                        i => ksMenu.Bool("RCast" + i.CharacterName, "Cast On " + i.CharacterName, false));
                }
            }
            var drawMenu = MainMenu.Add(new Menu("Draw", "Draw"));
            {
                drawMenu.Bool("Q", "Q Range", false);
                drawMenu.Bool("W", "W Range", false);
                drawMenu.Bool("E", "E Range", false);
                drawMenu.Bool("R", "R Range", false);
                drawMenu.Bool("KnockUp", "Auto Knock Up Status");
            }
            MainMenu.KeyBind("FleeW", "Use W To Flee", Keys.C);
            MainMenu.KeyBind("RFlash", "R-Flash To Mouse", Keys.Z);


            Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += OnDraw;

            AIBaseClient.OnBuffGain += (sender, args) =>
            {
                if (sender.IsMe)
                {
                    if (args.Buff.Name.Contains("blindmonkpassive_cosmetic") || args.Buff.Name == "BlindMonkSonicWave")
                    {
                        cPassive = 2;
                    }
                    if (args.Buff.Name.Contains("BlindMonkQTwoDash"))
                    {
                        isDashingQ = true;
                    }


                }
                else if (sender.IsEnemy)
                {
                    if (args.Buff.Name.Contains("BlindMonkQ"))
                    {
                        objQ = sender;
                    }
                    else if (args.Buff.Name == "blindmonkrroot" && Common.CanFlash)
                    {
                        CastRFlash((AIHeroClient)sender);
                    }
                }
            };
            AIBaseClient.OnBuffLose += (sender, args) =>
            {
                if (sender.IsMe)
                {
                    if (args.Buff.Name.Contains("blindmonkpassive_cosmetic") || args.Buff.Name == "BlindMonkSonicWave")
                    {

                        cPassive = 0;
                    }
                    if (args.Buff.Name.Contains("BlindMonkQTwoDash"))
                    {
                        isDashingQ = false;
                    }
                }
                else if (sender.IsEnemy && args.Buff.Name.Contains("BlindMonkQ"))
                {
                    objQ = null;
                }
            };
            AIBaseClient.OnBuffGain += (sender, args) =>
            {
                if (!sender.IsMe || args.Buff.Name != "blindmonkpassive_cosmetic")
                {
                    return;
                }
                cPassive = args.Buff.Count;
            };
            AIBaseClient.OnProcessSpellCast += (sender, args) =>
            {
                if (!sender.IsMe)
                {
                    return;
                }
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                        if (!args.SData.Name.ToLower().Contains("one"))
                        {
                            isDashingQ = true;
                        }
                        break;
                    case SpellSlot.W:
                        if (args.SData.Name.ToLower().Contains("one"))
                        {
                            lastW = Variables.TickCount;
                        }
                        break;
                    case SpellSlot.R:
                        lastR = Variables.TickCount;
                        break;
                }
                if (args.SData.Name == "SummonerFlash" && posBubbaKushFlash.IsValid())
                {
                    Game.Print("posBubbaKushFlash");
                    posBubbaKushFlash = Vector3.Zero;
                }
            };
        }

        #endregion

        #region Properties

        private static bool IsDashingW => Variables.TickCount - lastW <= 100 || (player.IsDashing() && !isDashingQ);

        private static bool IsEOne => E.Instance.SData.Name.ToLower().Contains("one");

        private static bool IsQOne => Q.Instance.SData.Name.ToLower().Contains("one");

        private static bool IsWOne => W.Instance.SData.Name.ToLower().Contains("one");

        #endregion

        #region Methods
        private static void AutoKnockUp()
        {
            if (!R.IsReady() || IsDashingW)
            {
                return;
            }
            var multi = GetMultiHit(MainMenu["KnockUp"]["RKill"], MainMenu["KnockUp"]["RCountA"], 0);
            if (multi.Item1 != null)
            {
                R.CastOnUnit(multi.Item1);
            }
        }

        private static void BubbaKush()
        {
            if (!R.IsReady() || IsDashingW)
            {
                return;
            }
            if (Variables.TickCount - lastBubbaKush < 1000)
            {
                if (posBubbaKushJump.IsValid() && posBubbaKushJump.DistanceToPlayer() < 80 && !IsWOne
                    && Variables.TickCount - lastW > 100 && Variables.TickCount - lastW < 1000)
                {
                    var targetSelect = TargetSelector.SelectedTarget;
                    if (targetSelect.IsValidTarget())
                    {
                        R.CastOnUnit(targetSelect);
                    }
                }
                return;
            }
            var isKill = MainMenu["BubbaKush"]["RKill"].GetValue<MenuBool>().Enabled;
            var minHit = MainMenu["BubbaKush"]["RCountA"].GetValue<MenuSlider>().Value;
            foreach (var multi in
                new Dictionary<string, Tuple<AIHeroClient, int, Vector2>>
                    {
                        { "N", GetMultiHit(isKill, minHit, 0) },
                        {
                            "F",
                            MainMenu["BubbaKush"]["RMode"].GetValue<MenuList>().Index != 1 && Common.CanFlash
                                ? GetMultiHit(isKill, minHit, 1)
                                : new Tuple<AIHeroClient, int, Vector2>(null, 0, Vector2.Zero)
                        },
                        {
                            "W",
                            MainMenu["BubbaKush"]["RMode"].GetValue<MenuList>().Index > 0 && Ward.CanJump
                                ? GetMultiHit(isKill, minHit, 2)
                                : new Tuple<AIHeroClient, int, Vector2>(null, 0, Vector2.Zero)
                        }
                    }.Where(
                        i => i.Value.Item1 != null).OrderByDescending(i => i.Value.Item2))
            {
                if (multi.Key == "N")
                {
                    R.CastOnUnit(multi.Value.Item1);
                    return;
                }
                posBubbaKushFlash = posBubbaKushJump = Vector3.Zero;
                Common.SetTarget(null);
                if (multi.Key == "W")
                {
                    posBubbaKushJump = multi.Value.Item3.ToVector3();
                    lastBubbaKush = Variables.TickCount;
                    Common.SetTarget(multi.Value.Item1);
                    Ward.Place(posBubbaKushJump);
                    return;
                }
                if (multi.Key == "F" && R.CastOnUnit(multi.Value.Item1))
                {
                    posBubbaKushFlash = multi.Value.Item3.ToVector3();
                    lastBubbaKush = Variables.TickCount;
                    Common.SetTarget(multi.Value.Item1);
                }
            }
        }

        private static bool CanE2(AIBaseClient target)
        {
            var buff = target.GetBuff("blindmonkeone");
            return buff != null && buff.EndTime - Game.Time <= 0.2 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanQ2(AIBaseClient target)
        {
            var buff = target.GetBuff("blindmonkqone");
            return buff != null && buff.EndTime - Game.Time <= 0.3 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanR(AIHeroClient target)
        {
            var buff = target.GetBuff("BlindMonkRKick");
            return buff != null && buff.EndTime - Game.Time <= 0.7 * (buff.EndTime - buff.StartTime);
        }

        private static void CastE(bool isCombo = true)
        {
            if (!E.IsReady() || isDashingQ || IsDashingW || Variables.TickCount - lastCast <= 500)
            {
                return;
            }
            if (isCombo)
            {
                CastECombo();
            }
            else
            {
                CastELaneClear();
            }
        }

        private static void CastECombo()
        {
            if (IsEOne)
            {
                var target =
                    TargetSelector.GetTargets(E.Range + 20, E.DamageType)
                        .Where(i => E.CanHitCircle(i))
                        .ToList();
                if (target.Count == 0)
                {
                    return;
                }
                if (((cPassive == 0 && player.Mana >= 80) || target.Count > 2
                     || (Orbwalker.GetTarget() == null
                             ? target.Any(i => i.DistanceToPlayer() > i.GetRealAutoAttackRange() + 50)
                             : cPassive < 2)) && E.Cast())
                {
                    lastCast = Variables.TickCount;
                }
            }
            else if (MainMenu["Combo"]["E2"])
            {
                var target = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(E2.Range) && HaveE(i)).ToList();
                if (target.Count == 0)
                {
                    if (cPassive == 0 && Orbwalker.GetTarget() != null
                        && Common.ListEnemies(true).Any(i => i.IsValidTarget(E2.Range) && HaveE(i)) && E2.Cast())
                    {
                        lastCast = Variables.TickCount + 300;
                    }
                    return;
                }
                if ((cPassive == 0 || target.Count > 2
                     || target.Any(i => CanE2(i) || i.DistanceToPlayer() > i.GetRealAutoAttackRange() + 50))
                    && E2.Cast())
                {
                    lastCast = Variables.TickCount;
                }
            }
        }

        private static void CastELaneClear()
        {
            if (IsEOne)
            {
                if (cPassive > 0)
                {
                    return;
                }
                var minion = Common.ListMinions().Where(i => i.IsValidTarget(E.Range)).ToList();
                if (minion.Count == 0)
                {
                    return;
                }
                if (
                    (minion.Any(
                        i =>
                        i.Team == GameObjectTeam.Neutral || i.GetMinionType().HasFlag(MinionTypes.Super)
                        || i.GetMinionType().HasFlag(MinionTypes.Siege)) || minion.Count > 1 || player.Mana >= 130)
                    && E.Cast())
                {
                    lastCast = Variables.TickCount;
                }
            }
            else
            {
                var minion = Common.ListMinions().Where(i => i.IsValidTarget(E2.Range) && HaveE(i)).ToList();
                if (minion.Count == 0)
                {
                    if (cPassive == 0 && Orbwalker.GetTarget() != null
                        && Common.ListEnemies(true).Any(i => i.IsValidTarget(E2.Range) && HaveE(i)) && E2.Cast())
                    {
                        lastCast = Variables.TickCount + 300;
                    }
                    return;
                }
                if ((cPassive == 0 || minion.Any(CanE2)) && E2.Cast())
                {
                    lastCast = Variables.TickCount;
                }
            }
        }

        private static void CastRFlash(AIHeroClient target)
        {
            if (!TargetSelector.SelectedTarget.Compare(target)
                || target.Health + target.AllShield <= R.GetDamage(target))
            {
                return;
            }
            var pos = Vector3.Zero;
            if (MainMenu["RFlash"].GetValue<MenuKeyBind>().Active)
            {
                pos = Game.CursorPos;
            }
            else if (MainMenu["BubbaKush"]["R"].GetValue<MenuKeyBind>().Active && posBubbaKushFlash.IsValid())
            {
                var multi = GetMultiHit(MainMenu["BubbaKush"]["RKill"], MainMenu["BubbaKush"]["RCountA"], 1);
                if (multi.Item1.Compare(target))
                {
                    posBubbaKushFlash = multi.Item3.ToVector3();
                }
                pos = posBubbaKushFlash;
            }
            if (pos.IsValid())
            {
                player.Spellbook.CastSpell(
                    Flash,
                    target.Position.Extend(pos, -(player.BoundingRadius / 2 + target.BoundingRadius + 50)));
            }
        }

        private static void CastW(bool isCombo = true)
        {
            if (!W.IsReady() || isDashingQ || IsDashingW || Variables.TickCount - lastCast <= 500)
            {
                return;
            }
            var target = Orbwalker.GetTarget();
            if (target == null)
            {
                return;
            }
            var canWHero = target is AIHeroClient
                           && (player.HealthPercent < 5
                               || (player.HealthPercent < target.HealthPercent && player.HealthPercent < 20));
            if (IsWOne)
            {
                if (Variables.TickCount - lastW <= 500)
                {
                    return;
                }
                var minion = target as AIMinionClient;
                if ((canWHero
                     || (minion != null && minion.Team == GameObjectTeam.Neutral
                         && minion.GetJungleType() != JungleType.Small && player.HealthPercent < 30) || cPassive == 0)
                    && W.Cast())
                {
                    lastCast = Variables.TickCount + 500;
                }
            }
            else if ((!isCombo || MainMenu["Combo"]["W2"])
                     && (canWHero || Variables.TickCount - lastW >= 2800 || cPassive == 0) && W.Cast())
            {
                lastCast = Variables.TickCount;
            }
        }
        private static void Combo()
        {
            if (R.IsReady() && !IsRecentR(500) && MainMenu["Combo"]["StarKill"] && Q.IsReady() && !IsQOne
                && objQ.IsValidTarget(1000) && MainMenu["Combo"]["Q"] && MainMenu["Combo"]["Q2"])
            {
                var target = objQ as AIHeroClient;
                if (target != null
                    && target.Health + target.AllShield
                    > Q.GetDamage(target, DamageStage.SecondCast) + player.GetAutoAttackDamage(target)
                    && target.Health + target.AllShield
                    <= GetQ2Dmg(target, R.GetDamage(target)) + player.GetAutoAttackDamage(target))
                {
                    if (R.CastOnUnit(target))
                    {
                        return;
                    }
                    if (MainMenu["Combo"]["StarKillWJ"] && W.IsReady() && IsWOne && !isDashingQ
                        && target.DistanceToPlayer() > R.Range + target.BoundingRadius
                        && target.DistanceToPlayer() < Ward.Range + R.Range - 50 && player.Mana >= 80)
                    {
                        Flee(target.Position, true);
                    }
                }
            }
            if (Common.CantAttack)
            {
                if (MainMenu["Combo"]["Q"] && Q.IsReady())
                {
                    if (IsQOne)
                    {
                        var target = Q.GetTarget(Q.Width / 2);
                        if (!R.IsReady() && IsRecentR(5000))
                        {
                            var targetR =
                                TargetSelector.GetTargets(Q.Range, Q.DamageType)
                                    .FirstOrDefault(i => i.HasBuff("BlindMonkRKick"));
                            if (targetR != null)
                            {
                                target = targetR;
                            }
                        }
                        if (target != null && Q.CastSpellSmite(target, MainMenu["Combo"]["QCol"]))
                        {
                            lastCast = Variables.TickCount;
                            return;
                        }
                    }
                    else if (MainMenu["Combo"]["Q2"] && !IsDashingW && objQ.IsValidTarget(Q2.Range)
                             && Variables.TickCount - lastCast > 500)
                    {
                        var target = objQ as AIHeroClient;
                        if (target != null)
                        {
                            if ((CanQ2(target) || (!R.IsReady() && IsRecentR() && CanR(target))
                                 || target.Health + target.AllShield
                                 <= Q.GetDamage(target, DamageStage.SecondCast) + player.GetAutoAttackDamage(target)
                                 || ((R.IsReady() || (!target.HasBuff("BlindMonkRKick") && !IsRecentR(1000)))
                                     && target.DistanceToPlayer() > target.GetRealAutoAttackRange() + 100)
                                 || cPassive == 0) && Q2.Cast())
                            {
                                lastCast = Variables.TickCount;
                                return;
                            }
                        }
                        else if (MainMenu["Combo"]["Q2Obj"])
                        {
                            var targetQ2 = Q2.GetTarget(200);
                            if (targetQ2 != null && objQ.Distance(targetQ2) < targetQ2.DistanceToPlayer()
                                && !targetQ2.InAutoAttackRange() && Q2.Cast())
                            {
                                lastCast = Variables.TickCount + 500;
                                return;
                            }
                        }
                    }
                }
                if (MainMenu["Combo"]["E"])
                {
                    CastE();
                }
                if (MainMenu["Combo"]["W"])
                {
                    CastW();
                }
            }
            var subTarget = W.GetTarget();
            if (MainMenu["Combo"]["Item"])
            {
                UseItem(subTarget);
            }
            if (subTarget != null && MainMenu["Combo"]["Ignite"] && Common.CanIgnite && subTarget.HealthPercent < 30
                && subTarget.DistanceToPlayer() <= IgniteRange)
            {
                player.Spellbook.CastSpell(Ignite, subTarget);
            }
        }
        private static void Flee(Vector3 pos, bool isStarCombo = false)
        {
            if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
            {
                return;
            }
            var posplayer = player.Position;
            var posJump = pos.Distance(posplayer) < W.Range ? pos : posplayer.Extend(pos, W.Range);
            var objJumps = new List<AIBaseClient>();
            objJumps.AddRange(GameObjects.AllyHeroes.Where(i => !i.IsMe));
            objJumps.AddRange(GameObjects.AllyWards.Where(i => i.IsWard()));
            objJumps.AddRange(
                GameObjects.AllyMinions.Where(
                    i => i.IsMinion() || i.IsPet() || SpecialPet.Contains(i.SkinName.ToLower())));
            var objJump =
                objJumps.Where(
                    i => i.IsValidTarget(W.Range, false) && i.Distance(posJump) < (isStarCombo ? R.Range - 50 : 200))
                    .MinOrDefault(i => i.Distance(posJump));
            if (objJump != null)
            {
                W.CastOnUnit(objJump);
            }
            else
            {
                Ward.Place(posJump);
            }
        }

        private static Tuple<AIHeroClient, int, Vector2> GetMultiHit(bool checkKill, int minHit, int mode)
        {
            foreach (var target in
                TargetSelector.GetTargets(R.Range + (mode == 2 ? Ward.Range : 0), R.DamageType)
                    .Where(
                        i =>
                        (mode != 2 || i.DistanceToPlayer() < R.Range + (Ward.Range - 180) - 50)
                        && i.Health + i.AllShield > R.GetDamage(i) && !i.HasBuffOfType(BuffType.SpellShield)
                        && !i.HasBuffOfType(BuffType.SpellImmunity))
                    .OrderByDescending(i => i.MaxHealth))
            {
                R2.From = R2.RangeCheckFrom = target.Position;
                R2.Delay = R.Delay;
                R2.Width = target.BoundingRadius;
                if (mode == 0)
                {
                    var rect = new RectanglePoly(R2.From, R2.From.Extend(R.From, -R2.Range), R2.Width);
                    var hit = (from enemy in GameObjects.EnemyHeroes.Where(i => !i.Compare(target))
                               let pred = R2.GetPrediction(enemy)
                               where pred.Hitchance >= HitChance.Medium && rect.IsInside(pred.UnitPosition)
                               select enemy).ToList();
                    if (checkKill && hit.Any(i => i.Health + i.AllShield <= GetRColDmg(target, i)))
                    {
                        return new Tuple<AIHeroClient, int, Vector2>(target, 5, rect.End);
                    }
                    if (hit.Count >= minHit)
                    {
                        return new Tuple<AIHeroClient, int, Vector2>(target, minHit, rect.End);
                    }
                }
                else
                {
                    var bestHit = 0;
                    var bestPos = Vector2.Zero;
                    for (var angle = 0; angle < 360; angle += 20)
                    {
                        var rotatePos = new Vector2(
                            R2.From.X + 180 * (float)Math.Cos(Math.PI * angle / 180),
                            R2.From.Y + 180 * (float)Math.Sin(Math.PI * angle / 180));
                        if (mode == 2)
                        {
                            if (rotatePos.IsWall() || rotatePos.DistanceToPlayer() >= Ward.Range)
                            {
                                continue;
                            }
                            R2.Delay = R.Delay + rotatePos.DistanceToPlayer() / W.Speed + Game.Ping / 2000f + 0.06f;
                        }
                        var rect = new RectanglePoly(R2.From, R2.From.Extend(rotatePos, -R2.Range), R2.Width);
                        var hit = (from enemy in GameObjects.EnemyHeroes.Where(i => !i.Compare(target))
                                   let pred = R2.GetPrediction(enemy)
                                   where pred.Hitchance >= HitChance.Medium && rect.IsInside(pred.UnitPosition)
                                   select enemy).ToList();
                        if (mode == 2)
                        {
                            rect.End = rotatePos;
                        }
                        if (checkKill && hit.Any(i => i.Health + i.AllShield <= GetRColDmg(target, i)))
                        {
                            return new Tuple<AIHeroClient, int, Vector2>(target, 5, rect.End);
                        }
                        if (hit.Count >= minHit && hit.Count > bestHit)
                        {
                            bestHit = hit.Count;
                            bestPos = rect.End;
                        }
                    }
                    if (bestPos.IsValid())
                    {
                        return new Tuple<AIHeroClient, int, Vector2>(target, minHit, bestPos);
                    }
                }
            }
            return new Tuple<AIHeroClient, int, Vector2>(null, 0, Vector2.Zero);
        }

        private static double GetQ2Dmg(AIBaseClient target, double subHp)
        {
            var dmg = new[] { 50, 80, 110, 140, 170 }[Q.Level - 1] + 0.9 * player.FlatPhysicalDamageMod
                      + 0.08 * (target.MaxHealth - (target.Health - subHp));
            return player.CalculateDamage(
                target,
                DamageType.Physical,
                target is AIMinionClient ? Math.Min(dmg, 400) : dmg) + subHp;
        }

        private static float GetRColDmg(AIHeroClient kickTarget, AIHeroClient hitTarget)
        {
            return R.GetDamage(hitTarget)
                   + (float)
                     player.CalculateDamage(
                         hitTarget,
                         DamageType.Physical,
                         new[] { 0.12, 0.15, 0.18 }[R.Level - 1] * kickTarget.MaxHealth);
        }
        private static bool HaveE(AIBaseClient target)
        {
            return target.HasBuff("BlindMonkEOne");
        }

        private static bool HaveQ(AIBaseClient target)
        {
            return target.HasBuff("BlindMonkQOne");
        }

        private static bool IsRecentR(int time = 2500)
        {
            return Variables.TickCount - lastR < time;
        }

        private static void KillSteal()
        {
            if (MainMenu["KillSteal"]["Q"] && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);
                    if (target != null
                        && (target.Health + target.AllShield <= Q.GetDamage(target)
                            || (MainMenu["KillSteal"]["Q2"]
                                && target.Health + target.AllShield
                                <= GetQ2Dmg(target, Q.GetDamage(target)) + player.GetAutoAttackDamage(target)
                                && player.Mana - Q.Instance.ManaCost >= 30))
                        && Q.Casting(
                            target,
                            false,
                            CollisionObjects.Heroes | CollisionObjects.Minions | CollisionObjects.YasuoWall)
                               .IsCasted())
                    {
                        return;
                    }
                }
                else if (MainMenu["KillSteal"]["Q2"] && !IsDashingW && objQ.IsValidTarget(Q2.Range))
                {
                    var target = objQ as AIHeroClient;
                    if (target != null
                        && target.Health + target.AllShield
                        <= Q.GetDamage(target, DamageStage.SecondCast) + player.GetAutoAttackDamage(target) && Q2.Cast())
                    {
                        return;
                    }
                }
            }
            if (MainMenu["KillSteal"]["E"] && E.IsReady() && IsEOne
                && TargetSelector.GetTargets(E.Range, E.DamageType)
                       .Any(i => E.CanHitCircle(i) && i.Health + i.MagicalShield <= E.GetDamage(i)) && E.Cast())
            {
                return;
            }
            if (MainMenu["KillSteal"]["R"] && R.IsReady())
            {
                var target =
                    TargetSelector.GetTargets(R.Range, R.DamageType, false)
                        .FirstOrDefault(
                            i =>
                            MainMenu["KillSteal"]["RCast" + i.CharacterName]
                            && i.Health + i.AllShield <= R.GetDamage(i));
                if (target != null)
                {
                    R.CastOnUnit(target);
                }
                else if (MainMenu["KillSteal"]["Q"] && MainMenu["KillSteal"]["Q2"] && Q.IsReady() && !IsQOne)
                {
                    target = objQ as AIHeroClient;
                    if (target != null && target.IsValidTarget(R.Range)
                        && MainMenu["KillSteal"]["RCast" + target.CharacterName]
                        && target.Health + target.AllShield
                        <= GetQ2Dmg(target, R.GetDamage(target)) + player.GetAutoAttackDamage(target))
                    {
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (MainMenu["LaneClear"]["E"])
            {
                CastE(false);
            }
            if (MainMenu["LaneClear"]["W"])
            {
                CastW(false);
            }
            if (MainMenu["LaneClear"]["Q"] && Q.IsReady() && Variables.TickCount - lastCast > 500)
            {
                if (IsQOne)
                {
                    if (cPassive == 2)
                    {
                        return;
                    }
                    var minionQ = Common.ListMinions().Where(i => i.IsValidTarget(Q.Range - 10)).ToList();
                    if (minionQ.Count == 0)
                    {
                        return;
                    }
                    var minionJungle =
                        minionQ.Where(i => i.Team == GameObjectTeam.Neutral)
                            .OrderByDescending(i => i.MaxHealth)
                            .ThenBy(i => i.DistanceToPlayer())
                            .ToList();
                    if (MainMenu["LaneClear"]["QBig"] && minionJungle.Count > 0)
                    {
                        minionJungle =
                            minionJungle.Where(
                                i =>
                                i.GetJungleType() == JungleType.Legendary || i.GetJungleType() == JungleType.Large
                                || i.Name.Contains("Crab")).ToList();
                    }
                    if (minionJungle.Count > 0)
                    {
                        if (minionJungle.Any(i => Q.Casting(i).IsCasted()))
                        {
                            lastCast = Variables.TickCount;
                        }
                    }
                    else
                    {
                        var minionLane =
                            minionQ.Where(i => i.Team != GameObjectTeam.Neutral)
                                .OrderByDescending(i => i.GetMinionType().HasFlag(MinionTypes.Siege))
                                .ThenBy(i => i.GetMinionType().HasFlag(MinionTypes.Super))
                                .ThenBy(i => i.Health)
                                .ThenByDescending(i => i.MaxHealth)
                                .ToList();
                        if (minionLane.Count == 0)
                        {
                            return;
                        }
                        foreach (var minion in minionLane)
                        {
                            if (minion.InAutoAttackRange())
                            {
                                if (Q.GetHealthPrediction(minion) > Q.GetDamage(minion) && Q.Casting(minion).IsCasted())
                                {
                                    lastCast = Variables.TickCount;
                                    return;
                                }
                            }
                            else if ((Orbwalker.GetTarget() != null
                                          ? Q.CanLastHit(minion, Q.GetDamage(minion))
                                          : Q.GetHealthPrediction(minion) > Q.GetDamage(minion))
                                     && Q.Casting(minion).IsCasted())
                            {
                                lastCast = Variables.TickCount;
                                return;
                            }
                        }
                    }
                }
                else if (!IsDashingW && objQ.IsValidTarget(Q2.Range)
                         && (CanQ2(objQ) || objQ.Health <= Q.GetDamage(objQ, DamageStage.SecondCast)
                             || objQ.DistanceToPlayer() > objQ.GetRealAutoAttackRange() + 100 || cPassive == 0)
                         && Q2.Cast())
                {
                    lastCast = Variables.TickCount;
                }
            }
        }

        private static void LastHit()
        {
            if (!MainMenu["LastHit"]["Q"] || !Q.IsReady() || !IsQOne || player.Spellbook.IsAutoAttack)
            {
                return;
            }
            var minions =
                GameObjects.EnemyMinions.Where(
                    i => (i.IsMinion() || i.IsPet(false)) && i.IsValidTarget(Q.Range) && Q.CanLastHit(i, Q.GetDamage(i)))
                    .OrderByDescending(i => i.MaxHealth)
                    .ToList();
            if (minions.Count == 0)
            {
                return;
            }
            minions.ForEach(
                i =>
                Q.Casting(
                    i,
                    false,
                    CollisionObjects.Heroes | CollisionObjects.Minions | CollisionObjects.YasuoWall));
        }

        private static void OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type != OrbwalkerType.AfterAttack || Orbwalker.ActiveMode != OrbwalkerMode.Combo
                || !MainMenu["Combo"]["Item"])
            {
                return;
            }
            Common.CastTiamatHydra();
        }

        private static void OnDraw(EventArgs args)
        {
            if (player.IsDead || R.Level == 0)
            {
                return;
            }
            if (MainMenu["Draw"]["KnockUp"])
            {
                var menu = MainMenu["KnockUp"]["R"].GetValue<MenuKeyBind>();
                var text =
                    $"Auto Knock Up: {(menu.Active ? "On" : "Off")} <{MainMenu["KnockUp"]["RCountA"].GetValue<MenuSlider>().Value}> [{menu.Key}]";
                var pos = Drawing.WorldToScreen(player.Position);
                Drawing.DrawText(
                    pos.X - (float)Drawing.GetTextEntent((text)).Width / 2,
                    pos.Y + 20,
                    menu.Active ? Color.White : Color.Gray,
                    text);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (player.IsDead)
            {
                return;
            }
            if (MainMenu["Draw"]["Q"] && Q.Level > 0)
            {
                Render.Circle.DrawCircle(
                    player.Position,
                    (IsQOne ? Q : Q2).Range,
                    Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (MainMenu["Draw"]["W"] && W.Level > 0 && IsWOne)
            {
                Render.Circle.DrawCircle(player.Position, W.Range, W.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (MainMenu["Draw"]["E"] && E.Level > 0)
            {
                Render.Circle.DrawCircle(
                    player.Position,
                    (IsEOne ? E : E2).Range,
                    E.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (MainMenu["Draw"]["R"] && R.Level > 0)
            {
                Render.Circle.DrawCircle(player.Position, R.Range, R.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || MenuGUI.IsShopOpen || player.IsRecalling())
            {
                return;
            }
            KillSteal();
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    break;
                case OrbwalkerMode.LastHit:
                    LastHit();
                    break;
                case OrbwalkerMode.None:
                    if (MainMenu["FleeW"].GetValue<MenuKeyBind>().Active)
                    {
                        Orbwalker.Move(Game.CursorPos);
                        Flee(Game.CursorPos);
                    }
                    else if (MainMenu["RFlash"].GetValue<MenuKeyBind>().Active)
                    {
                        Orbwalker.Move(Game.CursorPos);
                        if (R.IsReady() && Common.CanFlash)
                        {
                            var target =
                                TargetSelector.GetTargets(R.Range, R.DamageType)
                                    .Where(i => i.Health + i.AllShield > R.GetDamage(i))
                                    .MaxOrDefault(i => TargetSelector.GetPriority(i));
                            if (target != null && R.CastOnUnit(target))
                            {
                                Common.SetTarget(target);
                            }
                        }
                    }
                    else if (MainMenu["BubbaKush"]["R"].GetValue<MenuKeyBind>().Active)
                    {
                        Orbwalker.Move(Game.CursorPos);
                        BubbaKush();
                    }
                    else if (MainMenu["Combo"]["Star"].GetValue<MenuKeyBind>().Active)
                    {
                        StarCombo();
                    }
                    else if (MainMenu["Insec"]["R"].GetValue<MenuKeyBind>().Active)
                    {
                        Insec.Start();
                    }
                    break;
            }
            if (MainMenu["KnockUp"]["R"].GetValue<MenuKeyBind>().Active
                && !MainMenu["RFlash"].GetValue<MenuKeyBind>().Active
                && !MainMenu["BubbaKush"]["R"].GetValue<MenuKeyBind>().Active
                && !MainMenu["Insec"]["R"].GetValue<MenuKeyBind>().Active)
            {
                AutoKnockUp();
            }
        }

        private static void StarCombo()
        {
            var target = Q.GetTarget(Q.Width / 2);
            if (!IsQOne)
            {
                target = objQ as AIHeroClient;
            }
            if (!Q.IsReady())
            {
                target = W.GetTarget();
            }
            buff.Orbwalk(target);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }
            if (Q.IsReady())
            {
                if (IsQOne)
                {
                    Q.CastSpellSmite(target, false);
                }
                else if (!IsDashingW && HaveQ(target)
                         && (target.Health + target.AllShield
                             <= Q.GetDamage(target, DamageStage.SecondCast) + player.GetAutoAttackDamage(target)
                             || (!R.IsReady() && IsRecentR() && CanR(target))) && Q2.Cast())
                {
                    return;
                }
            }
            if (E.IsReady() && IsEOne && E.CanHitCircle(target) && (!HaveQ(target) || player.Mana >= 80) && E.Cast())
            {
                return;
            }
            if (!R.IsReady() || IsRecentR(500) || !Q.IsReady() || IsQOne || !HaveQ(target))
            {
                return;
            }
            if (R.CastOnUnit(target))
            {
                return;
            }
            if (W.IsReady() && IsWOne && !isDashingQ && target.DistanceToPlayer() > R.Range + target.BoundingRadius
                && target.DistanceToPlayer() < Ward.Range + R.Range - 50 && player.Mana >= 80)
            {
                Flee(target.Position, true);
            }
        }

        private static void UseItem(AIHeroClient target)
        {
            if (target != null && (target.HealthPercent < 40 || player.HealthPercent < 50))
            {
                if (Bilgewater.IsReady)
                {
                    Bilgewater.Cast(target);
                }
                if (BotRuinedKing.IsReady)
                {
                    BotRuinedKing.Cast(target);
                }
            }
            if (Youmuu.IsReady && player.CountEnemyHeroesInRange(W.Range + E.Range) > 0)
            {
                Youmuu.Cast();
            }
            if (Common.CantAttack)
            {
                Common.CastTiamatHydra();
            }
        }

        #endregion

        private static class Insec
        {
            #region Static Fields

            internal static bool IsWardFlash;

            internal static int LastWardPlaceTime, LastWardJumpTime;

            private static Vector3 lastEndPos, lastFlashPos;

            private static int lastInsecTime, lastMoveTime, lastRFlashTime, lastFlashRTime;

            private static AIBaseClient lastObjQ;

            #endregion

            #region Properties

            private static bool CanWardFlash
                => MainMenu["Insec"]["Flash"] && MainMenu["Insec"]["FlashJump"] && Ward.CanJump && Common.CanFlash;

            private static AIHeroClient GetTarget
            {
                get
                {
                    AIHeroClient target;
                    if (MainMenu["Insec"]["TargetSelect"])
                    {
                        var sub = TargetSelector.SelectedTarget;
                        target = sub.IsValidTarget() ? sub : null;
                    }
                    else
                    {
                        var extraRange = 100
                                         + (CanWardFlash
                                                ? GetRange(null, true)
                                                : (Ward.CanJump ? Ward.Range : FlashRange));
                        if (MainMenu["Insec"]["Q"] && Q.IsReady() && IsQOne)
                        {
                            target = Q.GetTarget(extraRange);
                        }
                        else if (objQ.IsValidTarget(Q2.Range))
                        {
                            target = Q2.GetTarget(extraRange);
                        }
                        else
                        {
                            target = TargetSelector.GetTarget(extraRange, R.DamageType);
                        }
                    }
                    return target;
                }
            }

            private static bool IsReady
                => (Ward.CanJump || (MainMenu["Insec"]["Flash"] && Common.CanFlash) || IsRecent) && R.IsReady();

            private static bool IsRecent
                =>
                    Variables.TickCount - LastWardJumpTime < 5000 || Variables.TickCount - LastWardPlaceTime < 5000
                    || (MainMenu["Insec"]["Flash"] && Variables.TickCount - lastFlashRTime < 5000);

            #endregion

            #region Methods
            internal static void Init()
            {
                var insecMenu = MainMenu.Add(new Menu("Insec", "Insec"));
                {
                    insecMenu.KeyBind("R", "Keybind", Keys.T).ValueChanged += (sender, args) =>
                    {
                        var keybind = sender as MenuKeyBind;
                        if (keybind != null)
                        {
                            Orbwalker.AttackState = !keybind.Active;
                        }
                    };
                    insecMenu.Bool("TargetSelect", "Only Insec Target Selected", false);
                    insecMenu.List("Mode", "Mode", new[] { "Tower/Hero/Current", "Mouse Position", "Current Position" });
                    insecMenu.Separator("Draw Settings");
                    insecMenu.Bool("DLine", "Line");
                    insecMenu.Bool("DWardFlash", "WardJump Flash Range");
                    insecMenu.Separator("Flash Settings");
                    insecMenu.Bool("Flash", "Use Flash");
                    insecMenu.List("FlashMode", "Flash Mode", new[] { "R-Flash", "Flash-R", "Both" });
                    insecMenu.Bool("FlashJump", "Use WardJump To Gap For Flash");
                    insecMenu.Separator("Q Settings");
                    insecMenu.Bool("Q", "Use Q");
                    insecMenu.Bool("QCol", "Smite Collision");
                    insecMenu.Bool("QObj", "Use Q On Near Object");
                }

                Game.OnUpdate += args =>
                {
                    if (lastInsecTime > 0 && Variables.TickCount - lastInsecTime > 5000)
                    {
                        Clean();
                    }
                    if (lastMoveTime > 0 && Variables.TickCount - lastMoveTime > 500 && !R.IsReady())
                    {
                        lastMoveTime = 0;
                    }
                };
                Drawing.OnDraw += args =>
                {
                    if (player.IsDead || R.Level == 0 || !IsReady)
                    {
                        return;
                    }
                    var dLine = MainMenu["Insec"]["DLine"];
                    var dRange = MainMenu["Insec"]["DWardFlash"] && CanWardFlash;
                    if (!dLine && !dRange)
                    {
                        return;
                    }
                    var target = GetTarget;
                    if (dLine && target != null)
                    {
                        var posTarget = target.Position;
                        var posEnd = GetPosEndEx(target);
                        Render.Circle.DrawCircle(posTarget, 100, Color.BlueViolet);
                        Render.Circle.DrawCircle(GetPosBehind(target, posEnd), 100, Color.BlueViolet);
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(posTarget),
                            Drawing.WorldToScreen(posEnd),
                            1,
                            Color.BlueViolet);
                    }
                    if (dRange)
                    {
                        Render.Circle.DrawCircle(player.Position, GetRange(target, true), Color.Orange);
                    }
                };
                AIBaseClient.OnBuffGain += (sender, args) =>
                {
                    if (!sender.IsEnemy)
                    {
                        return;
                    }
                    if (args.Buff.Name.Contains("BlindMonkQ"))
                    {
                        lastObjQ = sender;
                    }
                    else if (args.Buff.Name == "blindmonkrroot" && Common.CanFlash
                             && MainMenu["Insec"]["R"].GetValue<MenuKeyBind>().Active && MainMenu["Insec"]["Flash"]
                             && Variables.TickCount - lastRFlashTime < 5000)
                    {
                        var target = sender as AIHeroClient;
                        if (target != null && TargetSelector.SelectedTarget.Compare(target)
                            && target.Health + target.AllShield > R.GetDamage(target))
                        {
                            player.Spellbook.CastSpell(Flash, GetPosBehind(target, GetPosEndEx(target)));
                        }
                    }
                };
                AIBaseClient.OnProcessSpellCast += (sender, args) =>
                {
                    if (!lastFlashPos.IsValid() || !sender.IsMe
                        || !MainMenu["Insec"]["R"].GetValue<MenuKeyBind>().Active
                        || args.SData.Name != "SummonerFlash" || !MainMenu["Insec"]["Flash"]
                        || Variables.TickCount - lastFlashRTime > 2500 || args.End.Distance(lastFlashPos) > 80)
                    {
                        return;
                    }
                    lastFlashRTime = Variables.TickCount;
                };
                AIBaseClient.OnDoCast += (sender, args) =>
                {
                    if (!sender.IsMe || args.Slot != SpellSlot.R)
                    {
                        return;
                    }
                    Clean();
                };
            }

            internal static void Start()
            {
                var target = GetTarget;
                if (Orbwalker.CanMove() && Variables.TickCount - lastMoveTime > 250)
                {
                    var pos = Game.CursorPos;
                    if (target != null && lastMoveTime > 0 && IsReady)
                    {
                        var posEnd = GetPosEndEx(target);
                        if (posEnd.DistanceToPlayer() > target.Distance(posEnd))
                        {
                            pos = GetPosBehind(target, posEnd);
                        }
                    }
                    Orbwalker.Move(pos);
                }
                if (target == null || !IsReady)
                {
                    return;
                }
                if (!IsRecent)
                {
                    if (!IsWardFlash)
                    {
                        if (CanWardFlash
                            && (!isDashingQ
                                || (!lastObjQ.Compare(target) && target.Distance(lastObjQ) > GetRange(target)))
                            && target.DistanceToPlayer() < GetRange(target, true))
                        {
                            IsWardFlash = true;
                            return;
                        }
                        if (Ward.CanJump)
                        {
                            InsecWardJump(target);
                        }
                        else if (Common.CanFlash)
                        {
                            InsecFlash(target);
                        }
                    }
                    else
                    {
                        Common.SetTarget(target);
                        Ward.Place(target.Position);
                        return;
                    }
                }
                if (R.IsInRange(target))
                {
                    var posEnd = GetPosEndEx(target);
                    var segment =
                        target.Position.Extend(player.Position, -R2.Range)
                            .ProjectOn(target.Position, posEnd.Extend(target.Position, -(R2.Range / 2)));
                    if (segment.IsOnSegment && segment.SegmentPoint.Distance(posEnd) <= R2.Range / 2
                        && R.CastOnUnit(target))
                    {
                        return;
                    }
                }
                CastQ(target);
            }

            private static void CastQ(AIHeroClient target)
            {
                if (!MainMenu["Insec"]["Q"] || !Q.IsReady() || IsDashingW)
                {
                    return;
                }
                if (CanWardFlash && (IsWardFlash || (IsQOne && player.Mana < 50 + 80)))
                {
                    return;
                }
                var minDist = GetRange(target, CanWardFlash);
                if (IsQOne)
                {
                    Q.CastSpellSmite(target, MainMenu["Insec"]["QCol"]);
                    if (!MainMenu["Insec"]["QObj"])
                    {
                        return;
                    }
                    var nearObj =
                        Common.ListEnemies(true)
                            .Where(
                                i =>
                                i.IsValidTarget(Q.Range) && !i.Compare(target)
                                && Q.GetHealthPrediction(i) > Q.GetDamage(i)
                                && i.Distance(target) < target.DistanceToPlayer() && i.Distance(target) < minDist - 80)
                            .OrderBy(i => i.Distance(target))
                            .ThenByDescending(i => i.Health)
                            .ToList();
                    if (nearObj.Count == 0)
                    {
                        return;
                    }
                    nearObj.ForEach(i => Q.Casting(i));
                }
                else if (target.DistanceToPlayer() > minDist
                         && (HaveQ(target)
                             || (objQ.IsValidTarget(Q2.Range) && objQ.Distance(target) < objQ.DistanceToPlayer()
                                 && objQ.Distance(target) < minDist - 80))
                         && ((Ward.CanJump && player.Mana >= 80) || (MainMenu["Insec"]["Flash"] && Common.CanFlash))
                         && Q2.Cast())
                {
                    Common.SetTarget(target);
                }
            }

            private static void Clean()
            {
                IsWardFlash = false;
                lastEndPos = lastFlashPos = Vector3.Zero;
                lastInsecTime = 0;
                Common.SetTarget(null);
            }

            private static float GetDistBehind(AIHeroClient target)
            {
                return Math.Min((player.BoundingRadius + target.BoundingRadius + 50) * 1.4f, 300);
            }

            private static Vector3 GetPosBehind(AIHeroClient target, Vector3 end)
            {
                return target.Position.Extend(end, -GetDistBehind(target));
            }

            private static Vector3 GetPosEnd(AIHeroClient target, Vector3 end)
            {
                return target.Position.Extend(end, R2.Range);
            }

            private static Vector3 GetPosEndEx(AIHeroClient target)
            {
                if (lastEndPos.IsValid())
                {
                    return lastEndPos;
                }
                var pos = player.Position;
                switch (MainMenu["Insec"]["Mode"].GetValue<MenuList>().Index)
                {
                    case 0:
                        var turret =
                            GameObjects.AllyTurrets.Where(
                                i =>
                                target.Distance(i) <= 1400 && i.Distance(target) - R2.Range <= 950
                                && i.Distance(target) > 225).MinOrDefault(i => i.DistanceToPlayer());
                        if (turret != null)
                        {
                            pos = turret.Position;
                        }
                        else
                        {
                            var hero =
                                GameObjects.AllyHeroes.Where(
                                    i =>
                                    i.IsValidTarget(1600, false, target.Position) && !i.IsMe
                                    && i.HealthPercent > 10 && i.Distance(target) > 325)
                                    .MaxOrDefault(i => i.CountAllyHeroesInRange(600));
                            if (hero != null)
                            {
                                pos = hero.Position;
                            }
                        }
                        break;
                    case 1:
                        pos = Game.CursorPos;
                        break;
                }
                return pos;
            }

            private static float GetRange(AIHeroClient target, bool isWardFlash = false)
            {
                return !isWardFlash
                           ? (Ward.CanJump ? Ward.Range : FlashRange) - GetDistBehind(target)
                           : Ward.Range
                             + (MainMenu["Insec"]["FlashMode"].GetValue<MenuList>().Index == 1 && target != null
                                    ? FlashRange - GetDistBehind(target)
                                    : R.Range - 50);
            }

            private static void InsecFlash(AIHeroClient target)
            {
                switch (MainMenu["Insec"]["FlashMode"].GetValue<MenuList>().Index)
                {
                    case 0:
                        InsecRFlash(target);
                        break;
                    case 1:
                        InsecFlashR(target);
                        break;
                    case 2:
                        if (R.IsInRange(target))
                        {
                            InsecRFlash(target);
                        }
                        else
                        {
                            InsecFlashR(target);
                        }
                        break;
                }
            }

            private static void InsecFlashR(AIHeroClient target)
            {
                var posEnd = GetPosEndEx(target);
                var posBehind = GetPosBehind(target, posEnd);
                if (posBehind.DistanceToPlayer() < FlashRange)
                {
                    if (Orbwalker.CanMove())
                    {
                        lastMoveTime = Variables.TickCount;
                    }
                    lastFlashPos = posBehind;
                    lastEndPos = GetPosEnd(target, posEnd);
                    lastInsecTime = lastFlashRTime = Variables.TickCount;
                    Common.SetTarget(target);
                    player.Spellbook.CastSpell(Flash, posBehind);
                }
            }

            private static void InsecRFlash(AIHeroClient target)
            {
                if (!R.CastOnUnit(target))
                {
                    return;
                }
                lastEndPos = GetPosEnd(target, GetPosEndEx(target));
                lastInsecTime = lastRFlashTime = Variables.TickCount;
                Common.SetTarget(target);
            }

            private static void InsecWardJump(AIHeroClient target)
            {
                var posEnd = GetPosEndEx(target);
                var posBehind = GetPosBehind(target, posEnd);
                if (posBehind.DistanceToPlayer() < Ward.Range)
                {
                    if (Orbwalker.CanMove())
                    {
                        lastMoveTime = Variables.TickCount;
                        Orbwalker.Move(
                            posBehind.Extend(posEnd, -(GetDistBehind(target) + player.BoundingRadius / 2)));
                    }
                    lastEndPos = GetPosEnd(target, posEnd);
                    lastInsecTime = LastWardPlaceTime = LastWardJumpTime = Variables.TickCount;
                    Common.SetTarget(target);
                    Ward.Place(posBehind, true);
                }
            }

            #endregion
        }
        private static class Ward
        {
            #region Constants

            internal const int Range = 600;

            #endregion

            #region Static Fields

            private static int lastJumpTime;

            private static Vector3 lastWardPos;

            #endregion

            #region Properties

            internal static bool CanJump => CanPlace && W.IsReady() && IsWOne;

            private static bool CanPlace => Variables.TickCount - lastJumpTime > 1250 && Items.GetWardSlot(player) != null;

            private static bool IsJumping => lastWardPos.IsValid() && Variables.TickCount - lastJumpTime < 1250;

            #endregion

            #region Methods

            internal static void Init()
            {
                Game.OnUpdate += args =>
                {
                    if (lastWardPos.IsValid() && Variables.TickCount - lastJumpTime > 1500)
                    {
                        lastWardPos = Vector3.Zero;
                    }
                    if (player.IsDead)
                    {
                        return;
                    }
                    if (IsJumping)
                    {
                        Jump(lastWardPos);
                    }
                };
                AIBaseClient.OnProcessSpellCast += (sender, args) =>
                {
                    if (!lastWardPos.IsValid() || !sender.IsMe || args.Slot != SpellSlot.W
                        || !args.SData.Name.ToLower().Contains("one"))
                    {
                        return;
                    }
                    var ward = args.Target as AIMinionClient;
                    if (ward == null || !ward.IsValid || !ward.IsWard() || ward.Distance(lastWardPos) > 80)
                    {
                        return;
                    }
                    if (Variables.TickCount - Insec.LastWardJumpTime < 2500)
                    {
                        Insec.LastWardJumpTime = Variables.TickCount;
                    }
                    Insec.IsWardFlash = false;
                    lastWardPos = Vector3.Zero;
                };
                GameObject.OnCreate += (sender, miniona) =>
                {
                    var minion = sender as AIMinionClient;

                    if (!lastWardPos.IsValid() || !minion.IsAlly || !minion.IsWard()
                        || minion.Distance(lastWardPos) > 80)
                    {
                        return;
                    }
                    if (Variables.TickCount - Insec.LastWardPlaceTime < 2500)
                    {
                        Insec.LastWardPlaceTime = Variables.TickCount;
                    }
                    if (IsJumping && W.IsReady() && IsWOne)
                    {
                        W.CastOnUnit(minion);
                    }
                };
            }

            internal static void Place(Vector3 pos, bool isInsec = false)
            {
                if (!CanJump)
                {
                    return;
                }
                var posplayer = player.Position;
                var posPlace = pos.Distance(posplayer) < Range ? pos : posplayer.Extend(pos, Range);
                player.Spellbook.CastSpell(Items.GetWardSlot(player).SpellSlot, posPlace);
                lastJumpTime = Variables.TickCount + (isInsec ? 0 : 1100);
                lastWardPos = posPlace;
            }

            private static void Jump(Vector3 pos)
            {
                if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
                {
                    return;
                }
                var ward =
                    GameObjects.AllyWards.Where(
                        i => i.IsValidTarget(W.Range, false) && i.IsWard() && i.Distance(pos) < 200)
                        .MinOrDefault(i => i.Distance(pos));
                if (ward != null)
                {
                    W.CastOnUnit(ward);
                }
            }

            #endregion

        }

    }
}
