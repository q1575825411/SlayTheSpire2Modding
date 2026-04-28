using System.Collections;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MyFirstStS2Mod.Scripts.Powers;

namespace MyFirstStS2Mod.Scripts.Patches;

[HarmonyPatch]
internal static class RenWangDunDebuffPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(PowerCmd)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == "Apply" && method.IsGenericMethodDefinition && method.GetParameters().Length == 4);
    }

    public static bool Prefix(ref Task __result, object __0, MethodBase __originalMethod)
    {
        if (__0?.GetType().GetProperty("Powers")?.GetValue(__0) is not IEnumerable powers)
        {
            return true;
        }

        var shield = powers.OfType<RenWangDunPower>().FirstOrDefault();
        if (shield is null)
        {
            return true;
        }

        var genericArguments = __originalMethod.GetGenericArguments();
        if (genericArguments.Length != 1 || !typeof(PowerModel).IsAssignableFrom(genericArguments[0]))
        {
            return true;
        }

        if (Activator.CreateInstance(genericArguments[0]) is not PowerModel candidatePower || candidatePower.Type != PowerType.Debuff)
        {
            return true;
        }

        if (!shield.TryConsumeDebuffImmunity())
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}
