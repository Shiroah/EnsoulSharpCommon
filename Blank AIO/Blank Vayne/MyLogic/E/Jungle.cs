using EnsoulSharp;
using EnsoulSharp.Common;
using PRADA_Vayne.MyUtils;
using System;
using System.Linq;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        private static readonly string[] _jungleMobs =
        {
            "SRU_Blue", "SRU_Red", "SRU_Krug", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "TT_Spiderboss",
            "TTNGolem",
            "TTNWraith", "TTNWolf"
        };

        public static void JungleUsage(EventArgs args)
        {
            if (Program.LaneClearMenu.Item("EJungleMobs").GetValue<bool>() && Program.E.IsReady())
            {
                var target = Program.Orbwalker.GetTarget();
                if (target != null && target is AIMinionClient)
                {
                    var minion = (AIMinionClient)target;
                    if (_jungleMobs.Contains(minion.CharacterName))
                        for (var i = 40; i < 425; i += 141)
                        {
                            var flags = NavMesh.GetCollisionFlags(minion.Position.To2D()
                                .Extend(Heroes.Player.Position.To2D(), -i).To3D());
                            if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                            {
                                Program.E.Cast(minion);
                                return;
                            }
                        }
                }
            }
        }
    }
}