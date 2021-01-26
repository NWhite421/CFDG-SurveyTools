
namespace CFDG.UI
{
    partial class InputDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtValue = new System.Windows.Forms.TextBox();
            this.cmdSubmit = new System.Windows.Forms.Button();
            this.CmdCancel = new System.Windows.Forms.Button();
            this.lblText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(12, 79);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(435, 26);
            this.txtValue.TabIndex = 1;
            // 
            // cmdSubmit
            // 
            this.cmdSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdSubmit.Location = new System.Drawing.Point(367, 111);
            this.cmdSubmit.Name = "cmdSubmit";
            this.cmdSubmit.Size = new System.Drawing.Size(80, 28);
            this.cmdSubmit.TabIndex = 2;
            this.cmdSubmit.Text = "Submit";
            this.cmdSubmit.UseVisualStyleBackColor = true;
            // 
            // CmdCancel
            // 
            this.CmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CmdCancel.Location = new System.Drawing.Point(12, 111);
            this.CmdCancel.Name = "CmdCancel";
            this.CmdCancel.Size = new System.Drawing.Size(80, 28);
            this.CmdCancel.TabIndex = 3;
            this.CmdCancel.Text = "Cancel";
            this.CmdCancel.UseVisualStyleBackColor = true;
            // 
            // lblText
            // 
            this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblText.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblText.Location = new System.Drawing.Point(12, 12);
            this.lblText.Multiline = true;
            this.lblText.Name = "lblText";
            this.lblText.ReadOnly = true;
            this.lblText.Size = new System.Drawing.Size(435, 61);
            this.lblText.TabIndex = 4;
            this.lblText.Text = "Sample text";
            // 
            // InputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(459, 151);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.CmdCancel);
            this.Controls.Add(this.cmdSubmit);
            this.Controls.Add(this.txtValue);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(475, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(475, 175);
            this.Name = "InputDialog";
            this.Text = "Input Request";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button cmdSubmit;
        private System.Windows.Forms.Button CmdCancel;
        private System.Windows.Forms.TextBox lblText;
    }
}

