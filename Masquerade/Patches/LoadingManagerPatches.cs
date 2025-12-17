using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.DLC;
using Il2CppVampireSurvivors.Framework.Loading;
using Masquerade.Models;
using System.Data;
using UnityEngine;

namespace Masquerade.Patches
{
    public class LoadingManagerPatches : IClassPatcher
    {
        public Type TargetClass => typeof(LoadingManager);

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            var instructions = new List<PatchInstruction>();
            instructions.Add(new PatchInstruction(TargetClass, nameof(LoadingManager.LoadDlcs), typeof(LoadingManagerPatches).GetMethod(nameof(PreLoadDlcs))));
            instructions.Add(new PatchInstruction(TargetClass, nameof(LoadingManager.MountDlc), typeof(LoadingManagerPatches).GetMethod(nameof(PreMountDlc))));
            return instructions;
        }

        public static void PreLoadDlcs(LoadingManager __instance)
        {
            DlcData data = ScriptableObject.CreateInstance<DlcData>();
            data._DlcType = Common.VSML_DLC_TYPE; data._Title = "Masquerade of Limerence";
            data._ContentGroupType = ContentGroupType.EXTRA; data._ExpectedVersion = Common.BMD_VERSION;
            data._HasBeenReleased = true;
            DlcSystem.DlcCatalog._DlcData.TryAdd(Common.VSML_DLC_TYPE, data);
            DlcSystem.SelectedDlc.TryAdd(Common.VSML_DLC_TYPE, true);
        }

        public static void PreMountDlc(LoadingManager __instance, DlcType dlcType, Action callback)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), DlcSystem.DlcCatalog._DlcData[dlcType]._Steam._AppID);
            AddressableLoader.SetInternalIdTransform();
            AddressableLoader.SetPath(path);
            if (!string.IsNullOrEmpty(path) && path != Directory.GetCurrentDirectory())
                __instance.MountedPaths.TryAdd(dlcType, path);
        }
    }
}
