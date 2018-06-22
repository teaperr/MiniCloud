using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MiniCloudGUI
{
    public class MCOwner
    {
        public ICollection<MCStructure> Structures { get; set; }
        public string Name { get; set; }
        public MCOwner(string name)
        {
            Structures=new List<MCStructure>();
            Name=name;
        }
    }

    public class MCStructure
    {
        public ICollection<MCStructure> Structures { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string OwnerName { get; set; }
        public bool IsFile { get; set; }

        public MCStructure(string name, string path, string ownerName, bool isFile)
        {
            Structures=new List<MCStructure>();
            Name=name;
            Path=path;
            OwnerName = ownerName;
            IsFile=isFile;
        }
    }
    public static class MCStructureGenerator
    {
        public static IEnumerable<MCOwner> GetStructure(string xmlStructure)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            var xmlOwners = xml.FirstChild.ChildNodes;
            var owners = new List<MCOwner>();
            foreach (XmlElement xmlOwner in xmlOwners)
            {
                string ownerName = xmlOwner.GetAttribute("name");
                var owner = new MCOwner(ownerName);
                owners.Add(owner);
                owner.Structures = GetDirectoriesStructure(xmlOwner, ownerName);
            }
            return owners;

        }
        private static ICollection<MCStructure> GetDirectoriesStructure(XmlElement xmlOwner, string ownerName)
        {
            var result = new List<MCStructure>();
            foreach (XmlElement child in xmlOwner.ChildNodes)
            {
                string attributeName = child.GetAttribute("name");
                string path = $"{attributeName}";
                var directory = new MCStructure(attributeName, path, ownerName, false);
                GenerateDirectoryStructure(directory, ownerName, child);
                result.Add(directory);
            }
            return result;
        }
        private static void GenerateDirectoryStructure(MCStructure parentDir, string ownerName, XmlElement parentNode)
        {
            foreach (XmlElement child in parentNode.ChildNodes)
            {
                var attributeName = child.GetAttribute("name");
                var path = $"{parentDir.Path}\\{attributeName}";
                if (child.Name == "file")
                {
                    parentDir.Structures.Add(new MCStructure(attributeName, path, ownerName, true));
                }
                else
                {
                    var directory = new MCStructure(attributeName, path, ownerName, false);
                    parentDir.Structures.Add(directory);
                    GenerateDirectoryStructure(directory, ownerName, child);
                }

            }
        }
    }
}
