namespace Masquerade.Models
{
    internal class EquipmentFactory<T> : ContentFactory<T> where T : ModEquipment
    {
        internal T GetEquipment(int weaponTypeId) => _content.SingleOrDefault(x => x.WeaponTypeId == weaponTypeId);

        internal bool DoesEquipmentExist(int weaponTypeId) => _content.Any(x => x.WeaponTypeId == weaponTypeId);
    }
}
