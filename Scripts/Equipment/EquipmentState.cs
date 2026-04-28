using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Cards;
using MyFirstStS2Mod.Scripts.Cards;

namespace MyFirstStS2Mod.Scripts.Equipment;

internal static class EquipmentState
{
    private sealed class OwnerEquipmentState
    {
        public Dictionary<EquipmentSlotType, EquipmentCard> Equipped { get; } = new();
    }

    private static readonly ConditionalWeakTable<object, OwnerEquipmentState> States = new();

    public static EquipmentCard? GetEquipped(object owner, EquipmentSlotType slot)
    {
        return States.GetOrCreateValue(owner).Equipped.GetValueOrDefault(slot);
    }

    public static TEquipment? GetEquipped<TEquipment>(object owner, EquipmentSlotType slot)
        where TEquipment : EquipmentCard
    {
        return GetEquipped(owner, slot) as TEquipment;
    }

    public static IReadOnlyDictionary<EquipmentSlotType, EquipmentCard> GetAllEquipped(object owner)
    {
        return States.GetOrCreateValue(owner).Equipped;
    }

    public static bool IsEquipped(object owner, EquipmentCard equipment)
    {
        return GetEquipped(owner, equipment.SlotType) == equipment;
    }

    public static async Task Equip(object owner, EquipmentCard equipment)
    {
        var state = States.GetOrCreateValue(owner);
        if (state.Equipped.TryGetValue(equipment.SlotType, out var previous))
        {
            await previous.HandleUnequipped();
        }

        state.Equipped[equipment.SlotType] = equipment;
        await equipment.HandleEquipped();
    }

    public static async Task UnequipIfCurrent(object owner, EquipmentCard equipment)
    {
        var state = States.GetOrCreateValue(owner);
        if (!state.Equipped.TryGetValue(equipment.SlotType, out var current) || current != equipment)
        {
            return;
        }

        state.Equipped.Remove(equipment.SlotType);
        await equipment.HandleUnequipped();
    }

    public static async Task UnequipSlot(object owner, EquipmentSlotType slot)
    {
        var state = States.GetOrCreateValue(owner);
        if (!state.Equipped.TryGetValue(slot, out var current))
        {
            return;
        }

        state.Equipped.Remove(slot);
        await current.HandleUnequipped();
    }

    public static bool HasEquipment(object owner, EquipmentSlotType slot)
    {
        return GetEquipped(owner, slot) is not null;
    }
}
