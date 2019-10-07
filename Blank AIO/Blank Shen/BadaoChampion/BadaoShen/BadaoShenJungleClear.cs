using BadaoShen;
using EnsoulSharp;
using EnsoulSharp.Common;

namespace BadaoChampion.BadaoShen
{
    internal static class BadaoShenJungleClear
    {
        public static void BadaoActivate()
        {
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (args.Target.Team != GameObjectTeam.Neutral)
                return;
            if (BadaoShenHelper.UseQLaneClear()) BadaoMainVariables.Q.Cast();
        }
    }
}