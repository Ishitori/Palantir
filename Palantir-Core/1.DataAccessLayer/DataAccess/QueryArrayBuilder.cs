namespace Ix.Palantir.DataAccess
{
    using System.Text;

    public static class QueryArrayBuilder
    {
        private const string CONST_ArrayTemplate = "array[{0}]";

        public static string GetString(long[] ids)
        {
            return Format(ids);
        }

        public static string GetString(int[] ids)
        {
            return Format(ids);
        }

        private static string Format<T>(T[] ids)
        {
            var str = new StringBuilder();

            if (ids.Length == 0)
            {
                return string.Format(CONST_ArrayTemplate, str);
            }

            for (int i = 1; i < ids.Length; i++)
            {
                str.AppendFormat("{0},", ids[i]);
            }

            return string.Format(CONST_ArrayTemplate, str.ToString().TrimEnd(','));
        }
    }
}
