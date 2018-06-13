using MiniCloudServer.Core;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MiniCloudServer.Services
{
    public class DirectoryService : IDirectoryService
    {
        public const string mainPath = @"C:\MiniCloud";

        public void CreateDirectory(string userName, string path, string directoryName)
        {
            string serverPath = GenerateServerPath(userName, path);
            if (!Directory.Exists(serverPath))
                throw new MiniCloudException("Path is not valid");
            Directory.CreateDirectory($"{serverPath}//{directoryName}");
        }
        public void CreateUserDirectory(string userName)
        {
            Directory.CreateDirectory(GenerateServerPath(userName));
        }
        public XDocument GetDirectoryStructure(string userName)
        {
            var path = GenerateServerPath(userName);
            var dir = new DirectoryInfo(path);

            var doc = new XDocument(GetDirectoryXml(dir));
            return doc;
        }
        private XElement GetDirectoryXml(DirectoryInfo dir)
        {
            var info = new XElement("dir", new XAttribute("name", dir.Name));

            foreach (var file in dir.GetFiles())
            {
                info.Add(new XElement("file", new XAttribute("name", file.Name)));
            }

            foreach (var subDir in dir.GetDirectories())
            {
                info.Add(GetDirectoryXml(subDir));
            }

            return info;
        }
        public void RemoveDirectory(string userName, string path)
        {
            string serverPath = GenerateServerPath(userName, path);
            Directory.Delete(serverPath, true);
        }
        private string GenerateServerPath(string userName, string path = "")
        {
            return $@"{mainPath}\{userName}\{path}";
        }
    }
}
