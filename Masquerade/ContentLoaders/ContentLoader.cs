using Masquerade.Interfaces;

namespace Masquerade.ContentLoaders
{
    internal abstract class ContentLoader : IMelonInitializable
    {
        public abstract void Deinitialize();
        public abstract void Initialize();
    }
}
