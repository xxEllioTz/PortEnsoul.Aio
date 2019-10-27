using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using Color = System.Drawing.Color;

namespace LeeSin
{
    class Program
    {
        private const string ChampionName = "LeeSin";
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell _q, _w, _e, _r;
        public static Menu TargetSelectorMenu;
        public static Menu _config;
        private static AIHeroClient _player;
        private static AIBaseClient insobj;
        private static SpellSlot _igniteSlot;
        private static SpellSlot _flashSlot;
        private static SpellSlot _smitedmgSlot;
        private static SpellSlot _smitehpSlot;
        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _youmuu;
        public static Vector3 WardCastPosition;
        private static Vector3 insdirec;
        private static Vector3 insecpos;
        private static Vector3 movepoint;
        private static Vector3 jumppoint;
        private static Vector3 wpos;
        private static Vector3 wallcheck;
        private static Vector3 firstpos;
        public static int canmove = 1;
        private static int instypecheck;
        private static float wardtime;
        private static float inscount;
        private static float counttime;
        private static float qcasttime;
        private static float q2casttime;
        private static float wcasttime;
        private static float ecasttime;
        private static float casttime;
        private static bool walljump;
        private static bool checker;
        private static int bCount;

        public static void Game_OnGameLoad()
        {
            try
            {
                _player = ObjectManager.Player;
                if (ObjectManager.Player.CharacterName != ChampionName) return;
                _q = new Spell(SpellSlot.Q, 1100f);
                _w = new Spell(SpellSlot.W, 700f);
                _e = new Spell(SpellSlot.E, 425f);
                _r = new Spell(SpellSlot.R, 375f);
                _q.SetSkillshot(0.25f, 70f, 1800f, true, false, SkillshotType.Line);

                _bilge = new Items.Item(3144, 475f);
                _blade = new Items.Item(3153, 425f);
                _hydra = new Items.Item(3074, 250f);
                _tiamat = new Items.Item(3077, 250f);
                _rand = new Items.Item(3143, 490f);
                _lotis = new Items.Item(3190, 590f);
                _youmuu = new Items.Item(3142, 10);

                _igniteSlot = _player.GetSpellSlot("SummonerDot");
                _flashSlot = _player.GetSpellSlot("SummonerFlash");
                _smitedmgSlot = _player.GetSpellSlot(SmitetypeDmg());
                _smitehpSlot = _player.GetSpellSlot(SmitetypeHp());

                _config = new Menu("LeeSinE#", "LeeSinE#", true);

                TargetSelectorMenu = new Menu("Target", "Target Selector");


                var _combo = new Menu("Combo", "Combo");
                _combo.Add(new MenuKeyBind("ActiveCombo", "Combo!", System.Windows.Forms.Keys.Space, KeyBindType.Press, "Space"));
                _combo.Add(new MenuBool("UseIgnitecombo", "Use Ignite(rush for it)", true));
                _combo.Add(new MenuBool("UseSmitecombo", "Use Smite(rush for it)", true));
                _combo.Add(new MenuBool("UseWcombo", "Use W", false));
                _config.Add(_combo);
                var _insc = new Menu("Insec", "Insec");
                _insc.Add(new MenuKeyBind("insc", "insec active", System.Windows.Forms.Keys.T, KeyBindType.Press, "T"));
                _insc.Add(new MenuBool("minins", "insec using minions?", false));
                _insc.Add(new MenuBool("fins", "flash if no wards", true));
                _config.Add(_insc);

                var _harass = new Menu("Harass", "Harass");
                _harass.Add(new MenuKeyBind("ActiveHarass", "Harass!", System.Windows.Forms.Keys.C, KeyBindType.Press));
                _harass.Add(new MenuBool("UseItemsharass", "Use Tiamat/Hydra", true));
                _harass.Add(new MenuBool("UseEHar", "Use E", true));
                _harass.Add(new MenuBool("UseQ1Har", "Use Q1 Harass", true));
                _harass.Add(new MenuBool("UseQ2Har", "Use Q2 Harass", true));
                _config.Add(_harass);

                var _item = new Menu("items", "items");
                _item.Add(new MenuSeparator("Offensive", "Offensive"));
                _item.Add(new MenuBool("Youmuu", "Use Youmuu's"));
                _item.Add(new MenuBool("Tiamat", "Use Tiamat"));
                _item.Add(new MenuBool("Hydra", "Use Hydra"));
                _item.Add(new MenuBool("Bilge", "Use Bilge"));
                _item.Add(new MenuSlider("BilgeEnemyhp", "If Enemy Hp <", 85, 1, 100));
                _item.Add(new MenuSlider("Bilgemyhp", "Or your Hp < ", 85, 1, 100));
                _item.Add(new MenuBool("Blade", "Use Blade"));
                _item.Add(new MenuSlider("BladeEnemyhp", "If Enemy Hp <", 85, 1, 100));
                _item.Add(new MenuSlider("Blademyhp", "Or Your  Hp <", 85, 1, 100));
                _item.Add(new MenuSeparator("Deffensive", "Deffensive"));
                _item.Add(new MenuBool("Omen", "Use Randuin Omen"));
                _item.Add(new MenuSlider("Omenenemys", "Randuin if enemys>", 2, 1, 5));
                _item.Add(new MenuBool("lotis", "Use Iron Solari"));
                _item.Add(new MenuSlider("lotisminhp", "Solari if Ally Hp<", 35, 1, 100));
                _config.Add(_item);
                //Farm
                var _farm = new Menu("Farm", "Farm");
                _farm.Add(new MenuSeparator("LaneFarm", "LaneFarm"));
                _farm.Add(new MenuBool("UseItemslane", "Use Tiamat/Hydra", true));
                _farm.Add(new MenuBool("UseQL", "Q LaneClear"));
                _farm.Add(new MenuBool("UseEL", "E LaneClear"));
                _farm.Add(new MenuKeyBind("Activelane", "Lane clear!", System.Windows.Forms.Keys.V, KeyBindType.Press));
                _farm.Add(new MenuSeparator("UseQLH", "Q LastHit"));
                _farm.Add(new MenuKeyBind("Activelast", "LastHit!", System.Windows.Forms.Keys.X, KeyBindType.Press));
                _farm.Add(new MenuSeparator("Jungle", "Jungle"));
                _farm.Add(new MenuBool("UseItemsjungle", "Use Tiamat/Hydra", true));
                _farm.Add(new MenuBool("UseQJ", "Q Jungle"));
                _farm.Add(new MenuBool("UseWJ", "W Jungle"));
                _farm.Add(new MenuBool("UseEJ", "E Jungle"));
                _farm.Add(new MenuBool("PriW", "W > E ? (off E > W)"));
                _farm.Add(new MenuKeyBind("Activejungle", "Jungle!", System.Windows.Forms.Keys.V, KeyBindType.Press));
                _config.Add(_farm);
                //Misc
                var _misc = new Menu("Misc", "Misc");
                _misc.Add(new MenuBool("UseIgnitekill", "Use Ignite KillSteal", true));
                _misc.Add(new MenuBool("UseEM", "Use E KillSteal", true));
                _misc.Add(new MenuBool("UseRM", "Use R KillSteal", true));
                _misc.Add(new MenuKeyBind("wjump", "ward jump", System.Windows.Forms.Keys.G, KeyBindType.Press));
                _misc.Add(new MenuBool("wjmax", "ward jump max range?", false));
                _config.Add(_misc);


                //Drawings
                var _draw = new Menu("Drawings", "Drawings");
                _draw.Add(new MenuBool("DrawQ", "Draw Q", true));
                _draw.Add(new MenuBool("DrawE", "Draw E", true));
                _draw.Add(new MenuBool("DrawR", "Draw R", true));
                _draw.Add(new MenuBool("damagetest", "Damage Text", true));
                _draw.Add(new MenuBool("CircleLag", "Lag Free Circles", true));
                _draw.Add(new MenuSlider("CircleQuality", "Circles Quality", 100, 100, 10));
                _draw.Add(new MenuSlider("CircleThickness", "Circles Thickness", 1, 10, 1));
                _config.Add(_draw);
                new AssassinManager();
                _config.Add(TargetSelectorMenu);
                _config.Attach();
                new DamageIndicator();

                DamageIndicator.DamageToUnit = ComboDamage;


                Drawing.OnDraw += Drawing_OnDraw;
                Tick.OnTick += Game_OnUpdate;
                AIBaseClient.OnProcessSpellCast += OnProcessSpell;
                Game.OnWndProc += OnWndProc;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.Print("Error something went wrong");
            }

        }
        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    break;
            }
            if (_config["Combo"]["ActiveCombo"].GetValue<MenuKeyBind>().Active)
            {
                Combo(GetEnemy);

            }
            if (_config["Misc"]["wjump"].GetValue<MenuKeyBind>().Active)
            {
                wjumpflee();
            }

