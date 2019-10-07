using EnsoulSharp;
using EnsoulSharp.Common;
using KoreanAnnie.Common;
using Ricardo.AIO.Properties;
using SharpDX;

namespace KoreanAnnie
{
    internal class AnnieButtons
    {
        #region Fields

        private readonly Annie annie;

        #endregion Fields

        #region Constructors and Destructors

        public AnnieButtons(Annie annie)
        {
            this.annie = annie;

            ShowEasyButton = KoreanUtils.GetParamBool(annie.MainMenu, "showeeasybutton");

            var posX = KoreanUtils.GetParamInt(annie.MainMenu, "easybuttonpositionx");
            posX = posX == 0 ? 50 : posX;
            var posY = KoreanUtils.GetParamInt(annie.MainMenu, "easybuttonpositiony");
            posY = posY == 0 ? 50 : posY;
            var pos = new Vector2(posX, posY);

            StunButtonOn = new Render.Sprite(Resources.StunON, pos);
            StunButtonOn.Scale = new Vector2(0.90f, 0.90f);
            StunButtonOn.Add();

            StunButtonOff = new Render.Sprite(Resources.StunOFF, pos);
            StunButtonOff.Scale = new Vector2(0.90f, 0.90f);
            StunButtonOff.Add();

            if (ShowEasyButton)
            {
                StunButtonOn.Visible = KoreanUtils.GetParamBool(annie.MainMenu, "savestunforcombo");
                StunButtonOff.Visible = !StunButtonOn.Visible;
            }

            KoreanUtils.GetParam(annie.MainMenu, "savestunforcombo").ValueChanged += SaveStunForComboValueChanged;
            Game.OnWndProc += ButtonControl;
        }

        #endregion Constructors and Destructors

        #region Properties

        private bool ShowEasyButton { get; set; }
        private Render.Sprite StunButtonOff { get; }
        private Render.Sprite StunButtonOn { get; }

        #endregion Properties

        #region Methods

        private static bool MouseOnButton(Render.Sprite button)
        {
            var pos = Utils.GetCursorPos();

            return pos.X >= button.Position.X && pos.X <= button.Position.X + button.Width &&
                   pos.Y >= button.Position.Y && pos.Y <= button.Position.Y + button.Height;
        }

        private void ButtonControl(WndEventArgs args)
        {
            ShowEasyButton = KoreanUtils.GetParamBool(annie.MainMenu, "showeeasybutton");

            if (ShowEasyButton)
            {
                if (args.Msg == (uint)WindowsMessages.WM_LBUTTONUP &&
                    (MouseOnButton(StunButtonOn) || MouseOnButton(StunButtonOff)))
                {
                    if (StunButtonOn.Visible)
                        KoreanUtils.SetValueBool(annie.MainMenu, "savestunforcombo", false);
                    else if (StunButtonOff.Visible) KoreanUtils.SetValueBool(annie.MainMenu, "savestunforcombo", true);
                }
                else if (args.Msg == (uint)WindowsMessages.WM_MOUSEMOVE && args.WParam == 5 &&
                         (MouseOnButton(StunButtonOn) || MouseOnButton(StunButtonOff)))
                {
                    MoveButtons(new Vector2(Utils.GetCursorPos().X - StunButtonOn.Width / 2,
                        Utils.GetCursorPos().Y - 10));
                }

                StunButtonOn.Visible = KoreanUtils.GetParamBool(annie.MainMenu, "savestunforcombo");
                StunButtonOff.Visible = !StunButtonOn.Visible;
            }
            else
            {
                StunButtonOff.Visible = false;
                StunButtonOn.Visible = false;
            }
        }

        private void MoveButtons(Vector2 newPosition)
        {
            StunButtonOn.Position = newPosition;
            StunButtonOn.PositionUpdate = () => newPosition;
            StunButtonOn.PositionUpdate = null;

            StunButtonOff.Position = newPosition;
            StunButtonOff.PositionUpdate = () => newPosition;
            StunButtonOff.PositionUpdate = null;

            KoreanUtils.SetValueInt(annie.MainMenu, "easybuttonpositionx", (int)StunButtonOn.Position.X);
            KoreanUtils.SetValueInt(annie.MainMenu, "easybuttonpositiony", (int)StunButtonOn.Position.Y);
        }

        private void SaveStunForComboValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (ShowEasyButton)
            {
                StunButtonOn.Visible = e.GetNewValue<bool>();
                StunButtonOff.Visible = !StunButtonOn.Visible;
            }
        }

        #endregion Methods
    }
}