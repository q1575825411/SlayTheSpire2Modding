using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using STS2RitsuLib.Scaffolding.Characters;

namespace MyFirstStS2Mod.Scripts.Characters;

public sealed class SoldierCharacter
    : ModCharacterTemplate<SoldierCardPool, SoldierRelicPool, SoldierPotionPool>
{
    public override Color NameColor => new(0.90f, 0.79f, 0.47f);

    public override Color EnergyLabelOutlineColor => new(0.58f, 0.13f, 0.11f);

    public override Color MapDrawingColor => new(0.58f, 0.13f, 0.11f);

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 70;

    public override int StartingGold => 99;

    public override float AttackAnimDelay => 0f;

    public override float CastAnimDelay => 0f;

    public override bool RequiresEpochAndTimeline => false;

    public override string? PlaceholderCharacterId => "ironclad";

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}
