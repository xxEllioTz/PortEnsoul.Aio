using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using Color = System.Drawing.Color;

namespace AmumuSharp
{
    internal class Amumu
    {
        readonly Menu _menu;

        private static readonly AIHeroClient Player = ObjectManager.Player;
        private readonly Spell _spellQ;
        private readonly Spell _spellW;
        private readonly Spell _spellE;
        private readonly Spell _spellR;

        private bool _comboW;

        public Amumu()
        {
            if (Player.CharacterName != "Amumu")
                return;
            _menu = new Menu("AmumuSharp", "AmumuSharp", true);
            var comboMenu = new Menu("Combo", "Combo");
            comboMenu.Add(new MenuList("comboQ", "Use Q", new[] { "No", "Always", "If out of range" }, 1));
            _menu.Add(comboMenu);
            comboMenu.Add(new MenuBool("comboW", "Use W", true));
            comboMenu.Add(new MenuBool("comboE", "Use E", true));
            comboMenu.Add(new MenuSlider("comboR", "Auto Use R", 3, 0, 5));
            comboMenu.Add(new MenuSlider("comboWPercent", "Use W until Mana ", 10));

            var farmMenu = new Menu("Farming", "Farming");
            farmMenu.Add(new MenuList("farmQ", "Use Q", new[] { "No", "Always", "If out of range" }, 2));
            farmMenu.Add(new MenuBool("farmW", "Use W").SetValue(true));
            farmMenu.Add(new MenuBool("farmWignoremana", "Always Use W if got blue buff").SetValue(true));
            farmMenu.Add(new MenuBool("farmE", "Use E").SetValue(true));
            farmMenu.Add(new MenuSlider("farmWPercent", "Use W until Mana %", 20));
            _menu.Add(farmMenu);
            var drawMenu = new Menu("Draw", "Draw Settings");
            drawMenu.Add(new MenuBool("drawQ", "Draw Q range", true));
            drawMenu.Add(new MenuBool("drawW", "Draw W range", true));
            drawMenu.Add(new MenuBool("drawE", "Draw E range", true));
            drawMenu.Add(new MenuBool("drawR", "Draw R range", true));

            _menu.Add(drawMenu);

            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.Add(new MenuKeyBind("aimQ", "Q near mouse", System.Windows.Forms.Keys.T, KeyBindType.Press));
            miscMenu.Add(new MenuBool("packetCast", "Packet Cast", true));
            _menu.Add(miscMenu);
            _menu.Attach();

            _spellQ = new Spell(SpellSlot.Q, 1100);
            _spellW = new Spell(SpellSlot.W, 300f);
            _spellE = new Spell(SpellSlot.E, 350f);
            _spellR = new Spell(SpellSlot.R, 550f);

            _spellQ.SetSkillshot(.25f, 90, 2000, true, false, SkillshotType.Line);  //check delay
            _spellW.SetSkillshot(.25f, _spellW.Range, float.MaxValue, false, false, SkillshotType.Circle); //correct
            _spellE.SetSkillshot(.25f, _spellE.Range, float.MaxValue, false, false, SkillshotType.Circle); //check delay
            _spellR.SetSkillshot(.25f, _spellR.Range, float.MaxValue, false, false, SkillshotType.Circle); //check delay

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }


        void Game_OnUpdate(EventArgs args)
        {
            AutoUlt();

            if (_menu["Misc"]["aimQ"].GetValue<MenuKeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                CastQ(Program.Helper.EnemyTeam.Where(x => x.IsValidTarget(_spellQ.Range) && x.Distance(Game.CursorPos) < 1150).OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault());
            }

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                default:
                    RegulateWState();
                    break;


            }
        }

