using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;

namespace ShowCardDmgInRewards.ShowCardDmgInRewardsCode;

[HarmonyPatch]
[UsedImplicitly]
public static class CanonicalVarsPatch
{
    public static readonly List<MethodBase> PatchedMethods = [];
    
    [HarmonyTargetMethod]
    [UsedImplicitly]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return (from type in AccessTools.AllTypes()
                where typeof(CardModel).IsAssignableFrom(type)
                select type.GetNestedType("<>c", BindingFlags.NonPublic | BindingFlags.Public)).Where(t => t != null)
            .SelectMany(
                nested => nested.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
                (nested, m) => new { nested, m })
            .Where(t => t.m.Name.Contains("<get_CanonicalVars>b__", StringComparison.InvariantCultureIgnoreCase))
            .Select(t => t.m);
    }

    public static IEnumerable<CardModel> GetCards(CardModel card)
    {
        if (card.CombatState == null)
            return NRun.Instance?.GlobalUi.TopBar._player?.Deck.Cards ?? [];

        return card.Owner.PlayerCombatState?.AllCards ?? [];
    }

    [HarmonyTranspiler]
    [UsedImplicitly]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var list = new List<CodeInstruction>(instructions);

        var getOwner = AccessTools.PropertyGetter(typeof(CardModel), nameof(CardModel.Owner));
        var getPcs   = AccessTools.PropertyGetter(typeof(Player), nameof(Player.PlayerCombatState));
        var getAll   = AccessTools.PropertyGetter(typeof(PlayerCombatState), nameof(PlayerCombatState.AllCards));

        var getCards   = AccessTools.Method(typeof(CanonicalVarsPatch), nameof(GetCards));

        for (var i = 0; i < list.Count - 3; i++)
        {
            if (!list[i].Calls(getOwner) ||
                !list[i + 1].Calls(getPcs) ||
                !list[i + 2].Calls(getAll)) continue;
            list[i] = new CodeInstruction(OpCodes.Call, getCards);
            list[i + 1].MakeNop();
            list[i + 2].MakeNop();
            
            PatchedMethods.Add(original);
            break;
        }
        
        return list;
    }
}

[HarmonyPatch]
[UsedImplicitly]
public class CalculatedVarPatches
{
    [HarmonyPatch(typeof(CalculatedVar), nameof(CalculatedVar.Calculate))]
    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool CalculatedVarPrefix(CalculatedVar __instance, Creature target, ref decimal __result)
    {
        if (__instance._multiplierCalc == null 
            || __instance._owner == null) return true;
        var card = (CardModel)__instance._owner;
        var cardName = card.GetType().Name;
        if (CanonicalVarsPatch.PatchedMethods.All(m => !cardName.Equals(m.DeclaringType?.DeclaringType?.Name, StringComparison.InvariantCultureIgnoreCase))) return true;
        if (card.CombatState != null) return true;
        decimal d;
        try
        {
            d = __instance._multiplierCalc(card, target);
        }
        catch (Exception)
        {
            MainFile.Logger.Warn("Encountered error in card " + cardName);
            d = 0;
        }
        __result = __instance.GetBaseVar().BaseValue + __instance.GetExtraVar().BaseValue * d;
        return false;
    }
}

public static class Extensions
{
    public static void MakeNop(this CodeInstruction code)
    {
        code.opcode = OpCodes.Nop;
        code.operand = null;
    }
}
