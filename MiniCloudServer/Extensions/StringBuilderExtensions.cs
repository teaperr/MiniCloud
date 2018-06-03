using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniCloudServer.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendSpace(this StringBuilder stringBuilder)
        {
            return stringBuilder.Append(" ");
        }
    }
}
