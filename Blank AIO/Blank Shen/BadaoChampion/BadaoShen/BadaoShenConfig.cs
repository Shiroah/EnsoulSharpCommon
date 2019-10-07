using BadaoShen;
using EnsoulSharp;
using EnsoulSharp.Common;
using System.Drawing;
using System.Linq;
using Color = SharpDX.Color;

namespace BadaoChampion.BadaoShen
{
    internal static class BadaoShenConfig
    {
        public static Menu config;

        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q);
            BadaoMainVariables.Q.SetSkillshot(0f, 50f, 2500, false, SkillshotType.SkillshotLine,
                BadaoShenVariables.SwordPos.To3D());
            BadaoMainVariables.W = new Spell(SpellSlot.W, 300);
            BadaoMainVariables.E = new Spell(SpellSlot.E, 600);
            BadaoMainVariables.E.SetSkillshot(0f, 50f, 1600f, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.R = new Spell(SpellSlot.R);

            // main menu
            config = new Menu("BadaoKingdom " + ObjectManager.Player.CharacterName, ObjectManager.Player.CharacterName,
                true);
            config.SetFontStyle(FontStyle.Bold, Color.YellowGreen);

            // orbwalker menu
            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            BadaoMainVariables.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            config.AddSubMenu(orbwalkerMenu);

            // TS
            var ts = config.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            ;
            TargetSelector.AddToMenu(ts);

            // Combo
            var Combo = config.AddSubMenu(new Menu("Combo", "Combo"));
            BadaoShenVariables.ComboQ = Combo.AddItem(new MenuItem("ComboQ", "Q")).SetValue(true);
            BadaoShenVariables.ComboW = Combo.AddItem(new MenuItem("ComboW", "W")).SetValue(true);
            var EToTarget = Combo.AddSubMenu(new Menu("E to Target", "EToTarget"));
            foreach (var hero in HeroManager.Enemies)
                EToTarget.AddItem(new MenuItem("EToTarget" + hero.NetworkId,
                    hero.CharacterName + " (" + hero.Name + ")")).SetValue(true);
            BadaoShenVariables.ComboEIfHit = Combo.AddItem(new MenuItem("ComboEIfHit", "E If Hit")).SetValue(true);
            BadaoShenVariables.ComboEIfWillHit = Combo.AddItem(new MenuItem("ComboEIfWillHit", "If Will Hit"))
                .SetValue(new Slider(3, 1, 5));
            var Wtarget = Combo.AddSubMenu(new Menu("W to protect", "WProtect"));
            foreach (var hero in HeroManager.Allies)
                Wtarget.AddItem(new MenuItem("WProtect" + hero.NetworkId, hero.CharacterName + " (" + hero.Name + ")"))
                    .SetValue(true);

            // LaneClear
            var LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            BadaoShenVariables.LaneClearQ = LaneClear.AddItem(new MenuItem("LaneClearQ", "Q")).SetValue(true);

            // JungleClear
            var JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            BadaoShenVariables.JungleClearQ = JungleClear.AddItem(new MenuItem("JungleClearQ", "Q")).SetValue(true);

            // Auto
            var Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
                Auto.AddItem(new MenuItem("AutoR" + hero.NetworkId, "R " + hero.CharacterName + " (" + hero.Name + ")"))
                    .SetValue(true);

            BadaoShenVariables.RHp = Auto.AddItem(new MenuItem("RHp", "% Hp to R")).SetValue(new Slider(20));
        }
    }
}