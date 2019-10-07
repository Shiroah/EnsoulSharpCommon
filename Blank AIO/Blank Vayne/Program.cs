using EnsoulSharp.Common;
using PRADA_Vayne.MyInitializer;

namespace PRADA_Vayne
{
    public class Program
    {
        public static void VayneMain()
        {
            CustomEvents.Game.OnGameLoad += arg => PRADALoader.Init();
        }

        #region Fields and Objects

        public static Orbwalking.Orbwalker Orbwalker;

        #region Menu

        public static Menu MainMenu;
        public static Menu ComboMenu;
        public static Menu LaneClearMenu;
        public static Menu EscapeMenu;
        public static Menu DrawingsMenu;
        public static Menu SkinhackMenu;
        public static Menu OrbwalkerMenu;

        #endregion Menu

        #region Spells

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        #endregion Spells

        #endregion Fields and Objects
    }
}