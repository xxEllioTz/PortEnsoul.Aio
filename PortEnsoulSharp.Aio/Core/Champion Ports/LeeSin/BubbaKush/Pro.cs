namespace BubbaKushs
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BubbaKushs.Plu;
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using xxEliot.Core;

    #endregion

    internal class Pro
    {
        #region Constants

        internal const int FlashRange = 425, IgniteRange = 600, SmiteRange = 570;

        #endregion

        #region Static Fields

        internal static Items.Item Bilgewater, BotRuinedKing, Youmuu, Tiamat, Ravenous, Titanic;

        internal static SpellSlot Flash = SpellSlot.Unknown, Ignite = SpellSlot.Unknown, Smite = SpellSlot.Unknown;

        internal static Menu MainMenu;

        internal static AIHeroClient player;

        internal static Spell Q, Q2, Q3, W, W2, E, E2, R, R2, R3;

        private static readonly Dictionary<string, Tuple<Func<object>, int>> Plugins =
            new Dictionary<string, Tuple<Func<object>, int>>
                {
                    { "LeeSin", new Tuple<Func<object>, int>(() => new LeeSin(), 13) }
                };

        private static bool isSkinReset = true;
        #endregion

        #region Methods

        private static void CheckVersion()
        {
        }

        private static void InitItem()
        {
            Bilgewater = new Items.Item(ItemId.Bilgewater_Cutlass, 550);
            BotRuinedKing = new Items.Item(ItemId.Blade_of_the_Ruined_King, 550);
            Youmuu = new Items.Item(ItemId.Youmuus_Ghostblade, 0);
            Tiamat = new Items.Item(ItemId.Tiamat_Melee_Only, 400);
            Ravenous = new Items.Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Items.Item(3748, 0);
        }

        private static void InitMenu(bool isSupport)
        {
            MainMenu = new Menu("EnsoulSharp", "EnsoulLeeSin", true, player.CharacterName).Attach();
            MainMenu.Separator("Author: xxEliot");
            MainMenu.Separator("fb: fb.me/abcren0");
            if (isSupport)
            {
                var skinMenu = MainMenu.Add(new Menu("Skin", "Skin Changer"));
                {
                    skinMenu.Slider("Index", "Skin", 0, 0, Plugins[player.CharacterName].Item2).ValueChanged +=
                        (sender, args) => { isSkinReset = true; };
                    skinMenu.Bool("Own", "Keep Your Own Skin").ValueChanged += (sender, args) =>
                    {
                        var menuBool = sender as MenuBool;
                        if (menuBool != null)
                        {
                            isSkinReset = false;
                        }
                    };
                }
                Plugins[player.CharacterName].Item1.Invoke();

                Game.OnUpdate += args =>
                {
                    if (player.IsDead)
                    {
                        if (!isSkinReset)
                        {
                            isSkinReset = true;
                        }
                    }
                    else if (isSkinReset)
                    {
                        isSkinReset = false;
                        if (player.SkinID == 0 || !MainMenu["Skin"]["Own"])
                        {
                            player.SetSkin(MainMenu["Skin"]["Index"].GetValue<MenuSlider>().Value);
                        }
                    }
                };
            }
        }

        private static void InitSummonerSpell()
        {
            foreach (var smite in
                player.Spellbook.Spells.Where(
                    i =>
                    (i.Slot == SpellSlot.Summoner1 || i.Slot == SpellSlot.Summoner2)
                    && i.Name.ToLower().Contains("smite")))
            {
                Smite = smite.Slot;
                break;
            }
            Ignite = player.GetSpellSlot("SummonerDot");
            Flash = player.GetSpellSlot("SummonerFlash");
        }
        public static void Game_OnGameLoad()
        {
            Bootstrap.Init(null);
            player = GameObjects.Player;
            CheckVersion();
            var isSupport = Plugins.ContainsKey(player.CharacterName);
            InitMenu(isSupport);
            InitItem();
            InitSummonerSpell();
        }

        private static void PrintGame(string text)
        {
            Game.Print("New LeeSin xxEliot Loaded");
        }

        #endregion

    }
}
