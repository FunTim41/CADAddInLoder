using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        string addInOriginalPath;

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
                LoadHelper.CopyToTemp();
                LoadAddinToTreeView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void LoadAddinToTreeView()
        {
            CommandTree commandTree = new CommandTree();
            AddInOriginalPath = LoadHelper.AddInOriginalPath;
            addInTempPath = LoadHelper.AddInTempPath;
            if (!TempFiles.Instance.AddinsTempFiles.Exists(i => i == AddInOriginalPath))
            { //如果不存在该地址
                TempFiles.Instance.AddinsTempFiles.Add(AddInOriginalPath);
            }
            if (Commands.Count == 0)
            {
                commandTree.Name = Path.GetFileName(addInTempPath);
                commandTree.OriPath = AddInOriginalPath;
                commandTree.CommandMethodNames = LoadHelper.LoadAddinMethods(
                    addInTempPath,
                    commandTree.Name
                );
                Commands.Add(commandTree);
            }
            else if (Commands.Exists(i => i.Name == Path.GetFileName(addInTempPath)))
            {
                CommandTree command = Commands.First(i =>
                    i.Name == Path.GetFileName(addInTempPath)
                );
                command.CommandMethodNames = LoadHelper.LoadAddinMethods(
                    addInTempPath,
                    commandTree.Name
                );
            }
            else
            {
                commandTree.Name = Path.GetFileName(addInTempPath);
                commandTree.OriPath = AddInOriginalPath;
                commandTree.CommandMethodNames = LoadHelper.LoadAddinMethods(
                    addInTempPath,
                    commandTree.Name
                );
                Commands.Add(commandTree);
            }
            CommandsTrees.Refresh();
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
            if (CurrentAddinDll!=null)
            {
                AddInOriginalPath = CurrentAddinDll.OriPath;
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
                var targetAssembly = Assembly.Load(File.ReadAllBytes(CurrentCommand.tempPath));
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
