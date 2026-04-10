using HarmonyLib;
using Il2CppVampireSurvivors.Objects;
using Masquerade.Models;
using Masquerade.Util;

namespace Masquerade.Patches
{
    public class AccessoryPatches : ClassPatcher<Accessory>
    {
        public static void PostLevelUp(Accessory __instance)
        {
            if (!Masquerade.Api.ModdedEquipmentCheck(__instance.Type))
                return;

            var owner = __instance.Owner;
            var cont = Masquerade.Api.GetOrAddCharacterInstance(owner);
            if (cont == null)
            {
                LoggerHelper.Logger.Error("Character container failed to load!");
                return;
            }
            Masquerade.Api.GetOrAddModAccessoryInstance((int)__instance.Type, cont, owner).OnLevelUp(owner);
        }

        public override IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            var test = AccessTools.GetDeclaredMethods(TargetClass);
            return new List<PatchInstruction>()
            {
                new PatchInstruction(TargetClass, nameof(Accessory.LevelUp), typeof(AccessoryPatches).GetMethod(nameof(PostLevelUp)), prefix: false),
            };
        }
    }
}