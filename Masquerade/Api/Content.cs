namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        public ModCharacterEffect GetModCharacterEffect(MasqMod mod, string name)
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (!DoesModExist(mod.GetType())) throw new ArgumentException($"Mod {mod.Name} has not been loaded by Masquerade.");

            return CharacterEffectFactory.GetContent(mod, name);
        }

        public ModCharacterEffect GetModCharacterEffect(int contentId)
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);
            if (contentId < 0) throw new ArgumentOutOfRangeException(nameof(contentId));

            return CharacterEffectFactory.GetContent(contentId);
        }

        public T GetModCharacterEffect<T>() where T : ModCharacterEffect
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);

            return CharacterEffectFactory.GetContent<T>();
        }

        public bool IsModdedEquipment(int weaponTypeId)
        {
            return AccessoryFactory.DoesEquipmentExist(weaponTypeId);
        }

        public bool IsContentAccessory(int contentId)
        {
            if (!IsModdedEquipment(contentId)) return false;
            
            return AccessoryFactory.DoesContentExist(contentId);
        }

        public bool IsContentCharacterEffect(int contentId)
        {
            if (!IsModdedEquipment(contentId)) return false;

            return CharacterEffectFactory.DoesContentExist(contentId);
        }
    }
}
