using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudGUI
{
    public class MCOwner
    {
        public ICollection<MCDirectory> Directories { get; set; }
        public string Name { get; set; }
        public MCOwner(string name)
        {
            Directories=new List<MCDirectory>();
            Name=name;
        }
    }

    public class MCDirectory
    {
        public ICollection<MCDirectory> Directories { get; set; }
        public ICollection<MCFile> Files { get; set; }
        public string Name { get; set; }

        public MCDirectory(string name)
        {
            Directories=new List<MCDirectory>();
            Files=new List<MCFile>();
            Name=name;
        }
    }

    public class MCFile
    {
        public string Name { get; set; }
    }
}
