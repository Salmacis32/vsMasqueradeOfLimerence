using HarmonyLib;
using Il2CppVampireSurvivors.Objects;
using Masquerade.Api;
using Masquerade.Models;

namespace Masquerade.Patches
{
    public class AccessoryPatches : IClassPatcher
    {
        public Type TargetClass => typeof(Accessory);

        public static void PostLevelUp(Accessory __instance)
        {
            if (!MasqueradeApi.ModdedCheck(__instance.Type))
                return;

            var owner = __instance.Owner;
            var cont = Masquerade.Api.GetOrAddCharacterInstance(owner);
            if (cont == null)
            {
                Masquerade.Logger.Error("Character container failed to load!");
                return;
            }
            Masquerade.Api.GetOrAddModAccessoryInstance((int)__instance.Type, cont, owner).OnLevelUp(owner);
        }

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            var test = AccessTools.GetDeclaredMethods(TargetClass);
            return new List<PatchInstruction>()
            {
                new PatchInstruction(TargetClass, nameof(Accessory.LevelUp), typeof(AccessoryPatches).GetMethod(nameof(PostLevelUp)), prefix: false),
            };
        }
    }
}