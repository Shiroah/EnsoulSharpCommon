using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.Common;
using SharpDX;

namespace Blank_Darius
{
    internal class GlobalManager
    {
        // this for later XD
        public static int TickCount(AIHeroClient t)
        {
            var buff = t.Buffs.FirstOrDefault(x => x.Name == "dariushemo");
            return buff != null ? buff.Count : 0;
        }

        private static DamageToUnitDelegate _damageToUnit;
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static AIHeroClient Player;

        public static Spell Q, W, E, R;



        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += DrawingManager.Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }

        public static SpellSlot Ignite { get; private set; }

        private static float IgniteDamage(AIBaseClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.DamageSummonerSpell.Ignite);
        }

        public static float GetComboDamage(AIHeroClient enemy)
        {
            var damage = 0d;
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (R.IsReady() && Player.Mana >= R.Instance.ManaCost)
                damage += Player.GetSpellDamage(enemy, SpellSlot.R); //* RCount();

            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);

            return (float)damage;
        }

    }
}
