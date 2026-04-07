using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Models;

namespace Masquerade.Patches
{
    public class CharacterControllerPatches : IClassPatcher
    {
        public Type TargetClass => typeof(CharacterController);

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new List<PatchInstruction>()
            {
                new PatchInstruction(TargetClass, nameof(CharacterController.InternalUpdate), GetType().GetMethod(nameof(PostInternalUpdate)), prefix: false)
            };
        }

        public static void PostInternalUpdate(CharacterController __instance)
        {
            if (__instance.IsDead || !__instance._isInitialized)
                return;

            Masquerade.Instance.ModEffectSystem.InternalUpdateEffects(__instance);
        }
    }
}
