using Il2CppVampireSurvivors.Objects.Characters;

namespace Masquerade.Models
{
    internal class CharacterReference
    {  
        /*
        public CharacterReference(Il2CppVampireSurvivors.Objects.Equipment equip) 
        {
            Instance = equip.Owner;
            ModInstance = new ModCharacter();
            Equipment.Add(0, new EquipmentReference(equip));
        }
        internal CharacterController Instance { get; set; }
        */
        internal ModCharacter ModInstance { get; set; }
        internal IDictionary<int, EquipmentReference> Equipment { get; set; } = new Dictionary<int, EquipmentReference>();
    }
}
