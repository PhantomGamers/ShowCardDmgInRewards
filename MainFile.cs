using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace ShowCardDmgInRewards;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    private const string
        ModId = "ShowCardDmgInRewards"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        
        Logger.Info("Patched cards:");
        foreach (var m in ShowCardDmgInRewardsCode.CanonicalVarsPatch.PatchedMethods)
        {
            Logger.Info(m.DeclaringType?.DeclaringType?.Name ?? "null");
        }
    }
}