using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Autodesk.Civil.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace CFDG.UI.windows.Calculations
{
    /// <summary>
    /// Interaction logic for ExportPointGroup.xaml
    /// </summary>
    public partial class ExportPointGroup : Window
    {
        private PointGroupCollection pointGroups;

        public ExportPointGroup()
        {
            InitializeComponent();
        }

        public ExportPointGroup(PointGroupCollection pointGroups)
        {
            InitializeComponent();
            this.pointGroups = pointGroups;
            foreach (ObjectId pointGroupId in pointGroups)
            {
                PointGroup group = (PointGroup)pointGroupId.GetObject(OpenMode.ForRead);
                if (true)
                {
                    LbPointGroups.Items.Add(group.Name);
                }
            }
            this.DialogResult = false;
        }
    }
}
