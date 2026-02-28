using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Objects;
using Masquerade.Api;
using Masquerade.Models;

namespace Masquerade.Patches
{
    public class AccessoryPatches : IClassPatcher
    {
        public Type TargetClass => typeof(Accessory);

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new List<PatchInstruction>() { new PatchInstruction(TargetClass, nameof(Accessory.LevelUp), typeof(AccessoryPatches).GetMethod(nameof(PostLevelUp)), false) };
        }

        public static void PostLevelUp(Accessory __instance)
        {
            if (!MasqueradeApi.ModdedCheck(__instance.Type))
                return;

            var owner = __instance.Owner;
            var cc = owner.gameObject.GetOrAddComponent<GlobalInstanceComponent>();
            var cont = Masquerade.Api.GetOrAddCharacterInstance(owner, cc);
            if (cont == null)
            {
                Masquerade.Logger.Error("Character container failed to load!");
                return;
            }
            Masquerade.Api.GetOrAddModAccessoryInstance((int)__instance.Type, cont, owner).OnLevelUp();
        }
    }
}
