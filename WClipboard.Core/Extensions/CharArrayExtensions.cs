using System;

namespace WClipboard.Core.Extensions
{
    public static class CharArrayExtensions
    {
        /// <summary>
        /// Searches for search in array
        /// </summary>
        /// <param name="array">The buffer</param>
        /// <param name="search">The string to search for</param>
        /// <param name="progress">current progress of findin the search string, progress is set 0 when found</param>
        /// <param name="startIndex">the index to start from</param>
        /// <returns>Returns int.MinValue if not found, Returns the start index of search in array note: Returns negative if the progress was already started in a previous buffer</returns>
        public static int FirstIndexOf(this char[] array, string search, ref int progress, int startIndex = 0)
        {
            if (progress < 0 || progress > search.Length)
                throw new ArgumentOutOfRangeException(nameof(progress), $"{nameof(progress)} must be a value between 0 and the {nameof(search)}.{nameof(search.Length)}");

            char searchChar = search[progress];
            for (int i = startIndex; i < array.Length; i++)
            {
                char currentChar = array[i];
                if (currentChar == searchChar)
                {
                    progress += 1;
                    if (progress == search.Length)
                    {
                        progress = 0;
                        return i + 1 - search.Length;
                    }
                        
                    searchChar = search[progress];
                } 
                else if(progress != 0)
                {
                    progress = 0;
                    searchChar = search[progress];
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// Searches for search in array
        /// </summary>
        /// <param name="array">The buffer</param>
        /// <param name="search">The string to search for</param>
        /// <param name="startIndex">the index to start from</param>
        /// <returns>Returns int.MinValue if not found, Returns the start index of search in array</returns>
        public static int FirstIndexOf(this char[] array, string search, int startIndex = 0)
        {
            int _ = 0;
            return FirstIndexOf(array, search, ref _, startIndex);
        }
    }
}
