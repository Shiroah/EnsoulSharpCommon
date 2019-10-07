using EnsoulSharp.Common;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.EscapeMenu.SubMenu("antigapcloser").Item("antigc" + gapcloser.Sender.CharacterName)
                .GetValue<bool>())
                if (Heroes.Player.Distance(gapcloser.End) < 425)
                    Tumble.Cast(Heroes.Player.Position.Extend(gapcloser.End, -300));
        }
    }
}