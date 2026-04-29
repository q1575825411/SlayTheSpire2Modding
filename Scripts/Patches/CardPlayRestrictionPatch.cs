using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts.Equipment;
using MyFirstStS2Mod.Scripts.Relics;

namespace MyFirstStS2Mod.Scripts.Patches;

[HarmonyPatch(typeof(NCardPlay), "TryPlayCard", [typeof(Creature)])]
internal static class CardPlayRestrictionPatch
{
    private static readonly Func<NCardPlay, CardModel?> GetCard =
        AccessTools.MethodDelegate<Func<NCardPlay, CardModel?>>(
            AccessTools.DeclaredPropertyGetter(typeof(NCardPlay), "Card"));

    private static readonly MethodInfo CancelPlayCardMethod =
        AccessTools.DeclaredMethod(typeof(NCardPlay), "CancelPlayCard")!;

    public static bool Prefix(NCardPlay __instance, Creature? target)
    {
        var card = GetCard(__instance);
        if (card is null)
        {
            return true;
        }

        if (card is ShaCard && card.Owner?.Powers.OfType<Powers.ZhanChangPower>().Any() == true)
        {
            RuntimeReflection.TrySetCardCostForTurn(card, 0);
        }

        foreach (var power in card.Owner?.Powers.OfType<Powers.KuHanXingPower>() ?? [])
        {
            if (power.TryApplyTo(card))
            {
                break;
            }
        }

        if (card is ShaCard
            && (card.Owner is null || EquipmentQueries.GetEquipped<ZhuGeLianNu>(card.Owner, EquipmentSlotType.Weapon) is null)
            && BattleState.HasPlayedShaThisTurn(card))
        {
            CancelPlayCardMethod.Invoke(__instance, []);
            return false;
        }

        if (card is ShaCard shaCard && shaCard.Owner is not null && OtherRelicChecks.HasRelic<FeatherArrowRelic>(shaCard.Owner)
            && OtherRelicState.CanUseFeatherArrow(shaCard.Owner))
        {
            RuntimeReflection.TryReduceCardCostForTurn(shaCard, 1);
        }

        if (card is EquipmentCard
            && (card.Owner is null || !RuntimeReflection.IsEquipmentEnabledForOwner(card.Owner)))
        {
            CancelPlayCardMethod.Invoke(__instance, []);
            return false;
        }

        if (card is EquipmentCard equipmentCard && equipmentCard.Owner is not null
            && OtherRelicChecks.HasRelic<FormationChartRelic>(equipmentCard.Owner))
        {
            RuntimeReflection.TrySetCardCostForTurn(equipmentCard, 1);
        }

        if (card is Jiu && card.Owner is not null && BattleState.IsDrunk(card.Owner))
        {
            CancelPlayCardMethod.Invoke(__instance, []);
            return false;
        }

        if (card is Jiu jiu && jiu.Owner is not null && OtherRelicChecks.HasRelic<WinePouchRelic>(jiu.Owner))
        {
            RuntimeReflection.TrySetCardExhaust(jiu, false);
        }

        if (card is NanManRuQin && !RuntimeReflection.HasCardInHand<ShaCard>(card.Owner))
        {
            CancelPlayCardMethod.Invoke(__instance, []);
            return false;
        }

        if (card is TuXi && RuntimeReflection.FindFirstCardInHand<ShaCard>(card.Owner, card) is null)
        {
            CancelPlayCardMethod.Invoke(__instance, []);
            return false;
        }

        if (card is JianShouDaiYuan && RuntimeReflection.HasCardInHand<ShaCard>(card.Owner))
        {
            CancelPlayCardMethod.Invoke(__instance, []);
            return false;
        }

        return true;
    }
}
