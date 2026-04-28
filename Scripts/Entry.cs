using System.Reflection;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

namespace MyFirstStS2Mod.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "MyFirstStS2Mod";
    public static readonly RitsuModLogger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        Logger.LogInfo("MyFirstStS2Mod initialized.");
    }
}
