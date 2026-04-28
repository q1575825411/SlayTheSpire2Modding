using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Cards;

[RegisterCard(typeof(ColorlessCardPool), Inherit = true)]
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
