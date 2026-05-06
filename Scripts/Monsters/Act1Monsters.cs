using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstStS2Mod.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.MonsterMoves;

namespace MyFirstStS2Mod.Scripts.Monsters;

public abstract class MyFirstMonster : ModMonsterTemplate
{
    protected const string PlaceholderMonsterScenePath = $"res://{Entry.ModId}/scenes/monsters/placeholder_monster.tscn";

    public override MonsterAssetProfile AssetProfile => new(
        VisualsScenePath: PlaceholderMonsterScenePath
    );

    protected static int GetScaledOrBase(int baseValue)
    {
        return AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, baseValue, baseValue);
    }

    protected static int GetStrength(Creature creature)
    {
        return RuntimeReflection.GetPower<StrengthPower>(creature) is { } strength
            ? (int)strength.Amount
            : 0;
    }

    protected static Creature? GetSingleTarget(IReadOnlyList<Creature> targets)
    {
        return targets.FirstOrDefault();
    }
}

[RegisterMonster]
public sealed class DangHuan : MyFirstMonster
{
    public override int MinInitialHp => 100;
    public override int MaxInitialHp => 100;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<DangHuanJieDangPower>(Creature, 1, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var luanZheng = new MoveState(
            "LUAN_ZHENG",
            LuanZhengMove);
        var danHe = new MoveState(
            "DAN_HE",
            DanHeMove,
            new SingleAttackIntent(10));
        var jieDang = new MoveState(
            "JIE_DANG",
            JieDangMove);
        var biYou = new MoveState(
            "BI_YOU",
            BiYouMove,
            new DefendIntent());

        return ModMonsterMoveStateMachines.Cycle(luanZheng, danHe, jieDang, biYou);
    }

    private async Task LuanZhengMove(IReadOnlyList<Creature> targets)
    {
        var target = GetSingleTarget(targets);
        if (target is null)
        {
            return;
        }

        await PowerCmd.Apply<WeakPower>(target, 1, Creature, null);
        await PowerCmd.Apply<VulnerablePower>(target, 1, Creature, null);
    }

    private async Task DanHeMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd
            .Attack(10 + GetStrength(Creature))
            .FromMonster(this)
            .Execute(null);
    }

    private async Task JieDangMove(IReadOnlyList<Creature> targets)
    {
        var slot = RuntimeReflection.FindFirstEmptyEncounterSlot(RuntimeReflection.GetCombatState(Creature),
            ["reserve_left", "reserve_right", "minion_left", "minion_right"]);
        if (slot is not null)
        {
            RuntimeReflection.TrySummonMonsterToCombat<HuanDangZhaoYa>(RuntimeReflection.GetCombatState(Creature), slot);
        }

        await Task.CompletedTask;
    }

    private async Task BiYouMove(IReadOnlyList<Creature> targets)
    {
        var block = RuntimeReflection.GetLivingAllies(Creature)
            .Count(creature => RuntimeReflection.GetCreatureModel(creature) is HuanDangZhaoYa) * 3;
        await CreatureCmd.GainBlock(Creature, block, ValueProp.Move, null);
    }
}

[RegisterMonster]
public sealed class HuanDangZhaoYa : MyFirstMonster
{
    public override int MinInitialHp => 25;
    public override int MaxInitialHp => 25;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var gunDa = new MoveState(
            "GUN_DA",
            GunDaMove,
            new SingleAttackIntent(6));
        var huShen = new MoveState(
            "HU_SHEN",
            HuShenMove,
            new DefendIntent());
        var zhuShi = new MoveState(
            "ZHU_SHI",
            ZhuShiMove);

        return ModMonsterMoveStateMachines.Cycle(gunDa, huShen, zhuShi);
    }

    private async Task GunDaMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(6 + GetStrength(Creature)).FromMonster(this).Execute(null);
    }

    private async Task HuShenMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(Creature, 6, ValueProp.Move, null);
    }

    private async Task ZhuShiMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<StrengthPower>(Creature, 1, Creature, null);
    }
}

[RegisterMonster]
public sealed class HeJin : MyFirstMonster
{
    public override int MinInitialHp => 175;
    public override int MaxInitialHp => 175;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var jiQuan = new MoveState("JI_QUAN", JiQuanMove);
        var junLing = new MoveState("JUN_LING", JunLingMove);
        var gongJi1 = new MoveState("GONG_JI", AttackMove, new SingleAttackIntent(6));
        var gongJi2 = new MoveState("GONG_JI_REPEAT_1", AttackMove, new SingleAttackIntent(6));
        var gongJi3 = new MoveState("GONG_JI_REPEAT_2", AttackMove, new SingleAttackIntent(6));
        var zhengBei = new MoveState("ZHENG_BEI", ZhengBeiMove, new DefendIntent());

