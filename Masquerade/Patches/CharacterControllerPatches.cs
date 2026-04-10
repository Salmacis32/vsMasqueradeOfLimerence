using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Models;

namespace Masquerade.Patches
{
    public class CharacterControllerPatches : ClassPatcher<CharacterController>
    {
        public override IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new List<PatchInstruction>()
            {
                new PatchInstruction(TargetClass, nameof(CharacterController.InternalUpdate), GetType().GetMethod(nameof(PostInternalUpdate)), prefix: false),
                new PatchInstruction(TargetClass, nameof(CharacterController.ResetStats), GetType().GetMethod(nameof(PreResetStats)), prefix: true)
            };
        }

        public static void PostInternalUpdate(CharacterController __instance)
        {
            if (__instance.IsDead || !__instance._isInitialized)
                return;

            Masquerade.Instance.ModEffectSystem.InternalUpdateEffects(__instance);
        }

        public static void PreResetStats(CharacterController __instance)
        {
            Masquerade.Instance.ModEffectSystem.ResetCharacterEffects(__instance);
        }
    }
}
