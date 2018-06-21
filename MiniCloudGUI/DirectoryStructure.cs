using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
