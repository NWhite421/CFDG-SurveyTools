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
using System.IO;

namespace SurveyCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal TextBox ActiveText { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ActiveText = SC_StartElev;
            ActiveText.Focus();
        }

        private void LogUpdated(string text)
        {
            H_Log.Text += "• " + text + Environment.NewLine;
        }

        private void Btn_SC_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ActiveText.Text += btn.Content;
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (ActiveText != box)
                ActiveText = box;
        }

        private void Btn_SC_Shift(object sender, RoutedEventArgs e)
        { 
            Button btn = (Button)sender;
            TextBox[] boxes = new TextBox[] { SC_StartElev, SC_EndEl, SC_Distance };
            string name = btn.Name.ToLower();
            if (name.Contains("forward"))
            {
                if (ActiveText == SC_Distance)
                    ActiveText = SC_StartElev;
                else if (ActiveText == SC_EndEl)
                    ActiveText = SC_Distance;
                else
                    ActiveText = SC_EndEl;
            }
            else
            {
                if (ActiveText == SC_Distance)
                    ActiveText = SC_EndEl;
                else if (ActiveText == SC_EndEl)
                    ActiveText = SC_StartElev;
                else
                    ActiveText = SC_Distance;
            }
            ActiveText.Focus();
        }

        private void Btn_SC_DeleteClick(object sender, RoutedEventArgs e)
        {
            ActiveText.Text = ActiveText.Text.Remove(ActiveText.Text.Length - 1);
        }

        private void Btn_SC_Clear_Click(object sender, RoutedEventArgs e)
        {
            SC_StartElev.Text = "";
            SC_EndEl.Text = "";
            SC_Distance.Text = "";
        }

        private void Btn_SC_Calc_Click(object sender, RoutedEventArgs e)
        {
            OutLog.Text = "";
            if (!Double.TryParse(SC_StartElev.Text, out double StartingElevation))
                OutLog.Text += "Please set a starting elevation." + Environment.NewLine;
            if (!Double.TryParse(SC_EndEl.Text, out double EndingElevation))
                OutLog.Text += "Please set a ending elevation." + Environment.NewLine;
            if (!Double.TryParse(SC_Distance.Text, out double SC_Distance2D))
                OutLog.Text += "Please set a SC_Distance." + Environment.NewLine;
            if (!string.IsNullOrEmpty(OutLog.Text))
                return;
            double diff = Math.Abs(StartingElevation - EndingElevation);
            var value = diff / SC_Distance2D;
            SC_Output.Text = Math.Round((value * 100), 2).ToString() + "%";
            OutLog.Text = $"| {StartingElevation} - {EndingElevation} | / {SC_Distance2D} = {Math.Round(value, 5)} == {SC_Output.Text}";
            LogUpdated(OutLog.Text);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem item = (TabItem)TabController.SelectedItem;
                if (item.Header.ToString() == "SLOPE CALC")
                    ActiveText = SC_StartElev;
            }
        }
    }
}
