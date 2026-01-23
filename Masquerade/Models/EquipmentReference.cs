namespace Masquerade.Models
{
    internal class EquipmentReference
    {
        internal EquipmentReference(ModEquipment modInstance)
        {
            ModInstance = modInstance;
        }

        /*
        internal EquipmentReference(Il2CppVampireSurvivors.Objects.Equipment equip) 
        { 
            Instance = equip;
            ModInstance = Masquerade.Api.GetModAccessory((int)equip.Type);
        }
        
        internal Il2CppVampireSurvivors.Objects.Equipment Instance { get; set; }
        */
        internal ModEquipment ModInstance { get; }
    }
}
