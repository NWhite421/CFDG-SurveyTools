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
    public partial class SlopeDistanceByPoints : UserControl
    {
        TextBox ActiveBox { get; set; }
        public SlopeDistanceByPoints()
        {
            InitializeComponent();
            ActiveBox = PointA_Northing;
            PointA_Northing.BorderBrush = System.Windows.Media.Brushes.Green;
            PointA_Northing.BorderThickness = new Thickness(2, 2, 2, 2);
        }

        private void Txtbox_Focus_Gain(object s, RoutedEventArgs e)
        {
            TextBox box = (TextBox)s;
            box.BorderBrush = System.Windows.Media.Brushes.Green;
            box.BorderThickness = new Thickness(2);
            if (ActiveBox.BorderBrush != Brushes.Red)
            {
                ActiveBox.BorderBrush = Brushes.Black;
                ActiveBox.BorderThickness = new Thickness(1);
            }
            ActiveBox = box;
        }

        private void Button_Value_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ActiveBox.Text += btn.Content;
        }

        private void Button_Function_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string btnText = btn.Name.ToLower();
            switch (btnText)
            {
                case "btn_fnc_back":
                    {
                        switch (ActiveBox.Name)
                        {
                            case "PointA_Northing":
                                {
                                    PointB_Elevation.Focus();
                                    break;
                                }
                            case "PointA_Easting":
                                {
                                    PointA_Northing.Focus();
                                    break;
                                }
                            case "PointA_Elevation":
                                {
                                    PointA_Easting.Focus();
                                    break;
                                }
                            case "PointB_Northing":
                                {
                                    PointA_Elevation.Focus();
                                    break;
                                }
                            case "PointB_Easting":
                                {
                                    PointB_Northing.Focus();
                                    break;
                                }
                            case "PointB_Elevation":
                                {
                                    PointB_Easting.Focus();
                                    break;
                                }
                        }
                        break;
                    }
                case "btn_fnc_next":
                    {
                        switch (ActiveBox.Name)
                        {
                            case "PointA_Northing":
                                {
                                    PointA_Easting.Focus();
                                    break;
                                }
                            case "PointA_Easting":
                                {
                                    PointA_Elevation.Focus();
                                    break;
                                }
                            case "PointA_Elevation":
                                {
                                    PointB_Northing.Focus();
                                    break;
                                }
                            case "PointB_Northing":
                                {
                                    PointB_Easting.Focus();
                                    break;
                                }
                            case "PointB_Easting":
                                {
                                    PointB_Elevation.Focus();
                                    break;
                                }
                            case "PointB_Elevation":
                                {
                                    PointA_Northing.Focus();
                                    break;
                                }
                        }
                        break;
                    }
                case "btn_fnc_backspace":
                    {
                        ActiveBox.Text = ActiveBox.Text.Substring(0, ActiveBox.Text.Length - 1);
                        break;
                    }
                case "btn_fnc_clear":
                    {
                        PointA_Northing.Text = "";
                        PointA_Easting.Text = "";
                        PointA_Elevation.Text = "";
                        PointB_Northing.Text = "";
                        PointB_Easting.Text = "";
                        PointA_Elevation.Text = "";
                        PointA_Northing.Focus();
                        break;
                    }
                case "btn_fnc_invert":
                    {
                        if (ActiveBox.Text.Length < 1)
                            break;
                        if (ActiveBox.Text[0] == '-')
                            ActiveBox.Text = ActiveBox.Text.Substring(1, ActiveBox.Text.Length - 1);
                        else
                            ActiveBox.Text = "-" + ActiveBox.Text;
                        break;
                    }
            }
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
