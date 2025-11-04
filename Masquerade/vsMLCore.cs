using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(typeof(Masquerade.vsMLCore), "Masquerade of Limerence", "0.0.2", "Mercy", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]

namespace Masquerade
{
    public class vsMLCore : MelonMod
    {
        /// <summary>
        /// Harmony assembly
        /// </summary>
        public static Assembly AssemblyInstance;

        public override void OnDeinitializeMelon()
        {
            LoggerInstance.Msg("Deinitializing...");

            AssemblyInstance = null;

            LoggerInstance.Msg("Deinitialize Complete.");
        }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");

            LoggerInstance.Msg("Creating Variables...");

            AssemblyInstance = MelonAssembly.Assembly;

            LoggerInstance.Msg("Initialize Complete.");
        }
    }
}