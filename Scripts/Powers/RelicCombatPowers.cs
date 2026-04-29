using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Relics;

[RegisterPower]
public sealed class SecretLetterRelicPower : ModPowerTemplate
{
    private const string SelectionPrompt = "选择1张要保留到下回合的手牌";

    public override PowerType Type => PowerType.Buff;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || RuntimeReflection.GetCombatRoundNumber(CombatState) != 1)
        {
            return;
        }

        var handCards = RuntimeReflection.GetHandCards(Owner);
        if (handCards.Count > 0)
        {
            var prefs = new CardSelectorPrefs(SelectionPrompt, 1);
            var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, handCards, Owner, prefs)).FirstOrDefault();
            if (chosen is not null)
            {
                RuntimeReflection.TrySetCardRetain(chosen, true);
            }
        }

        await PowerCmd.Remove(this);
    }
}

[RegisterPower]
public sealed class WarDrumRelicPower : ModPowerTemplate
{
    private int _grantedStrength;
    private bool _removedStrength;

    public override PowerType Type => PowerType.Buff;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || _grantedStrength >= 3)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(Owner, 1, Owner, null);
        _grantedStrength++;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || _removedStrength || RuntimeReflection.GetCombatRoundNumber(CombatState) != 3)
        {
            return;
        }

        var strength = RuntimeReflection.GetPower<StrengthPower>(Owner);
        if (strength is not null && _grantedStrength > 0)
        {
            await PowerCmd.ModifyAmount(strength, -_grantedStrength, null, null);
        }

        _removedStrength = true;
        await PowerCmd.Remove(this);
    }
}

[RegisterPower]
public sealed class MilitaryRationsRelicPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !Owner.IsAlive)
        {
            return;
        }

        var maxHp = RuntimeReflection.GetMaxHp(Owner);
        if (maxHp <= 0)
        {
            return;
        }

        if (RuntimeReflection.GetCurrentHp(Owner) >= Math.Ceiling(maxHp * 0.5m))
        {
            return;
        }

        var healAmount = Math.Max(1, (int)Math.Ceiling(maxHp * 0.1m));
        await CreatureCmd.Heal(Owner, healAmount, true);
        await PowerCmd.Remove(this);
    }
}

[RegisterPower]
public sealed class AmuletRelicPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || RuntimeReflection.GetCurrentBlock(Owner) > 0)
        {
            return;
        }

        var shanCount = RuntimeReflection.CountCardsOwnedBy<Cards.Shan>(Owner);
        if (shanCount <= 0)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner, shanCount * 2m, ValueProp.Unpowered, null);
    }
}
