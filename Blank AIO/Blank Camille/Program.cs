using EnsoulSharp;
using EnsoulSharp.Common;
using hCamille.Champions;
using System;

namespace hCamille
{
    internal class Program
    {
        public static void CamilleMain()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharacterName == "Camille")
                new Camille();
        }
    }
}