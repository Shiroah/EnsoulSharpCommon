namespace BadaoChampion.BadaoShen
{
    internal static class BadaoShen
    {
        public static void BadaoActivate()
        {
            BadaoShenConfig.BadaoActivate();
            BadaoShenAuto.BadaoActivate();
            BadaoShenCombo.BadaoActive();
            BadaoShenJungleClear.BadaoActivate();
            BadaoShenLaneClear.BadaoActivate();
            BadaoShenSword.BadaoActivate();
            BadaoShenDrawing.BadaoActivate();
        }
    }
}