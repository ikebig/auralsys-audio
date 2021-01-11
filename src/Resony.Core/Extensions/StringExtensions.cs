using System.Linq;

namespace Resony
{
    public static class StringExtensions
    {
        public static bool IsIn(this string value, params string[] array)
        {
            if (value == null || array == null || array.Length == 0)
            {
                return false;
            }

            return array.Contains(value);
        }
    }
}
