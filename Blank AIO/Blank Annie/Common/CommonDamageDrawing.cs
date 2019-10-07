using EnsoulSharp;
using EnsoulSharp.Common;
using SharpDX;
using System;
using System.Linq;
using Color = System.Drawing.Color;

namespace KoreanAnnie.Common
{
    internal class CommonDamageDrawing
    {
        #region Delegates

        public delegate float DrawDamageDelegate(AIHeroClient hero);

        #endregion Delegates

        #region Constructors and Destructors

        public CommonDamageDrawing(ICommonChampion champion)
        {
            this.champion = champion;
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public DrawDamageDelegate AmountOfDamage
        {
            get { return amountOfDamage; }

            set
            {
                if (amountOfDamage == null) Drawing.OnDraw += DrawDamage;
                amountOfDamage = value;
            }
        }

        #endregion Public Properties

        #region Constants

        private const int Height = 8;

        private const int Width = 103;

        #endregion Constants

        #region Fields

        public bool Active = true;

        private readonly ICommonChampion champion;

        private readonly Render.Text text = new Render.Text(0, 0, "KILLABLE", 20, new ColorBGRA(255, 0, 0, 255));

        private DrawDamageDelegate amountOfDamage;

        #endregion Fields

        #region Methods

        private void DrawDamage(EventArgs args)
        {
            if (!Enabled()) return;

            var color = Color.Gray;
            var barColor = Color.White;

            if (KoreanUtils.GetParamStringList(champion.MainMenu, "damageindicatorcolor") == 1)
            {
                color = Color.Gold;
                barColor = Color.Olive;
            }

            if (KoreanUtils.GetParamStringList(champion.MainMenu, "damageindicatorcolor") == 2)
            {
                color = Color.FromArgb(100, Color.Black);
                barColor = Color.Lime;
            }

            foreach (var champ in ObjectManager.Get<AIHeroClient>()
                .Where(h => h.IsVisible && h.IsEnemy && h.IsValid && h.IsHPBarRendered))
            {
                var damage = amountOfDamage(champ);

                if (damage > 0)
                {
                    var pos = champ.HPBarPosition;

                    if (KoreanUtils.GetParamBool(champion.MainMenu, "killableindicator") && damage > champ.Health + 50f)
                    {
                        Render.Circle.DrawCircle(champ.Position, 100, Color.Red);
                        Render.Circle.DrawCircle(champ.Position, 75, Color.Red);
                        Render.Circle.DrawCircle(champ.Position, 50, Color.Red);
                        text.X = (int)pos.X + 40;
                        text.Y = (int)pos.Y - 20;
                        text.OnEndScene();
                    }

                    if (KoreanUtils.GetParamBool(champion.MainMenu, "damageindicator"))
                    {
                        var healthAfterDamage = Math.Max(0, champ.Health - damage) / champ.MaxHealth;
                        var posY = pos.Y + 20f;
                        var posDamageX = pos.X + 12f + Width * healthAfterDamage;
                        var posCurrHealthX = pos.X + 12f + Width * champ.Health / champ.MaxHealth;
                        var diff = posCurrHealthX - posDamageX + 3;
                        var pos1 = pos.X + 8 + 107 * healthAfterDamage;

                        for (var i = 0; i < diff - 3; i++)
                            Drawing.DrawLine(pos1 + i, posY, pos1 + i, posY + Height, 1, color);

                        Drawing.DrawLine(posDamageX, posY, posDamageX, posY + Height, 2, barColor);
                    }
                }
            }
        }

        private bool Enabled()
        {
            return Active && amountOfDamage != null &&
                   (KoreanUtils.GetParamBool(champion.MainMenu, "damageindicator") ||
                    KoreanUtils.GetParamBool(champion.MainMenu, "killableindicator"));
        }

        #endregion Methods
    }
}