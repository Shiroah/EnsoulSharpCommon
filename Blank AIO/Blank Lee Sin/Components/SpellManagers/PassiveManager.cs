using EnsoulSharp;
using System;

namespace ElLeeSin.Components.SpellManagers
{
    internal class PassiveManager
    {
        internal static void OnBuffAdd(AIBaseClient sender, AIBaseClientBuffGainEventArgs args)
        {
            try
            {
                if (!sender.IsMe) return;

                if (args.Buff.Name.Equals("BlindMonkFlurry", StringComparison.InvariantCultureIgnoreCase))
                    LeeSin.PassiveStacks = 2;

                if (args.Buff.Name.Equals("BlindMonkQTwoDash", StringComparison.InvariantCultureIgnoreCase))
                    LeeSin.isInQ2 = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("@PassiveManager.cs: Can not {0} -", e);
                throw;
            }
        }

        internal static void OnBuffLose(AIBaseClient sender, AIBaseClientBuffLoseEventArgs args)
        {
            try
            {
                if (!sender.IsMe) return;

                if (args.Buff.Name.Equals("BlindMonkFlurry", StringComparison.InvariantCultureIgnoreCase))
                    LeeSin.PassiveStacks = 0;

                if (args.Buff.Name.Equals("BlindMonkQTwoDash", StringComparison.InvariantCultureIgnoreCase))
                    LeeSin.isInQ2 = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("@PassiveManager.cs: Can not {0} -", e);
                throw;
            }
        }
    }
}