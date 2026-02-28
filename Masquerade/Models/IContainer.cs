namespace Masquerade.Models
{
    internal interface IContainer
    {
        int TypeId { get; }

        bool UpdateContainerNextTick { get; }

        void UpdateContainer();
    }
}
