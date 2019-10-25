using System;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using Color = System.Drawing.Color;
using static EnsoulSharp.SDK.Items;
using SPrediction;

namespace D_Rengar
{
    internal class Program
    {
        private static AIHeroClient _player;

        private static Spell _q, _w, _e, _r;



        private static SpellSlot _igniteSlot;

        private static Items.Item _youmuu, _tiamat, _hydra, _blade, _bilge, _rand, _lotis;

        private static SpellSlot _smiteSlot;

        private static Menu Menu { get; set; }

        public static Menu comboMenu, harassMenu, itemMenu, clearMenu, miscMenu, lasthitMenu, jungleMenu, drawMenu, smiteMenu;

        private static Spell _smite;

        private static int _lastTick;

        public static void RengarGame_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (_player.CharacterName != "Rengar") return;
            _q = new Spell(SpellSlot.Q, 250f);
            _w = new Spell(SpellSlot.W, 400);
            _e = new Spell(SpellSlot.E, 980f);
            _r = new Spell(SpellSlot.R, 2000f);

            _e.SetSkillshot(0.125f, 70f, 1500f, true, true, SkillshotType.Line);

            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _youmuu = new Items.Item(3142, 10);
            _igniteSlot = _player.GetSpellSlot("SummonerDot");
            _smite = new Spell(SpellSlot.Summoner1, 570f);
            _smiteSlot = SpellSlot.Summoner1;
            _smite = new Spell(SpellSlot.Summoner2, 570f);
            _smiteSlot = SpellSlot.Summoner2;
            Menu = new Menu("D-Rengar", "D-Rengar", true);

            comboMenu = new Menu("Combo", "Combo");
            comboMenu.Add(new MenuKeyBind("Switch", "Switch Empowered Priority", System.Windows.Forms.Keys.T, KeyBindType.Press));
            comboMenu.Add(new MenuList("ComboPrio", "Empowered Priority", new[] { "Q", "W", "E" }) { Index = 0 });
            comboMenu.Add(new MenuBool("smitecombo", "Use Smite in target"));
            comboMenu.Add(new MenuBool("UseQC", "Use Q"));
            comboMenu.Add(new MenuBool("UseWC", "Use W"));
            comboMenu.Add(new MenuBool("UseEC", "Use E"));
            comboMenu.Add(new MenuBool("UseEEC", "Use Empower E when Q(range) < target(range)"));
            Menu.Add(comboMenu);
            itemMenu = new Menu("Items", "items");
            itemMenu.Add(new MenuBool("Youmuu", "Use Youmuu's"));
            itemMenu.Add(new MenuBool("Tiamat", "Use Tiamat"));
            itemMenu.Add(new MenuBool("Hydra", "Use Hydra"));
            itemMenu.Add(new MenuBool("Bilge", "Use Bilge"));
            itemMenu.Add(new MenuSlider("BilgeEnemyhp", "If Enemy Hp <", 85, 1, 100));
            itemMenu.Add(new MenuSlider("Bilgemyhp", "Or Your Hp <", 85, 1, 100));
            itemMenu.Add(new MenuBool("Blade", "Use Bork"));
            itemMenu.Add(new MenuSlider("BladeEnemyhp", "If Enemy Hp <", 85, 1, 100));
            itemMenu.Add(new MenuSlider("Blademyhp", "Or Your Hp <", 85, 1, 100));
            itemMenu.Add(new MenuSeparator("Deffensive Items", "Deffensive Items"));
            itemMenu.Add(new MenuBool("useqss", "Use QSS/Mercurial Scimitar/Dervish Blade"));
            itemMenu.Add(new MenuBool("blind", "Blind"));
            itemMenu.Add(new MenuBool("charm", "Charm"));
            itemMenu.Add(new MenuBool("fear", "Fear"));
            itemMenu.Add(new MenuBool("flee", "Flee"));
            itemMenu.Add(new MenuBool("taunt", "Taunt"));
            itemMenu.Add(new MenuBool("snare", "Snare"));
            itemMenu.Add(new MenuBool("suppression", "Suppression"));
            itemMenu.Add(new MenuBool("stun", "Stun"));
            itemMenu.Add(new MenuBool("polymorph", "Polymorph"));
            itemMenu.Add(new MenuBool("silence", "Silence"));
            itemMenu.Add(new MenuList("Cleansemode", "Use Cleanse", new[] { "Always", "In Combo" }) { Index = 0 });
            itemMenu.Add(new MenuSeparator("Potions", "Potions"));
            itemMenu.Add(new MenuBool("usehppotions", "Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"));
            itemMenu.Add(new MenuSlider("usepotionhp", "If Health % <", 35, 1, 100));
            itemMenu.Add(new MenuBool("usemppotions", "Use Hunters/Corrupting/Biscuit"));
            itemMenu.Add(new MenuSlider("usepotionmp", "If Mana % <", 35, 1, 100));
            Menu.Add(itemMenu);
            harassMenu = new Menu("Harass", "Harass");
            harassMenu.Add(new MenuList("HarrPrio", "Empowered Priority", new[] { "Q", "W", "E" }) { Index = 0 });
            harassMenu.Add(new MenuBool("UseQH", "Use Q"));
            harassMenu.Add(new MenuBool("UseWH", "Use W"));
            harassMenu.Add(new MenuBool("UseEH", "Use E"));
            harassMenu.Add(new MenuKeyBind("harasstoggle", "AutoHarass (toggle)", System.Windows.Forms.Keys.L, KeyBindType.Toggle));
            Menu.Add(harassMenu);

