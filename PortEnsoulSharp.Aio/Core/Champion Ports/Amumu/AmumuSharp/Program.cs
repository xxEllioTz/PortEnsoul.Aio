using EnsoulSharp;

namespace AmumuSharp
{
    class Program
    {
        public static Helper Helper;
        public static void Game_OnGameLoad()
        {
            Game.Print("xxEliotz - Amumu");
            Helper = new Helper();
            new Amumu();
        }
    }
}
