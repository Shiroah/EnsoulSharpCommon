using EnsoulSharp;
using EnsoulSharp.Common;

namespace KoreanAnnie.Common
{
    internal static class FlashSpell
    {
        #region Public Methods and Operators

        public static bool IsReady(AIHeroClient player)
        {
            return Slot(player) != SpellSlot.Unknown && player.Spellbook.CanUseSpell(Slot(player)) == SpellState.Ready;
        }

        public static SpellSlot Slot(AIHeroClient player)
        {
            return player.GetSpellSlot("SummonerFlash");
        }

        #endregion Public Methods and Operators
    }
}