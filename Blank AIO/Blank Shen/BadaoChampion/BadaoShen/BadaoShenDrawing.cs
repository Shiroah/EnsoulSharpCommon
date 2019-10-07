using EnsoulSharp;
using System;

namespace BadaoChampion.BadaoShen
{
    internal static class BadaoShenDrawing
    {
        public static void BadaoActivate()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
        }
    }
}