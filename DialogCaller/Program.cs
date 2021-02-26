using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            CFDG.UI.windows.Calculations.ExportPointGroup window = new CFDG.UI.windows.Calculations.ExportPointGroup();
            window.Show();
        }
    }
}
