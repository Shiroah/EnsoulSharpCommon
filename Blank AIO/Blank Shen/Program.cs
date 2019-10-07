using EnsoulSharp;
using System.Collections.Generic;

namespace BadaoShen
{
    internal static class Program
    {
        public static readonly List<string> SupportedChampion = new List<string>
        {
            "Shen"
        };

        public static void ShenMain()
        {
            if (!SupportedChampion.Contains(ObjectManager.Player.CharacterName)) return;
            Chat.Print("<font color=\"#24ff24\">Badao </font>" + "<font color=\"#ff8d1a\">" +
                       ObjectManager.Player.CharacterName + "</font>" + "<font color=\"#24ff24\"> loaded!</font>");
            BadaoChampion.BadaoShen.BadaoShen.BadaoActivate();
        }
    }
}