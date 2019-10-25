using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using PortAIO.Dual_Port;


namespace PortAIO
{
        public static class Init
        {
            public static void Initialize()
            {
                
                Misc.Load();
                LoadChampion();
                Console.WriteLine("[PortAIO] Core loading : Module " + ObjectManager.Player.CharacterName + " - Champion Script Loaded");
                Game.OnUpdate += Game_OnUpdate;
              
            }

            private static void LoadChampion()
            {
              try
              {
                switch(ObjectManager.Player.CharacterName)
                {
                    case "Amumu":
                        AmumuSharp.Program.Game_OnGameLoad();
                        break;
                    case "Darius":
                        Darius.main.Game_OnGameLoad();
                        break;
                    case "LeeSin":
                        LeeSin.Program.Game_OnGameLoad();
                        break;
                    case "Viktor":
                        Viktor.Program.Game_OnGameLoad();
                        break;
                    default:
                        Game.Print("We don't support" + ObjectManager.Player.CharacterName);
                        break;
                                
                }

              }catch (Exception e)
              {
                Console.WriteLine("{0}", e);
              }
           }

                private static void Game_OnUpdate(EventArgs args)
            {
                Orbwalker.AttackState = true;
                Orbwalker.MovementState = true;
            }


        }
}
  
