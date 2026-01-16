using Il2Cpp;
using MelonLoader.Utils;
using System.Reflection;
using UnityEngine;
using UnityEngine.Bindings;

namespace Masquerade.Util
{
    public static class TextureHelpers
    {
        /// Adapted from https://github.com/BepInEx/Il2CppInterop/issues/202#issuecomment-2692008152. Thanks DustinBryant, for the temporary fix
        public static Texture2D LoadImageToTexture2d(Assembly assembly, string rootPath, string imageName, FilterMode filter = FilterMode.Point)
        {
            var imageLocation = rootPath + imageName;
            using (Stream stream = assembly.GetManifestResourceStream(imageLocation))
            {
                var image = new Texture2D(2, 2);
                var imageAsBytes = new byte[stream.Length];
                stream.Read(imageAsBytes, 0, imageAsBytes.Length);

                unsafe
                {
                    var intPtr = UnityEngine.Object.MarshalledUnityObject.MarshalNotNull(image);

                    fixed (byte* ptr = imageAsBytes)
                    {
                        var managedSpanWrapper = new ManagedSpanWrapper(ptr, imageAsBytes.Length);

                        ImageConversion.LoadImage_Injected(intPtr, ref managedSpanWrapper, false);
                    }
                }

                image.name = Path.GetFileNameWithoutExtension(imageName);
                image.filterMode = filter;
                return image;
            }
        }
    }
}