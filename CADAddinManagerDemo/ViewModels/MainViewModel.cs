using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using Autodesk.AutoCAD.Customization;
using CADAddinManagerDemo.Files;
using CADAddinManagerDemo.TreeViewInfo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CADAddinManagerDemo.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// CAD的Dll插件原路径
        /// </summary>
        [ObservableProperty]
        string addInOriginalPath = null;

        /// <summary>
        /// CAD的Dll插件临时文件路径
        /// </summary>
        string addInTempPath;

        /// <summary>
        /// 当前选中的命令
        /// </summary>
        [ObservableProperty]
        MethodTree currentCommand;

        /// <summary>
        /// 当前选中命令所在的dll
        /// </summary>
        [ObservableProperty]
        CommandTree currentAddinDll;

        List<CommandTree> Commands = new();

        [ObservableProperty]
        ICollectionView commandsTrees;

        public MainViewModel()
        {
            CommandsTrees = CollectionViewSource.GetDefaultView(Commands);
            CommandsTrees.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending)
            );
            LoadPath();
        }

        /// <summary>
        /// 加载命令方法到treeview
        /// </summary>
        [RelayCommand]
        void LoadAddin()
        {
            try
            {
                if (LoadHelper.SelectFile())
                {
                    LoadAddinToTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void LoadAddinToTreeView()
        {
            CommandTree commandTree = new CommandTree();
            string originalPath = LoadHelper.AddInOriginalPath;
            addInTempPath = LoadHelper.AddInTempPath;
            if (!TempFiles.Instance.AddinsTempFiles.Exists(i => i == originalPath))
            { //如果不存在该地址
                TempFiles.Instance.AddinsTempFiles.Add(originalPath);
            }
            if (Commands.Count == 0)
            {
                Commands.Add(CreateTree(commandTree, originalPath));
            }
            else if (Commands.Exists(i => i.Name == Path.GetFileName(addInTempPath)))
            {
                CommandTree command = Commands.First(i =>
                    i.Name == Path.GetFileName(addInTempPath)
                );
                CreateTree(commandTree, originalPath);
            }
            else
            {
                Commands.Add(CreateTree(commandTree, originalPath));
            }
            CommandsTrees.Refresh();
        }

        /// <summary>
        /// 给节点添加内容
        /// </summary>
        /// <param name="commandTree"></param>
        /// <param name="originalPath"></param>
        /// <returns></returns>
        private CommandTree CreateTree(CommandTree commandTree, string originalPath)
        {
            commandTree.Name = Path.GetFileName(addInTempPath);
            commandTree.OriPath = originalPath;
            commandTree.CommandMethodNames = LoadHelper.LoadAddinMethods(
                addInTempPath,
                commandTree.Name
            );
            return commandTree;
        }

        /// <summary>
        /// 从treeview移除命令方法
        /// </summary>
        [RelayCommand]
        void RemoveAddin()
        {
            try
            {
                if (CurrentAddinDll == null && CurrentCommand == null)
                {
                    return;
                }
                if (CurrentAddinDll != null && CurrentCommand == null)
                {
                    Commands.Remove(CurrentAddinDll);
                    TempFiles.Instance.AddinsTempFiles.Remove(CurrentAddinDll.OriPath);
                }
                else
                {
                    CurrentAddinDll.CommandMethodNames.Remove(CurrentCommand);
                    if (CurrentAddinDll.CommandMethodNames.Count == 0)
                    {
                        Commands.Remove(CurrentAddinDll);

                        TempFiles.Instance.AddinsTempFiles.Remove(CurrentAddinDll.OriPath);
                    }
                }
                CommandsTrees.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        /// <summary>
        /// 选中命令发生变化
        /// </summary>
        [RelayCommand]
        void SelChange(object selectedItem)
        {
            CurrentCommand = null;
            CurrentAddinDll = null;
            if (selectedItem != null && selectedItem as MethodTree != null)
            {
                CurrentCommand = selectedItem as MethodTree;
                foreach (var item in Commands)
                {
                    if (item.Name == CurrentCommand.DllName)
                    {
                        CurrentAddinDll = item;
                        break;
                    }
                }

                //MessageBox.Show(CurrentCommand);
            }
            else if (selectedItem != null && selectedItem as CommandTree != null)
            {
                CurrentAddinDll = selectedItem as CommandTree;
            }
            //删除操作会自动触发SelectedItemChanged事件
            if (CurrentAddinDll != null || CurrentCommand != null)
            {
                AddInOriginalPath = CurrentAddinDll.OriPath;
            }
            else
            {
                AddInOriginalPath = null;
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        [RelayCommand]
        void RunMethod()
        {
            Action cmd;
            if (CurrentCommand == null)
            {
                return;
            }
            try
            {
                var targetAssembly = CurrentCommand.assembly;
                var targetType = targetAssembly.GetType(CurrentCommand.ClassName);
                var targetMethod = targetType.GetMethod(CurrentCommand.Name);
                var targetObject = Activator.CreateInstance(targetType);
                cmd = () => targetMethod.Invoke(targetObject, null);

                cmd?.Invoke();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Tips");
            }
        }

        /// <summary>
        /// 在关闭前保存当前以加载的dll的地址
        /// </summary>
        [RelayCommand]
        void SavePath()
        {
            string folderPath = Path.GetTempPath();
            string filePath = Path.Combine(folderPath, "CADAddinManager");
            LoadHelper.ClearFolder(filePath);
            TempFiles.Instance.TempFilesSave();
        }

        /// <summary>
        /// 启动时加载上次加载的插件
        /// </summary>
        private void LoadPath()
        {
            try
            {
                var files = TempFiles.Instance.AddinsTempFiles;
                foreach (var file in files)
                {
                    LoadHelper.CopyToTempByOripath(file);
                    LoadAddinToTreeView();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("An error occurred: " + ex.Message);
                return;
            }
        }
    }
}
