using Masquerade.Models;
using UnityEngine;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.DLC;
using Masquerade.Examples;
using Il2CppVampireSurvivors.Data.Weapons;

namespace Masquerade.Patches
{
    public class DlcLoaderPatches : IClassPatcher
    {
        public Type TargetClass => typeof(DlcLoader);

        public static bool PreLoadDlc(DlcLoader __instance, DlcType dlcType, Action<BundleManifestData> onComplete)
        {
            if (dlcType != Common.VSML_DLC_TYPE) return true;
            Masquerade.Logger.Msg("<DLCLoader.LoadDlc> dlcType:" + dlcType);
            DlcLoader.ResetLoader();
            var dlcNullable = new Il2CppSystem.Nullable<DlcType>(Common.VSML_DLC_TYPE);
            dlcNullable.value = dlcType;
            DlcLoader._dlcType = dlcNullable;
            DlcLoader._onComplete = onComplete;
            DlcLoader.UpdateProgress();
            Masquerade.Logger.Msg("<DLCLoader.LoadDlc> pointed At DLC " + dlcType);

            Action<BundleManifestData> action = (bmd) =>
            {
                Masquerade.Logger.Msg("<DLCLoader.LoadDlc> LoadManifest on complete");
                DlcLoader._manifestState = ((!DlcLoader.DidTaskError(DlcLoader._manifestState)) ? DlcLoadState.Complete : DlcLoadState.Error);
                DlcLoader._manifest = bmd;
                DlcLoader._locationsState = DlcLoadState.Complete;
                DlcLoader._spritesState = DlcLoadState.Complete;
                DlcLoader.UpdateProgress();
                Masquerade.Logger.Msg("Successfully loaded modded dlc bundle!");
            };
            BundleManifestData modDlcData = CreateBundle();
            Masquerade.Logger.Msg("Created modded dlc bundle.");

            ManifestLoader.LoadManifest(modDlcData, Common.VSML_DLC_TYPE, action);
            var modAccessory = Masquerade.Api.GetModAccessory(typeof(Masquerade), nameof(ExampleAccessory));
            Masquerade.Logger.Msg($"Acknlowedging mod accessory Id {modAccessory.ContentId}: {modAccessory.FullName}");
            var modAccessoryGen = Masquerade.Api.GetModAccessory<ExampleAccessory>();
            Masquerade.Logger.Msg($"Acknlowedging mod accessory (from generic method) Id {modAccessoryGen.ContentId}: {modAccessoryGen.FullName}");
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
            modDlcData._Version = Common.BMD_VERSION; modDlcData.name = Common.BMD_NAME;
            modDlcData._DataFiles = PopulateDataSettings();
            return modDlcData;
        }

        private static DataManagerSettings PopulateDataSettings()
        {
            var settings = new DataManagerSettings();

            settings._WeaponDataJsonAsset = PopulateWeaponData();

            return settings;
        }

        private static TextAsset PopulateWeaponData()
        {

            var weaponData = new TextAsset();
            var dictionary = new Dictionary<WeaponType, List<WeaponData>>();

            foreach (var accessory in Masquerade.Api.AccessoryFactory.GetAllContent())
            {

            }

            return weaponData;
        }
    }
}