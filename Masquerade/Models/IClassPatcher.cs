namespace Masquerade.Models
{
    public interface IClassPatcher
    {
        Type TargetClass { get; }
        IEnumerable<PatchInstruction> GeneratePatchInstructions();
    }
}
