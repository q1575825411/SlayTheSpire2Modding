using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MyFirstStS2Mod.Scripts.Powers;

namespace MyFirstStS2Mod.Scripts.Patches;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.GainBlock))]
internal static class ChiShouBlockPatch
{
    public static async void Postfix(Creature creature, decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        foreach (var power in creature.Powers.OfType<ChiShouPower>())
        {
            await power.TriggerFromBlockGain(new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext());
        }
    }
}
