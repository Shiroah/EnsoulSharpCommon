using EnsoulSharp;
using EnsoulSharp.Common;
using PRADA_Vayne.MyLogic.E;
using PRADA_Vayne.MyLogic.Others;
using Events = PRADA_Vayne.MyLogic.Q.Events;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void LoadLogic()
        {
            #region Q

            Orbwalking.AfterAttack += Events.AfterAttack;
            Orbwalking.BeforeAttack += Events.BeforeAttack;
            Orbwalking.OnAttack += Events.OnAttack;
            Spellbook.OnCastSpell += Events.OnCastSpell;
            Gapclosers.OnGapcloser += Events.OnGapcloser;
            AIBaseClient.OnProcessSpellCast += Events.OnProcessSpellCast;
            Game.OnUpdate += Events.OnUpdate;

            #endregion Q

            #region E

            GameObject.OnCreate += AntiAssasins.OnCreateGameObject;
            AIBaseClient.OnProcessSpellCast += MyLogic.E.Events.OnProcessSpellCast;
            Game.OnUpdate += MyLogic.E.Events.OnUpdate;
            Interrupters.OnInterrupter += MyLogic.E.Events.OnPossibleToInterrupt;
            Game.OnUpdate += MyLogic.E.Events.JungleUsage;

            #endregion E

            #region R

            Spellbook.OnCastSpell += MyLogic.R.Events.OnCastSpell;

            #endregion R

            #region Others

            Game.OnUpdate += MyLogic.Others.Events.OnUpdate;
            AIBaseClient.OnProcessSpellCast += MyLogic.Others.Events.OnProcessSpellcast;
            Drawing.OnDraw += MyLogic.Others.Events.OnDraw;
            Game.OnUpdate += SkinHack.OnUpdate;

            #endregion Others
        }
    }
}