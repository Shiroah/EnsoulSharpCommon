using EnsoulSharp;
using EnsoulSharp.Common;
using myCommon;

namespace Flowers_Darius.Manager.Spells
{
    internal class SpellManager : Logic
    {
        internal static int RMana => R.Level == 0 || R.Level == 3 ? 0 : 100;

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 425f);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 535);
            R = new Spell(SpellSlot.R, 460f);
            E.SetSkillshot(0.20f, 100f, float.MaxValue, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        internal static bool CanQHit(AIHeroClient target)
        {
            if (target == null) return false;

            if (target.DistanceToPlayer() > Q.Range) return false;

            if (target.DistanceToPlayer() <= 240) return false;

            if (target.Health < DamageCalculate.GetRDamage(target) && R.IsReady() &&
                target.IsValidTarget(R.Range)) return false;

            return true;
        }

        internal static void CastItem()
        {
            if (Items.HasItem(3077) && Items.CanUseItem(3077)) Items.UseItem(3077);

            if (Items.HasItem(3074) && Items.CanUseItem(3074)) Items.UseItem(3074);

            if (Items.HasItem(3053) && Items.CanUseItem(3053)) Items.UseItem(3053);
        }
    }
}