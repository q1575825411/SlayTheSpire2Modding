using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib;

namespace MyFirstStS2Mod.Scripts.Relics;

public static class IdentityBadgeRuntime
{
    private static IDisposable? _relicObtainedSubscription;

    public static void Initialize()
    {
        _relicObtainedSubscription ??= RitsuLibFramework.SubscribeLifecycle<RelicObtainedEvent>(OnRelicObtained);
    }

    private static void OnRelicObtained(RelicObtainedEvent evt)
    {
        if (evt.Relic is not IdentityBadgeRelic relic)
        {
            return;
        }

        relic.InitializeIdentity();
    }
}
