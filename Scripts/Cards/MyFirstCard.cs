using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MyFirstStS2Mod.Scripts.Characters;

namespace MyFirstStS2Mod.Scripts.Cards;

[RegisterCard(typeof(SoldierCardPool), Inherit = true)]
public abstract class MyFirstCard : ModCardTemplate
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://{Entry.ModId}/images/cards/{GetType().Name}.png"
    );

    protected MyFirstCard(
        int energyCost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
