namespace Blank_Brand
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += delegate
            {
                if (ObjectManager.Player.CharacterName != "Brand")
                    return;

                Brand.OnLoad();
            };
        }
    }
}
