using BadaoShen;
using EnsoulSharp;
using EnsoulSharp.Common;
using System;
using System.Linq;

namespace BadaoChampion.BadaoShen
{
    internal static class BadaoShenAuto
    {
        public static AIHeroClient Player => ObjectManager.Player;

        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe && x.BadaoIsValidTarget(float.MaxValue, false)))
                if (BadaoShenHelper.UseRAuto(hero))
                    BadaoMainVariables.R.Cast(hero);
        }
    }
}