using Il2CppVampireSurvivors.UI;
using Masquerade.Models;

namespace Masquerade.Patches
{
    public class MainMenuPagePatches : ClassPatcher<MainMenuPage>
    {
        public override IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new PatchInstruction[0];
        }
    }
}
