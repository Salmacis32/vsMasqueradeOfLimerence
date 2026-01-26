using Il2CppInterop.Runtime.InteropTypes.Fields;
using MelonLoader;
using UnityEngine;

namespace Masquerade.Models
{
    [RegisterTypeInIl2Cpp]
    public class GlobalInstanceComponent(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Il2CppValueField<int> GlobalInstanceId;
    }
}
