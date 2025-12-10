using MelonLoader;

namespace Masquerade
{
    public abstract class MasqMod : MelonMod
    {
        public string Name { get => MelonTypeName; }

        public bool IgnoreWeapons { get; protected set; }
    }
}
