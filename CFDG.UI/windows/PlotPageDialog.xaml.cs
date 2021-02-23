﻿using Autodesk.AutoCAD.DatabaseServices;
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

namespace CFDG.UI
{
    /// <summary>
    /// Interaction logic for PlotPageDialog.xaml
    /// </summary>
    public partial class PlotPageDialog : Window
    {


        public PlotPageDialog()
        {
            InitializeComponent();
        }

        public PlotPageDialog(IEnumerable<Layout> layouts)
        {
            InitializeComponent();
            var list = layouts.OrderBy(l => l.TabOrder);
            foreach (Layout lo in list)
            {
                if (lo.LayoutName.ToLower() != "model")
                    ViewsAvalible.Items.Add(lo.LayoutName);
            }

            PlotDate.Text = DateTime.Now.ToString("MM/dd/yyyy");

        }

        private void Cancel_form(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Accept_form(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
