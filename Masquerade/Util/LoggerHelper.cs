using MelonLoader;

namespace Masquerade.Util
{
    public static class LoggerHelper
    {
        public static MelonLogger.Instance Logger => Masquerade.Instance.LoggerInstance;
    }
}
