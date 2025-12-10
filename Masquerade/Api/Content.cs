using Masquerade.Models;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        internal IEnumerable<MasqMod> LoadedMods;
        internal ContentFactory<ModAccessory> AccessoryFactory { get; set; }

        internal MasqueradeApi() { AccessoryFactory = new ContentFactory<ModAccessory>(); LoadedMods = []; }

        public ModAccessory GetModAccessory(MasqMod mod, string name)
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (!DoesModExist(mod)) throw new ArgumentException($"Mod {mod.Name} has not been loaded by Masquerade.");

            return AccessoryFactory.GetContent(mod, name);
        }

        public T GetModAccessory<T>() where T : ModAccessory
        {
            if (LoadedMods == null) throw new NullReferenceException(ExceptionMessages.MODS_LIST_NOT_INIT);

            return AccessoryFactory.GetContent<T>();
        }

        public bool DoesModExist(MasqMod mod) => LoadedMods.Any(x => x == mod);
        public bool DoesModExist(string name) => LoadedMods.Any(x => x.Name.Equals(name));

        public MasqMod GetMod(string name)
        {
            return LoadedMods.SingleOrDefault(x => x.Name.Equals(name));
        }
    }
}
