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
        public Models.ExportPointGroupModel PointGroupModel { get; set; }

        public ExportPointGroup()
        {
            InitializeComponent();
        }

        public ExportPointGroup(List<string> pointGroups, string path)
        {
            InitializeComponent();
            PointGroupModel = new Models.ExportPointGroupModel();
            LbPointGroups.Items.Add("!All Points");
            LbPointGroups.Items.Add("!Comp Points");
            foreach (string group in pointGroups)
            {
                LbPointGroups.Items.Add(group);
            }
            LbPointGroups.Items.IsLiveSorting = true;
            string jobNumber = API.JobNumber.Parse(path, API.JobNumberFormats.ShortHyphan);
            this.Title = $"Export Point Groups - {jobNumber}";

            PointGroupModel.FileName = API.JobNumber.GetPath(jobNumber);
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
