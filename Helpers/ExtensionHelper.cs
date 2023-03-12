using System.Collections;

namespace CSharpWpfChatGPT.Helpers
{
    public static class ExtensionHelper
    {
        public static bool IsBlank(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotBlank(this string str)
        {
            return !IsBlank(str);
        }

        public static bool IsEmpty(this ICollection c)
        {
            return (c == null) || (c.Count == 0);
        }

        public static bool IsNotEmpty(this ICollection c)
        {
            return !IsEmpty(c);
        }

        public static bool In<T>(this T item, params T[] testValues)
        {
            if (item != null)
            {
                foreach (T test_value in testValues)
                {
                    if (test_value != null && test_value.Equals(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool NotIn<T>(this T item, params T[] testValues)
        {
            return !In(item, testValues);
        }
    }
}
