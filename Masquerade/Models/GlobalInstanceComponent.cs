using MelonLoader;
using UnityEngine;

namespace Masquerade.Models
{
    [RegisterTypeInIl2Cpp]
    public class GlobalInstanceComponent : MonoBehaviour
    {
        public GlobalInstanceComponent(IntPtr ptr) : base(ptr)
        {
        }

        public int GlobalInstanceId = -1;
    }
}
