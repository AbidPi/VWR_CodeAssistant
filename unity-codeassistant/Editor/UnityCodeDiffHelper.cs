using System.Collections.Generic;

namespace UnityCodeAssistant
{
    public static class UnityCodeDiffHelper
    {
        public static List<string> DiffLines(string[] a, string[] b)
        {
            var result = new List<string>();
            int i = 0, j = 0;
            while (i < a.Length && j < b.Length)
            {
                if (a[i] == b[j])
                {
                    result.Add("  " + a[i]);
                    i++; j++;
                }
                else
                {
                    result.Add("- " + a[i]);
                    result.Add("+ " + b[j]);
                    i++; j++;
                }
            }

            while (i < a.Length) result.Add("- " + a[i++]);
            while (j < b.Length) result.Add("+ " + b[j++]);

            return result;
        }
    }
}
