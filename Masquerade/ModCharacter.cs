using Masquerade.Models;

namespace Masquerade
{
    public class ModCharacter : IInstanced
    {
        public int InstanceId { get; internal set; } = -1;
    }
}
