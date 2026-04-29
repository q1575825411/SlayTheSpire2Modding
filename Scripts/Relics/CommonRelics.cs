using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace MyFirstStS2Mod.Scripts.Relics;

public sealed class FireOilRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
}

public sealed class SecretLetterRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
        {
            return;
        }

        if (RuntimeReflection.GetPower<SecretLetterRelicPower>(Owner.Creature) is null)
        {
            await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<SecretLetterRelicPower>(Owner.Creature, 1, Owner.Creature, null);
        }
    }
}

public sealed class WarDrumRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
        {
            return;
        }

        if (RuntimeReflection.GetPower<WarDrumRelicPower>(Owner.Creature) is null)
        {
            await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<WarDrumRelicPower>(Owner.Creature, 1, Owner.Creature, null);
        }
    }
}

public sealed class MilitaryRationsRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
        {
            return;
        }

        if (RuntimeReflection.GetPower<MilitaryRationsRelicPower>(Owner.Creature) is null)
        {
            await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<MilitaryRationsRelicPower>(Owner.Creature, 1, Owner.Creature, null);
        }
    }
}

public sealed class FeatherArrowRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
}

public sealed class ColdIronRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
}

public sealed class FormationChartRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
}

public sealed class AmuletRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
        {
            return;
        }

        if (RuntimeReflection.GetPower<AmuletRelicPower>(Owner.Creature) is null)
        {
            await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<AmuletRelicPower>(Owner.Creature, 1, Owner.Creature, null);
        }
    }
}

public sealed class ChainSchemeRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
}

public sealed class TigerTallyRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
}

public sealed class OfficialSealRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal originalPrice)
    {
        if (player != Owner)
        {
            return originalPrice;
        }

        OfficialSealRuntime.RegisterCurrentRoom(NMerchantRoom.Instance);
        OfficialSealRuntime.ObserveEntry(entry);
        return OfficialSealRuntime.IsFreePurchaseSpent ? originalPrice : 0m;
    }

    private static class OfficialSealRuntime
    {
        private static readonly HashSet<object> TrackedEntries = [];
        private static object? _currentRoom;

        public static bool IsFreePurchaseSpent
        {
            get
            {
                foreach (var entry in TrackedEntries.ToArray())
                {
                    if (RuntimeReflection.IsMerchantEntryPurchased(entry))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void RegisterCurrentRoom(object? room)
        {
            if (!ReferenceEquals(_currentRoom, room))
            {
                _currentRoom = room;
                TrackedEntries.Clear();
            }
        }

        public static void ObserveEntry(object entry)
        {
            TrackedEntries.Add(entry);
        }
    }
}

public sealed class FortressMapRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side)
        {
            return;
        }

        if (RuntimeReflection.GetLivingOpponents(Owner).Any(RuntimeReflection.IsIntentAttack))
        {
            return;
        }

        await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
        Flash();
    }
}

public sealed class WinePouchRelic : MyFirstRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}
