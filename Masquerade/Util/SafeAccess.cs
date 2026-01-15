using MelonLoader;

namespace Masquerade.Util
{
    /// <summary>
    /// Static methods for safely getting properties from Il2Cpp classes.
    /// </summary>
    /// <remarks>
    /// Credit to whendeh for creating these
    /// </remarks>
    public static class SafeAccess
    {
        public static T GetProperty<T>(object il2cppObject, string propertyName, T defaultValue = default(T))
        {
            try
            {
                if (il2cppObject == null) return defaultValue;

                var property = il2cppObject.GetType().GetProperty(propertyName);
                if (property == null) return defaultValue;

                var value = property.GetValue(il2cppObject);
                return value is T typedValue ? typedValue : defaultValue;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Attempted to access property {propertyName}: {ex.Message}");
                return defaultValue;
            }
        }

        public static void SetProperty<T>(object il2cppObject, string propertyName, T value)
        {
            try
            {
                if (il2cppObject == null) return;

                var property = il2cppObject.GetType().GetProperty(propertyName);
                if (property?.CanWrite == true)
                {
                    property.SetValue(il2cppObject, value);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error setting property {propertyName}: {ex.Message}");
            }
        }
    }
}
