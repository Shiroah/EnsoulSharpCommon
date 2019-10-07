using EnsoulSharp;
using EnsoulSharp.Common;
using EnsoulSharp.SDK;
using System;
using System.Linq;

namespace BadaoChampion.BadaoShen
{
    public static class BadaoShenSword
    {
        private static int CheckTick;

        public static void BadaoActivate()
        {
            if (HeroManager.Enemies.Any(x => x.CharacterName == "Shen"))
                Game.OnUpdate += Game_OnUpdate1;
            else
                Game.OnUpdate += Game_OnUpdate2;
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (CheckTick >= Utils.GameTimeTickCount - 200)
                return;
            CheckTick = Utils.GameTimeTickCount;
            var ShenBeam = ObjectManager.Get<AIMinionClient>()
                .FirstOrDefault(x => x.IsAlly && x.Name == "ShenThingUnit");
            if (ShenBeam != null)
                BadaoShenVariables.SwordPos = ShenBeam.Position.To2D();
        }

        private static void Game_OnUpdate2(EventArgs args)
        {
            if (CheckTick >= Utils.GameTimeTickCount - 200)
                return;
            CheckTick = Utils.GameTimeTickCount;
            var ShenBeam = GameObjects.ParticleEmitters.FirstOrDefault(x => x.Name == "Shen_Base_Q_beam.troy");
            if (ShenBeam != null)
                BadaoShenVariables.SwordPos = ShenBeam.Position.To2D();
        }
    }
}