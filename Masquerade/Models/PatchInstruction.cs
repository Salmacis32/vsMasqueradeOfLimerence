using System.Reflection;

namespace Masquerade.Models
{
    public struct PatchInstruction
    {
        public PatchInstruction(Type classType, string originalMethod, MethodInfo patchMethod, bool prefix = true, IEnumerable<Type> parameters = null)
        {
            ClassOrigin = classType;
            MethodToPatch = originalMethod;
            PatchMethod = patchMethod;
            IsPrefix = prefix;
            Parameters = (parameters == null) ? Array.Empty<Type>() : parameters.ToArray();
        }

        public Type ClassOrigin { get; }
        public bool IsPrefix { get; }
        public string MethodToPatch { get; }
        public MethodInfo PatchMethod { get; }
        public Type[] Parameters { get; set; }
    }
}