using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Relics;

public abstract class MyFirstRelic : ModRelicTemplate
{
    private const string PlaceholderRelicPath = $"res://{Entry.ModId}/images/relics/placeholder_relic.svg";

    public override RelicAssetProfile AssetProfile => new(
        IconPath: ResolveRelicAssetPath(),
        IconOutlinePath: ResolveRelicAssetPath(),
        BigIconPath: ResolveRelicAssetPath());

    private string ResolveRelicAssetPath()
    {
        var pngPath = $"res://{Entry.ModId}/images/relics/{GetType().Name}.png";
        var svgPath = $"res://{Entry.ModId}/images/relics/{GetType().Name}.svg";

        if (ResourceLoader.Exists(pngPath))
        {
            return pngPath;
        }

        if (ResourceLoader.Exists(svgPath))
        {
            return svgPath;
        }

        return PlaceholderRelicPath;
    }
}
