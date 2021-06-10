using System;

namespace API.Extension
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the string content before the first match of @search
        /// </summary>
        /// <param name="this">string to be searched</param>
        /// <param name="search">string to search for</param>
        /// <returns>The string found before @search parameter, or null if not found</returns>
        public static string GetBefore(this string @this, string search)
        {
            if (String.IsNullOrWhiteSpace(@this)) return null;
            int pos = @this.IndexOf(search);
            if (pos == -1) return null;

            return @this.Substring(0, pos);
        }

        /// <summary>
        /// Gets the string content before the last match of @search
        /// </summary>
        /// <param name="this">string to be searched</param>
        /// <param name="search">string to search for</param>
        /// <returns>The string found before the last @search parameter, or null if not found</returns>
        public static string GetBeforeLast(this string @this, string search)
        {
            if (String.IsNullOrWhiteSpace(@this)) return null;
            int pos = @this.LastIndexOf(search);
            if (pos == -1) return null;

            return @this.Substring(0, pos);
        }

        /// <summary>
        /// Gets the string content after the first match of @search
        /// </summary>
        /// <param name="this">string to be searched</param>
        /// <param name="search">string to search for</param>
        /// <returns>The string found after the first occurrence of @search, or null if not found</returns>
        public static string GetAfter(this string @this, string search)
        {
            if (String.IsNullOrWhiteSpace(@this)) return null;
            int pos = @this.IndexOf(search);
            if (pos == -1) return null;

            return @this.Substring(pos + search.Length);
        }

        /// <summary>
        /// Gets the string content after the last match of @search
        /// </summary>
        /// <param name="this">string to be searched</param>
        /// <param name="search">string to search for</param>
        /// <returns>The string found after the last occurrence of @search, or null if not found</returns>
        public static string GetAfterLast(this string @this, string search)
        {
            if (String.IsNullOrWhiteSpace(@this)) return null;
            int pos = @this.LastIndexOf(search);
            if (pos == -1) return null;

            return @this.Substring(pos + search.Length);
        }

        /// <summary>
        /// Gets the string content between @begin and @end
        /// </summary>
        /// <param name="this">string to be searched</param>
        /// <param name="begin">beginning of content (not included)</param>
        /// <param name="end">end of content (not included)</param>
        /// <returns>The string found between @begin and @end, the string after @begin if @end not found, or null if both were not found</returns>
        public static string GetBetween(this string @this, string begin, string end)
        {
            if (begin == null) throw new ArgumentNullException("begin");
            if (end == null) throw new ArgumentNullException("end");

            if (String.IsNullOrWhiteSpace(@this)) return null;
            int beginPos = @this.IndexOf(begin);
            if (beginPos == -1) return null;

            int beginStart = beginPos + begin.Length;
            int endPos = @this.IndexOf(end, beginStart);

            // after @begin
            if (endPos == -1)
                return @this.Substring(beginStart);

            // between @begin and @end
            return @this.Substring(beginStart, endPos - beginStart);
        }

        /// <summary>
        /// Ensures that the string will begin with a forward slash
        /// </summary>
        public static string EnsureStartingSlash(this string @this)
        {
            if (@this == null) return null;
            if (!@this.StartsWith("/"))
                @this = '/' + @this;

            return @this;
        }

        /// <summary>
        /// Ensures that the string will end with a forward slash
        /// </summary>
        public static string EnsureTrailingSlash(this string @this)
        {
            if (@this == null) return null;
            if (!@this.EndsWith("/"))
                @this += '/';

            return @this;
        }

        /// <summary>
        /// Contains() method for case-(in)sensitive
        /// </summary>
        public static bool Contains(this string @this, string search, StringComparison comparison)
        {
            return @this.IndexOf(search, comparison) != -1;
        }
    }
}