using EnsoulSharp;
using EnsoulSharp.Common;

namespace KoreanAnnie.Common
{
    internal class CommonSpell : Spell
    {
        #region Constructors and Destructors

        public CommonSpell(SpellSlot slot, float range = 0,
            TargetSelector.DamageType damageType = TargetSelector.DamageType.Physical) : base(slot, range, damageType)
        {
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators

        public bool CanCast()
        {
            return Instance.ToggleState <= 1;
        }

        #endregion Public Methods and Operators

        #region Public Properties

        public float LastTimeUsed => Instance.CooldownExpires - Instance.Cooldown;

        public float UsedforXSecAgo => Game.Time - LastTimeUsed;

        public bool UseOnCombo => UseOnComboMenu != null && UseOnComboMenu.GetValue<bool>();

        public MenuItem UseOnComboMenu { get; set; }

        public bool UseOnHaras => UseOnHarasMenu != null && UseOnHarasMenu.GetValue<bool>();

        public MenuItem UseOnHarasMenu { get; set; }

        public bool UseOnLaneClear => UseOnComboMenu != null && UseOnLaneClearMenu.GetValue<bool>();

        public MenuItem UseOnLaneClearMenu { get; set; }

        #endregion Public Properties
    }
}