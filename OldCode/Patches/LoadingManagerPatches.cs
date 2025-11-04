using Il2CppNewtonsoft.Json.Linq;
using Il2CppSystem.Reflection;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.DLC;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppZenject;
using Masquerade.Factories;
using UnityEngine;
using Il2Col = Il2CppSystem.Collections.Generic;
using MelonLoader;
using HarmonyLib;

namespace Masquerade.Patches
{
    /// <summary>
    /// Patches for the LoadingManager class. Creates all of the data into the DLC asset bundle and loads the manifest into the game.
    /// </summary>
    //[HarmonyPatch(typeof(LoadingManager))]
    public static class LoadingManagerPatches
    {
        private const string BMD_NAME = "BundleManifestData - Masquerade";
        private const string BMD_VERSION = "0.0.2";
        private const string BMD_LOG = "Masquerade of Limerance - Version ";
        private const DlcType VSML_DLC_TYPE = (DlcType)10000;
        public static bool ModLoaded;

        //[HarmonyPatch(nameof(LoadingManager.ValidateVersion))]
        //[HarmonyPostfix]
        private static void AddManifest(object[] __args, MethodBase __originalMethod, object __instance)
        {
            if (ModLoaded) return;
            AddManifestPost();
            ModLoaded = true;
        }

        /// <summary>
        /// Main method that creates the BundleManifestData and loads in all the custom assets, then applies it to the game's loaded data.
        /// </summary>
        private static void AddManifestPost()
        {
            MelonLogger.Msg("Loading mod DLC assets");

            var modDlcData = ScriptableObject.CreateInstance<BundleManifestData>();
            modDlcData._Version = BMD_VERSION; modDlcData.name = BMD_NAME; modDlcData._DataFiles = new DataManagerSettings();

            if (vsMLCore.ShouldLoadWeapons)
            {
                MelonLogger.Msg("Loading custom weapon assets");
                WeaponAdder(modDlcData, VSML_DLC_TYPE);
                MelonLogger.Msg("Custom weapon assets loaded!");
            }
            if (vsMLCore.ShouldLoadMusic)
            {
                MelonLogger.Msg("Loading custom music assets");
                MusicAdder(modDlcData);
                MelonLogger.Msg("Custom music assets loaded!");
            }
            MelonLogger.Msg("Applying Bundle to Game");
            DlcSystem.MountedPaths.Add(VSML_DLC_TYPE, String.Empty);
            DlcSystem.LoadedDlc.TryAdd(VSML_DLC_TYPE, modDlcData);
            Action<BundleManifestData> DlcLoaderLoadDlc = (bmd) =>
            {
                bmd = modDlcData;
                DlcLoader._manifest = bmd;
                DlcLoader._manifestState = DlcLoadState.Complete;
                UnityEngine.Debug.Log(BMD_LOG + BMD_VERSION);
            };
            ManifestLoader.ApplyBundleCore(VSML_DLC_TYPE, modDlcData, DlcLoaderLoadDlc);
            ManifestLoader.DoRuntimeReload();
            MelonLogger.Msg("Bundle successfully applied!");

            MelonLogger.Msg("Mod DLC assets successfully loaded!");
        }

        private static Weapon GetWeaponPrefab(Il2Col.KeyValuePair<WeaponType, Il2Col.List<WeaponData>> newWeapon)
        {
            var comp = ProjectContext.Instance.Container.InstantiateComponentOnNewGameObject<Weapon>();
            comp.name = newWeapon.Value[0].name;
            return comp;
        }

        private static void MusicAdder(BundleManifestData manifestData)
        {
            TextAsset textAsset = new TextAsset(vsMLCore.MusicJson);
            //TextAsset albumAsset = new TextAsset(vsMLCore.AlbumJson);
            //var bytes = File.ReadAllBytes("F:\\SteamLibrary\\steamapps\\common\\Vampire Survivors\\UserData\\CustomAudio\\PacmanCE\\pacalbum.png");
            //var album = new Texture2D(256, 256);
            //ImageConversion.LoadImage(album, bytes);
            //var handle = Addressables.LoadAssetAsync<Texture2D>(album);
            manifestData.DataFiles._MusicDataJsonAsset = textAsset;
            manifestData._DynamicSoundGroup = DynamicSoundGroupFactory.DefaultModdedGroup();
            //manifestData.DataFiles._AlbumDataJsonAsset = albumAsset;
            manifestData._AssetReferenceLibrary = new AssetReferenceLibrary();
            manifestData._AssetReferenceLibrary._AssetRefs = new AssetReferenceLibrary.AssetRefsDictionary();
        }

        private static void WeaponAdder(BundleManifestData modDlcData, DlcType dlcType)
        {
            // TODO: Turn this into a function off of the list of weapon information rather than having two seperate lists.
            if (vsMLCore.Il2CppCustomWeapons == null || vsMLCore.Il2CppCustomWeapons.Count == 0) return;
            modDlcData._WeaponFactory = ScriptableObject.CreateInstance<WeaponFactory>();
            foreach (var newWeapon in vsMLCore.Il2CppCustomWeapons)
            {
                Weapon comp = GetWeaponPrefab(newWeapon);
                modDlcData._WeaponFactory._weapons.Add(newWeapon.Key, comp);
            }
            if (modDlcData.DataFiles != null)
            {
                JObject dlc = JObject.FromObject(vsMLCore.Il2CppCustomWeapons);
                TextAsset textAsset = new TextAsset(dlc.ToString());
                modDlcData.DataFiles._WeaponDataJsonAsset = textAsset;
            }
        }
        
    }
}