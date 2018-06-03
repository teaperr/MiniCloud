using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniCloudServer.Extensions
{
    public static class StringExtensions
    {
        public static string ToUnderScore(this string input)
        {
            string result = string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            return result;
        }
    }
}
