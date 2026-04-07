using Masquerade.Util;
using MelonLoader;

namespace Masquerade.Models
{
    internal class ContentFactory<T> where T : ModContent
    {
        protected HashSet<T> _content;

        internal HashSet<T> GetAllContent() => [.. _content];
        internal IEnumerable<int> GetContentIds() => _content.Select(x => x.ContentId);

        internal ContentFactory()
        {
            _content = new HashSet<T>();
        }

        internal bool AddContent(T content)
        {
            if (content == null)
            {
                LoggerHelper.Logger.Error("Content being added is null! Skipping...");
                return false;
            }
            if (_content.Any(x => x.ContentId == content.ContentId))
            {
                LoggerHelper.Logger.Error("ContentId already exists! Skipping...");
                return false;
            }
            return _content.Add(content);
        }

        internal T GetContent(int contentId) => _content.SingleOrDefault(x => x.ContentId == contentId);

        internal T GetContent(MasqMod Mod, string ContentName) => _content.SingleOrDefault(x => x.ContentName == ContentName && x.Mod == Mod);

        internal Y GetContent<Y>() where Y : T => _content.SingleOrDefault(x => x.GetType() == typeof(Y)) as Y;

        internal bool DoesContentExist(int contentId) => _content.Any(x => x.ContentId == contentId);
    }
}
