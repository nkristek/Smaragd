namespace NKristek.Smaragd
{
    internal static class Extensions
    {
        internal static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }
    }
}
