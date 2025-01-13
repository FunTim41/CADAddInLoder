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
            
        }

       
    }
}
