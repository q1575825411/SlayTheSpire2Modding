using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Characters;

public sealed class SoldierCardPool : TypeListCardPoolModel
{
    public override string Title => "soldier";

    public override string EnergyColorName => "soldier";

    public override Color DeckEntryCardColor => new(0.77f, 0.23f, 0.20f);

    public override Color EnergyOutlineColor => new(0.77f, 0.23f, 0.20f);

    public override bool IsColorless => false;
}
