using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADAddinManagerDemo.Files
{
    public class TempFiles
    {
        private static volatile TempFiles m_inst;
        public static TempFiles Instance
        {
            get
            {
                if (TempFiles.m_inst == null)
                {
                    lock (typeof(TempFiles))
                    {
                        if (TempFiles.m_inst == null)
                        {
                            TempFiles.m_inst = new TempFiles();
                        }
                    }
                }
                return TempFiles.m_inst;
            }
        }

        private TempFiles() { AddinsTempFiles = new List<string>(); }

        /// <summary>
        /// 在临时文件夹下的插件地址集合
        /// </summary>
        public List<string> AddinsTempFiles { get; set; }
    }
}
