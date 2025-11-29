using Il2CppVampireSurvivors.Objects;
using Masquerade.Stats;

namespace Masquerade
{
    public abstract class ModAccessory : ModEquipment
    {
        public virtual CharacterModifierStats StartingModifierStats { get; protected set; }
    }
}
