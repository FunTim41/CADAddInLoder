using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using CADAddinManagerDemo.TreeViewInfo;

namespace CADAddinManagerDemo
{
    public class LoadHelper
    {
        private static string addInTempPath;

        /// <summary>
        /// 临时文件夹下的插件路径
        /// </summary>
        public static string AddInTempPath
        {
            get { return addInTempPath; }
        }
        private static string addInOriginalPath;

        /// <summary>
        /// 源文件夹下的插件路径
        /// </summary>
        public static string AddInOriginalPath
        {
            get { return addInOriginalPath; }
        }

        /// <summary>
        /// 把VS生成的插件所在的整个文件夹复制到临时文件夹中新建的CADAddinManager文件夹下,
        /// 并返回临时文件夹下的插件路径
        /// </summary>
        public static void CopyToTemp()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLL文件|*.dll";
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Title = "选择插件文件";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户临时文件夹路径
                string userTempPath = System.IO.Path.GetTempPath();

                // 新建CADAddinManager文件夹路径
                string newFolderPath = Path.Combine(userTempPath, "CADAddinManager");
                // 在临时文件夹下创建CADAddinManager文件夹
                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }
                else
                {
                    DateTime newTime = DateTime.Now;
                    // 更新文件夹的创建时间、访问时间和修改时间
                    Directory.SetCreationTime(newFolderPath, newTime);
                    Directory.SetLastAccessTime(newFolderPath, newTime);
                    Directory.SetLastWriteTime(newFolderPath, newTime);
                }
                //CAD插件路径
                addInOriginalPath = openFileDialog.FileName;
                //插件全名
                var Dllfilename = Path.GetFileName(AddInOriginalPath);
                var Dllname = Path.GetFileNameWithoutExtension(Dllfilename);
                // 获取dll文件的原目录路径
                string OriginalPath = Path.GetDirectoryName(AddInOriginalPath);
                //为每个Dll插件创建单独项目文件夹
                string targetFolderPath = Path.Combine(newFolderPath, Dllname);
                // 检查目标文件夹是否存在，如果不存在则创建
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }
                ClearFolder(targetFolderPath);
                // 获取原dll路径下的所有文件
                string[] files = Directory.GetFiles(OriginalPath);
                // 复制文件到临时文件夹
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(targetFolderPath, fileName);
                    File.Copy(file, destFile, true);
                    if (fileName == Dllfilename)
                    {
                        addInTempPath = Path.Combine(targetFolderPath, fileName);
                    }
                }
            }
        }

        static void ClearFolder(string folderPath)
        {
            try
            {
                // 删除文件夹中的所有文件
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                    Console.WriteLine("已删除文件: " + file);
                }
                // 删除文件夹中的所有子文件夹
                foreach (string subFolder in Directory.GetDirectories(folderPath))
                {
                    Directory.Delete(subFolder, true);
                    Console.WriteLine("已删除子文件夹: " + subFolder);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        internal static List<MethodTree> LoadAddinMethods(string tempPath)
        {
            var tempAssembly = Assembly.Load(File.ReadAllBytes(tempPath));
            Type attributeType = typeof(CommandMethodAttribute);
            
            // 获取所有带有指定特性的方法
            var methodsWithAttribute = tempAssembly
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0)
                .ToList();
            List<MethodTree> methods = new List<MethodTree>();
            methodsWithAttribute.ForEach(i =>
            {
                MethodTree method = new MethodTree();
                method.Name = i.Name;
                methods.Add(method);
            });
            return methods;
        }
    }
}
