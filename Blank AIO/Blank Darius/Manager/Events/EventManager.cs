using EnsoulSharp;

namespace Flowers_Darius.Manager.Events
{
    internal class EventManager
    {
        internal static void Init()
        {
            Game.OnUpdate += LoopManager.Init;
            AIBaseClient.OnDoCast += DoCastManager.Init;
            AIBaseClient.OnProcessSpellCast += SpellCastManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}