            lasthitMenu = new Menu("LastHit", "LastHit");
            lasthitMenu.Add(new MenuList("LastPrio", "Empowered Priority", new[] { "Q", "W", "E" }) { Index = 0 });
            lasthitMenu.Add(new MenuBool("LastSave", "Save Ferocity"));
            lasthitMenu.Add(new MenuBool("UseQLH", "Q LastHit"));
            lasthitMenu.Add(new MenuBool("UseWLH", "W LastHit"));
            lasthitMenu.Add(new MenuBool("UseELH", "E LastHit"));
            Menu.Add(lasthitMenu);
            clearMenu = new Menu("LaneClear", "LaneClear");
            clearMenu.Add(new MenuList("LanePrio", "Empowered Priority", new[] { "Q", "W", "E" }) { Index = 0 });
            clearMenu.Add(new MenuBool("LaneSave", "Save Ferocity"));
            clearMenu.Add(new MenuBool("UseItemslane", "Use Items"));
            clearMenu.Add(new MenuBool("UseQL", "Q LaneClear"));
            clearMenu.Add(new MenuBool("UseWL", "W LaneClear"));
            clearMenu.Add(new MenuBool("UseEL", "E LaneClear"));
            Menu.Add(clearMenu);
            jungleMenu = new Menu("JungleClear", "JungleClear");
            jungleMenu.Add(new MenuList("JunglePrio", "Empowered Priority", new[] { "Q", "W", "E" }) { Index = 0 });
            jungleMenu.Add(new MenuBool("JungleSave", "Save Ferocity"));
            jungleMenu.Add(new MenuBool("UseItemsjungle", "Use Items"));
            jungleMenu.Add(new MenuBool("UseQJ", "Q JungleClear"));
            jungleMenu.Add(new MenuBool("UseWJ", "W JungleClear"));
            jungleMenu.Add(new MenuBool("UseEJ", "E JungleClear"));
            Menu.Add(jungleMenu);
            smiteMenu = new Menu("Smite", "Smite");
            smiteMenu.Add(new MenuKeyBind("Usesmite", "Use Smite(toggle)", System.Windows.Forms.Keys.N, KeyBindType.Toggle));
            smiteMenu.Add(new MenuBool("Usered", "Smite Red Early"));
            smiteMenu.Add(new MenuSlider("healthJ", "Smite Red Early if HP% <", 35, 1, 100));
            Menu.Add(smiteMenu);
            miscMenu = new Menu("Misc", "Misc");
            miscMenu.Add(new MenuBool("UseIgnite", "Use Ignite KillSteal"));
            miscMenu.Add(new MenuBool("UseQM", "Use Q KillSteal"));
            miscMenu.Add(new MenuBool("UseWM", "Use W KillSteal"));
            miscMenu.Add(new MenuBool("UseRM", "Use R KillSteal"));
            miscMenu.Add(new MenuBool("UseEInt", "E to Interrupt"));
            miscMenu.Add(new MenuBool("AutoW", "use W to Heal"));
            miscMenu.Add(new MenuSlider("AutoWHP", "If Health % <", 35, 1, 100));
            miscMenu.Add(new MenuList("Echange", "E Hit", new[] { "Low", "Medium", "High", "Very High" }) { Index = 0 });
            Menu.Add(miscMenu);
            drawMenu = new Menu("Drawings", "Drawings");
            drawMenu.Add(new MenuBool("DrawQ", "Draw Q", false));
            drawMenu.Add(new MenuBool("DrawW", "Draw W", false));
            drawMenu.Add(new MenuBool("DrawE", "Draw E", false));
            drawMenu.Add(new MenuBool("Drawsmite", "Draw smite", true));
            drawMenu.Add(new MenuBool("Drawharass", "Draw AutoHarass", true));
            drawMenu.Add(new MenuBool("combomode", "Draw Combo Mode", true));
            drawMenu.Add(new MenuBool("DamageAfterCombo", "Draw damage after combo", true));
            Menu.Add(drawMenu);
            Menu.Attach();
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;

