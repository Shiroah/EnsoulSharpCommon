namespace Blank_Brand
{
    using EnsoulSharp.SDK.MenuUI.Values;
     internal class MenuWrapper
    {
        public class Combo
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool Qstun = new MenuBool("qstun", "Only use Q when target can be stunned");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
            public static readonly MenuBool R = new MenuBool("r", "Use R");
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
        }

        public class Laneclear
        {
            public static MenuBool W = new MenuBool("w", "Use W");
            public static MenuBool E = new MenuBool("e", "Use E");
            public static MenuBool Elasthit = new MenuBool("elasthit", "^ only to last hit");
        }

        public class Jungleclear
        {
            public static MenuBool Q = new MenuBool("q", "Use Q");
            public static MenuBool W = new MenuBool("w", "Use W");
            public static MenuBool E = new MenuBool("e", "Use E");
        }

        public class Killsteal
        {
            public static MenuBool Q = new MenuBool("q", "Use Q");
            public static MenuBool W = new MenuBool("w", "Use W");
            public static MenuBool E = new MenuBool("e", "Use E");
            public static MenuBool R = new MenuBool("r", "Use R");
        }

        public class Drawing
        {
            public static readonly MenuBool Q = new MenuBool("q", "Draw Q Range");
            public static readonly MenuBool W = new MenuBool("w", "Draw W Range");
            public static readonly MenuBool E = new MenuBool("e", "Draw E Range");
            public static readonly MenuBool R = new MenuBool("r", "Draw R Range");
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready");
        }

        public class Misc
        {
            public static MenuSliderButton Rppl = new MenuSliderButton("r", "Only use R when there is x people", 2, 1, 5);
        }

    }
}
