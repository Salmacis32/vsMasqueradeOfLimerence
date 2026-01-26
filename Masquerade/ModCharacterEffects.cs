using Masquerade.Models;

namespace Masquerade
{
    public abstract class ModCharacterEffects : IInstanced
    {
        public int InstanceId { get; internal set; } = -1;
    }
}
