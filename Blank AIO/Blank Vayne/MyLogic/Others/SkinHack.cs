using EnsoulSharp;
using EnsoulSharp.Common;
using System;

namespace PRADA_Vayne.MyLogic.Others
{
    public static class SkinHack
    {
        public static void Load()
        {
            Utility.DelayAction.Add(250, RefreshSkin);
            Game.OnUpdate += OnUpdate;
            Game.OnNotify += OnNotify;
        }

        private static void OnNotify(GameNotifyEventArgs args)
        {
        }

        public static void OnUpdate(EventArgs args)
        {
        }

        public static void RefreshSkin()
        {
            if (Program.SkinhackMenu.Item("shkenabled").GetValue<bool>())
            {
            }
        }
    }
}