            Game.Print("<font color='#881df2'>D-Rengar by Diabaths</font> Loaded.");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AIBaseClient.OnProcessSpellCast += OnProcessSpellCast;
            Interrupter.OnInterrupterSpell += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnAction += OnBeforeAttack;
            Orbwalker.OnAction += OnAfterAttack;
            GameEvent.OnGameLoad += Dash;
            Game.Print(
                "<font color='#f2f21d'>Do you like it???  </font> <font color='#ff1900'>Drop 1 Upvote in Database </font>");
            Game.Print(
                "<font color='#f2f21d'>Buy me cigars </font> <font color='#ff1900'>ssssssssssmith@hotmail.com</font> (10) S");
        }


        public static bool getMenuBoolItem(Menu m, string item)
        {
            return m[item].GetValue<MenuBool>().Enabled;
        }

        public static int getMenuSliderItem(Menu m, string item)
        {
            return m[item].GetValue<MenuSlider>().Value;
        }

        public static bool getMenuKeyBindItem(Menu m, string item)
        {
            return m[item].GetValue<MenuKeyBind>().Active;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].GetValue<MenuList>().Index;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getMenuBoolItem(drawMenu, "DamageAfterCombo");
            if (_player.IsDead) return;
            if (getMenuBoolItem(miscMenu, "AutoW") && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                AutoHeal();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo
                && (Orbwalker.ActiveMode == OrbwalkerMode.Harass
                    || getMenuKeyBindItem(harassMenu, "harasstoggle")))
            {
                Harass();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                Laneclear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                JungleClear();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }

            Usepotion();
            if (getMenuKeyBindItem(smiteMenu, "Usesmite"))
            {
                Smiteuse();
            }

            _player = ObjectManager.Player;



