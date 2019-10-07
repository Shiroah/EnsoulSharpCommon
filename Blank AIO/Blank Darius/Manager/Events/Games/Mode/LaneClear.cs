using EnsoulSharp.Common;
using myCommon;
using System.Linq;

namespace Flowers_Darius.Manager.Events.Games.Mode
{
    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (minions.Any())
                {
                    if (Menu.GetBool("LaneClearQ") && Q.IsReady() && minions.Count >= Menu.GetSlider("LaneClearQCount"))
                        Q.Cast(true);

                    if (Menu.GetBool("LaneClearW") && W.IsReady())
                    {
                        var minion = minions.FirstOrDefault(x => x.Health <= DamageCalculate.GetWDamage(x));

                        if (minion != null)
                        {
                            W.Cast(true);
                            Orbwalker.ForceTarget(minion);
                        }
                    }
                }
            }
        }
    }
}