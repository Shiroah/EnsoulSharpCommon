using EnsoulSharp;
using EnsoulSharp.Common;
using KoreanAnnie.Common;
using System;
using System.Drawing;

namespace KoreanAnnie
{
    internal class AnnieDrawings
    {
        #region Fields

        private readonly Annie annie;

        #endregion Fields

        #region Constructors and Destructors

        public AnnieDrawings(Annie annie)
        {
            this.annie = annie;

            Drawing.OnDraw += DrawAvailableRange;
        }

        #endregion Constructors and Destructors

        #region Methods

        private void DrawAvailableRange(EventArgs args)
        {
            if (!annie.GetParamBool("drawskillranges")) return;

            if (annie.GetParamKeyBind("flashtibbers") && annie.Spells.R.IsReady() && FlashSpell.IsReady(annie.Player) &&
                annie.CheckStun())
                Render.Circle.DrawCircle(annie.Player.Position, annie.Spells.RFlash.Range, Color.DarkGreen, 3);
            else
                Render.Circle.DrawCircle(annie.Player.Position, annie.Spells.MaxRangeCombo, Color.DarkGreen, 3);
        }

        #endregion Methods
    }
}