        return ModMonsterMoveStateMachines.Cycle(jiQuan, junLing, gongJi1, gongJi2, gongJi3, zhengBei);
    }

    private async Task JiQuanMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<HeJinBingQuanPower>(Creature, 3, Creature, null);
        await PowerCmd.Apply<StrengthPower>(Creature, -1, Creature, null);
    }

    private async Task JunLingMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<HeJinJunLingPower>(Creature, 1, Creature, null);
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        var singleHitDamage = Math.Max(0, 6 + GetStrength(Creature));
        var bingQuan = RuntimeReflection.GetPower<HeJinBingQuanPower>(Creature);
        var extraHits = bingQuan is null ? 0 : (int)bingQuan.Amount;
        var hitCount = 1 + extraHits;

        var totalDamage = singleHitDamage * hitCount;
        var junLing = RuntimeReflection.GetPower<HeJinJunLingPower>(Creature);
        if (junLing is not null)
        {
            totalDamage = (int)Math.Ceiling(totalDamage * 1.5m);
        }

        var baseHitDamage = hitCount == 0 ? 0 : totalDamage / hitCount;
        var remainder = hitCount == 0 ? 0 : totalDamage % hitCount;
        for (var i = 0; i < hitCount; i++)
        {
            var hitDamage = baseHitDamage + (i < remainder ? 1 : 0);
            await DamageCmd.Attack(hitDamage).FromMonster(this).Execute(null);
        }

        if (bingQuan is not null)
        {
            if (bingQuan.Amount <= 1)
            {
                await PowerCmd.Remove(bingQuan);
            }
            else
            {
                bingQuan.Amount -= 1;
            }
        }

        if (junLing is not null)
        {
            await PowerCmd.Remove(junLing);
        }
    }

    private async Task ZhengBeiMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<StrengthPower>(Creature, 1, Creature, null);
        await CreatureCmd.Heal(Creature, 8, true);
    }
}

[RegisterMonster]
public sealed class QingYiKuiShou : MyFirstMonster
{
    public override int MinInitialHp => 150;
    public override int MaxInitialHp => 150;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var qingTan = new MoveState("QING_TAN", QingTanMove);
        var biFa = new MoveState("BI_FA", BiFaMove, new SingleAttackIntent(13));
        var dianPing = new MoveState("DIAN_PING", DianPingMove);
        var ziBian = new MoveState("ZI_BIAN", ZiBianMove, new DefendIntent());

        return ModMonsterMoveStateMachines.Cycle(qingTan, biFa, dianPing, ziBian);
    }

    private async Task QingTanMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<QingTanPower>(Creature, 1, Creature, null);
    }

    private async Task BiFaMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(13 + GetStrength(Creature)).FromMonster(this).Execute(null);
    }

    private async Task DianPingMove(IReadOnlyList<Creature> targets)
    {
        var target = GetSingleTarget(targets);
        if (target is null)
        {
            return;
        }

        if (Random.Shared.Next(2) == 0)
        {
            await PowerCmd.Apply<QingYiForbidAttackPower>(target, 1, Creature, null);
        }
        else
        {
            await PowerCmd.Apply<QingYiForbidSkillPower>(target, 1, Creature, null);
        }
    }

    private async Task ZiBianMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(Creature, 12, ValueProp.Move, null);
        await PowerCmd.Apply<QingYiReflectPower>(Creature, 3, Creature, null);
    }
}

[RegisterMonster]
public sealed class JinWuWei : MyFirstMonster
{
    public override int MinInitialHp => 80;
    public override int MaxInitialHp => 80;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<JinWuWeiJieYanPower>(Creature, 1, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var zhenYa = new MoveState("ZHEN_YA", ZhenYaMove, new SingleAttackIntent(10));
        var lieZhen = new MoveState("LIE_ZHEN", LieZhenMove, new DefendIntent());

        return ModMonsterMoveStateMachines.Cycle(zhenYa, lieZhen);
    }

    private async Task ZhenYaMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(10 + GetStrength(Creature)).FromMonster(this).Execute(null);
    }

    private async Task LieZhenMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(Creature, 8, ValueProp.Move, null);
    }
}

public abstract class HuangMenXiaoGuiBase : MyFirstMonster
{
    protected abstract string[] MoveOrder { get; }

    public override int MinInitialHp => 40;
    public override int MaxInitialHp => 40;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var counters = new Dictionary<string, int>(StringComparer.Ordinal);
        var cycle = MoveOrder.Select(baseId =>
        {
            counters[baseId] = counters.TryGetValue(baseId, out var count) ? count + 1 : 1;
            var id = $"{baseId}_{counters[baseId]}";
            return baseId switch
            {
                "QIANG_SUO" => new MoveState(id, QiangSuoMove),
                "MAN_MA" => new MoveState(id, ManMaMove, new SingleAttackIntent(2)),
                "BAO_TUAN" => new MoveState(id, BaoTuanMove, new DefendIntent()),
                _ => new MoveState(id, CheLiMove)
            };
        }).ToArray();

