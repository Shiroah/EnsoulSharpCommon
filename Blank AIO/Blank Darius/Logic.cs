using EnsoulSharp;
using EnsoulSharp.Common;

namespace Flowers_Darius
{
    internal class Logic
    {
        internal static int lastETime;
        internal static Spell Q, W, E, R;
        internal static SpellSlot Ignite = SpellSlot.Unknown;
        internal static Menu Menu;
        internal static AIHeroClient Me = ObjectManager.Player;
        internal static EnsoulSharp.Common.Orbwalking.Orbwalker Orbwalker;
    }
}
