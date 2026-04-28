using MyFirstStS2Mod.Scripts.Equipment;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class TreasureCard : EquipmentCard
{
    public sealed override EquipmentSlotType SlotType => EquipmentSlotType.Treasure;

    protected TreasureCard(int energyCost, CardRarity rarity)
        : base(energyCost, rarity)
    {
    }
}
