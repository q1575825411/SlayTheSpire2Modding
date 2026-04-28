using MyFirstStS2Mod.Scripts.Equipment;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class MountCard : EquipmentCard
{
    public sealed override EquipmentSlotType SlotType => EquipmentSlotType.Mount;

    protected MountCard(int energyCost, CardRarity rarity)
        : base(energyCost, rarity)
    {
    }
}
