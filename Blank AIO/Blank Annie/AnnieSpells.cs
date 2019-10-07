using EnsoulSharp;
using EnsoulSharp.Common;
using KoreanAnnie.Common;

namespace KoreanAnnie
{
    internal static class AnnieSpells
    {
        #region Public Methods and Operators

        public static void Load(ICommonChampion champion)
        {
            var q = new CommonSpell(SpellSlot.Q, 625, TargetSelector.DamageType.Magical);
            var w = new CommonSpell(SpellSlot.W, 625, TargetSelector.DamageType.Magical);
            var e = new CommonSpell(SpellSlot.E, 0, TargetSelector.DamageType.Magical);
            var r = new CommonSpell(SpellSlot.R, 600, TargetSelector.DamageType.Magical);

            q.SetTargetted(0.25f, 1400f);
            w.SetSkillshot(0.5f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);
            r.SetSkillshot(0.2f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            champion.Spells.AddSpell(q);
            champion.Spells.AddSpell(w);
            champion.Spells.AddSpell(e);
            champion.Spells.AddSpell(r);
        }

        #endregion Public Methods and Operators
    }
}