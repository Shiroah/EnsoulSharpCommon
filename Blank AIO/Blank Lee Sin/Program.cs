using EnsoulSharp.Common;

namespace ElLeeSin
{
    internal class Program
    {
        public static void LeeSinMain()
        {
            CustomEvents.Game.OnGameLoad += ElLeeSin.LeeSin.OnGameLoad;
        }
    }
}