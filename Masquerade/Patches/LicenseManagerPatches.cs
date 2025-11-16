using Masquerade.Models;
using Il2CppVampireSurvivors.Framework.DLC;

namespace Masquerade.Patches
{
    public class LicenseManagerPatches : IClassPatcher
    {
        public Type TargetClass => typeof(LicenseManager);

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new PatchInstruction[] { new PatchInstruction(TargetClass, nameof(LicenseManager.SortDlcLists), typeof(LicenseManagerPatches).GetMethod(nameof(PostSortDlcLists)), prefix: false ) };
        }

        public void OnLoad()
        {
        }

        public void OnUnload()
        {
        }

        public static void PostSortDlcLists(LicenseManager __instance)
        {
            __instance.IncludedDlc.Add(Common.VSML_DLC_TYPE);
        }
    }
}
