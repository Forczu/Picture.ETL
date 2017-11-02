using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Capitalizes the the first letter of a string. If it's null or empty, nothing happens.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>String with capitalized first letter if it's not null or empty. If so, returns the original.</returns>
        public static string Capitalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
        }
    }
}