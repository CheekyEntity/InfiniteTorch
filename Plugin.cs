using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;

namespace InfiniteTorch;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    
    // This flag persists as long as the game is open
    public static bool HasShownWelcomePopup = false;

    public override void Load()
    {
        Log = base.Log;
        
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} by CheekyEntity is loaded!");
    }
}

[HarmonyPatch(typeof(Torch), nameof(Torch.Start))]
public static class TorchStartPatch
{
    [HarmonyPostfix]
    public static void Postfix(Torch __instance)
    {
        // We still apply the infinite logic to EVERY torch found
        if (__instance.info != null)
        {
            __instance.info.infiniteDuration = true;
        }
        __instance.FreezeDuration = true;

        // But we only show the popup ONCE
        if (!Plugin.HasShownWelcomePopup && MasterUI.Instance != null)
        {
            MasterUI.Instance.CreatePopUp(
                "Torch Blessed: Infinite Light!", 
                new Il2CppSystem.Nullable<Color>(), 
                true, 
                3f
            );
            
            // Set the flag to true so this block never runs again until the game restarts
            Plugin.HasShownWelcomePopup = true;
            
            Plugin.Log.LogInfo("Shown infinite torch welcome message.");
        }
    }
}