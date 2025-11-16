using System.Reflection;

namespace Masquerade.Models
{
    public struct PatchInstruction
    {
        public PatchInstruction(Type classType, string originalMethod, MethodInfo patchMethod, bool prefix = true)
        {
            ClassOrigin = classType;
            MethodToPatch = originalMethod;
            PatchMethod = patchMethod;
            IsPrefix = prefix;
        }

        public Type ClassOrigin { get; }
        public bool IsPrefix { get; }
        public string MethodToPatch { get; }
        public MethodInfo PatchMethod { get; }
    }
}