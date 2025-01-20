using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;

namespace HelloCad
{
    public class Command
    {
        [CommandMethod("HelloCAD")]
        public void NewCommand()
        {
            try
            {
                MessageBox.Show("Hello,World！！！");
                throw new System.Exception("get exception");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
