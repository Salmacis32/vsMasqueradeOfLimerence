using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.DLC;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppZenject;
using Masquerade.Examples;
using Masquerade.Models;
using Masquerade.Systems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using UnityEngine;

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
            if (!Masquerade.IgnoreWeaponsGlobal)
            {
                modDlcData._AccessoriesFactory = PopulateAccessories();
            }
            return modDlcData;
        }

        private static DataManagerSettings PopulateDataSettings()
        {
            var settings = new DataManagerSettings();

            if (!Masquerade.IgnoreWeaponsGlobal)
            {
                settings._WeaponDataJsonAsset = PopulateWeaponData(null);
            }

            return settings;
        }

        private static TextAsset PopulateWeaponData(IDictionary<int, JArray> modData)
        {
            // this is just testing
            var dict = new Dictionary<MasqMod, IEnumerable<ModEquipment>>();
            dict.Add(Masquerade.Instance, Masquerade.Api.AccessoryFactory.GetAllContent());

            var generation = WeaponDataSystem.GenerateCustomWeaponData(dict);
            var weaponData = new TextAsset(JsonConvert.SerializeObject(generation));
            Masquerade.Logger.Msg(weaponData.text);

            return weaponData;
        }

        private static AccessoriesFactory PopulateAccessories()
        {
            var factory = ScriptableObject.CreateInstance<AccessoriesFactory>();
            var content = Masquerade.Api.AccessoryFactory.GetAllContent();

            foreach (var item in content)
            {
                factory._accessories.Add((WeaponType)item.ContentId, CreateBaseAccessory(item));
            }
            
            return factory;
        }

        private static Accessory CreateBaseAccessory(ModAccessory template)
        {
            var acc = ProjectContext.Instance.Container.InstantiateComponentOnNewGameObject<Accessory>();

            acc.name = template.ContentId.ToString();

            return acc;
        }
    }
}