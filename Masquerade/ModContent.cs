namespace Masquerade
{
    public abstract class ModContent
    {
        public Type Mod { get; internal set; }
        internal int ContentId;
        public virtual string ContentName { get => GetType().Name; }

        public string FullName => Mod.Name + "." + ContentName;
    }
}
