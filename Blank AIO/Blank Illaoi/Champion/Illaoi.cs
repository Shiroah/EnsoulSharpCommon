using EnsoulSharp.Common;
using Illaoi___Tentacle_Kitty.Handlers;

namespace Illaoi___Tentacle_Kitty.Champion
{
    internal class Illaoi
    {
        public static Menu Config;
        public Menus Menus = new Menus(Config);
        public Spells Spell = new Spells();

        public Illaoi()
        {
            Menus.Init();
        }
    }
}