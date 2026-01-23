namespace Masquerade
{
    public abstract class ModContent
    {
        public ModContent() { }
        public ModContent(ModContent orig)
        {
            Mod = orig.Mod;
            ContentId = orig.ContentId;
        }
        public MasqMod Mod { get; internal set; }
        internal int ContentId;
        public virtual string ContentName { get => GetType().Name; }

        public string FullName => Mod.Name + "." + ContentName;
    }
}
