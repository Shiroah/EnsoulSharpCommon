using EnsoulSharp;
using EnsoulSharp.Common;

namespace KoreanAnnie
{
    internal class Program
    {
        public static void AnnieMain()
        {
            if (ObjectManager.Player.CharacterName.ToLowerInvariant() == "annie")
                CustomEvents.Game.OnGameLoad += arg => new Annie();
        }
    }
}