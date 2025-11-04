namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit;
    internal class RequiredMemberAttribute : Attribute;
    internal class CompilerFeatureRequiredAttribute(string name) : Attribute
    {
        internal string Name { get; } = name;
    }
    internal class IsUnmanagedAttribute : Attribute;
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor)]
    internal class SetsRequiredMembersAttribute : Attribute;
}
