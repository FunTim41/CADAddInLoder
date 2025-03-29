using System.Collections.Specialized;
using System.IO;
using System.Windows.Shapes;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace CADAddinManagerDemo
{
    public class Command
    {       
        public static MainView mainView;

        [CommandMethod("AddinManager")]
        public void ShowAddInManager()
        {
            try
            {
                if (mainView != null)
                {
                    return;
                }
                mainView = new MainView();
                mainView.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "加载窗口失败");
            }
        }

        
    }
}