            KillSteal();
            ChangeComboMode();
        }

        public static void Dash()
        {
            var useQ = getMenuBoolItem(comboMenu, "UseQC");
            var useW = getMenuBoolItem(comboMenu, "UseWC");
            var useE = getMenuBoolItem(comboMenu, "UseEC");
            var useEE = getMenuBoolItem(comboMenu, "UseEEC");
            var iYoumuu = getMenuBoolItem(itemMenu, "Youmuu");
            var iTiamat = getMenuBoolItem(itemMenu, "Tiamat");
            var iHydra = getMenuBoolItem(itemMenu, "Hydra");
            //if (!sender.IsMe) return;
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                if (_player.Mana <= 4)
                {
                    if (useQ)
                    {
                        var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                        if (tq.IsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                    }

                    var th = TargetSelector.GetTarget(_e.Range, DamageType.Magical);

                    if (iTiamat && _tiamat.IsReady && th.IsValidTarget(_tiamat.Range))
                    {
                        _tiamat.Cast();
                    }

                    if (iHydra && _hydra.IsReady && th.IsValidTarget(_hydra.Range))
                    {
                        _hydra.Cast();
                    }


                    if (useE)
                    {
                        var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                        var predE = _e.GetPrediction(te);
                        if (te.IsValidTarget(_e.Range) && _e.IsReady()
                            && predE.Hitchance >= Echange() && predE.CollisionObjects.Count == 0)
                            _e.Cast(te);
                    }
                }

                if (_player.Mana == 5)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (useQ
                        && (getBoxItem(comboMenu, "ComboPrio") == 1
                            || (getBoxItem(comboMenu, "ComboPrio") == 2
                              && _player.InAutoAttackRange())))
                        if (tq.IsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                    var th = TargetSelector.GetTarget(_e.Range, DamageType.Magical);

                    if (iTiamat && _tiamat.IsReady && th.IsValidTarget(_tiamat.Range))
                    {
                        _tiamat.Cast();
                    }

                    if (iHydra && _hydra.IsReady && th.IsValidTarget(_hydra.Range))
                    {
                        _hydra.Cast();
                    }

                    if (useE && getBoxItem(comboMenu, "ComboPrio") == 2)
                    {
                        var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                        var predE = _e.GetPrediction(te);
                        if (te.IsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                            && predE.CollisionObjects.Count == 0)
                            _e.Cast(te);
                    }
                }
            }
        }

        private static void OnAfterAttack(object targeti, OrbwalkerActionArgs args)
        {
            var target = TargetSelector.GetTarget(900);
            var combo = Orbwalker.ActiveMode == OrbwalkerMode.Combo;
            var Q = getMenuBoolItem(comboMenu, "UseQC");
            if (!target.IsMe) return;
            if (combo && _q.IsReady() && Q && target.IsValidTarget(_q.Range))
            {
                _q.Cast();
            }
        }

        private static void OnBeforeAttack(object targete, OrbwalkerActionArgs args)
        {
            var target = TargetSelector.GetTarget(900);
            var combo = Orbwalker.ActiveMode == OrbwalkerMode.Combo;
            var harass = Orbwalker.ActiveMode == OrbwalkerMode.Harass;
            var QC = getMenuBoolItem(comboMenu, "UseQC");
            var QH = getMenuBoolItem(harassMenu, "UseQH");
            var mode = getBoxItem(comboMenu, "ComboPrio") == 0
                       || getBoxItem(comboMenu, "ComboPrio") == 2;
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            if (_player.HasBuff("rengarpassivebuff") || _player.HasBuff("RengarR"))
            {
                return;
            }

            if (_player.Mana <= 4)
            {
                if (combo && QC && _q.IsReady() && _player.InAutoAttackRange()
                    && args.Target.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (harass && QH && _q.IsReady() && _player.InAutoAttackRange()
                    && args.Target.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }
            }

            if (_player.Mana == 5)
            {
                if (combo && QC && _q.IsReady() && _player.InAutoAttackRange() && mode
                    && args.Target.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }


                if (harass && QH && _q.IsReady() && _player.InAutoAttackRange() && mode
                    && args.Target.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }
            }
        }

        private static void ChangeComboMode()
        {
            var changetime = Environment.TickCount - _lastTick;


            if (getMenuKeyBindItem(comboMenu, "Switch"))
            {
                if (getBoxItem(comboMenu, "ComboPrio") == 0 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    comboMenu["ComboPrio"].GetValue<MenuList>().Index = 1;
                }

                if (getBoxItem(comboMenu, "ComboPrio") == 1 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    comboMenu["ComboPrio"].GetValue<MenuList>().Index = 2;
                }
                if (getBoxItem(comboMenu, "ComboPrio") == 2 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    comboMenu["ComboPrio"].GetValue<MenuList>().Index = 0;
                }
            }

        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter.InterruptSpellArgs args)
        {
            if (_player.Mana < 5) return;
            if (_e.IsReady() && unit.IsValidTarget(_e.Range) && getMenuBoolItem(miscMenu, "UseEInt"))
            {
                var predE = _e.GetPrediction(unit);
                if (predE.Hitchance >= Echange() && predE.CollisionObjects.Count == 0) _e.Cast(unit);
            }
        }

        private static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }

            if (spell.Name.ToLower().Contains("rengarq") || spell.Name.ToLower().Contains("rengare"))
            {
                Orbwalker.ResetAutoAttackTimer();
            }
        }

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            var hero = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget(570));
            var smiteDmg = _player.GetSummonerSpellDamage(hero, SummonerSpell.Smite);
            var usesmite = getMenuBoolItem(comboMenu, "smitecombo");
            if (usesmite && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                if (hero != null && (!hero.HasBuffOfType(BuffType.Stun) || !hero.HasBuffOfType(BuffType.Slow)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
                else if (hero != null && smiteDmg >= hero.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }
            if (usesmite && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready && hero.IsValidTarget(570))
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
            }
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(
                _player.Position,
                _q.Range);
            var iusehppotion = getMenuBoolItem(itemMenu, "usehppotions");
            var iusepotionhp = _player.Health
                               <= (_player.MaxHealth * (getMenuSliderItem(itemMenu, "usepotionhp")) / 100);
            var iusemppotion = getMenuBoolItem(itemMenu, "usemppotions");
            var iusepotionmp = _player.Mana
                               <= (_player.MaxMana * (getMenuSliderItem(itemMenu, "usepotionmp")) / 100);
            if (_player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (ObjectManager.Player.CountEnemyHeroesInRange(800) > 0
                || (mobs.Count > 0 && Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && _smite != null))
            {
                if (iusepotionhp && iusehppotion
                    && !(ObjectManager.Player.HasBuff("RegenerationPotion")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")))
                {

                }

                if (iusepotionmp && iusemppotion
                    && !(ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")))
                {
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = getMenuKeyBindItem(harassMenu, "harasstoggle");
            var Rengar = Drawing.WorldToScreen(_player.Position);
            if (getMenuBoolItem(drawMenu, "combomode"))
            {
                if (getBoxItem(comboMenu, "ComboPrio") == 0) Drawing.DrawText(Rengar[0] - 45, Rengar[1] + 20, Color.PaleTurquoise, "Empower:Q");
                else if (getBoxItem(comboMenu, "ComboPrio") == 1) Drawing.DrawText(Rengar[0] - 45, Rengar[1] + 20, Color.PaleTurquoise, "Empower:W");
                else if (getBoxItem(comboMenu, "ComboPrio") == 2) Drawing.DrawText(Rengar[0] - 45, Rengar[1] + 20, Color.PaleTurquoise, "Empower:E");
            }
            if (getMenuBoolItem(drawMenu, "Drawharass"))
            {
                if (harass)
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.GreenYellow,
                        "Auto harass Enabled");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.OrangeRed,
                        "Auto harass Disabled");
            }
            if (getMenuBoolItem(drawMenu, "Drawsmite") && _smite != null)
            {

                if (getMenuKeyBindItem(smiteMenu, "Usesmite"))
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.GreenYellow,
                        "Smite Jungle On");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.OrangeRed,
                        "Smite Jungle Off");


                if (getMenuBoolItem(comboMenu, "smitecombo"))
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.90f,
                        System.Drawing.Color.GreenYellow,
                        "Smite Target On");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.90f,
                        System.Drawing.Color.OrangeRed,
                        "Smite Target Off");

            }

            if (getMenuBoolItem(drawMenu, "DrawQ") && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.GreenYellow);
            }
            if (getMenuBoolItem(drawMenu, "DrawW") && _w.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.GreenYellow);
            }
            if (getMenuBoolItem(drawMenu, "DrawE") && _e.Level > 0)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    _e.Range,
                    _e.IsReady() ? System.Drawing.Color.GreenYellow : System.Drawing.Color.OrangeRed);
            }
        }

        private static double ComboDamage(AIBaseClient enemy)
        {
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready) damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, SummonerSpell.Ignite);
            //if (Items.HasItem(3077) && Items.CanUseItem(3077)) damage += _player.GetAutoAttackDamage(enemy, Damage.DamageItems.Tiamat);
            //if (Items.HasItem(3074) && Items.CanUseItem(3074)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            //if (Items.HasItem(3153) && Items.CanUseItem(3153)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            //if (Items.HasItem(3144) && Items.CanUseItem(3144)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (_q.IsReady()) damage += _player.GetSpellDamage(enemy, SpellSlot.Q) * 2;
            if (_q.IsReady()) damage += _player.GetSpellDamage(enemy, SpellSlot.W);
            if (_e.IsReady()) damage += _player.GetSpellDamage(enemy, SpellSlot.E);

            damage += _player.GetAutoAttackDamage(enemy) * 3;
            return (float)damage;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(2000, DamageType.Physical);
            var useQ = getMenuBoolItem(comboMenu, "UseQC");
            var useW = getMenuBoolItem(comboMenu, "UseWC");
            var useE = getMenuBoolItem(comboMenu, "UseEC");
            var useEE = getMenuBoolItem(comboMenu, "UseEEC");
            var iYoumuu = getMenuBoolItem(itemMenu, "Youmuu");
            var iTiamat = getMenuBoolItem(itemMenu, "Tiamat");
            var iHydra = getMenuBoolItem(itemMenu, "Hydra");
            var usesmite = getMenuBoolItem(comboMenu, "smitecombo");
            if (usesmite && target.IsValidTarget(570) && (_smite != null)
                && _player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                Smiteontarget();
            }

            if (target.IsValidTarget(600) && getMenuBoolItem(miscMenu, "UseIgnite")
                && _igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(target) > target.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, target);
                }
            }

            if (iYoumuu && _youmuu.IsReady)
            {
                if (_player.HasBuff("RengarR"))
                {
                    _youmuu.Cast();
                }
                else if (target.IsValidTarget(_e.Range))
                {
                    _youmuu.Cast();
                }
            }

            if (_player.Mana <= 4)
            {
                if (useQ)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (tq.IsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                }

                if (useW)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.IsValidTarget(_w.Range) && _w.IsReady() && !_player.HasBuff("rengarpassivebuff")) _w.Cast();
                }

                var th = TargetSelector.GetTarget(_w.Range, DamageType.Magical);

                if (iTiamat && _tiamat.IsReady && th.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (iHydra && _hydra.IsReady && th.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }


                if (useE)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (!_player.HasBuff("rengarpassivebuff") && te.IsValidTarget(_e.Range) && _e.IsReady()
                        && predE.Hitchance >= Echange() && predE.CollisionObjects.Count == 0)
                        _e.Cast(te);
                }
            }

            if (_player.Mana == 5)
            {
                var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                if (useQ
                    && (getBoxItem(comboMenu, "ComboPrio") == 0
                        || (getBoxItem(comboMenu, "ComboPrio") == 2
                            && _player.InAutoAttackRange())))
                    if (tq.IsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();

                if (useW && getBoxItem(comboMenu, "ComboPrio") == 1)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.IsValidTarget(_w.Range) && _w.IsReady() && !_player.HasBuff("rengarpassivebuff")) _w.Cast();
                }

                var th = TargetSelector.GetTarget(_w.Range, DamageType.Magical);

                if (iTiamat && _tiamat.IsReady && th.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (iHydra && _hydra.IsReady && th.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }

                if (useE && getBoxItem(comboMenu, "ComboPrio") == 2)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (te.IsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                        && predE.CollisionObjects.Count == 0 && !_player.HasBuff("rengarpassivebuff"))
                        _e.Cast(te);
                }

                if (useEE && !_player.HasBuff("RengarR")
                    && (getBoxItem(comboMenu, "ComboPrio") == 2
                        || getBoxItem(comboMenu, "ComboPrio") == 0))
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);

                    if (_player.Distance(te) > _q.Range + 100f)
                    {
                        var predE = _e.GetPrediction(te);
                        if (te.IsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                            && predE.CollisionObjects.Count == 0)
                            _e.Cast(te);
                    }
                }
            }

            UseItemes();
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var useQ = getMenuBoolItem(harassMenu, "UseQH");
            var useW = getMenuBoolItem(harassMenu, "UseWH");
            var useE = getMenuBoolItem(harassMenu, "UseEH");
            var useItemsH = getMenuBoolItem(harassMenu, "UseItemsharass");
            if (_player.Mana <= 4)
            {
                if (useQ)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (tq.IsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                }

                if (useW)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.IsValidTarget(_w.Range) && _w.IsReady()) _w.Cast();
                }

                if (useE)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (te.IsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                        && predE.CollisionObjects.Count == 0)
                        _e.Cast(te);
                }
            }

            if (_player.Mana == 5)
            {
                if (useQ && getBoxItem(harassMenu, "HarrPrio") == 0)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (tq.IsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                }

                if (useW && getBoxItem(harassMenu, "HarrPrio") == 1)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.IsValidTarget(_w.Range) && _w.IsReady() && !_player.HasBuff("rengarpassivebuff")) _w.Cast();
                }

                if (useE && getBoxItem(harassMenu, "HarrPrio") == 2)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (te.IsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                        && predE.CollisionObjects.Count == 0)
                        _e.Cast(te);
                }
            }

            if (useItemsH && _tiamat.IsReady && target.IsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsH && _hydra.IsReady && target.IsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }
        }

        private static HitChance Echange()
        {
            switch (getBoxItem(miscMenu, "Echange"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = _player.GetSummonerSpellDamage(hero, SummonerSpell.Ignite);
                var qDmg = _player.GetSpellDamage(hero, SpellSlot.Q);
                var wDmg = _player.GetSpellDamage(hero, SpellSlot.W);
                var eDmg = _player.GetSpellDamage(hero, SpellSlot.E);

                if (hero.IsValidTarget(600) && getMenuBoolItem(miscMenu, "UseIgnite")
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, hero);
                    }
                }

                if (_q.IsReady() && getMenuBoolItem(miscMenu, "UseQM") && _player.Distance(hero) <= _q.Range)
                {
                    var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (t != null) if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && qDmg > t.Health) _q.Cast(t);
                }

                if (_w.IsReady() && getMenuBoolItem(miscMenu, "UseWM") && _player.Distance(hero) <= _w.Range)
                {
                    var t = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
                    if (t != null) if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && wDmg > t.Health) _w.Cast(t);
                }

                if (_q.IsReady() && getMenuBoolItem(miscMenu, "UseRM") && _player.Distance(hero) <= _e.Range)
                {
                    var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (t != null)
                        if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && eDmg > t.Health
                            && _e.GetPrediction(t).Hitchance >= HitChance.High)
                            _e.Cast(t);
                }
            }
        }

        public static readonly string[] Smitetype =
            {
                "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
                "s5_summonersmitequick", "itemsmiteaoe", "summonersmite"
            };


        private static int GetSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        //New map Monsters Name By SKO
        private static void Smiteuse()
        {
            var jungle = Orbwalker.ActiveMode == OrbwalkerMode.LaneClear;
            if (ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) != SpellState.Ready) return;
            var usered = getMenuBoolItem(smiteMenu, "Usered");
            var health = (100 * (_player.Health / _player.MaxHealth)) < getMenuSliderItem(smiteMenu, "healthJ");
            string[] jungleMinions;

            jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            jungleMinions = new string[]
                                {
                                        "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_RiftHerald",
                                        "SRU_Red", "SRU_Krug", "SRU_Dragon_Air", "SRU_Dragon_Water", "SRU_Dragon_Fire",
                                        "SRU_Dragon_Elder", "SRU_Baron"
                                };

            var minions = MinionManager.GetMinions(_player.Position, 1000);
            if (minions.Count() > 0)
            {
                int smiteDmg = GetSmiteDmg();

                foreach (AIBaseClient minion in minions)
                {
                    if (minion.Health <= smiteDmg
                        && jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name))
                        && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && usered && health && minion.Health >= smiteDmg
                             && jungleMinions.Any(name => minion.Name.StartsWith("SRU_Red"))
                             && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                }
            }
        }

        private static void Laneclear()
        {
            var minions = MinionManager.GetMinions(_player.Position, _e.Range).FirstOrDefault();
            var useItemsl = getMenuBoolItem(clearMenu, "UseItemslane");
            var useQl = getMenuBoolItem(clearMenu, "UseQL");
            var useWl = getMenuBoolItem(clearMenu, "UseWL");
            var useEl = getMenuBoolItem(clearMenu, "UseEL");
            var save = getMenuBoolItem(clearMenu, "LaneSave");
            if (minions == null) return;
            if (_player.Mana <= 4)
            {
                if (_q.IsReady() && useQl && minions.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useWl && minions.IsValidTarget(_w.Range))
                {
                    _w.Cast();
                }

                if (_e.IsReady() && useEl && minions.IsValidTarget(_e.Range))
                {
                    _e.Cast(minions);
                }
            }

            if (_player.Mana == 5)
            {
                if (save) return;
                if (_q.IsReady() && getBoxItem(clearMenu, "LanePrio") == 0 && useQl
                    && minions.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && getBoxItem(clearMenu, "LanePrio") == 1 && useWl
                    && minions.IsValidTarget(_w.Range))
                {
                    _w.Cast();
                }

                if (_e.IsReady() && getBoxItem(clearMenu, "LanePrio") == 2 && useEl
                    && minions.IsValidTarget(_e.Range))
                {
                    _e.Cast(minions);
                }
            }

            if (useItemsl && _tiamat.IsReady && minions.IsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsl && _hydra.IsReady && minions.IsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }
        }

        private static void JungleClear()
        {

            var mob = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(_e.Range));

            var useItemsJ = getMenuBoolItem(jungleMenu, "UseItemsjungle");
            var useQ = getMenuBoolItem(jungleMenu, "UseQJ");
            var useW = getMenuBoolItem(jungleMenu, "UseWJ");
            var useE = getMenuBoolItem(jungleMenu, "UseEJ");
            var save = getMenuBoolItem(jungleMenu, "JungleSave");
            if (mob == null)
            {
                return;
            }

            if (_player.Mana <= 4)
            {
                if (useQ && _q.IsReady() && mob.IsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useW && mob.IsValidTarget(_w.Range - 100) && !_player.HasBuff("rengarpassivebuff"))
                {
                    _w.Cast();
                }

                if (useItemsJ && _tiamat.IsReady && mob.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (useItemsJ && _hydra.IsReady && mob.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }

                if (_e.IsReady() && useE && mob.IsValidTarget(_e.Range))
                {
                    _e.Cast(mob);
                }
            }

            if (_player.Mana != 5 || save)
            {
                return;
            }

            if (mob.IsValidTarget(_q.Range) && _q.IsReady()
                && getBoxItem(jungleMenu, "JunglePrio") == 0 && useQ)
            {
                _q.Cast();
            }

            if (mob.IsValidTarget(_w.Range) && _w.IsReady()
                && getBoxItem(jungleMenu, "JunglePrio") == 1 && useW
                && !_player.HasBuff("rengarpassivebuff"))
            {
                _w.Cast();
            }

            if (useItemsJ && _tiamat.IsReady && mob.IsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsJ && _hydra.IsReady && mob.IsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }

            if (mob.IsValidTarget(_e.Range) && _e.IsReady()
                && getBoxItem(jungleMenu, "JunglePrio") == 2 && useE)
            {
                _e.Cast(mob.Position);
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, _e.Range);
            var useQ = getMenuBoolItem(lasthitMenu, "UseQLH");
            var useW = getMenuBoolItem(lasthitMenu, "UseWLH");
            var useE = getMenuBoolItem(lasthitMenu, "UseELH");
            var save = getMenuBoolItem(lasthitMenu, "LastSave");
            foreach (var minion in allMinions)
            {
                if (_player.Mana <= 4)
                {
                    if (useQ && _q.IsReady() && _player.Distance(minion) < _q.Range
                        && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        _q.Cast();
                    }

                    if (_w.IsReady() && useW && _player.Distance(minion) < _w.Range
                        && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W))
                    {
                        _w.Cast();
                    }

                    if (_e.IsReady() && useE && _player.Distance(minion) < _e.Range
                        && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                    {
                        _e.Cast(minion);
                    }
                }

                if (_player.Mana != 5 || save)
                {
                    return;
                }

                if (useQ && _q.IsReady() && _player.Distance(minion) < _q.Range
                    && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q)
                    && getBoxItem(lasthitMenu, "LastPrio") == 0)
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useW && _player.Distance(minion) < _w.Range
                    && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W)
                    && getBoxItem(lasthitMenu, "LastPrio") == 1)
                {
                    _w.Cast();
                }

                if (_e.IsReady() && useE && _player.Distance(minion) < _e.Range
                    && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E)
                    && getBoxItem(lasthitMenu, "LastPrio") == 2)
                {
                    _e.Cast(minion);
                }
            }
        }

        private static void AutoHeal()
        {
            var health = (100 * (_player.Health / _player.MaxHealth)) < getMenuSliderItem(miscMenu, "AutoWHP");

            if (_player.HasBuff("Recall") || _player.Mana <= 4) return;


            if (_w.IsReady() && health)
            {
                _w.Cast();
            }
        }

        private static void UseItemes()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var iBilge = getMenuBoolItem(itemMenu, "Bilge");
                var iBilgeEnemyhp = hero.Health
                                    <= (hero.MaxHealth * (getMenuSliderItem(itemMenu, "BilgeEnemyhp")) / 100);
                var iBilgemyhp = _player.Health
                                 <= (_player.MaxHealth * (getMenuSliderItem(itemMenu, "Bilgemyhp")) / 100);
                var iBlade = getMenuBoolItem(itemMenu, "Blade");
                var iBladeEnemyhp = hero.Health
                                    <= (hero.MaxHealth * (getMenuSliderItem(itemMenu, "BladeEnemyhp")) / 100);
                var iBlademyhp = _player.Health
                                 <= (_player.MaxHealth * (getMenuSliderItem(itemMenu, "Blademyhp")) / 100);
                var iYoumuu = getMenuBoolItem(itemMenu, "Youmuu");
                // var iTiamat = _config.Item("Tiamat").GetValue<bool>();
                // var iHydra = _config.Item("Hydra").GetValue<bool>();
                if (hero.IsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady)
                {
                    _bilge.Cast(hero);
                }

                if (hero.IsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady)
                {
                    _blade.Cast(hero);
                }

                /* if (iTiamat && _tiamat.IsReady() && hero.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();

                }
                if (iHydra && _hydra.IsReady() && hero.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();

                }*/

            }

        }
    }
}