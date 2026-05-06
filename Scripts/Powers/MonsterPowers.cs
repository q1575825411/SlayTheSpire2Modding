using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstStS2Mod.Scripts.Monsters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public sealed class DangHuanJieDangPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var livingMinions = RuntimeReflection.GetLivingAllies(Owner)
            .Count(creature => RuntimeReflection.GetCreatureModel(creature) is HuanDangZhaoYa);
        if (livingMinions > 0)
        {
            await PowerCmd.Apply<StrengthPower>(Owner, livingMinions, Owner, null);
        }
    }
}

[RegisterPower]
public sealed class HeJinBingQuanPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

[RegisterPower]
public sealed class HeJinJunLingPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

[RegisterPower]
public sealed class JinWuWeiJieYanPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
}

[RegisterPower]
public sealed class QingTanPower : ModPowerTemplate
{
    private int _cardsPlayedThisWindow;

    public override PowerType Type => PowerType.Buff;

    public async Task RegisterCardPlayed()
    {
        _cardsPlayedThisWindow++;
        if (_cardsPlayedThisWindow % 3 == 0)
        {
            await PowerCmd.Apply<DexterityPower>(Owner, 1, Owner, null);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

[RegisterPower]
public sealed class QingYiForbidAttackPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}

[RegisterPower]
public sealed class QingYiForbidSkillPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}

[RegisterPower]
public sealed class QingYiReflectPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || dealer is null || dealer.Side == Owner.Side)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, [dealer], Amount, ValueProp.Unpowered | ValueProp.Unblockable, Owner);
    }

    public override async Task AfterSideTurnStart(CombatSide side, MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

[RegisterPower]
public sealed class HuangMenBaoTuanPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !target.IsAlive || result.UnblockedDamage <= 0)
        {
            return;
        }

        await CreatureCmd.Heal(target, (int)Amount, true);
    }

    public override async Task AfterSideTurnStart(CombatSide side, MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
