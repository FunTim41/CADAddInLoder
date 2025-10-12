using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using HandyControl.Controls;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

using RibbonButton = Autodesk.Windows.RibbonButton;
using RibbonPanelSource = Autodesk.Windows.RibbonPanelSource;
using RibbonTab = Autodesk.Windows.RibbonTab;
namespace CADAddinManagerDemo
{
  public class ExApp : IExtensionApplication
  {

    public void Initialize()
    {

      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      Application.Idle += Application_Idle;
    }

    private void Application_Idle(object sender, EventArgs e)
    {
      var doc = Application.DocumentManager.MdiActiveDocument; ;
      if (null != doc)
      {
        Application.Idle -= Application_Idle;
        doc.Editor.WriteMessage("\nAddinManager Loaded\n");
        CreateRibbon();
        Application.SystemVariableChanged += Application_SystemVariableChanged;
      }
    }

    private void Application_SystemVariableChanged(object sender, SystemVariableChangedEventArgs e)
    {
      if (e.Name.ToLower() == "wscurrent")
      {
        CreateRibbon();
      }
    }

    private static void CreateRibbon()
    {
      RibbonTab tab = null;
      foreach (RibbonTab tab0 in ComponentManager.Ribbon.Tabs)
      {
        if (tab0.AutomationName == "附加模块")
        {
          tab = tab0;
          break;
        }
      }

      RibbonPanelSource rps = new RibbonPanelSource();
      rps.Title = "插件管理";
      RibbonPanel rp = new RibbonPanel();
      rp.Source = rps;
      tab.Panels.Add(rp);
      RibbonButton rb = NewRibbonBtn("CADAddinManager", "AddinManager ");


      rb.ShowImage = true;
      rb.LargeImage = new System.Windows.Media.Imaging.BitmapImage(
          new Uri("pack://application:,,,/CADAddinManagerDemo;component/ico/demo2.png")
      );
      rb.Size = RibbonItemSize.Large;
      rb.Orientation = System.Windows.Controls.Orientation.Vertical;
      for (int i = 0; i < rps.Items.Count; i++)
      {
        if (rps.Items[i].Text == rb.Text)
        {
          rps.Items[i] = rb;
          return;
        }
      }

      rps.Items.Add(rb);
    }

    public static RibbonButton NewRibbonBtn(string text, string cmd)
    {
      RibbonButton button = new RibbonButton();
      button.Text = text;
      button.ShowText = true;
      button.CommandHandler = new AdskCommonHandler();
      button.CommandParameter = cmd;

      return button;
    }

    private Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      if (sender is not AppDomain appDomain)
      {
        return null;
      }
      appDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
      try
      {
        var assembly = new AssemblyName(args.Name);
        return Assembly.Load(assembly);
      }
      catch (System.Exception)
      {

        return null;
      }
      finally
      {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      }
    }

    public void Terminate()
    {

    }
  }

  public class AdskCommonHandler : ICommand
  {
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      RibbonButton button = parameter as RibbonButton;
      if (button != null)
      {
        Application.DocumentManager.MdiActiveDocument.SendStringToExecute(
            (string)button.CommandParameter,
            true,
            false,
            true
        );
      }
    }
  }
}
