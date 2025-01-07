using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCad
{
    public class Command
    {
        [CommandMethod("HelloCAD")]
        public void NewCommand()
        {
            MessageBox.Show("Hello,CAD!!!");
        }
    }
}
