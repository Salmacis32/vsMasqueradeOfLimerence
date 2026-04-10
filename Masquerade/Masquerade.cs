using AudioImportLib;
using HarmonyLib;
using Masquerade;
using Masquerade.Api;
using Masquerade.Models;
using Masquerade.Systems;
using MelonLoader;
using MelonLoader.Utils;
using System.Diagnostics;
using System.Reflection;
using static Il2CppDoozy.Engine.Utils.ColorModels.XYZ;

[assembly: MelonInfo(typeof(Masquerade.Masquerade), Common.VSML_TITLE, Common.BMD_VERSION, "Mercy", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
[assembly: MelonAdditionalDependencies("AudioImportLib")]

namespace Masquerade
{
    public class Masquerade : MasqMod
    {
        public readonly bool LogMethodPatches = true;
        public readonly bool ShouldLoadMusic = true;
        public string AlbumJson;
        public IDictionary<int, SongData[]> CustomMusic;
        public string MusicJson;

        /// <summary>
        /// Harmony assembly
        /// </summary>
        internal static Assembly AssemblyInstance;

        internal ModEffectSystem ModEffectSystem;
        internal HarmonyLib.Harmony PatcherInstance;
        internal IEnumerable<IClassPatcher> Patchers;
        internal IEnumerable<string> ResourceManifest;
        private const string ALBUM_DATA_RESOURCE_FILE = "Masquerade.Data.albumData_Modded.json";
        private const string CUSTOM_AUDIO_FILE_PATH = "\\CustomAudio\\";
        private const string MUSIC_DATA_RESOURCE_FILE = "Masquerade.Data.musicData_Modded.json";
        private MasqueradeApi _api;
        private Stopwatch _initTimer;
        public static MasqueradeApi Api { get => Instance._api; }

        /// Dipswitches
        public bool IgnoreWeaponsGlobal { get; private set; }

        public static Masquerade Instance { get; private set; }
        public bool MasqueradeInitialized { get; private set; }
        public bool PreloadedDLC { get; private set; }

        public override void OnDeinitializeMelon()
        {
            LoggerInstance.Msg("Deinitializing...");

            _initTimer = null;
            ModEffectSystem = null;
            PatcherInstance = null;
            AssemblyInstance = null;
            ResourceManifest = null;

            _api = null;
            Instance = null;
            MasqueradeInitialized = false;

            LoggerInstance.Msg("Deinitialize Complete.");
        }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");
            _initTimer = new Stopwatch();
            _initTimer.Start();

            LoggerInstance.Msg("Creating variables...");

            Instance = this;
            AssemblyInstance = MelonAssembly.Assembly;
            PatcherInstance = HarmonyInstance;
            _api = new MasqueradeApi();
            ResourceManifest = AssemblyInstance.GetManifestResourceNames();
            ModEffectSystem = new ModEffectSystem();

            LoggerInstance.Msg("Variables created.");

            TimedLoadStep(() => { Patchers = InitializePatchers(); return Patchers?.Count() ?? 0; }, "Initializing patchers...", "Patched %0 classes.");

            _initTimer.Stop();
        }

        public override void OnLateInitializeMelon()
        {
            _initTimer.Start();
            TimedLoadStep(() =>
            {
                int typeCount = 0;
                Api.LoadedMods = Api.LoadedMods.AddItem(this);
                typeCount += PopulateAccessories(); typeCount += PopulateEffects();
                return typeCount;
            }, "Populating Content factories...", $"Populated %0 content type(s).");

            //LoggerInstance.Msg($"Initializing systems...");
            //LoggerInstance.Msg($"Systems initialized.");

            TimedLoadStep(() => PatchMethods(), "Patching methods...", "Patched %0 methods.");

            TimedLoadStep(() => LoadMusic(AssemblyInstance), "Loading music...", "%0 song(s) loaded.");

            MasqueradeInitialized = true;

            _initTimer.Stop();
            LoggerInstance.Msg($"Initialization complete in {_initTimer.ElapsedMilliseconds}ms!");
        }

        private void AddSong(int id, string name, string album, string[] paths)
        {
            var clips = new SongData[paths.Length];
            for (var i = 0; i < paths.Length; i++)
            {
                clips[i] = new SongData(API.LoadAudioClip(MelonEnvironment.UserDataDirectory + CUSTOM_AUDIO_FILE_PATH + album + "\\" + paths[i], true), name + i, album, (i != 0));
            }
            CustomMusic.Add(id, clips);
        }

        private IEnumerable<IClassPatcher> InitializePatchers()
        {
            var patchers = new List<IClassPatcher>();
            var types = AccessTools.GetTypesFromAssembly(AssemblyInstance).Where(x => x.GetInterface(nameof(IClassPatcher)) != null && !x.IsAbstract);
            foreach (var type in types)
            {
                var instance = AccessTools.CreateInstance(type);
                var patcher = instance as IClassPatcher;
                if (patcher != null)
                    patchers.Add(patcher);
            }
            return patchers;
        }

        private int LoadMusic(System.Reflection.Assembly ass)
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
            return CustomMusic.Count();
        }

        private int PatchMethods()
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

                if (LogMethodPatches)
                {
                    Instance.LoggerInstance.Msg($"Patched {instruction.ClassOrigin.Name}.{instruction.MethodToPatch} with {instruction.PatchMethod.Name} at {((instruction.IsPrefix) ? "Prefix" : "Postfix")}");
                }
            }

            return patched;
        }

        private int PopulateAccessories()
        {
            return InitializeContent<ModAccessory>().Count();
        }

        private IEnumerable<T> InitializeContent<T>() where T : ModContent
        {
            var contentList = new List<T>();
            var types = AccessTools.GetTypesFromAssembly(AssemblyInstance).Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract);
            foreach (var type in types)
            {
                var modType = type.Assembly.GetTypes().SingleOrDefault(x => typeof(MasqMod).IsAssignableFrom(x) && !x.IsAbstract);
                if (modType == null || !Api.DoesModExist(modType))
                    continue;
                var mod = Api.GetMod(modType);
                if (mod == null)
                    continue;
                var instance = AccessTools.CreateInstance(type);
                var content = instance as T;
                if (content == null)
                {
                    LoggerInstance.Error($"Failed to create ModContent of type {type.BaseType} for {type.Name}.");
                    continue;
                }
                content.Mod = mod;
                content.ContentId = Api.GetNextFreeContentId();
                string contentSpecificLog = string.Empty;

                if (typeof(T).IsAssignableFrom(typeof(ModCharacterEffect)))
                    Api.CharacterEffectFactory.AddContent(content as ModCharacterEffect);

                if (typeof(T).IsAssignableFrom(typeof(ModAccessory)))
                {
                    var accessory = content as ModAccessory;
                    accessory.Mod = mod;
                    accessory.ContentId = Api.GetNextFreeContentId();
                    accessory.WeaponTypeId = Api.GetNextFreeWeaponTypeId();
                    if (!ResourceManifest.Any(x => x.Contains(accessory.TextureName))) // Make this properly distinguish between texture and sprite name later
                    {
                        LoggerInstance.Warning($"Was unable to find {accessory.TextureName}.png registered in the SpriteManager! Resetting to default.");
                        accessory.TextureName = Common.MISSING_TEXTURE;
                    }
                    Api.AccessoryFactory.AddContent(accessory);
                    contentSpecificLog = "WeaponType " + accessory.WeaponTypeId;
                }

                LoggerInstance.Msg($"Added content of type {type.BaseType} {content.FullName} with id {content.ContentId} {contentSpecificLog}");
                contentList.Add(content);
            }
            return contentList;
        }

        private int PopulateEffects()
        {
            return InitializeContent<ModCharacterEffect>().Count();
        }

        private void TimedLoadStep(Func<int> action, string preLog, string postLog)
        {
            LoggerInstance.Msg(preLog);
            var timer = new Stopwatch();
            timer.Start();
            var variable = action.Invoke();
            timer.Stop();
            LoggerInstance.Msg(postLog.Replace("%0", variable.ToString()) + $" in {timer.ElapsedMilliseconds}ms.");
        }
    }
}