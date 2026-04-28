using MyFirstStS2Mod.Scripts.Cards;

namespace MyFirstStS2Mod.Scripts.Equipment;

public static class EquipmentQueries
{
    public static EquipmentCard? GetEquipped(object owner, EquipmentSlotType slot)
    {
        return EquipmentState.GetEquipped(owner, slot);
    }

    public static TEquipment? GetEquipped<TEquipment>(object owner, EquipmentSlotType slot)
        where TEquipment : EquipmentCard
    {
        return EquipmentState.GetEquipped<TEquipment>(owner, slot);
    }

    public static IReadOnlyDictionary<EquipmentSlotType, EquipmentCard> GetAllEquipped(object owner)
    {
        return EquipmentState.GetAllEquipped(owner);
    }

    public static bool HasEquipment(object owner, EquipmentSlotType slot)
    {
        return EquipmentState.HasEquipment(owner, slot);
    }
}
