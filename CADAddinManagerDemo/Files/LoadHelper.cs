using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shell;
using Autodesk.AutoCAD.Runtime;
using CADAddinManagerDemo.Files;
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
        /// 已加载的Dll合集
        /// </summary>
        private static List<Assembly> addInsDll = new();

        /// <summary>
        /// 已加载的Dll
        /// </summary>
        public static List<Assembly> AddInsDll
        {
            get { return addInsDll; }
        }

        /// <summary>
        /// 把VS生成的插件所在的整个文件夹复制到临时文件夹中新建的CADAddinManager文件夹下,
        /// 并返回临时文件夹下的插件路径,
        /// 与原文件夹下的插件路径
        /// </summary>
        public static bool SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLL文件|*.dll";
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Title = "选择插件文件";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //移除这个之后再加载这个，就会报无法找到资源。估计是和删除临时文件夹有关
                if (
                    Path.GetFileNameWithoutExtension(openFileDialog.FileName)
                    == "CADAddinManagerDemo"
                )
                {
                    return false;
                }
                if (openFileDialog.FileName != null)
                {
                    CopyToTempByOripath(openFileDialog.FileName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 从原文件夹复制到临时文件夹
        /// </summary>
        /// <param name="Oripath"></param>
        public static void CopyToTempByOripath(string Oripath)
        {
            try
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

                //CAD插件路径
                addInOriginalPath = Oripath;
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
                    if (fileName == Dllfilename)
                    {
                        addInTempPath = destFile;
                    }

                    if (fileName.Contains(Dllname))
                    { //复制到临时文件夹中的对于dll文件夹
                        File.Copy(file, destFile, true);
                    }
                    //直接加载其余被应用的dll
                    else
                    {
                        File.Copy(file, destFile, true);
                        if (fileName.Contains(".dll"))
                        {
                            Assembly.UnsafeLoadFrom(destFile);
                        }
                    }
                }
            }
            catch (System.IO.IOException) { }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "复制文件到临时文件夹失败");
                return;
            }
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="folderPath">需要清空的文件夹</param>
        public static void ClearFolder(string folderPath)
        {
            try
            {
                // 删除文件夹中的所有文件
                //foreach (string file in Directory.GetFiles(folderPath))
                //{
                //    File.Delete(file);

                //}
                // 删除文件夹中的所有子文件夹
                foreach (string subFolder in Directory.GetDirectories(folderPath))
                {
                    Directory.Delete(subFolder, true);
                }
            }
            catch (System.UnauthorizedAccessException) { }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        /// <summary>
        /// 加载所有带有特性的方法到treeview中
        /// </summary>
        /// <param name="tempPath">Dll文件地址</param>
        /// <param name="name">Dll名字</param>
        /// <returns></returns>
        public static ObservableCollection<MethodTree> LoadAddinMethods(
            string tempPath,
            string name
        )
        {
            try
            {
                string pdbname = Path.GetFileNameWithoutExtension(tempPath) + ".pdb";
                string padPath = Path.GetDirectoryName(tempPath);
                var tempAssembly = Assembly.Load(
                    File.ReadAllBytes(tempPath),
                    File.ReadAllBytes(Path.Combine(padPath, pdbname))
                );
                if (addInsDll.Contains(tempAssembly))
                { //把旧的移除
                    addInsDll.Remove(tempAssembly);
                }
                addInsDll.Add(tempAssembly);
                Type attributeType = typeof(CommandMethodAttribute);
                List<MethodInfo> methodsWithAttribute = new List<MethodInfo>();
                ObservableCollection<MethodTree> methods = new ObservableCollection<MethodTree>();
                // 遍历所有类型
                foreach (Type type in tempAssembly.GetTypes())
                {
                    // 获取所有方法
                    methodsWithAttribute = type.GetMethods().ToList();
                    foreach (MethodInfo dllmethod in methodsWithAttribute)
                    { // 检查方法是否具有指定特性
                        if (dllmethod.GetCustomAttribute(typeof(CommandMethodAttribute)) != null)
                        { //得到方法名，所属的类名
                            MethodTree method = new MethodTree();
                            method.Name = dllmethod.Name;
                            method.ClassName =
                                dllmethod.DeclaringType.Namespace
                                + "."
                                + dllmethod.DeclaringType.Name;
                            method.DllName = name;
                            method.tempPath = tempPath;
                            method.assembly = tempAssembly;
                            methods.Add(method);
                        }
                    }
                }

                return methods;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "反射程序集失败");
                TempFiles.Instance.AddinsTempFiles.Remove(AddInOriginalPath);
                return null;
            }
        }
    }
}