        return ModMonsterMoveStateMachines.Cycle(cycle);
    }

    private async Task QiangSuoMove(IReadOnlyList<Creature> targets)
    {
        var target = GetSingleTarget(targets);
        if (target is null)
        {
            return;
        }

        var lostGold = Math.Min(20, RuntimeReflection.GetCurrentGold(target));
        if (lostGold > 0)
        {
            RuntimeReflection.TryModifyPlayerGold(target, -lostGold);
            MonsterRuntime.RecordHuangMenStolenGold(Creature, lostGold);
        }

        await PowerCmd.Apply<WeakPower>(target, 1, Creature, null);
    }

    private async Task ManMaMove(IReadOnlyList<Creature> targets)
    {
        var singleHitDamage = Math.Max(0, 2 + GetStrength(Creature));
        for (var i = 0; i < 4; i++)
        {
            await DamageCmd.Attack(singleHitDamage).FromMonster(this).Execute(null);
        }
    }

    private async Task BaoTuanMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<HuangMenBaoTuanPower>(Creature, 2, Creature, null);
    }

    private async Task CheLiMove(IReadOnlyList<Creature> targets)
    {
        MonsterRuntime.MarkHuangMenEscaped(Creature);
        if (!RuntimeReflection.TryEscapeCreature(Creature))
        {
            RuntimeReflection.TryForceKillCreature(Creature);
        }

        await Task.CompletedTask;
    }
}

[RegisterMonster]
public sealed class HuangMenXiaoGuiLeft : HuangMenXiaoGuiBase
{
    protected override string[] MoveOrder =>
    [
        "QIANG_SUO", "MAN_MA", "BAO_TUAN",
        "QIANG_SUO", "MAN_MA", "BAO_TUAN",
        "QIANG_SUO", "MAN_MA", "BAO_TUAN",
        "CHE_LI"
    ];
}

[RegisterMonster]
public sealed class HuangMenXiaoGuiMiddle : HuangMenXiaoGuiBase
{
    protected override string[] MoveOrder =>
    [
        "MAN_MA", "BAO_TUAN", "QIANG_SUO",
        "MAN_MA", "BAO_TUAN", "QIANG_SUO",
        "MAN_MA", "BAO_TUAN", "QIANG_SUO",
        "CHE_LI"
    ];
}

[RegisterMonster]
public sealed class HuangMenXiaoGuiRight : HuangMenXiaoGuiBase
{
    protected override string[] MoveOrder =>
    [
        "BAO_TUAN", "QIANG_SUO", "MAN_MA",
        "BAO_TUAN", "QIANG_SUO", "MAN_MA",
        "BAO_TUAN", "QIANG_SUO", "MAN_MA",
        "CHE_LI"
    ];
}

[RegisterActEncounter(typeof(Overgrowth))]
public sealed class DangHuanEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    [
        ModelDb.Monster<DangHuan>(),
        ModelDb.Monster<HuanDangZhaoYa>()
    ];

    public override RoomType RoomType => RoomType.Boss;
    public override EncounterAssetProfile AssetProfile => new(
        EncounterScenePath: $"res://{Entry.ModId}/scenes/encounters/dang_huan_encounter.tscn"
    );

    public override IReadOnlyList<string> Slots => ["boss", "minion_left", "minion_right", "reserve_left", "reserve_right"];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<DangHuan>().ToMutable(), "boss"),
        (ModelDb.Monster<HuanDangZhaoYa>().ToMutable(), "minion_left"),
        (ModelDb.Monster<HuanDangZhaoYa>().ToMutable(), "minion_right")
    ];
}

[RegisterActEncounter(typeof(Overgrowth))]
public sealed class HeJinEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<HeJin>()];
    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<HeJin>().ToMutable(), null)
    ];
}

[RegisterActEncounter(typeof(Overgrowth))]
public sealed class QingYiKuiShouEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<QingYiKuiShou>()];
    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<QingYiKuiShou>().ToMutable(), null)
    ];
}

[RegisterActEncounter(typeof(Overgrowth))]
public sealed class JinWuWeiEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<JinWuWei>()];
    public override RoomType RoomType => RoomType.Elite;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<JinWuWei>().ToMutable(), null)
    ];
}

[RegisterActEncounter(typeof(Overgrowth))]
public sealed class HuangMenXiaoGuiEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    [
        ModelDb.Monster<HuangMenXiaoGuiLeft>(),
        ModelDb.Monster<HuangMenXiaoGuiMiddle>(),
        ModelDb.Monster<HuangMenXiaoGuiRight>()
    ];

    public override RoomType RoomType => RoomType.Elite;
    public override EncounterAssetProfile AssetProfile => new(
        EncounterScenePath: $"res://{Entry.ModId}/scenes/encounters/huang_men_xiao_gui_encounter.tscn"
    );

    public override IReadOnlyList<string> Slots => ["left", "center", "right"];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<HuangMenXiaoGuiLeft>().ToMutable(), "left"),
        (ModelDb.Monster<HuangMenXiaoGuiMiddle>().ToMutable(), "center"),
        (ModelDb.Monster<HuangMenXiaoGuiRight>().ToMutable(), "right")
    ];
}
