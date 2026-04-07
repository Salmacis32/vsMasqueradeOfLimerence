namespace Masquerade
{
    public abstract class ModContent
    {
        public ModContent() { }
        public MasqMod Mod { get; internal set; }
        public int ContentId { get; internal set; }
        public virtual string ContentName { get => GetType().Name; }

        public string FullName => Mod.Name + "." + ContentName;
    }
}
