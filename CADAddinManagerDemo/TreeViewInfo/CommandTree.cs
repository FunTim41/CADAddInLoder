using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADAddinManagerDemo.TreeViewInfo
{
    public class CommandTree
    {
        /// <summary>
        /// CAD的Dll插件名称，不含后缀
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 插件的命令集合
        /// </summary>
        public List<MethodTree> CommandMethodNames { get; set; } = new();
    }

    public class MethodTree
    {
        public string Name { get; set; }
    }
}
