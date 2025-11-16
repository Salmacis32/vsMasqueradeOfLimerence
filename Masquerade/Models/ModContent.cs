namespace Masquerade.Models
{
    public abstract class ModContent
    {
        public Type Mod { get; internal set; }
        internal int ContentId;
        public virtual string ContentName { get => this.GetType().Name; }

        public string FullName => Mod.Name + "." + ContentName;
    }
}
