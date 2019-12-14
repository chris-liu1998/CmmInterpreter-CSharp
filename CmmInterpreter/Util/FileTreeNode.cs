using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Util
{
    public class FileTreeNode
    {
        public List<FileTreeNode> Children { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsFile { get; set; }
        public string Path { get; set; }
    }
}
