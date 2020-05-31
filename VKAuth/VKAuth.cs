using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model.RequestParams;

namespace VKAuth
{
    class VKAuth
    {
        public const ulong ApplicationID = 7359471;
        public const int UserID = 100;
        public static VkApi vk;

        [STAThread]
        static void Main(string[] args)
        {
            // Если запущен с аргументом на удаление токена
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
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

            try
            {
                // Авторизация для получения токена
                if (Token.Get().Length == 0)
                {
                    throw new Exception("Token not found");
                }

                Task t = Task.Run(() =>
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
                    if (startIndex != -1)
                    {
                        vk.Messages.MarkAsRead(UserID.ToString());
                        string code = message.Substring(startIndex, 6);

                        // Add to clipboard
                        Thread thread = new Thread(() => Clipboard.SetText(code));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();

                        MessageBox.Show("Код скопирован в буфер обмена", "",
                            MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                    else
                    {
                        MessageBox.Show("Код подтверждения не найден", "",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                t.Wait();
            }
            catch (Exception)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new AuthForm());
            }
        }
    }
}
