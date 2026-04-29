using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstStS2Mod.Scripts.Equipment;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class ShaCard : MyFirstCard
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.DrunkPower>()
    ];

    protected ShaCard(int damage, CardRarity rarity)
        : base(1, CardType.Attack, rarity, TargetType.AnyEnemy)
    {
        BaseDamage = damage;
    }

    protected int BaseDamage { get; private set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BaseDamage, ValueProp.Move)
    ];

    protected int ResolveShaDamage(Creature target)
    {
        var damage = DynamicVars.Damage.BaseValue;
        if (BattleState.TryConsumeNextShaDealsDoubleDamage(Owner))
        {
            damage *= 2;
        }

        damage += BattleState.TryConsumeNextShaFlatDamage(Owner);

        damage += (int)Owner.Powers.OfType<Powers.ZhanChangPower>().Sum(power => power.Amount);

        if (EquipmentQueries.GetEquipped<HanBingJian>(Owner, EquipmentSlotType.Weapon) is not null)
        {
            damage = (int)Math.Floor(damage / 2m);
        }

        if (EquipmentQueries.GetEquipped<GuDingDao>(Owner, EquipmentSlotType.Weapon) is not null
            && RuntimeReflection.GetCurrentBlock(target) <= 0)
        {
            damage = (int)Math.Floor(damage * 1.25m);
        }

        return damage;
    }

    protected sealed record ShaAttackResult(Creature Target, bool DealtHpDamage);

    protected async Task<ShaAttackResult> DealShaDamage(PlayerChoiceContext choiceContext, CardPlay cardPlay, int damage)
    {
        var target = cardPlay.Target!;
        var hpBefore = target.CurrentHp;
        var ignoreBlock = EquipmentQueries.GetEquipped<QingGangJian>(Owner, EquipmentSlotType.Weapon) is not null;

        if (EquipmentQueries.GetEquipped<CiXiongShuangGuJian>(Owner, EquipmentSlotType.Weapon) is not null
            && RuntimeReflection.IsIntentAttack(target))
        {
            await PowerCmd.Apply<WeakPower>(target, 1, Owner, this);
        }

        if (ignoreBlock)
        {
            await CreatureCmd.Damage(choiceContext, [target], damage, ValueProp.Unblockable | ValueProp.Move, Owner);
        }
        else
        {
            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
        }

        var dealtHpDamage = target.CurrentHp < hpBefore;

        if (dealtHpDamage)
        {
            foreach (var power in Owner.Powers.OfType<Powers.LianJiPower>())
            {
                await power.OnShaDealtHpDamage(choiceContext);
            }

            if (EquipmentQueries.GetEquipped<FangTianHuaJi>(Owner, EquipmentSlotType.Weapon) is not null)
            {
                foreach (var other in RuntimeReflection.GetLivingOpponents(Owner).Where(creature => creature != target))
                {
                    await CreatureCmd.Damage(choiceContext, [other], 4m, ValueProp.Move, Owner);
                }
            }

            if (EquipmentQueries.GetEquipped<HanBingJian>(Owner, EquipmentSlotType.Weapon) is not null)
            {
                await PowerCmd.Apply<Powers.ColdPower>(target, 8, Owner, this);
            }

            if (EquipmentQueries.GetEquipped<GuanShiFu>(Owner, EquipmentSlotType.Weapon) is not null
                && await RuntimeReflection.TryDiscardCards(choiceContext, Owner, 2, this))
            {
                if (ignoreBlock)
                {
                    await CreatureCmd.Damage(choiceContext, [target], damage, ValueProp.Unblockable | ValueProp.Move, Owner);
                }
                else
                {
                    await DamageCmd.Attack(damage)
                        .FromCard(this)
                        .Targeting(target)
                        .Execute(choiceContext);
                }
            }
        }
        else if (EquipmentQueries.GetEquipped<QingLongYanYueDao>(Owner, EquipmentSlotType.Weapon) is not null
            && BattleState.TryConsumeQingLongTrigger(Owner)
            && await RuntimeReflection.TryDiscardCards(choiceContext, Owner, 1, this))
        {
            await CreatureCmd.Damage(choiceContext, [target], 8m, ValueProp.Move, Owner);
        }

        var wuLiuJianPower = Owner.Powers.OfType<Powers.WuLiuJianPower>().FirstOrDefault();
        if (wuLiuJianPower is not null)
        {
            wuLiuJianPower.NotifyShaPlayed();
        }

        return new ShaAttackResult(target, dealtHpDamage);
    }

    protected async Task ApplyCommonShaScorch(PlayerChoiceContext choiceContext, Creature target)
    {
        var extraScorch = BattleState.TryConsumeNextAttackExtraScorch(Owner);
        if (EquipmentQueries.GetEquipped<ZhuQueYuShan>(Owner, EquipmentSlotType.Weapon) is not null)
        {
            extraScorch += 4;
        }

        if (extraScorch > 0)
        {
            await PowerCmd.Apply<Powers.ScorchPower>(target, extraScorch, Owner, this);
        }
    }
}
