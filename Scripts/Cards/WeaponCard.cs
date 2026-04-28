using MyFirstStS2Mod.Scripts.Equipment;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class WeaponCard : EquipmentCard
{
    public sealed override EquipmentSlotType SlotType => EquipmentSlotType.Weapon;

    protected WeaponCard(int energyCost, CardRarity rarity)
        : base(energyCost, rarity)
    {
    }
}