            if (_config["Insec"]["insc"].GetValue<MenuKeyBind>().Active)
            {
                Insec(GetEnemy);

            }

            if (_config["Harass"]["ActiveHarass"].GetValue<MenuKeyBind>().Active)
            {
                Harass(GetEnemy);
            }
            if (_config["Farm"]["Activejungle"].GetValue<MenuKeyBind>().Active)
            {
                JungleClear();
            }
            //if (_config["Farm"]["Activelane"].GetValue<MenuKeyBind>().Active)
            //{
            //    LaneClear();
            //}
            if (_config["Farm"]["Activelast"].GetValue<MenuKeyBind>().Active)
            {
                LastHit();
            }
        }

        private static void OnWndProc(GameWndProcEventArgs args)
        {
            if (args.Msg == 515 || args.Msg == 513)
            {
                if (args.Msg == 515)
                {
                    insdirec = Game.CursorPos;
                    instypecheck = 1;
                }
                var boohoo = ObjectManager.Get<AIBaseClient>()
                         .OrderBy(obj => obj.Distance(_player.Position))
                         .FirstOrDefault(
                             obj =>
                                 obj.IsAlly && !obj.IsMe && !obj.IsMinion &&
                                  Game.CursorPos.Distance(obj.Position) <= 150);
                if (args.Msg == 513 && boohoo != null)
                {
                    insobj = boohoo;
                    instypecheck = 2;
                }

            }
        }

        private static void OnProcessSpell(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                casttime = Environment.TickCount;
            }

            if (sender.IsAlly || !sender.Type.Equals(GameObjectType.AIHeroClient) ||
                (((AIHeroClient)sender).CharacterName != "MonkeyKing" && ((AIHeroClient)sender).CharacterName != "Akali") ||
                sender.Position.Distance(_player.Position) >= 425 ||
                !_e.IsReady())
            {
                return;
            }
            if (args.SData.Name == "MonkeyKingDecoy" || args.SData.Name == "AkaliSmokeBomb")
            {
                _e.Cast();
            }

        }

        private static float ComboDamage(AIBaseClient enemy)
        {
            var damage = 0d;
            //if (_igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            //    damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, SummonerSpell.Ignite);
            //if (_smitedmgSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_smitedmgSlot) == SpellState.Ready)
            //    damage += 20 + 8 * _player.Level;
            //if (Items.HasItem(_player, 3077) && Items.CanUseItem(_player, 3077))
            //    damage += GetItemDamage(_player, enemy, ItemId.Tiamat);
            //if (Items.HasItem(_player, 3074) && Items.CanUseItem(_player, 3074))
            //    damage += GetItemDamage(_player, enemy, ItemId.Ravenous_Hydra);
            //if (Items.HasItem(_player, 3153) && Items.CanUseItem(_player, 3153))
            //    damage += GetItemDamage(_player, enemy, ItemId.Blade_of_the_Ruined_King);
            //if (Items.HasItem(_player, 3144) && Items.CanUseItem(_player, 3144))
            //    damage += GetItemDamage(_player, enemy, ItemId.Bilgewater_Cutlass);

            if (QStage == QCastStage.First)
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q) * 2;
            if (EStage == ECastStage.First)
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);

            return (float)damage;
        }

        private static bool Passive()
        {
            if (_player.HasBuff("blindmonkpassive_cosmetic"))
            {
                return true;
            }
            else
                return false;
        }

        private static void Combo(AIHeroClient t)
        {
            if (t == null) return;
            if (_config["Combo"]["UseIgnitecombo"].GetValue<MenuBool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, t);

                }
            }
            if (_config["Combo"]["UseWcombo"].GetValue<MenuBool>() && t.Distance(_player.Position) <= Extensions.GetRealAutoAttackRange(_player))
            {
                if (WStage == WCastStage.First || !Passive())
                    CastSelfW();
                if (WStage == WCastStage.Second && (!Passive() || Environment.TickCount > wcasttime + 2500))
                    _w.Cast();
            }

            if (_config["Combo"]["UseSmitecombo"].GetValue<MenuBool>() && _smitedmgSlot != SpellSlot.Unknown &&
                _player.Spellbook.CanUseSpell(_smitedmgSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.Spellbook.CastSpell(_smitedmgSlot, t);
                }
            }


            if (t.IsValidTarget() && _q.IsReady() && t.Distance(_player.Position) < 1100)
            {
                CastQ1(t);
            }
            if (t.HasBuff("BlindMonkQOne") || t.HasBuff("BlindMonkQTwo") && (ComboDamage(t) > t.Health || t.Distance(_player.Position) > 350 || Environment.TickCount > qcasttime + 2500))
                _q.Cast();

            CastECombo();
            //UseItemes(t);


        }
        private static void Harass(AIHeroClient t)
        {
            if (t == null) return;
            var jumpObject = ObjectManager.Get<AIBaseClient>()
               .OrderBy(obj => obj.Distance(firstpos))
               .FirstOrDefault(obj =>
                   obj.IsAlly && !obj.IsMe &&
                   !(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                   obj.Distance(t.Position) < 550);
            if (_config["Harass"]["UseEHar"].GetValue<MenuBool>())
                CastECombo();
            if (_config["Harass"]["UseQ1Har"].GetValue<MenuBool>())
                CastQ1(t);
            if (_config["Harass"]["UseQ2Har"].GetValue<MenuBool>() && (t.HasBuff("BlindMonkQOne") || t.HasBuff("BlindMonkQTwo")) && jumpObject != null && WStage == WCastStage.First)
            {
                _q.Cast();
                q2casttime = Environment.TickCount;
            }
            if (_player.Distance(t.Position) < 300 && !_q.IsReady() && q2casttime + 2500 > Environment.TickCount && Environment.TickCount > q2casttime + 500)
                CastW(jumpObject);

            var useItemsH = _config["Harass"]["UseItemsharass"].GetValue<MenuBool>();

            if (useItemsH && _tiamat.IsReady && t.Distance(_player.Position) < _tiamat.Range)
            {
                _tiamat.Cast();
            }
            if (useItemsH && _hydra.IsReady && t.Distance(_player.Position) < _hydra.Range)
            {
                _hydra.Cast();
            }
        }

        public static void WardJump(Vector3 pos, bool useWard = true, bool checkObjects = true, bool fullRange = false)
        {
            if (WStage != WCastStage.First)
            {
                return;
            }
            pos = fullRange ? _player.Position.ToVector2().Extend(pos.ToVector2(), 600).ToVector3() : pos;
            WardCastPosition = NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall)
                ? _player.GetPath(pos).Last()
                : pos;
            var jumpObject =
                ObjectManager.Get<AIBaseClient>()
                    .OrderBy(obj => obj.Distance(_player.Position))
                    .FirstOrDefault(
                        obj =>
                            obj.IsAlly && !obj.IsMe &&
                            (!(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                             Vector3.DistanceSquared(pos, obj.Position) <= 150 * 150));
            if (jumpObject != null && checkObjects && WStage == WCastStage.First)
            {
                CastW(jumpObject);
                return;
            }
            if (!useWard)
            {
                return;
            }

            if (Items.GetWardSlot(ObjectManager.Player) == null || Items.GetWardSlot(ObjectManager.Player).Id == 0)
            {
                return;
            }
            placeward(WardCastPosition);
        }

        private static void placeward(Vector3 castpos)
        {
            if (WStage != WCastStage.First || Environment.TickCount < wardtime + 2000)
            {
                return;
            }
            var ward = Items.GetWardSlot(ObjectManager.Player);
            _player.Spellbook.CastSpell(ward.SpellSlot, castpos);
            wardtime = Environment.TickCount;
        }

        private static void wjumpflee()
        {
            if (WStage != WCastStage.First)
                _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            else
            {
                if (_config["Misc"]["wjmax"].GetValue<MenuBool>())
                {
                    _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    WardJump(Game.CursorPos, true, true, true);
                }
                else if (_player.Distance(Game.CursorPos) >= 700 || walljump == true)
                {
                    if (Game.CursorPos.Distance(wallcheck) > 50)
                    {
                        walljump = false;
                        checker = false;
                        for (var i = 0; i < 40; i++)
                        {
                            var p = Game.CursorPos.Extend(_player.Position, 10 * i);
                            if (NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Wall))
                            {
                                jumppoint = p;
                                wallcheck = Game.CursorPos;
                                walljump = true;
                                break;


                            }
                        }
                        if (walljump == true)
                        {
                            foreach (
                              var qPosition in
                                GetPossibleJumpPositions(jumppoint)
                                .OrderBy(qPosition => qPosition.Distance(jumppoint)))
                            {
                                if (_player.Position.Distance(qPosition) < _player.Position.Distance(jumppoint))
                                {
                                    movepoint = qPosition;
                                    if (movepoint.Distance(jumppoint) > 600)
                                        wpos = movepoint.Extend(jumppoint, 595);
                                    else
                                        wpos = jumppoint;

                                    break;
                                }
                                if (qPosition == null)
                                    movepoint = jumppoint;
                                checker = true;
                                break;
                            }


                        }
                    }
                    var jumpObj = ObjectManager.Get<AIBaseClient>()
                         .OrderBy(obj => obj.Distance(_player.Position))
                         .FirstOrDefault(obj => obj.IsAlly && !obj.IsMe && obj.Distance(movepoint) <= 700 &&
                             (!(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                             obj.Distance(jumppoint) <= 200));
                    if (walljump == false || movepoint.Distance(Game.CursorPos) > _player.Distance(Game.CursorPos) + 150)
                    {
                        movepoint = Game.CursorPos;
                        jumppoint = Game.CursorPos;

                    }
                    if (jumpObj == null && Items.GetWardSlot(ObjectManager.Player) != null && Items.GetWardSlot(ObjectManager.Player).Id != 0)
                        placeward(wpos);
                    if (_player.Position.Distance(jumppoint) <= 700 && jumpObj != null)
                    {
                        CastW(jumpObj);
                        walljump = false;
                    }
                    _player.IssueOrder(GameObjectOrder.MoveTo, movepoint);

                }
                else
                    WardJump(jumppoint, true, true, false);
            }
        }

        private static IEnumerable<Vector3> GetPossibleJumpPositions(Vector3 pos)
        {
            var pointList = new List<Vector3>();

            for (var j = 680; j >= 50; j -= 50)
            {
                var offset = (int)(2 * Math.PI * j / 50);

                for (var i = 0; i <= offset; i++)
                {
                    var angle = i * Math.PI * 2 / offset;
                    var point = new Vector3((float)(pos.X + j * Math.Cos(angle)),
                        (float)(pos.Y - j * Math.Sin(angle)),
                        pos.Z);

                    if (!NavMesh.GetCollisionFlags(point).HasFlag(CollisionFlags.Wall) && point.Distance(_player.Position) < pos.Distance(_player.Position) - 400 &&
                        point.Distance(pos.Extend(_player.Position, 600)) <= 250)
                        pointList.Add(point);
                }
            }

            return pointList;
        }

        private static void Insec(AIHeroClient t)
        {
            if (t == null) return;

            if (insobj != null && instypecheck == 2)
            {
                insdirec = insobj.Position;
            }
            if (t.Position.Distance(insdirec) + 100 <= _player.Position.Distance(insdirec) && _r.IsReady())
            {
                _r.CastOnUnit(t);

                inscount = Environment.TickCount;
                canmove = 1;


            }
            if (canmove == 1)
            {
                _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }



            if (!_r.IsReady() || ((Items.GetWardSlot(ObjectManager.Player) == null || Items.GetWardSlot(ObjectManager.Player).Id == 0 || WStage != WCastStage.First) && _player.Spellbook.CanUseSpell(_flashSlot) == SpellState.Cooldown))
            {
                canmove = 1;
                return;
            }


            insecpos = t.Position.Extend(insdirec, -300);
            if (t == null) return;
            if ((_player.Position.Distance(insecpos) > 550 || inscount + 500 > Environment.TickCount) && QStage == QCastStage.First)
            {

                var qpred = _q.GetPrediction(t);
                if (qpred.Hitchance >= HitChance.Medium)
                    _q.Cast(t);
                if (qpred.Hitchance == HitChance.Collision && _config["Insec"]["minins"].GetValue<MenuBool>().Enabled)
                {
                    var enemyqtry = ObjectManager.Get<AIBaseClient>().Where(enemyq => (enemyq.IsValidTarget() || (enemyq.IsMinion && enemyq.IsEnemy)) && enemyq.Distance(insecpos) < 500);
                    foreach (
                        var enemyhit in enemyqtry.OrderBy(enemyhit => enemyhit.Distance(insecpos)))
                    {

                        if (_q.GetPrediction(enemyhit).Hitchance >= HitChance.Medium && enemyhit.Distance(insecpos) < 500 && _player.GetSpellDamage(enemyhit, SpellSlot.Q) < enemyhit.Health)
                            _q.Cast(enemyhit);
                    }
                }
            }
            if (QStage == QCastStage.Second)
            {
                var enemy = ObjectManager.Get<AIBaseClient>().FirstOrDefault(unit => unit.IsEnemy && (unit.HasBuff("BlindMonkQOne") || unit.HasBuff("blindmonkqonechaos")));
                if (enemy.Position.Distance(insecpos) < 600)
                {
                    _q.Cast();
                    canmove = 0;
                }
            }
            if (_player.Position.Distance(insecpos) < 600)
            {
                if ((Items.GetWardSlot(ObjectManager.Player) == null || Items.GetWardSlot(ObjectManager.Player).Id == 0 || WStage != WCastStage.First) && _config["Insec"]["fins"].GetValue<MenuBool>().Enabled &&
                    _player.Spellbook.CanUseSpell(_flashSlot) == SpellState.Ready && _player.Position.Distance(t.Position) < _r.Range && Environment.TickCount > counttime + 3000)
                {
                    _r.CastOnUnit(t);
                    DelayAction.Add(Game.Ping + 125, () => _player.Spellbook.CastSpell(_flashSlot, insecpos));
                    canmove = 0;
                }
                else
                    WardJump(insecpos, true, true, false);
                counttime = Environment.TickCount;
                canmove = 0;
            }

        }

        static AIHeroClient GetEnemy
        {
            get
            {
                var assassinRange = TargetSelectorMenu["AssassinSearchRange"].GetValue<MenuSlider>().Value;

                var vEnemy = ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                            enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                            TargetSelectorMenu["Assassin" + enemy.CharacterName] != null &&
                            TargetSelectorMenu["Assassin" + enemy.CharacterName].GetValue<MenuBool>().Enabled &&
                            ObjectManager.Player.Distance(enemy.Position) < assassinRange);

                if (TargetSelectorMenu["AssassinSelectOption"].GetValue<MenuList>().Index == 1)
                {
                    vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
                }

                AIHeroClient[] objAiHeroes = vEnemy as AIHeroClient[] ?? vEnemy.ToArray();

                AIHeroClient t = !objAiHeroes.Any()
                    ? TargetSelector.GetTarget(1400, DamageType.Magical)
                    : objAiHeroes[0];

                return t;

            }
        }
        internal enum QCastStage
        {
            First,
            Second,
            Cooldown
        }

        internal enum ECastStage
        {
            First,
            Second,
            Cooldown
        }

        internal enum WCastStage
        {
            First,
            Second,
            Cooldown
        }

        private static QCastStage QStage
        {
            get
            {
                if (!_q.IsReady()) return QCastStage.Cooldown;

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne"
                    ? QCastStage.First
                    : QCastStage.Second);

            }
        }

        private static ECastStage EStage
        {
            get
            {
                if (!_e.IsReady()) return ECastStage.Cooldown;

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Name == "BlindMonkEOne"
                    ? ECastStage.First
                    : ECastStage.Second);

            }
        }

        private static WCastStage WStage
        {
            get
            {
                if (!_w.IsReady()) return WCastStage.Cooldown;

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWTwo"
                    ? WCastStage.Second
                    : WCastStage.First);

            }
        }

        private static void CastSelfW()
        {
            if (500 >= Environment.TickCount - wcasttime || WStage != WCastStage.First) return;

            _w.Cast();
            wcasttime = Environment.TickCount;

        }

        private static void CastW(AIBaseClient obj)
        {
            if (500 >= Environment.TickCount - wcasttime || WStage != WCastStage.First) return;

            _w.CastOnUnit(obj);
            wcasttime = Environment.TickCount;

        }

        private static void CastECombo()
        {
            if (!_e.IsReady()) return;
            if (ObjectManager.Get<AIHeroClient>()
                .Count(
                    hero =>
                        hero.IsValidTarget() &&
                        hero.Distance(ObjectManager.Player.Position) <= _e.Range) > 0)
            {
                CastE1();
            }
            if (EStage == ECastStage.Second && ((Environment.TickCount > casttime + 200 && !Passive()) || Environment.TickCount > ecasttime + 2700))
                _e.Cast();
        }

        private static void CastE1()
        {
            if (500 >= Environment.TickCount - ecasttime || EStage != ECastStage.First) return;
            _e.Cast();
            ecasttime = Environment.TickCount;
        }
        private static void CastQ1(AIBaseClient target)
        {
            if (QStage != QCastStage.First) return;
            var qpred = _q.GetPrediction(target);
            if (qpred.Hitchance >= HitChance.Medium && qpred.CastPosition.Distance(_player.Position) < 1100)
            {
                _q.Cast(target);
                firstpos = _player.Position;
                qcasttime = Environment.TickCount;
            }
        }

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        private static string SmitetypeDmg()
        {
            if (SmiteBlue.Any(a => Items.HasItem(_player, a)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(a => Items.HasItem(_player, a)))
            {
                return "s5_summonersmiteduel";

            }
            return "summonersmite";
        }
        private static string SmitetypeHp()
        {
            if (SmitePurple.Any(a => Items.HasItem(_player, a)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        private static void UseItemes(AIHeroClient target)
        {
            var iBilge = _config["items"]["Bilge"].GetValue<MenuBool>();
            var iBilgeEnemyhp = target.Health <=
                                (target.MaxHealth * (_config["items"]["BilgeEnemyhp"].GetValue<MenuSlider>().Value) / 100);
            var iBilgemyhp = _player.Health <=
                             (_player.MaxHealth * (_config["items"]["BilgeEnemyhp"]["Bilgemyhp"].GetValue<MenuSlider>().Value) / 100);
            var iBlade = _config["items"]["Blade"].GetValue<MenuBool>();
            var iBladeEnemyhp = target.Health <=
                                (target.MaxHealth * (_config["items"]["BladeEnemyhp"].GetValue<MenuSlider>().Value) / 100);
            var iBlademyhp = _player.Health <=
                             (_player.MaxHealth * (_config["items"]["Blademyhp"].GetValue<MenuSlider>().Value) / 100);
            var iOmen = _config["items"]["Omen"].GetValue<MenuBool>();
            var iOmenenemys = ObjectManager.Get<AIHeroClient>().Count(hero => hero.IsValidTarget(450)) >=
                              _config["items"]["Omenenemys"].GetValue<MenuSlider>().Value;
            var iTiamat = _config["items"]["Tiamat"].GetValue<MenuBool>();
            var iHydra = _config["items"]["Hydra"].GetValue<MenuBool>();
            var ilotis = _config["items"]["lotis"].GetValue<MenuBool>();
            var iYoumuu = _config["items"]["Youmuu"].GetValue<MenuBool>();

            if (_player.Distance(target.Position) <= 450 && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady)
            {
                _bilge.Cast(target);

            }
            if (_player.Distance(target.Position) <= 450 && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady)
            {
                _blade.Cast(target);

            }
            if (_player.Distance(target.Position) <= 300 && iTiamat && _tiamat.IsReady)
            {
                _tiamat.Cast();

            }
            if (_player.Distance(target.Position) <= 300 && iHydra && _hydra.IsReady)
            {
                _hydra.Cast();

            }
            if (iOmenenemys && iOmen && _rand.IsReady)
            {
                _rand.Cast();

            }
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (_config["items"]["lotisminhp"].GetValue<MenuSlider>().Value) / 100) &&
                        hero.Distance(_player.Position) <= _lotis.Range && _lotis.IsReady)
                        _lotis.Cast();
                }
            }
            if (_player.Distance(target.Position) <= 350 && iYoumuu && _youmuu.IsReady)
            {
                _youmuu.Cast();

            }
        }
        private static void LaneClear()
        {
            var allMinionsQ = GameObjects.GetMinions(ObjectManager.Player.Position, _q.Range);
            var allMinionsE = GameObjects.GetMinions(ObjectManager.Player.Position, _e.Range);
            var useItemsl = _config["Farm"]["UseItemslane"].GetValue<MenuBool>();
            var useQl = _config["Farm"]["UseQL"].GetValue<MenuBool>();
            var useEl = _config["Farm"]["UseEL"].GetValue<MenuBool>();
            if(!allMinionsQ.Any())
            {
                return;
            }
            if(!allMinionsE.Any())
            {
                return;
            }
            if (allMinionsQ.Count == 0)
                return;
            if (allMinionsE.Count == 0)
                return;
            if (EStage == ECastStage.Second && ((Environment.TickCount > casttime + 200 && !Passive()) || Environment.TickCount > ecasttime + 2700))
                _e.Cast();
            if (QStage == QCastStage.Second && (Environment.TickCount > qcasttime + 2700 || Environment.TickCount > casttime + 200 && !Passive()))
                _q.Cast();

            foreach (var minion in allMinionsQ)
            {
                if (!Extensions.InAutoAttackRange(minion) && useQl &&
                    minion.Health < _player.GetSpellDamage(minion, SpellSlot.Q) * 0.70)
                    _q.Cast(minion);
                else if (Extensions.InAutoAttackRange(minion) && useQl &&
                    minion.Health > _player.GetSpellDamage(minion, SpellSlot.Q) * 2)
                    CastQ1(minion);
            }



            if (_e.IsReady() && useEl)
            {
                if (allMinionsE.Count > 2)
                {
                    CastE1();
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!Extensions.InAutoAttackRange(minion) &&
                            minion.Health < 0.90 * _player.GetSpellDamage(minion, SpellSlot.E))
                            CastE1();
            }
            if (useItemsl && _tiamat.IsReady && allMinionsE.Count > 2)
            {
                _tiamat.Cast();
            }
            if (useItemsl && _hydra.IsReady && allMinionsE.Count > 2)
            {
                _hydra.Cast();
            }
        }
        private static void LastHit()
        {
            var allMinionsQ = GameObjects.GetMinions(_player.Position, _q.Range, MinionTypes.All, MinionTeam.Enemy);
            var useQ = _config["Farm"]["UseQLH"].GetValue<MenuBool>();
            foreach (var minion in allMinionsQ)
            {
                if (QStage == QCastStage.First && useQ && _player.Distance(minion.Position) < _q.Range &&
                    minion.Health < 0.90 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    CastQ1(minion);
                }
            }
        }

        private static void JungleClear()
        {
            var useItemsJ = _config["Farm"]["UseItemsjungle"].GetValue<MenuBool>();
            var useQ = _config["Farm"]["UseQJ"].GetValue<MenuBool>();
            var useW = _config["Farm"]["UseWJ"].GetValue<MenuBool>();
            var useE = _config["Farm"]["UseEJ"].GetValue<MenuBool>();

            var minions = ObjectManager.Get<AIBaseClient>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            var minionQ = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(_q.Range) && e.IsMinion()).Cast<AIBaseClient>().ToList(); ;

            if(!minions.Any())
            {
                return;
            }
            if(minionQ.Count == 0)
            {
                return;
            }
            foreach (var minion in minions)
            {
                if (useItemsJ && _tiamat.IsReady && _player.Distance(minion.Position) < _tiamat.Range)
                {
                    _tiamat.Cast();
                }
                if (useItemsJ && _hydra.IsReady && _player.Distance(minion.Position) < _hydra.Range)
                {
                    _hydra.Cast();
                }

                if (QStage == QCastStage.Second && (minion.Health < _q.GetDamage(minion) && ((minion.HasBuff("BlindMonkQOne") || minion.HasBuff("blindmonkqonechaos"))) || Environment.TickCount > qcasttime + 2700 || ((Environment.TickCount > casttime + 200 && !Passive()))))
                    _q.Cast(minion.Position);
                if (WStage == WCastStage.Second && ((Environment.TickCount > casttime + 200 && !Passive()) || Environment.TickCount > wcasttime + 2700))
                    _w.Cast();
                if (EStage == ECastStage.Second && ((Environment.TickCount > casttime + 200 && !Passive()) || Environment.TickCount > ecasttime + 2700))
                    _e.Cast();
                if (!Passive() && useQ && _q.IsReady() && Environment.TickCount > casttime + 200 || minion.Health < _q.GetDamage(minion) * 2)
                    CastQ1(minion);
                else if (!Passive() && _config["Farm"]["PriW"].GetValue<MenuBool>() && useW && _w.IsReady() && Environment.TickCount > casttime + 200)
                    CastSelfW();
                else if (!Passive() && useE && _e.IsReady() && minion.Distance(_player.Position) < _e.Range && Environment.TickCount > casttime + 200 || minion.Health < _e.GetDamage(minion))
                    CastE1();
            }

        }
        private static void KillSteal()
        {
            var enemyVisible =
                        ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && _player.Distance(enemy.Position) <= 600).FirstOrDefault();

            {
                if (_player.GetSummonerSpellDamage(enemyVisible, SummonerSpell.Ignite) > enemyVisible.Health && _igniteSlot != SpellSlot.Unknown &&
                _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, enemyVisible);
                }
            }
            if (_r.IsReady() && _config["Misc"]["UseRM"].GetValue<MenuBool>())
            {
                var t = TargetSelector.GetTarget(_r.Range, DamageType.Physical);
                if (_player.GetSpellDamage(t, SpellSlot.R) > t.Health && _player.Distance(t.Position) <= _r.Range)
                    _r.CastOnUnit(t);
            }


            if (_e.IsReady() && _config["Misc"]["UseEM"].GetValue<MenuBool>())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                if (_e.GetDamage(t) > t.Health && _player.Distance(t.Position) <= _e.Range)
                {
                    _e.Cast();
                }
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_config["Drawings"]["damagetest"].GetValue<MenuBool>())
            {
                foreach (
                     var enemyVisible in
                         ObjectManager.Get<AIHeroClient>().Where(enemyVisible => enemyVisible.IsValidTarget()))

                    if (ComboDamage(enemyVisible) > enemyVisible.Health)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40, Color.Red,
                            "Combo=Rekt");
                    }

            }

            if (_config["Drawings"]["CircleLag"].GetValue<MenuBool>())
            {
                if (_config["Drawings"]["DrawQ"].GetValue<MenuBool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Blue);
                }
                if (_config["Drawings"]["DrawE"].GetValue<MenuBool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White);

                }
                if (_config["Drawings"]["DrawR"].GetValue<MenuBool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.Blue);

                }
            }
            else
            {
                if (_config["Drawings"]["DrawQ"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.White);
                }
                if (_config["Drawings"]["DrawE"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White);

                }
                if (_config["Drawings"]["DrawR"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.White);

                }
            }
        }

        //public static double GetItemDamage(AIHeroClient source, AIBaseClient target, ItemId id)
        //{
        //    switch (id)
        //    {
        //        case ItemId.Bilgewater_Cutlass:
        //            return source.CalculateDamage(target, DamageType.Magical, 100);

        //        case ItemId.Blade_of_the_Ruined_King:
        //            return source.CalculateDamage(target, DamageType.Physical, target.MaxHealth * 0.1);
        //        case ItemId.Remnant_of_the_Watchers:
        //            return source.CalculateDamage(target, DamageType.Magical, 50 + 5 * source.Level);
        //        case ItemId.Hextech_Gunblade:
        //            return source.CalculateDamage(target, DamageType.Magical, 150 + 0.4 * source.FlatMagicDamageMod);
        //        case ItemId.Ravenous_Hydra:
        //            return source.CalculateDamage(
        //                target,
        //                DamageType.Physical,
        //                source.BaseAttackDamage + source.FlatPhysicalDamageMod);
        //        case ItemId.Tiamat:
        //            return source.CalculateDamage(
        //                target,
        //                DamageType.Physical,
        //                source.BaseAttackDamage + source.FlatPhysicalDamageMod);
        //        case ItemId.Liandrys_Torment:
        //            var d = target.Health * .2f * 3f;
        //            return (target.CanMove || target.HasBuff("slow")) ? d : d * 2;
        //    }
        //    return 1d;
        //}
    }
}
