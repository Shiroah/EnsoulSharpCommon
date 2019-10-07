using EnsoulSharp;
using EnsoulSharp.Common;
using myCommon;
using System.Linq;

namespace Flowers_Darius.Manager.Events
{
    internal class SpellCastManager : Logic
    {
        internal static void Init(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsMe || Args.SData == null) return;

            if (Args.SData.Name.Contains("DariusAxeGrabCone")) lastETime = Utils.TickCount;

            if (Args.SData.Name.Contains("ItemTiamatCleave"))
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (!HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me)))
                        return;

                    if (Menu.GetBool("ComboW") && W.IsReady()) W.Cast();
                }
        }
    }
}