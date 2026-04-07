using Masquerade.Models;

namespace Masquerade
{
    public abstract class ModCharacterEffect : ModContent, IInstanced
    {
        public CharacterContainer Owner { get; internal set; }
        public string DisplayName { get; set; }
        public int InstanceId { get; internal set; } = -1;
    }
}
