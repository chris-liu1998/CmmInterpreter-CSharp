using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmInterpreter.Util
{
    internal class TreeNodeData
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public List<TreeNodeData> Children { get; set; }

        public TreeNodeData()
        {
            Children = new List<TreeNodeData>();
        }
    }
}
