using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CADAddinManagerDemo.Files
{
    public class TempFiles
    {
        private static TempFiles m_inst;
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
        public readonly string pathFile = "CADAddinManager\\output.txt";

        private TempFiles()
        {
            AddinsTempFiles = new List<string>();
            TempFilesLoad();
        }

        /// <summary>
        /// 在临时文件夹下的插件地址集合
        /// </summary>
        public List<string> AddinsTempFiles { get; set; }

        public void TempFilesSave()
        {
           
            var list = AddinsTempFiles.Distinct().ToList();
            string folderPath = Path.GetTempPath();
            string filePath = Path.Combine(folderPath, pathFile);
            try
            {
                // 确保文件可以被写入
                using (
                    FileStream fs = new FileStream(
                        filePath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None
                    )
                )
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (string line in list)
                    {
                       
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return;
            }
        }

        public void TempFilesLoad()
        {
            try
            {
                string filePath = Path.Combine(Path.GetTempPath(), pathFile);
                List<string> list = new List<string>();

                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
                
                list.RemoveAll(i => i.Contains("CADAddinManagerDemo"));
                // 读取文件的所有行并存储到List中
                AddinsTempFiles = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动前加载失败: " + ex.Message);
                return;
            }
        }
    }
}
