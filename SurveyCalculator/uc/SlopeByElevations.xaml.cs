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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SurveyCalculator.uc
{
    /// <summary>
    /// Interaction logic for SlopeDistanceByPoints.xaml
    /// </summary>
    public partial class SlopeByElevation : UserControl
    {
        TextBox ActiveBox { get; set; }
        public SlopeByElevation()
        {
            InitializeComponent();
            ActiveBox = StartElevation;
        }

        private void Txtbox_Focus_Gain(object s, RoutedEventArgs e)
        {
            TextBox box = (TextBox)s;
            box.BorderBrush = System.Windows.Media.Brushes.Green;
            box.BorderThickness = new Thickness(2);
            if (((TextBox)s).BorderBrush != Brushes.Red)
            {
                ((TextBox)s).BorderBrush = Brushes.Black;
                ((TextBox)s).BorderThickness = new Thickness(1);
            }
            ActiveBox = box;
        }

        private bool VerifyInput()
        {
            if (Double.TryParse(ActiveBox.Text, out double _) || string.IsNullOrEmpty(ActiveBox.Text))
                return true;
            else
                return false;
        }

        private void Value_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock txt = (TextBlock)sender;
            Clipboard.SetText(txt.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!VerifyInput())
            {
                ((TextBox)sender).BorderBrush = System.Windows.Media.Brushes.Red;
                ((TextBox)sender).BorderThickness = new Thickness(2);
            }
            else
            {
                ((TextBox)sender).BorderBrush = System.Windows.Media.Brushes.Green;
                ((TextBox)sender).BorderThickness = new Thickness(2);
            }

        }
    }
}
