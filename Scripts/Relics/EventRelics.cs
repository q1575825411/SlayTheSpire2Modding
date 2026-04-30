using MegaCrit.Sts2.Core.Entities.Relics;

namespace MyFirstStS2Mod.Scripts.Relics;

public abstract class EventRelic : MyFirstRelic
{
    public sealed override RelicRarity Rarity => RelicRarity.Event;
}

public sealed class JinNangCanYeRelic : EventRelic;

public sealed class HaoJieRelic : EventRelic;

public sealed class HaoJieYouYiRelic : EventRelic;

public sealed class ZhengZhaoLingRelic : EventRelic;

public sealed class HuangJinShanYiRelic : EventRelic;
