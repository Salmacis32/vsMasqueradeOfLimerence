namespace Masquerade.Models
{
    public interface IClassPatcher
    {
        Type TargetClass { get; }
        IEnumerable<PatchInstruction> GeneratePatchInstructions();
    }
    public abstract class ClassPatcher<T> : IClassPatcher where T : Il2CppSystem.Object
    {
        public Type TargetClass { get => typeof(T); }
        public abstract IEnumerable<PatchInstruction> GeneratePatchInstructions();
    }
}
