using EnsoulSharp;
using EnsoulSharp.Common;
using System;

namespace Illaoi___Tentacle_Kitty.Handlers
{
    internal class Spells
    {
        public Spells()
        {
            Q.SetSkillshot(.484f, 0, 500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.066f, 50, 1900, true, SkillshotType.SkillshotLine);
            Console.WriteLine("Spell LOADED");
        }

        public static Spell Q => new Spell(SpellSlot.Q, 850);
        public static Spell W => new Spell(SpellSlot.W);
        public static Spell E => new Spell(SpellSlot.E, 900);
        public static Spell R => new Spell(SpellSlot.R, 450);
    }
}