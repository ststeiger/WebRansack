
namespace WebRansack
{


    public static class StringAndBuilderExtensions
    {


        public static string Replace(this string str, string oldValue, string newValue, System.StringComparison comparisonType)
        {
            newValue = newValue ?? "";

            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue) || oldValue.Equals(newValue, comparisonType))
                return str;

            int foundAt = 0;
            while ((foundAt = str.IndexOf(oldValue, foundAt, comparisonType)) != -1)
            {
                str = str.Remove(foundAt, oldValue.Length).Insert(foundAt, newValue);
                foundAt += newValue.Length;
            }
            return str;
        }




        /// <summary>
        /// Returns the index of the start of the contents in a StringBuilder
        /// </summary>        
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns></returns>
        public static int IndexOf(this System.Text.StringBuilder sb, string value, int startIndex, bool ignoreCase)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (char.ToLower(sb[i]) == char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (char.ToLower(sb[i + index]) == char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        } // End Function IndexOf 


        public static int IndexOf(this System.Text.StringBuilder sb, string value, bool ignoreCase)
        {
            return IndexOf(sb, value, 0, ignoreCase);
        }


    } // End Class 


} // End Namespace 
