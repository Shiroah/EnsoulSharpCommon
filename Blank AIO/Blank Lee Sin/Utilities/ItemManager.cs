using ElLeeSin.Components;
using EnsoulSharp;
using EnsoulSharp.Common;
using System.Linq;
using ItemData = EnsoulSharp.Common.Data.ItemData;

namespace ElLeeSin.Utilities
{
    internal class ItemManager
    {
        /// <summary>
        ///     Gets the Blade of the Ruined king item.
        /// </summary>
        public static Items.Item Botrk => ItemData.Blade_of_the_Ruined_King.GetItem();

        /// <summary>
        ///     Gets the Bilgewater Cutlass item.
        /// </summary>
        public static Items.Item Cutlass => ItemData.Bilgewater_Cutlass.GetItem();

        /// <summary>
        ///     Gets the Youmuu Ghostblade item.
        /// </summary>
        public static Items.Item Youmuus => ItemData.Youmuus_Ghostblade.GetItem();

        /// <summary>
        ///     Gets the Tiamat item.
        /// </summary>
        public static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets the Ravenous Hydra item.
        /// </summary>
        public static Items.Item RavenousHydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets the Titanic Hydra item.
        /// </summary>
        public static Items.Item TitanicHydra => new Items.Item(3748, 385);

        /// <summary>
        ///     Gets if the item is ready.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        ///     If the item is valid and ready.
        /// </returns>
        public static bool IsValidAndReady(Items.Item item)
        {
            return item != null && item.IsReady();
        }

        /// <summary>
        ///     Checks the menu item.
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            var name = "Items" + LeeSin.Orbwalker.ActiveMode.ToString().ToLower();
            var item = MyMenu.Menu.Item(name);
            return item != null && item.IsActive();
        }

        /// <summary>
        ///     Item casting handler.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CastItems(AIBaseClient target)
        {
            if (ObjectManager.Player.IsDashing() || ObjectManager.Player.Spellbook.IsAutoAttack) return false;

            var botrk = Botrk;
            if (IsValidAndReady(botrk) && botrk.Cast(target)) return true;

            var cutlass = Cutlass;
            if (IsValidAndReady(cutlass) && cutlass.Cast(target)) return true;

            if (!LeeSin.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.LaneClear))
            {
                var youmuus = Youmuus;
                if (IsValidAndReady(youmuus) && youmuus.Cast()) return true;
            }

            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is AITurretClient));
            var heroes = ObjectManager.Player.GetEnemiesInRange(385).Count;
            var count = units + heroes;

            var tiamat = Tiamat;
            if (IsValidAndReady(tiamat) && count > 0 && tiamat.Cast()) return true;

            var hydra = RavenousHydra;
            if (IsValidAndReady(hydra) && count > 0 && hydra.Cast()) return true;

            var titanic = TitanicHydra;
            return IsValidAndReady(titanic) && count > 0 && titanic.Cast();
        }
    }
}