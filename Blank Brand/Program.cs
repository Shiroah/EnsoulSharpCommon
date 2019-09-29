namespace Blank_Brand
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    public class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Brand")
            {
                return;
            }

            Brand.OnLoad();
        }
    }
}
