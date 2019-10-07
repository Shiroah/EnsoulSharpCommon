using ElLeeSin.Components;
using ElLeeSin.Components.SpellManagers;
using EnsoulSharp;
using EnsoulSharp.Common;
using SharpDX;
using System;
using System.Linq;
using ItemData = EnsoulSharp.Common.Data.ItemData;

namespace ElLeeSin.Utilities
{
    using static LeeSin;

    internal static class Misc
    {
        #region Properties

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        internal static Items.Item Hydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets the E Instance name
        /// </summary>
        /// <value>
        ///     E instance name
        /// </value>
        internal static bool IsEOne
            => spells[Spells.E].Instance.Name.Equals("BlindMonkEOne", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets the Q Instance name
        /// </summary>
        /// <value>
        ///     Q instance name
        /// </value>
        internal static bool IsQOne
            => spells[Spells.Q].Instance.Name.Equals("BlindMonkQOne", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets the W Instance name
        /// </summary>
        /// <value>
        ///     W instance name
        /// </value>
        internal static bool IsWOne
            => spells[Spells.W].Instance.Name.Equals("BlindMonkWOne", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        internal static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        internal static Items.Item Titanic => ItemData.Titanic_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     The ward flash range.
        /// </summary>
        internal static float WardFlashRange => InsecManager.WardRange + spells[Spells.R].Range - 100;

        /// <summary>
        ///     Gets the W Instance name
        /// </summary>
        /// QState
        /// <value>
        ///     W instance name
        /// </value>
        internal static Wardmanager.WCastStage WStage
        {
            get
            {
                if (!spells[Spells.W].IsReady()) return Wardmanager.WCastStage.Cooldown;

                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W)
                    .Name.Equals("blindmonkwtwo", StringComparison.InvariantCultureIgnoreCase)
                    ? Wardmanager.WCastStage.Second
                    : Wardmanager.WCastStage.First;
            }
        }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        internal static Items.Item Youmuu => ItemData.Youmuus_Ghostblade.GetItem();

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Compares gameobjects.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="@object"></param>
        /// <returns></returns>
        internal static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return gameObject != null && gameObject.IsValid && @object != null && @object.IsValid
                   && gameObject.NetworkId == @object.NetworkId;
        }

        /// <summary>
        ///     Orbwalker.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="target"></param>
        public static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        /// <summary>
        ///     Gets the Q2 damage
        /// </summary>
        /// <param name="target"></param>
        /// <param name="subHP"></param>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static float Q2Damage(AIBaseClient target, float subHP = 0, bool monster = false)
        {
            var dmg = new[] { 50, 80, 110, 140, 170 }[spells[Spells.Q].Level - 1]
                      + 0.9 * ObjectManager.Player.FlatPhysicalDamageMod
                      + 0.08 * (target.MaxHealth - (target.Health - subHP));

            return
                (float)
                (ObjectManager.Player.CalcDamage(
                     target,
                     Damage.DamageType.Physical,
                     target is AIMinionClient ? Math.Min(dmg, 400) : dmg) + subHP);
        }

        /// <summary>
        ///     Gets the distance to player.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static float DistanceToPlayer(this AIBaseClient source)
        {
            return EnsoulSharp.Common.Geometry.Distance(ObjectManager.Player, source);
        }

        /// <summary>
        ///     Gets the menu item.
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static bool GetMenuItem(string paramName)
        {
            return MyMenu.Menu.Item(paramName).IsActive();
        }

        /// <summary>
        ///     Returns if target has any buffs.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static bool HasAnyBuffs(this AIBaseClient unit, string s)
        {
            return
                unit.Buffs.Any(
                    a => a.Name.ToLower().Contains(s.ToLower()) || a.Name.ToLower().Contains(s.ToLower()));
        }

        /// <summary>
        ///     Checks if a target has the BlindMonkTempest buff.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static bool HasBlindMonkTempest(AIBaseClient target)
        {
            return target.HasBuff("BlindMonkTempest");
        }

        /// <summary>
        ///     Returns if target has Q buff.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        internal static bool HasQBuff(this AIBaseClient unit)
        {
            return unit.HasAnyBuffs("BlindMonkQOne") || unit.HasAnyBuffs("blindmonkqonechaos")
                                                     || unit.HasAnyBuffs("BlindMonkSonicWave");
        }

        /// <summary>
        ///     Returns the Q buff
        /// </summary>
        /// <returns></returns>
        internal static AIBaseClient ReturnQBuff()
        {
            try
            {
                return
                    ObjectManager.Get<AIBaseClient>()
                        .Where(a => a.IsValidTarget(1300))
                        .FirstOrDefault(unit => unit.HasQBuff());
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }

            return null;
        }

        #endregion Methods
    }
}