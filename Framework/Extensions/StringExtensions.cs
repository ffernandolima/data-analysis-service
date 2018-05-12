
using System.Text;

namespace DataAnalysis.Framework.Extensions
{
    public static class StringExtensions
    {
        public static StringBuilder RTrim(this StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                var length = sb.Length;
                var cursor = length - 1;

                while ((cursor > -1) && char.IsWhiteSpace(sb[cursor]))
                {
                    cursor--;
                }

                if (cursor < (length - 1))
                {
                    sb.Remove(cursor + 1, (length - cursor) - 1);
                }
            }

            return sb;
        }

        public static StringBuilder LTrim(this StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                var length = sb.Length;
                var cursor = 0;

                while ((cursor < length) && char.IsWhiteSpace(sb[cursor]))
                {
                    cursor++;
                }

                if (cursor > 0)
                {
                    sb.Remove(0, cursor);
                }
            }

            return sb;
        }

        public static StringBuilder Trim(this StringBuilder sb)
        {
            var t = sb;
            t.LTrim();
            t.RTrim();
            return sb;
        }
    }
}
