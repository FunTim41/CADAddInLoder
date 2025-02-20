using CADAddinManagerDemo.ViewModels;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CADAddinManagerDemo
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
       
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            // 获取屏幕的工作区（不包括任务栏）
            //System.Windows.SystemParameters.PrimaryScreenWidth; // 主屏幕宽度
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double windowWidth = this.Width;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowHeight = this.Height;
            // 设置窗口的左边界，使其位于屏幕右侧
            this.Left = screenWidth - windowWidth*1.5;

            // 可选：设置窗口顶部位置（例如，屏幕顶部）
            this.Top = (screenHeight - windowHeight )/2 ;
        }

       
    }
}
