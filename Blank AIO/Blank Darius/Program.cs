using EnsoulSharp;
using EnsoulSharp.Common;
using Flowers_Darius.Manager.Events;
using Flowers_Darius.Manager.Menu;
using Flowers_Darius.Manager.Spells;
using System;

namespace Flowers_Darius
{
    internal class Program
    {
        public static void DariusMain()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs Args)
        {
            if (ObjectManager.Player.CharacterName != "Darius") return;

            Chat.Print("<font color='#00a8ff'>Flowers' Darius Load! by NightMoon</font>");
            SpellManager.Init();
            MenuManager.Init();
            EventManager.Init();
        }
    }
}