using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace VKAuth
{
    public partial class TwoFactorForm : Form
    {
        public TwoFactorForm()
        {
            InitializeComponent();
        }
        public string Code { get; set; }
        private bool Success = false;

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (InputTextBox.TextLength == 0)
                return;

            Code = InputTextBox.Text;
            Success = true;
            Close();
        }

        private void TwoFactorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(!Success) Process.GetCurrentProcess().Kill();
        }
    }
}
