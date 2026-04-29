using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Patches;

[HarmonyPatch]
internal static class CardExhaustTriggerPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(CardCmd)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method =>
                (method.Name == "Exhaust" || method.Name == "MoveToExhaust")
                && method.GetParameters().Any(parameter => typeof(CardModel).IsAssignableFrom(parameter.ParameterType)));
    }

    private static void Postfix(object?[] __args)
    {
        var card = __args.OfType<CardModel>().FirstOrDefault();
        var owner = card?.Owner;
        if (owner is null)
        {
            return;
        }

        foreach (var power in owner.Powers.OfType<Powers.JuanTuChongLaiPower>())
        {
            power.NotifyCardExhausted(card!);
        }
    }
}
