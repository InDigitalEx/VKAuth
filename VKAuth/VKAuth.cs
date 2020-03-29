using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Forms;
using VkNet;
using VkNet.Model.RequestParams;
using VkNet.AudioBypassService.Extensions;

namespace VKAuth
{
    class VKAuth
    {
        public const ulong ApplicationID = 7359471;
        public const int UserID = 100;
        public static VkApi vk;
        public static void CreateAuthForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AuthForm());
        }
        [STAThread]
        static void Main(string[] args)
        {
            // Если запущен с аргументом на удаление токена
            if(args.Length > 0)
            {
                for(int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-resettoken")
                    {
                        Token.Clear();
                        MessageBox.Show("Токен сброшен", "Сброс токена",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            // Инициализация VkNet
            var services = new ServiceCollection();
            services.AddAudioBypass();

            vk = new VkApi(services);
            // Авторизация для получения токена
            if (Token.Get().Length == 0)
            {
                CreateAuthForm();
                return;
            }
            try
            {
                // Авторизация по токену
                vk.Authorize(new VkNet.Model.ApiAuthParams()
                {
                    ApplicationId = ApplicationID,
                    AccessToken = Token.Get()
                });

                // Пользователь уже авторизован, теперь ищем сообщение от вк и копируем код
                var history = vk.Messages.GetHistory(new MessagesGetHistoryParams()
                {
                    UserId = UserID,
                    Count = 1
                });
                // Копируем текст сообщения
                string message = history.Messages.First().Text;
                int startIndex = message.LastIndexOf(": ") + 2; // Строка имеет вид ": 000000"
                if(startIndex != -1)
                {
                    vk.Messages.MarkAsRead(UserID.ToString());
                    string code = message.Substring(startIndex, 6);
                    Clipboard.SetText(code);
                    MessageBox.Show("Код скопирован в буфер обмена", "",
                        MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    MessageBox.Show("Код подтверждения не найден", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            catch (Exception)
            {
                CreateAuthForm();
                return;
            }
        }
    }
}
