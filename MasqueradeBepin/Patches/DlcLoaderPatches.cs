using MasqueradeBepin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VampireSurvivors.App.Data;
using VampireSurvivors.Data;
using VampireSurvivors.Framework.DLC;

namespace MasqueradeBepin.Patches
{
    public class DlcLoaderPatches : IClassPatcher
    {
        public Type TargetClass => typeof(DlcLoader);

        public static bool PreLoadDlc(DlcLoader __instance, DlcType dlcType, Action<BundleManifestData> onComplete)
        {
            if (dlcType != Common.VSML_DLC_TYPE) return true;
            Masquerade.Log.LogDebug("<DLCLoader.LoadDlc> dlcType:" + dlcType);
            DlcLoader.ResetLoader();
            var dlcNullable = new Il2CppSystem.Nullable<DlcType>(Common.VSML_DLC_TYPE);
            dlcNullable.value = dlcType;
            DlcLoader._dlcType = dlcNullable;
            DlcLoader._onComplete = onComplete;
            DlcLoader.UpdateProgress();
            Masquerade.Log.LogDebug("<DLCLoader.LoadDlc> pointed At DLC " + dlcType);

            Action<BundleManifestData> action = (bmd) =>
            {
                Masquerade.Log.LogDebug("<DLCLoader.LoadDlc> LoadManifest on complete");
                DlcLoader._manifestState = ((!DlcLoader.DidTaskError(DlcLoader._manifestState)) ? DlcLoadState.Complete : DlcLoadState.Error);
                DlcLoader._manifest = bmd;
                DlcLoader._locationsState = DlcLoadState.Complete;
                DlcLoader._spritesState = DlcLoadState.Complete;
                DlcLoader.UpdateProgress();
                Masquerade.Log.LogInfo("Successfully loaded modded dlc bundle!");
            };
            BundleManifestData modDlcData = CreateBundle();
            Masquerade.Log.LogInfo("Created modded dlc bundle.");

            ManifestLoader.LoadManifest(modDlcData, Common.VSML_DLC_TYPE, action);
            return false;
        }

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            var instructions = new List<PatchInstruction>();
            instructions.Add(new PatchInstruction(TargetClass, nameof(DlcLoader.LoadDlc), typeof(DlcLoaderPatches).GetMethod(nameof(PreLoadDlc))));
            return instructions;
        }

        private static BundleManifestData CreateBundle()
        {
            var modDlcData = ScriptableObject.CreateInstance<BundleManifestData>();
            modDlcData._Version = Common.BMD_VERSION; modDlcData.name = Common.BMD_NAME; modDlcData._DataFiles = new DataManagerSettings();
            return modDlcData;
        }
    }
}