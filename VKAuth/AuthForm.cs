using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VKAuth
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            Token.Clear();
            InitializeComponent();
        }
        private void AuthButton_Click(object sender, EventArgs e)
        {
            if (UsernameTextBox.TextLength == 0 || PasswordTextBox.TextLength == 0) return;
            try
            {
                Task t = Task.Run(() =>
                {
                    VKAuth.vk.Authorize(new ApiAuthParams()
                    {
                        ApplicationId = VKAuth.ApplicationID,
                        Login = UsernameTextBox.Text,
                        Password = PasswordTextBox.Text,
                        Settings = Settings.All,
                        TwoFactorAuthorization = () =>
                        {
                            var tfForm = new TwoFactorForm();
                            tfForm.Location = this.Location;
                            tfForm.ShowDialog();
                            return tfForm.Code;
                        }
                    });
                    Token.Set(VKAuth.vk.Token);
                });
                t.Wait();
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show((exception.Message.Length == 0 ? "Неизвестная ошибка" : exception.Message),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AuthForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Token.Get().Length != 0)
            {
                MessageBox.Show("Токен получен.\nДля получения последнего кода запустите программу",
                    "", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        private void AuthorButton_Click(object sender, EventArgs e)
        {
            Process.Start("http://vk.com/in_dgtl");
        }
    }
}
