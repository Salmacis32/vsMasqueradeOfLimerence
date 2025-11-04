using HarmonyLib;
using Il2CppNewtonsoft.Json;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using MelonLoader;
using MelonLoader.Utils;
using Masquerade.Models;
using Masquerade.Patches;
using Il2Col = Il2CppSystem.Collections.Generic;
using System.Reflection;
using AudioImportLib;

[assembly: MelonInfo(typeof(Masquerade.vsMLCore), "Masquerade of Limerence", "0.0.2", "Mercy", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
[assembly: MelonAdditionalDependencies("AudioImportLib")]

namespace Masquerade
{
    /// <summary>
    /// Main class for loading, unloading, and keeping track of global variables.
    /// </summary>
    public class vsMLCore : MelonMod
    {
        /// <summary>
        /// The data for all custom weapons organized by WeaponType in a collection that can be read directly by the game. 
        /// </summary>
        public static Il2Col.Dictionary<WeaponType, Il2Col.List<WeaponData>> Il2CppCustomWeapons;
        /// <summary>
        /// The data for all the custom weapons.
        /// </summary>
        public static IEnumerable<WeaponInfo> CustomWeapons;
        /// <summary>
        /// Collection of all custom music
        /// </summary>
        public static IDictionary<int, SongData[]> CustomMusic;
        /// <summary>
        /// The JSON containing the music data that will be used for the DLC manifest bundle asset.
        /// </summary>
        public static string MusicJson;
        /// <summary>
        /// The JSON containing the album data that will be used for the DLC manifest bundle asset.
        /// </summary>
        public static string AlbumJson;
        /// <summary>
        /// Harmony assembly
        /// </summary>
        public static Assembly AssemblyInstance;
        private const string MUSIC_DATA_RESOURCE_FILE = "Masquerade.Data.musicData_Modded.json";
        private const string ALBUM_DATA_RESOURCE_FILE = "Masquerade.Data.ALBUM_DATA.json";
        private const string WEAPON_DATA_RESOURCE_FILE = "Masquerade.Data.WEAPON_DATA.json";
        private const string CUSTOM_AUDIO_FILE_PATH = "\\CustomAudio\\";

        // Dipswitches for turning on and off functionality. (for testing purposes)
        public static readonly bool ShouldLoadMusic = false;
        public static readonly bool ShouldLoadWeapons = false;

        public override void OnDeinitializeMelon()
        {
            LoggerInstance.Msg("Deinitializing...");

            CustomWeapons = null;
            Il2CppCustomWeapons = null;
            CustomMusic = null;
            MusicJson = null;
            ProjectilePatches.Deinitialize();
            GameManagerPatches.Deinitialize();
            LocalizationManagerPatches.Deinitialize();
            AssemblyInstance = null;

            LoggerInstance.Msg("Deinitialize Complete.");
        }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");

            LoggerInstance.Msg("Creating Variables...");

            AssemblyInstance = MelonAssembly.Assembly;
            CustomWeapons = new List<WeaponInfo>();
            Il2CppCustomWeapons = new Il2Col.Dictionary<WeaponType, Il2Col.List<WeaponData>>();
            LoggerInstance.Msg("Variables Created.");

            if (ShouldLoadWeapons)
            {
                LoggerInstance.Msg("Loading Weapons...");

                LoadWeaponJson(AssemblyInstance);


                LoggerInstance.Msg("Patching Projectile Methods.");
                ProjectilePatching();
                LoggerInstance.Msg("Projectiles Patched.");

                LoggerInstance.Msg("Weapons Loaded.");
            }

            if (ShouldLoadMusic)
            {
                LoggerInstance.Msg("Loading Music...");
                LoadMusic(AssemblyInstance);
                LoggerInstance.Msg("Music Loaded.");
            }

            LoggerInstance.Msg("Initialize Complete.");
        }

        private void ProjectilePatching()
        {
            ProjectilePatches.Initialize();
            var methods = ProjectilePatches.Methods;
            HarmonyInstance.Patch(methods[0], new HarmonyMethod(typeof(ProjectilePatches).GetMethod(nameof(ProjectilePatches.InitPrefix))));
            HarmonyInstance.Patch(methods[1], new HarmonyMethod(typeof(ProjectilePatches).GetMethod(nameof(ProjectilePatches.DespawnPrefix))));
            HarmonyInstance.Patch(methods[2], new HarmonyMethod(typeof(ProjectilePatches).GetMethod(nameof(ProjectilePatches.InternalUpdatePrefix))));
        }

        private static void AddSong(int id, string name, string album, string[] paths)
        {
            var clips = new SongData[paths.Length];
            for (var i = 0; i < paths.Length; i++)
            {
                clips[i] = new SongData(API.LoadAudioClip(MelonEnvironment.UserDataDirectory + CUSTOM_AUDIO_FILE_PATH + album + "\\" + paths[i], true), name + i, album, (i != 0));
            }
            CustomMusic.Add(id, clips);
        }

        private static void LoadMusic(System.Reflection.Assembly ass)
        {
            var mdmj = ass.GetManifestResourceStream(MUSIC_DATA_RESOURCE_FILE);
            var albmj = ass.GetManifestResourceStream(ALBUM_DATA_RESOURCE_FILE);
            var read2 = new StreamReader(mdmj);
            MusicJson = read2.ReadToEnd();
            //read2 = new StreamReader(albmj);
            //AlbumJson = read2.ReadToEnd();
            CustomMusic = new Dictionary<int, SongData[]>();
            int id = Constants.MUSIC_START_ID;

            //AddSong(id, "PacmanCE", "PAC TRONICA", ["BGM_Pactronica1.wav", "BGM_Pactronica2.wav"]); id++;
            //AddSong(id, "PacmanCE", "PAC MADNESS", ["BGM_Pacmadness1.wav", "BGM_Pacmadness2.wav"]); id++;
            //AddSong(id, "PacmanCE", "PAC TOY BOX", ["BGM_Pactoybox1.wav", "BGM_Pactoybox2.wav"]); id++;
            //AddSong(id, "PacmanCE", "PAC BABY", ["BGM_Pacbaby1.wav", "BGM_Pacbaby2.wav"]); id++;
            AddSong(id, "Mob Smash", "Smash", ["BGM_MobSmash1.wav", "BGM_MobSmash2.wav"]);
        }

        private static void LoadWeaponJson(System.Reflection.Assembly ass)
        {
            var wdj = ass.GetManifestResourceStream(WEAPON_DATA_RESOURCE_FILE);
            var read = new StreamReader(wdj);
            var jobj = JsonConvert.DeserializeObject<Il2Col.Dictionary<WeaponType, Il2Col.List<WeaponData>>>(read.ReadToEnd());
            if (jobj != null) Il2CppCustomWeapons = jobj;
            foreach (var il2obj in jobj)
            {
                CustomWeapons = CustomWeapons.AddItem(new WeaponInfo(il2obj));
            }
        }
    }
}