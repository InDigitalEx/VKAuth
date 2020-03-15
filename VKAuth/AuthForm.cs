using System;
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
                // Авторизуем пользователя
                VKAuth.vk.Authorize(new ApiAuthParams()
                {
                    ApplicationId = VKAuth.ApplicationID,
                    Login = UsernameTextBox.Text,
                    Password = PasswordTextBox.Text,
                    Settings = Settings.All,
                    TwoFactorAuthorization = () =>
                    {
                        //TODO: Переделать в дочернюю форму
                        var tfForm = new TwoFactorForm();
                        tfForm.Location = this.Location;
                        tfForm.ShowDialog();
                        return tfForm.Code;
                    }
                });

                // Сохраняем токен в настройки
                Token.Set(VKAuth.vk.Token);

                // Закрываем форму и выводим сообщение
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
            this.Visible = false;
            if (Token.Get().Length != 0)
            {
                MessageBox.Show("Токен получен.\nДля получения последнего кода запустите программу",
                    "", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("vk.com/in_dgtl", "Автор",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
