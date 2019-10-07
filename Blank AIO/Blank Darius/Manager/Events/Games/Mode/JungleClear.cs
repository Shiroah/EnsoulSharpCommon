using EnsoulSharp.Common;
using myCommon;
using System.Linq;

namespace Flowers_Darius.Manager.Events.Games.Mode
{
    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                    if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                        Q.Cast(true);

                if (Menu.GetBool("JungleClearW") && W.IsReady())
                {
                    var mob = mobs.FirstOrDefault(x => x.Health <= DamageCalculate.GetWDamage(x));

                    if (mobs != null)
                    {
                        W.Cast(true);
                        Orbwalker.ForceTarget(mob);
                    }
                }
            }
        }
    }
}