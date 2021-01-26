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
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace CFDG.UI
{
    /// <summary>
    /// Interaction logic for SlopeDistance.xaml
    /// </summary>
    public partial class SlopeDistance : Window
    {
        public SlopeDistance(Point3d startPoint, Point3d endPoint)
        {
            InitializeComponent();
            Calculate(startPoint, endPoint);
        }

        public void Calculate(Point3d startPoint, Point3d endPoint)
        {
            PntAEasting.Text = string.Format("{0:0.00}", startPoint.X);
            PntANorthing.Text = string.Format("{0:0.00}", startPoint.Y);
            PntAElevation.Text = string.Format("{0:0.00}", startPoint.Z);

            PntBEasting.Text = string.Format("{0:0.00}", endPoint.X);
            PntBNorthing.Text = string.Format("{0:0.00}", endPoint.Y);
            PntBElevation.Text = string.Format("{0:0.00}", endPoint.Z);
            //CalculateValues();
        }

        private void OnTextboxChange(object sender, TextChangedEventArgs e)
        {
            if (Double.TryParse(((TextBox)sender).Text, out _))
            {
                CalculateValues();
                Distance.Foreground = Brushes.White;
                Slope.Foreground = Brushes.White;
                SlopeAct.Foreground = Brushes.White;
            }
            else
            {
                Distance.Foreground = Brushes.Red;
                Slope.Foreground = Brushes.Red;
                SlopeAct.Foreground = Brushes.Red;
            }
        }

        private void CalculateValues()
        {
            var deltaX = Math.Abs(Convert.ToDouble(PntBEasting.Text) - Convert.ToDouble(PntAEasting.Text));
            var deltaY = Math.Abs(Convert.ToDouble(PntBNorthing.Text) - Convert.ToDouble(PntANorthing.Text));
            var deltaZ = Convert.ToDouble(PntAElevation.Text) - Convert.ToDouble(PntBElevation.Text);

            var distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            var slope = deltaZ / distance;

            Distance.Text = string.Format("±{0:0.0}LF", Math.Round(distance, 1));
            Slope.Text = string.Format("{0:0.00}%", Math.Round(slope * 100, 2));
            SlopeAct.Text = string.Format("{0:0.00000}", Math.Round(slope, 5));
        }
    }
}
