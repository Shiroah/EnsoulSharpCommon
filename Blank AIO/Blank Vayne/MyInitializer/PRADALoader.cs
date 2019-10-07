using EnsoulSharp;
using PRADA_Vayne.MyLogic.Others;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void Init()
        {
            if (ObjectManager.Player.CharacterName == "Vayne")
            {
                Cache.Load();
                LoadMenu();
                LoadSpells();
                LoadLogic();
                ShowNotifications();
                SkinHack.Load();
            }
        }
    }
}