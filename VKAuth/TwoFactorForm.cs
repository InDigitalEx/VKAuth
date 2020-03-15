using System;
using System.Windows.Forms;

namespace VKAuth
{
    public partial class TwoFactorForm : Form
    {
        public TwoFactorForm()
        {
            InitializeComponent();
        }
        public string Code { get; set; }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (InputTextBox.TextLength == 0) return;
            Code = InputTextBox.Text;
            this.Close();
        }
    }
}
