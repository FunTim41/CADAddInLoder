using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
namespace CADAddinManagerDemo
{
    public class ExApp : IExtensionApplication
    {
       
        public void Initialize()
        {
            
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Application.Idle += Application_Idle; }

        private void Application_Idle(object sender, EventArgs e)
        {
           var doc=Application.DocumentManager.MdiActiveDocument; ;
            if (null!=doc)
            {
                Application.Idle -= Application_Idle;
                doc.Editor.WriteMessage("AddinManager Loaded");
            }
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
                var assembly=new AssemblyName(args.Name);
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
}
