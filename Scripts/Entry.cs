using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts.Characters;
using MyFirstStS2Mod.Scripts.Monsters;
using MyFirstStS2Mod.Scripts.Relics;

namespace MyFirstStS2Mod.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "MyFirstStS2Mod";
    public static readonly RitsuModLogger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var harmony = new Harmony("sts2.local.myfirststs2mod");
        harmony.PatchAll(assembly);
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        AoHuiCurseRuntime.Initialize();
        Act2CurseRuntime.Initialize();
        QiQiaoRuntime.Initialize();
        IdentityBadgeRuntime.Initialize();
        OtherRelicRuntime.Initialize();
        EventRelicRuntime.Initialize();
        MonsterRuntime.Initialize();
        RitsuLibFramework.CreateContentPack(ModId)
            .Character<SoldierCharacter>(entry => entry
                .AddStartingCard<Sha>(3)
                .AddStartingCard<Shan>(4)
                .AddStartingCard<Duel>(1)
                .AddStartingCard<BeiZhan>(1)
                .AddStartingCard<XuShiDaiFa>(1)
                .AddStartingRelic<IdentityBadgeRelic>())
            .Relic<SoldierRelicPool, IdentityBadgeRelic>()
            .Relic<SoldierRelicPool, FireOilRelic>()
            .Relic<SoldierRelicPool, SecretLetterRelic>()
            .Relic<SoldierRelicPool, WarDrumRelic>()
            .Relic<SoldierRelicPool, MilitaryRationsRelic>()
            .Relic<SoldierRelicPool, FeatherArrowRelic>()
            .Relic<SoldierRelicPool, ColdIronRelic>()
            .Relic<SoldierRelicPool, FormationChartRelic>()
            .Relic<SoldierRelicPool, AmuletRelic>()
            .Relic<SoldierRelicPool, ChainSchemeRelic>()
            .Relic<SoldierRelicPool, TigerTallyRelic>()
            .Relic<SoldierRelicPool, OfficialSealRelic>()
            .Relic<SoldierRelicPool, FortressMapRelic>()
            .Relic<SoldierRelicPool, WinePouchRelic>()
            .Relic<EventRelicPool, JinNangCanYeRelic>()
            .Relic<EventRelicPool, HaoJieRelic>()
            .Relic<EventRelicPool, HaoJieYouYiRelic>()
            .Relic<EventRelicPool, ZhengZhaoLingRelic>()
            .Relic<EventRelicPool, HuangJinShanYiRelic>()
            .Apply();
        Logger.LogInfo("MyFirstStS2Mod initialized.");
    }
}
