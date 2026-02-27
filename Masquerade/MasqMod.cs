using MelonLoader;

namespace Masquerade
{
    public abstract class MasqMod : MelonMod
    {
        public string Name { get => this.Info.Name; }

        public bool IgnoreWeapons { get; protected set; }
    }
}
