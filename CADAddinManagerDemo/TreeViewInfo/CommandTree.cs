using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
        /// Dll的原始路径
        /// </summary>
        public string OriPath { get; set; }

        /// <summary>
        /// 插件的命令集合
        /// </summary>
        public ObservableCollection<MethodTree> CommandMethodNames { get; set; } = new();
    }

    public class MethodTree
    {
        public string Name { get; set; }
        public string DllName { get; set; }
        public string ClassName { get; set; }
        public string tempPath { get; set; }
        public Assembly assembly { get; set; }
    }
}
