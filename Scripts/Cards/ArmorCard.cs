using MyFirstStS2Mod.Scripts.Equipment;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class ArmorCard : EquipmentCard
{
    public sealed override EquipmentSlotType SlotType => EquipmentSlotType.Armor;

    protected ArmorCard(int energyCost, CardRarity rarity)
        : base(energyCost, rarity)
    {
    }
}
