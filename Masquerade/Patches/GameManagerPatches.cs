using Il2CppVampireSurvivors.Framework;
using Masquerade.Models;

namespace Masquerade.Patches
{
    public class GameManagerPatches : IClassPatcher
    {
        public Type TargetClass => typeof(GameManager);

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new PatchInstruction[] {
                new PatchInstruction(TargetClass, nameof(GameManager.InitializeGameSessionPostLoad), this.GetType().GetMethod(nameof(PostInitializeGameSession)), prefix: false),
                new PatchInstruction(TargetClass, nameof(GameManager.ResetGameSession), this.GetType().GetMethod(nameof(PostResetGameSession)), prefix: false)
            };
        }

        public static void PostInitializeGameSession(GameManager __instance)
        {
            
        }

        public static void PostResetGameSession(GameManager __instance)
        {
            Masquerade.Api.CleanUpInstances();
        }
    }
}
