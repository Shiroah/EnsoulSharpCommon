using EnsoulSharp;
using EnsoulSharp.Common;
using Illaoi___Tentacle_Kitty.Extensions;
using System.Drawing;
using System.Linq;

namespace Illaoi___Tentacle_Kitty.Handlers
{
    internal class Menus
    {
        public Menus(Menu menu)
        {
            Config = menu;
            Init();
        }

        public static Menu Config { get; set; }
        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        public static void Init()
        {
            Config = new Menu("Illaoi - Tentacle Kitty", "Illaoi - Tentacle Kitty", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("q.ghost.combo", "Use Q (Ghost)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.min.hit", "(R) Min. Hit").SetValue(new Slider(3, 1, 5)));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("q.ghost.harass", "Use Q (Ghost)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }

                var clearMenu = new Menu("Clear Settings", "Clear Settings");
                {
                    clearMenu.AddItem(new MenuItem("q.clear", "Use Q").SetValue(true)); //
                    clearMenu.AddItem(new MenuItem("q.minion.hit", "(Q) Min. Hit").SetValue(new Slider(3, 1, 6)));
                    clearMenu.AddItem(new MenuItem("clear.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var eMenu = new Menu("E Settings", "E Settings");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "E Whitelist"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharacterData.CharacterName,
                                $"E: {enemy.CharacterData.CharacterName}")
                            .SetValue(Provider.HighChamps.Contains(enemy.CharacterData.CharacterName)));
                    Config.AddSubMenu(eMenu);
                }

                var ksMenu = new Menu("KillSteal Settings", "KillSteal Settings");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Use Q").SetValue(true));
                    Config.AddSubMenu(ksMenu);
                }

                var drawMenu = new Menu("Draw Settings", "Draw Settings");
                {
                    var damageDraw = new Menu("Damage Draw", "Damage Draw");
                    {
                        damageDraw.AddItem(
                            new MenuItem("aa.indicator", "AA Indicator").SetValue(new Circle(true, Color.Gold)));
                        drawMenu.AddSubMenu(damageDraw);
                    }
                    drawMenu.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                    drawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, Color.DodgerBlue)));
                    drawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, Color.GreenYellow)));
                    drawMenu.AddItem(
                        new MenuItem("passive.draw", "Passive Draw").SetValue(new Circle(true, Color.Gold)));
                    Config.AddSubMenu(drawMenu);
                }
            }
        }
    }
}