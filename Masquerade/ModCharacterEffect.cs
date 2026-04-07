using Il2CppVampireSurvivors.Interfaces;
using Il2CppVampireSurvivors.Objects.Characters;
using Masquerade.Models;

namespace Masquerade
{
    public abstract class ModCharacterEffect : ModContent
    {
        public ModCharacterEffect()
        {
            if (DisplayName == null) DisplayName = ContentName;
        }
        public string DisplayName { get; set; }

        public virtual void OnUpdate(CharacterController owner)
        {

        }
    }
}
