using Microsoft.EntityFrameworkCore;
using MiniCloudServer.Core;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services.Interfaces;
using MiniCloudServer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MiniCloudServer.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly MiniCloudContext _context;
        public DirectoryService()
        {
            _context=new MiniCloudContext();
        }

        public void CreateDirectory(string userName, string path, string directoryName)
        {
            string serverPath = PathUtilities.GenerateFullPath(userName, path);
            if (!Directory.Exists(serverPath))
                throw new MiniCloudException("Path is not valid");
            Directory.CreateDirectory($"{serverPath}//{directoryName}");
        }
        public void CreateUserDirectory(string userName)
        {
            Directory.CreateDirectory(PathUtilities.GenerateFullPath(userName));
        }
        public async Task<XDocument> GetDirectoryStructure(string userName)
        {
            var resultDocument = new XDocument();
            var root=new XElement("root");

            var user = await _context.Users.Include(x => x.ResourceAccesses).ThenInclude(x=>x.OwnerUser).AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserName == userName);
            var resourceGroupedByOwnerName= user.ResourceAccesses.GroupBy(x=>x.OwnerUser.UserName).ToDictionary(x=>x.Key, y=>y.ToList());
            foreach (var resourcesByOwner in resourceGroupedByOwnerName)
            {
                var userElement = new XElement("owner", new XAttribute("name", resourcesByOwner.Key));
                foreach (var resource in resourcesByOwner.Value)
                {
                    
                    var fullPath = PathUtilities.ConvertUserPathToFullPath(resource.Path);
                    var resourceDirInfo = new DirectoryInfo(fullPath);
                    var resourceStructure = GetDirectoryXml(resourceDirInfo);
                    if (resourceStructure.Attribute("name").Value == resource.OwnerUser.UserName)
                           resourceStructure.Attribute("name").Value = "\\";
                    userElement.Add(resourceStructure);
                }
                root.Add(userElement);
            }
            resultDocument.Add(root);
            return resultDocument;
        }
        private XElement GetDirectoryXml(DirectoryInfo dir)
        {
            var doc = new XElement("dir", new XAttribute("name", dir.Name));
            foreach (var file in dir.GetFiles())
            {
                doc.Add(new XElement("file", new XAttribute("name", file.Name)));
            }
            foreach (var subDir in dir.GetDirectories())
            {
                doc.Add(GetDirectoryXml(subDir));
            }
            return doc;
        }
        public void RemoveDirectory(string userName, string path)
        {
            string serverPath = PathUtilities.GenerateFullPath(userName, path);
            Directory.Delete(serverPath, true);
        }
    }
}
