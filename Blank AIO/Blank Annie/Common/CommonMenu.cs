using EnsoulSharp.Common;

namespace KoreanAnnie.Common
{
    internal class CommonMenu
    {
        #region Constructors and Destructors

        public CommonMenu(string displayName, bool misc)
        {
            MenuName = displayName.Replace(" ", "_").ToLowerInvariant();

            MainMenu = new Menu(displayName, MenuName, true);

            AddOrbwalker(MainMenu);
            AddTargetSelector(MainMenu);

            var modes = new Menu("Modes", string.Format("{0}.modes", MenuName));
            MainMenu.AddSubMenu(modes);

            HarasMenu = AddHarasMenu(modes);
            LaneClearMenu = AddLaneClearMenu(modes);
            ComboMenu = AddComboMenu(modes);

            if (misc) MiscMenu = AddMiscMenu(MainMenu);

            DrawingMenu = AddDrawingMenu(MainMenu);
        }

        #endregion Constructors and Destructors

        #region Properties

        private string MenuName { get; }

        #endregion Properties

        #region Public Properties

        public Menu ComboMenu { get; }

        public Menu DrawingMenu { get; }

        public Menu HarasMenu { get; }

        public Menu LaneClearMenu { get; }

        public Menu MainMenu { get; }

        public Menu MiscMenu { get; }

        public Orbwalking.Orbwalker Orbwalker { get; private set; }

        #endregion Public Properties

        #region Methods

        private Menu AddComboMenu(Menu rootMenu)
        {
            var newMenu = new Menu("Combo", string.Format("{0}.combo", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.useqtocombo", MenuName), "Use Q").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usewtocombo", MenuName), "Use W").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.useetocombo", MenuName), "Use E").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usertocombo", MenuName), "Use R").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.minenemiestor", MenuName), "Use R if it Will Hit at Least")
                .SetValue(new Slider(3, 1, 5)));
            newMenu.AddItem(new MenuItem(string.Format("{0}.disableaa", MenuName), "Disable AA When").SetValue(
                new StringList(new[]
                    {"Never", "Always", "Some Skills Ready", "Harass-Combo Ready", "Full-Combo Ready"})));
            newMenu.AddItem(new MenuItem(string.Format("{0}.forceultusingmouse", MenuName),
                "Force Ultimate Using Mouse-buttons (Cursor Sprite)").SetValue(true));

            return newMenu;
        }

        private Menu AddDrawingMenu(Menu rootMenu)
        {
            var newMenu = new Menu("Drawings", string.Format("{0}.drawings", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.drawskillranges", MenuName), "Skill Ranges")
                .SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.damageindicator", MenuName), "Damage Indicator").SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.damageindicatorcolor", MenuName), "Color Scheme").SetValue(
                    new StringList(new[] { "Normal", "Colorblind" })));
            newMenu.AddItem(new MenuItem(string.Format("{0}.killableindicator", MenuName), "Killable Indicator")
                .SetValue(true));

            return newMenu;
        }

        private Menu AddHarasMenu(Menu rootMenu)
        {
            var newMenu = new Menu("Harass", string.Format("{0}.haras", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.useqtoharas", MenuName), "Use Q").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usewtoharas", MenuName), "Use W").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.useetoharas", MenuName), "Use E").SetValue(true));

            var manaLimitToHaras =
                new MenuItem(string.Format("{0}.manalimittoharas", MenuName), "Mana % Limit").SetValue(new Slider());
            newMenu.AddItem(manaLimitToHaras);

            return newMenu;
        }

        private Menu AddLaneClearMenu(Menu rootMenu)
        {
            var newMenu = new Menu("Lane Clear", string.Format("{0}.laneclear", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.useqtolaneclear", MenuName), "Use Q").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usewtolaneclear", MenuName), "Use W").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.useetolaneclear", MenuName), "Use E").SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.manalimittolaneclear", MenuName), "Mana % Limit").SetValue(
                    new Slider(50)));
            newMenu.AddItem(new MenuItem(string.Format("{0}.harasonlaneclear", MenuName),
                "Harass Enemies in Lane Clear").SetValue(true));

            return newMenu;
        }

        private Menu AddMiscMenu(Menu rootMenu)
        {
            var newMenu = new Menu("Options", string.Format("{0}.misc", MenuName));
            rootMenu.AddSubMenu(newMenu);

            return newMenu;
        }

        private void AddOrbwalker(Menu rootMenu)
        {
            var orbwalkerMenu = new Menu("Orbwalker", string.Format("{0}.orbwalker", MenuName));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            rootMenu.AddSubMenu(orbwalkerMenu);
        }

        private void AddTargetSelector(Menu rootMenu)
        {
            var targetselectorMenu = new Menu("Target Selector", string.Format("{0}.targetselector", MenuName));
            TargetSelector.AddToMenu(targetselectorMenu);
            rootMenu.AddSubMenu(targetselectorMenu);
        }

        #endregion Methods
    }
}