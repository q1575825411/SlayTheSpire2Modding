using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstStS2Mod.Scripts.Equipment;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class EquipmentCard : MyFirstCard
{
    private static readonly CardType ResolvedEquipmentCardType = ResolveEquipmentCardType();

    public abstract EquipmentSlotType SlotType { get; }

    protected EquipmentCard(
        int energyCost,
        CardRarity rarity,
        TargetType targetType = TargetType.None,
        bool shouldShowInCardLibrary = true)
        : base(energyCost, ResolvedEquipmentCardType, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public bool IsCurrentlyEquipped => Owner is not null && EquipmentState.IsEquipped(Owner, this);

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await EquipmentState.Equip(Owner, this);
        await OnEquippedFromPlay(choiceContext, cardPlay);
    }

    protected virtual Task OnEquippedFromPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnEquipped()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnUnequipped()
    {
        return Task.CompletedTask;
    }

    // Equipment cards follow the current design rule: they do not participate in card upgrades.
    protected sealed override void OnUpgrade()
    {
    }

    internal Task HandleEquipped()
    {
        return OnEquipped();
    }

    internal Task HandleUnequipped()
    {
        return OnUnequipped();
    }

    private static CardType ResolveEquipmentCardType()
    {
        return Enum.TryParse<CardType>("Equipment", out var equipmentType)
            ? equipmentType
            : CardType.Power;
    }
}
