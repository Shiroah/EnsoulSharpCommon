using EnsoulSharp.Common;
using System;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnPossibleToInterrupt(ActiveInterrupter interrupter)
        {
            if (interrupter.Sender == null) return;
            if (interrupter.DangerLevel == InterrupterDangerLevel.High && Program.E.IsReady() &&
                Program.E.IsInRange(interrupter.Sender) && interrupter.Sender.CharacterName != "Shyvana" &&
                interrupter.Sender.CharacterName != "Vayne")
            {
                Console.WriteLine("Interrupter");
                Program.E.Cast(interrupter.Sender);
            }
        }
    }
}