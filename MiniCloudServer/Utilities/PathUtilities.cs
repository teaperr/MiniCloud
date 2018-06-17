using System;
using System.Collections.Generic;
using System.Text;

namespace MiniCloudServer.Utilities
{
    public static class PathUtilities
    {
        public const string mainPath = @"C:\MiniCloud";

        public static string GenerateFullPath(string userName, string path = "")
        {
            return $@"{mainPath}\{GenerateUserPath(userName,path)}";
        }
        public static string GenerateUserPath(string userName, string path = "")
        {
            return $@"{userName}\{path}";
        }
        public static string ConvertUserPathToFullPath(string userPath)
        {
            return $@"{mainPath}\{userPath}";
        }
    }
}
