using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MasqueradeBepin.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MasqueradeBepin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("VampireSurvivors.exe")]
public class Masquerade : BasePlugin
{
    internal static new ManualLogSource Log;
    internal static Harmony HarmonyInstance;
    internal static IEnumerable<IClassPatcher> Patchers;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        HarmonyInstance = new Harmony(MyPluginInfo.PLUGIN_GUID);
        Log.LogInfo($"Harmony Instance {MyPluginInfo.PLUGIN_GUID} created!");
        Patchers = InitializePatchers();
        Log.LogInfo($"Patchers Initialized. Patched {Patchers?.Count()} class(es).");
        var patched = PatchMethods();
        Log.LogInfo($"Methods Patched. Patched {patched} methods.");
    }

    private static IEnumerable<IClassPatcher> InitializePatchers()
    {
        var patchers = new List<IClassPatcher>();
        var types = AccessTools.GetTypesFromAssembly(typeof(Masquerade).Assembly).Where(x => x.GetInterface(nameof(IClassPatcher)) != null);
        foreach (var type in types)
        {
            var instance = AccessTools.CreateInstance(type);
            var patcher = instance as IClassPatcher;
            if (patcher != null)
                patchers.Add(patcher);
        }
        return patchers;
    }

    private static int PatchMethods()
    {
        if (HarmonyInstance is null || Patchers is null || !Patchers.Any()) return 0;
        var patches = new List<PatchInstruction>();
        var patched = 0;
        foreach (var patch in Patchers.Select(x => x.GeneratePatchInstructions()))
        {
            patches.AddRange(patch);
        }

        foreach (var instruction in patches)
        {
            var method = AccessTools.Method(instruction.ClassOrigin, instruction.MethodToPatch);
            if (instruction.IsPrefix)
                HarmonyInstance.Patch(method, prefix: new HarmonyMethod(instruction.PatchMethod));
            else
                HarmonyInstance.Patch(method, postfix: new HarmonyMethod(instruction.PatchMethod));
            patched++;
        }

        return patched;
    }

    public override bool Unload()
    {
        HarmonyInstance = null;
        Patchers = null;
        return base.Unload();
    }
}
