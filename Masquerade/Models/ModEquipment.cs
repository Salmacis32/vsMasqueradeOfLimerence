using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Graphics;
using UnityEngine;

namespace Masquerade.Models
{
    public abstract class ModEquipment : ModContent
    {
        public IList<WeaponData> WeaponData { get; protected set; }

        public string DisplayName { get; protected set; }

        public string Description { get; protected set; }

        public Texture2D Texture { get; protected set; }

        public ModEquipment() 
        {
            WeaponData = new List<WeaponData>();
            DisplayName = ContentName;
            Description = string.Empty;
            var levelOne = new WeaponData() 
                { level = 1, bulletType = (WeaponType)ContentId, name = DisplayName, description = Description, isPowerUp = true };
            WeaponData.Add(levelOne);
        }
    }
}
