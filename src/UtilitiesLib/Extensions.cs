using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace gov.sandia.sld.common.utilities
{
    public static class Extensions
    {
        // Took this code from this Jon Skeet post:
        // http://stackoverflow.com/a/145967/706747
        public static string JoinStrings<T>(this IEnumerable<T> source,
                                            Func<T, string> projection, string separator)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (T element in source)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(separator);
                }
                builder.Append(projection(element));
            }
            return builder.ToString();
        }

        public static string JoinStrings<T>(this IEnumerable<T> source, string separator)
        {
            return JoinStrings(source, t => t.ToString(), separator);
        }

        public static bool IsIPAddress(this string ip)
        {
            return string.IsNullOrEmpty(ip) == false && string.Compare("0.0.0.0", ip) != 0 && IPAddress.TryParse(ip, out IPAddress address);
        }

        /// <summary>
        /// Found this guy at
        /// http://stackoverflow.com/a/24087164/706747
        /// </summary>
        /// <typeparam name="T">The type of object in the list</typeparam>
        /// <param name="source">The list to split</param>
        /// <param name="chunkSize">How many sub-lists to return</param>
        /// <returns></returns>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            chunkSize = Math.Max(chunkSize, 1);
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        /// <summary>
        /// Found this at: http://stackoverflow.com/a/15728577/706747
        /// </summary>
        public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict,
                                           TKey oldKey, TKey newKey)
        {
            TValue value;
            if (!dict.TryGetValue(oldKey, out value))
                return false;

            dict.Remove(oldKey);  // do not change order
            dict[newKey] = value;  // or dict.Add(newKey, value) depending on ur comfort
            return true;
        }

        public static long NextLong(this Random r)
        {
            byte[] bytes = new byte[8];
            r.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