        void AutoUlt()
        {
            var comboR = _menu["Combo"]["comboR"].GetValue<MenuSlider>().Value;
            if (comboR > 0 && _spellR.IsReady())
            {
                int enemiesHit = 0;
                int killableHits = 0;
                foreach (AIHeroClient enemy in Program.Helper.EnemyTeam.Where(x => x.IsValidTarget(_spellR.Range)))
                {
                    var prediction = SpellPrediction.GetPrediction(enemy, _spellR.Delay);
                    if (prediction != null && prediction.UnitPosition.Distance(Player.Position) <= _spellR.Range)
                    {
                        enemiesHit++;

                        if (Player.GetSpellDamage(enemy, SpellSlot.W) >= enemy.Health)
                            killableHits++;
                    }
                }
                if (enemiesHit >= comboR || (killableHits >= 1 && Player.Health / Player.MaxHealth <= 0.1))
                    CastR();
            }
        }

        void CastE(AIBaseClient target)
        {
            if (!_spellE.IsReady() || target == null || !target.IsValidTarget())
                return;

            if (_spellE.GetPrediction(target).UnitPosition.Distance(Player.Position) <= _spellE.Range)
                _spellE.CastOnUnit(Player);
        }

        void JungleClear()
        {
            var farmQ = _menu["Farming"]["farmQ"].GetValue<MenuList>().Index;
            var farmW = _menu["Farming"]["farmW"].GetValue<MenuBool>();
            var farmE = _menu["Farming"]["farmE"].GetValue<MenuBool>();

            var allMinions = GameObjects.GetMinions(ObjectManager.Player.Position, _spellE.Range, MinionTypes.All, MinionTeam.All, MinionOrderTypes.MaxHealth);
            if (GetManaPercent() >= _menu["Farming"]["farmWPercent"].GetValue<MenuSlider>().Value)
            {
                if (farmQ > 0 && _spellQ.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            _spellQ.GetPrediction(minion);
                            _spellQ.CastIfHitchanceEquals(minion, HitChance.High);
                        }
                    }
                }
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    if (farmW && _spellW.IsReady() && ObjectManager.Get<AIMinionClient>().Any(minion => minion.IsValidTarget(_spellW.Range)))
                    {
                        _spellW.Cast();
                    }
                }
                if (farmE && _spellE.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            _spellE.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        void LaneClear()
        {
            var farmQ = _menu["Farming"]["farmQ"].GetValue<MenuList>().Index;
            var farmW = _menu["Farming"]["farmW"].GetValue<MenuBool>();
            var farmE = _menu["Farming"]["farmE"].GetValue<MenuBool>();

            List<AIBaseClient> minions;
            if (farmQ > 0 && _spellQ.IsReady())
            {
                AIBaseClient minion = GameObjects.GetMinions(Player.Position, _spellQ.Range, MinionTypes.All, MinionTeam.All, MinionOrderTypes.MaxHealth).FirstOrDefault(x => _spellQ.GetPrediction(x).Hitchance >= HitChance.Medium);

                if (minion != null)
                    if (farmQ == 1 || (farmQ == 2 && !Extensions.InAutoAttackRange(minion)))
                        CastQ(minion, HitChance.Medium);
            }
            if (farmE && _spellE.IsReady())
            {
                minions = GameObjects.GetMinions(Player.Position, _spellE.Range, MinionTypes.All, MinionTeam.All);
                CastE(minions.OrderBy(x => x.Distance(Player)).FirstOrDefault());
            }
            if (!farmW || !_spellW.IsReady())
                return;
            _comboW = false;
            minions = GameObjects.GetMinions(Player.Position, _spellW.Range, MinionTypes.All, MinionTeam.All);
            bool anyJungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            var enoughMana = GetManaPercent() > _menu["Farming"]["farmWPercent"].GetValue<MenuSlider>().Value;

            if (enoughMana || (Player.HasBuff("CrestOfInsight") && _menu["Farming"]["farmWignoremana"].GetValue<MenuBool>()) && ((minions.Count >= 3 || anyJungleMobs) && Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1))
                _spellW.Cast();
            else if (!enoughMana || ((minions.Count <= 2 && !anyJungleMobs) && Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2))
                RegulateWState(!enoughMana);

        }

        public bool PacketsNoLel()
        {
            return _menu["Misc"]["packetCast"].GetValue<MenuBool>();
        }
        void Combo()
        {
            var comboQ = _menu["Combo"]["comboQ"].GetValue<MenuList>().Index;
            var comboW = _menu["Combo"]["comboW"].GetValue<MenuBool>();
            var comboE = _menu["Combo"]["comboE"].GetValue<MenuBool>();
            var comboR = _menu["Combo"]["comboR"].GetValue<MenuSlider>().Value;
            if (comboQ > 0 && _spellQ.IsReady())
            {
                if (_spellR.IsReady() && comboR > 0)
                {
                    int maxTargetsHit = 0;
                    AIBaseClient unitMostTargetsHit = null;

                    foreach (AIBaseClient unit in ObjectManager.Get<AIBaseClient>().Where(x => x.IsValidTarget(_spellQ.Range) && _spellQ.GetPrediction(x).Hitchance >= HitChance.High)) //causes troubles?
                    {
                        int targetsHit = unit.CountEnemyHeroesInRange((int)_spellR.Range); //unitposition might not reflect where you land with Q

                        if (targetsHit > maxTargetsHit || (unitMostTargetsHit != null && targetsHit >= maxTargetsHit && unit.Type == GameObjectType.AIHeroClient))
                        {
                            maxTargetsHit = targetsHit;
                            unitMostTargetsHit = unit;
                        }
                    }

                    if (maxTargetsHit >= comboR)
                        CastQ(unitMostTargetsHit);

                }

                AIHeroClient target1 = TargetSelector.GetTarget(_spellQ.Range, DamageType.Magical);
                if (!target1.IsValidTarget())
                {
                    return;
                }
                if (comboQ == 1 || (comboQ == 2 && _spellQ.IsReady() && target1.IsValidTarget(_spellQ.Range) && _spellQ.GetPrediction(target1).Hitchance >= HitChance.High))
                {

                    _spellQ.CastIfHitchanceEquals(target1, HitChance.High);
                }
            }


            if (comboW && _spellW.IsReady())
            {
                var target = TargetSelector.GetTarget(_spellW.Range, DamageType.Magical);

                if (target != null)
                {

                    var enoughMana = GetManaPercent() >= _menu["Combo"]["comboWPercent"].GetValue<MenuSlider>().Value;

                    if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        if (Player.Distance(target.Position) <= _spellW.Range && enoughMana)
                        {
                            _comboW = true;
                            _spellW.Cast();
                        }
                    }
                    else if (!enoughMana)
                        RegulateWState(true);
                }
                else
                    RegulateWState();
            }
            if (comboE && _spellE.IsReady())
                CastE(Program.Helper.EnemyTeam.OrderBy(x => x.Distance(Player)).FirstOrDefault());


        }

        void RegulateWState(bool ignoreTargetChecks = false)
        {
            if (!_spellW.IsReady() || Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 2)
                return;

            var target = TargetSelector.GetTarget(_spellW.Range, DamageType.Magical);
            var minions = EnsoulSharp.SDK.GameObjects.GetMinions(Player.Position, _spellW.Range, MinionTypes.All, MinionTeam.Enemy);

            if (!ignoreTargetChecks && (target != null || (!_comboW && minions.Count != 0)))
                return;

            _spellW.Cast();
            _comboW = false;
        }

        public float GetManaPercent()
        {
            return (Player.Mana / Player.MaxMana) * 100f;
        }



        void CastQ(AIBaseClient target, HitChance hitChance = HitChance.High)
        {
            if (!_spellQ.IsReady())
                return;
            if (target == null || !target.IsValidTarget())
                return;

            _spellQ.CastIfHitchanceEquals(target, hitChance);
        }



        void CastR()
        {
            if (!_spellR.IsReady())
                return;
            _spellR.Cast();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (_menu["Draw"]["drawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(Player.Position, _spellQ.Range, Color.Aqua);
            }
            if (_menu["Draw"]["drawW"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(Player.Position, _spellW.Range, Color.Aqua);
            }
            if (_menu["Draw"]["drawE"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(Player.Position, _spellW.Range, Color.Aqua);
            }
            if (_menu["Draw"]["drawR"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(Player.Position, _spellW.Range, Color.Aqua);
            }



        }
    }
}