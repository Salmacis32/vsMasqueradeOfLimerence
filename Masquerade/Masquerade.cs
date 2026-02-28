using AudioImportLib;
using HarmonyLib;
using Masquerade;
using Masquerade.Api;
using Masquerade.Models;
using MelonLoader;
using MelonLoader.Utils;
using System.Reflection;

[assembly: MelonInfo(typeof(Masquerade.Masquerade), Common.VSML_TITLE, Common.BMD_VERSION, "Mercy", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
[assembly: MelonAdditionalDependencies("AudioImportLib")]

namespace Masquerade
{
    public class Masquerade : MasqMod
    {
        private const string CUSTOM_AUDIO_FILE_PATH = "\\CustomAudio\\";
        private const string MUSIC_DATA_RESOURCE_FILE = "Masquerade.Data.musicData_Modded.json";
        private const string ALBUM_DATA_RESOURCE_FILE = "Masquerade.Data.albumData_Modded.json";
        public static int CurrentContentId = Common.CONTENT_START_ID;
        public static string MusicJson;
        public static string AlbumJson;
        public static IDictionary<int, SongData[]> CustomMusic;
        /// <summary>
        /// Harmony assembly
        /// </summary>
        internal static Assembly AssemblyInstance;
        internal static HarmonyLib.Harmony PatcherInstance;
        internal static IEnumerable<IClassPatcher> Patchers;
        internal static MelonLogger.Instance Logger;
        public static bool MasqueradeInitialized { get; private set; }
        public static MasqueradeApi Api { get; private set; }
        public static Masquerade Instance { get; private set; }
        /// Dipswitches
        public static bool IgnoreWeaponsGlobal { get; private set; }
        public static bool PreloadedDLC { get; private set; }
        public static readonly bool ShouldLoadMusic = true;


        public override void OnDeinitializeMelon()
        {
            LoggerInstance.Msg("Deinitializing...");

            AssemblyInstance = null;
            PatcherInstance = null;
            Logger = null;
            MasqueradeInitialized = false;
            Api = null;
            Instance = null;
            CurrentContentId = Common.CONTENT_START_ID;

            LoggerInstance.Msg("Deinitialize Complete.");
        }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");

            LoggerInstance.Msg("Creating variables...");

            Instance = this;
            AssemblyInstance = MelonAssembly.Assembly;
            PatcherInstance = HarmonyInstance;
            Logger = LoggerInstance;
            Api = new MasqueradeApi();

            LoggerInstance.Msg("Variables created.");

            LoggerInstance.Msg("Initializing patchers...");

            Patchers = InitializePatchers();

            LoggerInstance.Msg($"Patchers initialized. Patched {Patchers?.Count()} class(es).");

            LoggerInstance.Msg("Pre-initialization complete.");
        }

        public override void OnLateInitializeMelon()
        {
            LoggerInstance.Msg("Populating Content factories...");

            Api.LoadedMods = Api.LoadedMods.AddItem(this);
            var contentTypes = PopulateAccessories();

            LoggerInstance.Msg($"Content factories populated. Populated {contentTypes} content type(s).");

            var patched = PatchMethods();

            LoggerInstance.Msg($"Methods patched. Patched {patched} methods.");

            LoggerInstance.Msg($"Loading music...");

            LoadMusic(AssemblyInstance);

            LoggerInstance.Msg($"Music loaded.");

            MasqueradeInitialized = true;

            LoggerInstance.Msg("Initialization complete!");
        }

        private int PopulateAccessories()
        {
            return InitializeAccessories().Count();
        }

        private static IEnumerable<IClassPatcher> InitializePatchers()
        {
            var patchers = new List<IClassPatcher>();
            var types = AccessTools.GetTypesFromAssembly(AssemblyInstance).Where(x => x.GetInterface(nameof(IClassPatcher)) != null);
            foreach (var type in types)
            {
                var instance = AccessTools.CreateInstance(type);
                var patcher = instance as IClassPatcher;
                if (patcher != null)
                    patchers.Add(patcher);
            }
            return patchers;
        }

        private IEnumerable<ModAccessory> InitializeAccessories()
        {
            var accessories = new List<ModAccessory>();
            var types = AccessTools.GetTypesFromAssembly(AssemblyInstance).Where(x => typeof(ModAccessory).IsAssignableFrom(x) && !x.IsAbstract);
            foreach (var type in types)
            {
                var modType = type.Assembly.GetTypes().SingleOrDefault(x => typeof(MasqMod).IsAssignableFrom(x) && !x.IsAbstract);
                if (modType == null || !Api.DoesModExist(modType))
                    continue;
                var mod = Api.GetMod(modType);
                if (mod == null || mod.IgnoreWeapons)
                    continue;
                var instance = AccessTools.CreateInstance(type);
                var accessory = instance as ModAccessory;
                if (accessory != null)
                {
                    accessory.Mod = mod;
                    accessory.ContentId = CurrentContentId; CurrentContentId++;
                    Api.AccessoryFactory.AddContent(accessory);
                    LoggerInstance.Msg($"Added accessory {accessory.FullName}");
                    accessories.Add(accessory);
                }
            }
            return accessories;
        }

        private static int PatchMethods()
        {
            if (PatcherInstance is null || Patchers is null || !Patchers.Any()) return 0;
            var patches = new List<PatchInstruction>();
            var patched = 0;
            foreach (var patch in Patchers.Select(x => x.GeneratePatchInstructions()))
            {
                patches.AddRange(patch);
            }

            foreach (var instruction in patches)
            {
                var method = (instruction.Parameters.Any()) 
                    ? AccessTools.DeclaredMethod(instruction.ClassOrigin, instruction.MethodToPatch, parameters: instruction.Parameters) 
                    : AccessTools.DeclaredMethod(instruction.ClassOrigin, instruction.MethodToPatch);
                if (instruction.IsPrefix)
                    PatcherInstance.Patch(method, prefix: new HarmonyMethod(instruction.PatchMethod));
                else
                    PatcherInstance.Patch(method, postfix: new HarmonyMethod(instruction.PatchMethod));
                patched++;
            }

            return patched;
        }

        private static void LoadMusic(System.Reflection.Assembly ass)
        {
            var mdmj = ass.GetManifestResourceStream(MUSIC_DATA_RESOURCE_FILE);
            var albmj = ass.GetManifestResourceStream(ALBUM_DATA_RESOURCE_FILE);
            var read2 = new StreamReader(mdmj);
            MusicJson = read2.ReadToEnd();
            read2 = new StreamReader(albmj);
            AlbumJson = read2.ReadToEnd();
            CustomMusic = new Dictionary<int, SongData[]>();
            int id = Common.MUSIC_START_ID;

            AddSong(id, "Mob Smash", "Smash", ["BGM_MobSmash1.wav", "BGM_MobSmash2.wav"]); id++;
            AddSong(id, "PAC MADNESS", "PacmanCE", ["BGM_Pacmadness1.wav", "BGM_Pacmadness2.wav"]); id++;
            AddSong(id, "PAC TOY BOX", "PacmanCE", ["BGM_Pactoybox1.wav", "BGM_Pactoybox2.wav"]); id++;
            AddSong(id, "PAC BABY", "PacmanCE", ["BGM_Pacbaby1.wav", "BGM_Pacbaby2.wav"]); id++;
            AddSong(id, "PAC TRONICA", "PacmanCE", ["BGM_Pactronica1.wav", "BGM_Pactronica2.wav"]); id++;
            AddSong(id, "PAC STEP", "PacmanCE", ["BGM_Pacstep1.wav", "BGM_Pacstep2.wav"]);
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
    }
}