using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;

namespace HelloCad
{
    public class Command
    {
        [CommandMethod("HelloCAD")]
        public void NewCommand()
        {
            MessageBox.Show("Hello,Revit！！！");
            MessageBox.Show("Hello,CAD！！！");
        }
    }
}
