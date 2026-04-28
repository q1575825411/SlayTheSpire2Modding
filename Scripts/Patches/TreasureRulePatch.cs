using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts.Equipment;

namespace MyFirstStS2Mod.Scripts.Patches;

[HarmonyPatch(typeof(NCardPlay), "TryPlayCard", [typeof(Creature)])]
internal static class TreasureRulePatch
{
    private sealed class PendingPlayState
    {
        public bool ShouldConsumeChuanGuoFreeUse { get; set; }
        public int EnergyBeforePlay { get; set; }
    }

    private static readonly Func<NCardPlay, CardModel?> GetCard =
        AccessTools.MethodDelegate<Func<NCardPlay, CardModel?>>(
            AccessTools.DeclaredPropertyGetter(typeof(NCardPlay), "Card"));
    private static readonly ConditionalWeakTable<NCardPlay, PendingPlayState> PendingStates = new();

    public static void Prefix(NCardPlay __instance)
    {
        var card = GetCard(__instance);
        if (card?.Owner is null)
        {
            return;
        }

        var state = PendingStates.GetOrCreateValue(__instance);
        state.ShouldConsumeChuanGuoFreeUse = false;
        state.EnergyBeforePlay = card.Owner.PlayerCombatState is null
            ? 0
            : RuntimeReflection.GetCurrentEnergy(card.Owner.PlayerCombatState);

        if (EquipmentQueries.GetEquipped<TaiPingYaoShu>(card.Owner, EquipmentSlotType.Treasure) is not null
            && (card.Type.ToString().Contains("Status", StringComparison.OrdinalIgnoreCase)
                || card.Type.ToString().Contains("Curse", StringComparison.OrdinalIgnoreCase)))
        {
            RuntimeReflection.TrySetCardPlayable(card, true);
            RuntimeReflection.TrySetCardExhaust(card, true);
        }

        if (EquipmentQueries.GetEquipped<ChuanGuoYuXi>(card.Owner, EquipmentSlotType.Treasure) is not null)
        {
            var power = card.Owner.Powers.OfType<Powers.ChuanGuoYuXiPower>().FirstOrDefault();
            if (power is not null && power.HasFreeCardAvailable())
            {
                RuntimeReflection.TrySetCardCostForTurn(card, 0);
                state.ShouldConsumeChuanGuoFreeUse = true;
            }
        }
    }

    public static void Postfix(NCardPlay __instance)
    {
        var card = GetCard(__instance);
        if (card?.Owner is null)
        {
            return;
        }

        var state = PendingStates.GetOrCreateValue(__instance);
        var cardStillInHand = RuntimeReflection.GetHandCards(card.Owner).Contains(card);
        if (state.ShouldConsumeChuanGuoFreeUse && !cardStillInHand)
        {
            card.Owner.Powers.OfType<Powers.ChuanGuoYuXiPower>().FirstOrDefault()?.ConsumeFreeCard();
        }

        if (EquipmentQueries.GetEquipped<YeMingZhu>(card.Owner, EquipmentSlotType.Treasure) is not null
            && card.Owner.PlayerCombatState is not null
            && !cardStillInHand)
        {
            var currentEnergy = RuntimeReflection.GetCurrentEnergy(card.Owner.PlayerCombatState);
            var power = card.Owner.Powers.OfType<Powers.YeMingZhuPower>().FirstOrDefault();
            if (power is not null
                && state.EnergyBeforePlay > 0
                && currentEnergy == 0
                && power.TryConsumeEnergyRecovery())
            {
                card.Owner.PlayerCombatState.GainEnergy(1);
            }
        }
    }
}
