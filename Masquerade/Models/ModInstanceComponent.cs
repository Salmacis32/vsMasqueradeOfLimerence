using MelonLoader;
using UnityEngine;

namespace Masquerade.Models
{
    [RegisterTypeInIl2Cpp]
    public class ModInstanceComponent : MonoBehaviour
    {
        public ModInstanceComponent(IntPtr ptr) : base(ptr)
        {
        }

        public int ModInstanceId = -1;
    }
}
