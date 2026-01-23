using HarmonyLib;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppVampireSurvivors.Objects;
using Masquerade;
using Masquerade.Api;
using Masquerade.Examples;
using Masquerade.Models;
using Masquerade.Patches;
using Masquerade.Systems;
using MelonLoader;
using MelonLoader.NativeUtils;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(Masquerade.Masquerade), Common.VSML_TITLE, Common.BMD_VERSION, "Mercy", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]

namespace Masquerade
{
    public class Masquerade : MasqMod
    {
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


        public override void OnDeinitializeMelon()
        {
            LoggerInstance.Msg("Deinitializing...");

            AssemblyInstance = null;
            PatcherInstance = null;
            Logger = null;
            MasqueradeInitialized = false;
            Api = null;
            Instance = null;

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

            MasqueradeInitialized = true;

            LoggerInstance.Msg("Initialization complete!");
        }

        private int PopulateAccessories()
        {
            Api.AccessoryFactory.AddContent(new ExampleAccessory() { Mod = this, ContentId = 10000 });
            return 1;
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
                var method = (instruction.Parameters.Any()) ? AccessTools.DeclaredMethod(instruction.ClassOrigin, instruction.MethodToPatch, parameters: instruction.Parameters) : AccessTools.DeclaredMethod(instruction.ClassOrigin, instruction.MethodToPatch);
                if (instruction.IsPrefix)
                    PatcherInstance.Patch(method, prefix: new HarmonyMethod(instruction.PatchMethod));
                else
                    PatcherInstance.Patch(method, postfix: new HarmonyMethod(instruction.PatchMethod));
                patched++;
            }

            var originalMethods = PatcherInstance.GetPatchedMethods();
            foreach (var method in originalMethods) 
            {
                Logger.Msg($"{method.Name} {method.DeclaringType} {method.ReflectedType.Name}");
            }

            return patched;
        }
    }
}