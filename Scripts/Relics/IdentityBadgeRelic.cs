using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Utils;

namespace MyFirstStS2Mod.Scripts.Relics;

public sealed class IdentityBadgeRelic : MyFirstRelic
{
    private static readonly SavedAttachedState<IdentityBadgeRelic, IdentityRole> AssignedRole =
        new("assigned_role", () => IdentityRole.Unassigned);

    private static readonly SavedAttachedState<IdentityBadgeRelic, bool> LordBonusApplied =
        new("lord_bonus_applied", () => false);

    private static readonly SavedAttachedState<IdentityBadgeRelic, int> LastTraitorActIndex =
        new("traitor_last_act_index", () => -1);

    public override RelicRarity Rarity => RelicRarity.Starter;

    public void InitializeIdentity()
    {
        EnsureIdentityAssigned();
        EnsureLordBonusApplied();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        InitializeIdentity();

        if (player != Owner || ResolveAssignedRole() != IdentityRole.Rebel || combatState.RoundNumber != 1)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, 2, player);
        Flash();
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        InitializeIdentity();

        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
        {
            return;
        }

        if (ResolveAssignedRole() != IdentityRole.Loyalist)
        {
            return;
        }

        await PowerCmd.Apply<PlatingPower>(Owner.Creature, 3, Owner.Creature, null);
        Flash();
    }

    public override Task AfterActEntered()
    {
        InitializeIdentity();

        if (ResolveAssignedRole() != IdentityRole.Traitor)
        {
            return Task.CompletedTask;
        }

        var currentAct = RuntimeReflection.GetCurrentActIndex(Owner);
        if (currentAct >= 0 && LastTraitorActIndex[this] == currentAct)
        {
            return Task.CompletedTask;
        }

        LastTraitorActIndex[this] = currentAct;
        Flash();
        Entry.Logger.LogWarn("IdentityBadgeRelic: '内奸' 的额外卡牌奖励仍是占位实现，当前仅记录阶段进入。");
        return Task.CompletedTask;
    }

    private IdentityRole ResolveAssignedRole()
    {
        return EnsureIdentityAssigned();
    }

    private IdentityRole EnsureIdentityAssigned()
    {
        if (AssignedRole[this] != IdentityRole.Unassigned)
        {
            return AssignedRole[this];
        }

        AssignedRole[this] = (IdentityRole)Random.Shared.Next((int)IdentityRole.Lord, (int)IdentityRole.Traitor + 1);
        return AssignedRole[this];
    }

    private void EnsureLordBonusApplied()
    {
        if (LordBonusApplied[this] || ResolveAssignedRole() != IdentityRole.Lord)
        {
            return;
        }

        if (!RuntimeReflection.TryIncreaseMaxHp(Owner?.Creature, 5))
        {
            return;
        }

        LordBonusApplied[this] = true;
        Flash();
    }
}
