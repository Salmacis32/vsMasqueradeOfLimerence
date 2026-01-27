namespace Masquerade
{
    public abstract class ModContent
    {
        public ModContent() { }
        public MasqMod Mod { get; internal set; }
        internal int ContentId;
        public virtual string ContentName { get => GetType().Name; }

        public string FullName => Mod.Name + "." + ContentName;
    }
}
