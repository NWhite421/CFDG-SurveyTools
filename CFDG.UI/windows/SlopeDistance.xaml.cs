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

namespace CFDG.UI
{
    /// <summary>
    /// Interaction logic for SlopeDistance.xaml
    /// </summary>
    public partial class SlopeDistance : Window
    {
        public SlopeDistance()
        {
            InitializeComponent();
        }

        public void Calculate(CogoPoint startPoint, CogoPoint endPoint)
        {
            PntANumber.Text = $"#{startPoint.PointNumber}";
            PntAEasting.Text = string.Format("{0:0.00}", startPoint.Easting);
            PntANorthing.Text = string.Format("{0:0.00}", startPoint.Northing);
            PntAElevation.Text = string.Format("{0:0.00}", startPoint.Elevation);

            PntBNumber.Text = $"#{endPoint.PointNumber}";
            PntBEasting.Text = string.Format("{0:0.00}", endPoint.Easting);
            PntBNorthing.Text = string.Format("{0:0.00}", endPoint.Northing);
            PntBElevation.Text = string.Format("{0:0.00}", endPoint.Elevation);

            var deltaX = Math.Abs(endPoint.Easting - startPoint.Easting);
            var deltaY = Math.Abs(endPoint.Northing - startPoint.Northing);
            var deltaZ = startPoint.Elevation - endPoint.Elevation;

            var distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            var slope = deltaZ / distance;

            Distance.Text = string.Format("±{0:0.0}LF", Math.Round(distance, 1));
            Slope.Text = string.Format("{0:0.00}%", Math.Round(slope * 100, 2));
            SlopeAct.Text = string.Format("{0:0.00000}", Math.Round(slope, 5));
        }
    }
}
