using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace DeathGodX.AIO
{
    class Program
    {
        private static AIHeroClient Player => ObjectManager.Player;
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += GameEventOnOnGameLoad;
        }

        private static void GameEventOnOnGameLoad()
        {
            switch (Player.CharacterName)
            {
                case "Ekko":
                    Ekko.Program.EkkoOnLoadingComplete();
                    break;
                case "Cassiopeia":
                    Cassiopeia_Du_Couteau_2.Program.CassiopeiaLoading_OnLoadingComplete();
                    break;
                case "Lux":
                    ChewyMoonsLux.ChewyMoonsLux.LuxOnGameLoad();
                    break;
                case "Shaco":
                    ChewyMoonsShaco.ChewyMoonShaco.ShacoOnGameLoad();
                    break;
                case "Diana":
                    Diana___Bloody_Lunari.Program.DianaLoading_OnLoadingComplete();
                    break;
                case "Corki":
                    Corki7.Program.CorkiOnLoadingComplete();
                    break;
                case "Ezreal":
                    Ezreal.Program.EzrealOnLoadingComplete();
                    break;
                case "Graves":
                    Graves7.Program.GravesLoading_OnLoadingComplete();
                    break;
                case "Hecarim":
                    Hecarim7.Program.HecarimOnLoadingComplete();
                    break;
                case "Jax":
                    Jax.Program.JaxOnLoadingComplete();
                    break;
                case "Kassadin":
                    Kassadin7.Program.KassadinOnLoadingComplete();
                    break;
                case "KogMaw":
                    KogMaw.Program.KogMawOnLoadingComplete();
                    break;
                case "Malphite":
                    Malphite.Program.MalphiteOnLoadingComplete();
                    break;
                case "Olaf":
                    Olaf7.Program.OlafOnLoadingComplete();
                    break;
                case "Renekton":
                    Renekton7.Program.RenektonOnLoadingComplete();
                    break;
                case "Sejuani":
                    Sejuani7.Program.SejuaniOnLoadingComplete();
                    break;
                case "Talon":
                    Talon7.Program.TalonOnLoadingComplete();
                    break;
                case "Tristana":
                    Tristana.Program.TristanaOnLoadingComplete();
                    break;
                case "Trundle":
                    Trundle7.Program.TrundleOnLoadingComplete();
                    break;
                case "Tryndamere":
                    Tryndamere.Program.TryndamereOnLoadingComplete();
                    break;
                case "Twitch":
                    Twitch.Program.TwitchOnLoadingComplete();
                    break;
                case "Vladimir":
                    Vladimir.Program.VladimirOnLoadingComplete();
                    break;
                case "MonkeyKing":
                    Doctor_s_WuKong.Program.MonkeyKingOnLoadingComplete();
                    break;
                case "XinZhao":
                    XinZhao7.Program.XinZhaoOnLoadingComplete();
                    break;
                case "MasterYi":
                    Yi.Program.MasterYiOnLoadingComplete();
                    break;
                case "Ziggs":
                    Ziggs7.Program.ZiggsOnLoadingComplete();
                    break;
                case "Rengar":
                    D_Rengar.Program.RengarGame_OnGameLoad();
                    break;
                case "Rumble":
                    ElRumble.Rumble.RumbleOnLoad();
                    break;
                case "Vayne":
                    hi_im_gosu.Vayne.VayneGame_OnGameLoad();
                    break;
                case "Leona":
                    The_Horizon_Leona.Program.LeonaLoading_OnLoadingComplete();
                    break;
                case "Sona":
                    The_Horizon_Sona.Program.SonaLoading_OnLoadingComplete();
                    break;
                case "Amumu":
                    StonedAmumu.Program.AmumuGame_OnGameLoad();
                    break;
                case "JarvanIV":
                    StonedJarvan.Program.JarvanGame_OnGameLoad();
                    break;
                case "Nidalee":
                    T7_Nidalee.Program.NidaleeOnLoad();
                    break;
                case "Volibear":
                    VoliPower.Program.VolibearGame_OnLoad();
                    break;
            }
        }
    }
}
