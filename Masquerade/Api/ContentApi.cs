using Masquerade.Models;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        private const string LOADED_MODS_EXCEPTION_MESSAGE = "Mods list not initialized!";
        internal IEnumerable<Type> LoadedMods;
        internal ContentFactory<ModAccessory> AccessoryFactory { get; set; }

        internal MasqueradeApi() { AccessoryFactory = new ContentFactory<ModAccessory>(); LoadedMods = []; }

        public ModAccessory GetModAccessory(Type mod, string name)
        {
            if (LoadedMods == null) throw new NullReferenceException(LOADED_MODS_EXCEPTION_MESSAGE);
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (!LoadedMods.Any(x => x == mod)) throw new ArgumentException($"Mod {mod.Name} has not been loaded by Masquerade.");

            return AccessoryFactory.GetContent(mod, name);
        }

        public T GetModAccessory<T>() where T : ModAccessory
        {
            if (LoadedMods == null) throw new NullReferenceException(LOADED_MODS_EXCEPTION_MESSAGE);

            return AccessoryFactory.GetContent<T>();
        }
    }
}
