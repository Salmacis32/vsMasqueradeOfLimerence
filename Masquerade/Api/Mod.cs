namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        public bool DoesModExist(Type mod) => LoadedMods.Any(x => x.GetType() == mod);
        public bool DoesModExist(string name) => LoadedMods.Any(x => x.Name.Equals(name));

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
