namespace Masquerade.Models
{
    internal sealed class ContentFactory<T> where T : ModContent
    {
        private HashSet<T> _content;

        internal HashSet<T> GetAllContent() => [.. _content];

        internal ContentFactory()
        {
            _content = new HashSet<T>();
        }

        internal bool AddContent(T content)
        {
            if (content == null)
            {
                Masquerade.Logger.Error("Content being added is null! Skipping...");
                return false;
            }
            if (_content.Any(x => x.ContentId == content.ContentId))
            {
                Masquerade.Logger.Error("ContentId already exists! Skipping...");
                return false;
            }
            return _content.Add(content);
        }

        internal T GetContent(int ContentId) => _content.SingleOrDefault(x => x.ContentId == ContentId);

        internal T GetContent(Type Mod, string ContentName) => _content.SingleOrDefault(x => x.ContentName == ContentName && x.Mod == Mod);

        internal Y GetContent<Y>() where Y : T => _content.SingleOrDefault(x => x.GetType() == typeof(Y)) as Y;
    }
}
