using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        [ObservableProperty]
        ObservableCollection<CommandTree> commands = new();

        [RelayCommand]
        void LoadAddin()
        {
            try
            {
                CommandTree commandTree = new CommandTree();
                LoadHelper.CopyToTemp();
                AddInOriginalPath = LoadHelper.AddInOriginalPath;
                commandTree.Name = Path.GetFileName(AddInOriginalPath);
                var tempPath = LoadHelper.AddInTempPath;
                TempFiles.Instance.AddinsTempFiles.Add(tempPath);
                commandTree.CommandMethodNames= LoadHelper.LoadAddinMethods(tempPath);
                Commands.Add(commandTree);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
