using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CFDG.UI
{
    public partial class InputDialog : Form
    {
        public string Value { get; set; }

        public InputDialog(string message)
        {
            InitForm(message, "Input Dialog");
        }
        public InputDialog(string message, string title)
        {
            InitForm(message, title);
        }

        private void InitForm(string message, string title)
        {
            InitializeComponent();
            txtValue.Text = message;
            this.Text = title;
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Value = txtValue.Text;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
