using Masquerade.Models;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        public ModAccessory GetModAccessory(MasqMod mod, string name)
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (!DoesModExist(mod.GetType())) throw new ArgumentException($"Mod {mod.Name} has not been loaded by Masquerade.");

            return AccessoryFactory.GetContent(mod, name);
        }

        public ModAccessory GetModAccessory(int contentId)
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);
            if (contentId < 0) throw new ArgumentOutOfRangeException(nameof(contentId));

            return AccessoryFactory.GetContent(contentId);
        }

        public T GetModAccessory<T>() where T : ModAccessory
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);

            return AccessoryFactory.GetContent<T>();
        }

        public CharacterContainer GetEquipmentOwner(ModEquipment modEquipment)
        {
            if (modEquipment == null || modEquipment.ContentId < 0)
            {
                Masquerade.Logger.Warning($"ModEquipment {modEquipment.FullName} has no instance id attached!");
                return null;
            }
            var character = _characters.SingleOrDefault(x => x.HasEquipment(modEquipment.ContentId));
            if (character == null)
            {
                Masquerade.Logger.Warning($"No character owns ModEquipment {modEquipment.FullName}");
                return null;
            }

            return character;
        }

        public bool DoesModExist(Type mod) => LoadedMods.Any(x => x.GetType() == mod);
        public bool DoesModExist(string name) => LoadedMods.Any(x => x.Name.Equals(name));
        public bool IsTypeModdedContent(int typeId)
        {
            if (AccessoryFactory.DoesContentExist(typeId))
                return true;

            return false;
        }

        public bool IsContentAccessory(int contentId)
        {
            if (!IsTypeModdedContent(contentId)) return false;
            
            return AccessoryFactory.DoesContentExist(contentId);
        }

        public MasqMod GetMod(string name)
        {
            return LoadedMods.SingleOrDefault(x => x.Name.Equals(name));
        }

        public MasqMod GetMod(Type type)
        {
            return LoadedMods.SingleOrDefault(x => x.GetType() == type);
        }
    }
}
