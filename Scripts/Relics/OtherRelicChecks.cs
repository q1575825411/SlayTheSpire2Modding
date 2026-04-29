using MegaCrit.Sts2.Core.Entities.Relics;

namespace MyFirstStS2Mod.Scripts.Relics;

internal static class OtherRelicChecks
{
    public static bool HasRelic<TRelic>(object? owner) where TRelic : RelicModel
    {
        return RuntimeReflection.GetOwnedRelics(owner).Any(relic => relic is TRelic);
    }

    public static int ModifyScorchAmount(object? owner, object? target, int amount)
    {
        var modified = amount;
        if (owner is not null && target is not null && HasRelic<ChainSchemeRelic>(owner))
        {
            modified += 1;
        }

        return modified;
    }

    public static int ModifyColdAmount(object? owner, object? target, int amount)
    {
        var modified = amount;
        if (owner is not null && target is not null && HasRelic<ChainSchemeRelic>(owner))
        {
            modified += 1;
        }

        if (owner is not null && target is not null && HasRelic<ColdIronRelic>(owner) && OtherRelicState.TryConsumeColdIron(owner))
        {
            modified += 2;
        }

        return modified;
    }
}
