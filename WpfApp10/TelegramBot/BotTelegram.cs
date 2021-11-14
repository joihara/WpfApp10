using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WpfApp10.TelegramBot
{
    public class BotTelegram
    {
        
        public TelegramBotClient bot;
        private MainWindow main;
        public ObservableCollection<MessageLog> BotMessageLog { get; set; }

        public async Task InitAsync(string token, MainWindow mainWindow)
        {
            main = mainWindow;
            BotMessageLog = new ObservableCollection<MessageLog>();
            main.logList.ItemsSource = BotMessageLog;
            bot = new TelegramBotClient(token);
            var me = await bot.GetMeAsync();

            var cts = new CancellationTokenSource();

            var handler = new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync);

            bot.StartReceiving(handler, cts.Token);

            //cts.Cancel();
        }

        /// <summary>
        /// Обработка ошибок
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            Console.WriteLine("Telegram API Error");
            return Task.CompletedTask;
        }


        /// <summary>
        /// Метод обработки событий
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Message.Text != null)
            {

                var messageText = update.Message.Text;

                main.Dispatcher.Invoke(() =>
                {
                    BotMessageLog.Add(
                    new MessageLog(
                        DateTime.Now.ToLongTimeString(),
                        messageText,
                        update.Message.Chat.FirstName,
                        update.Message.Chat.Id));
                });
            }

            Task handler = null;
            switch (update.Type)
            {
                case UpdateType.Message:
                    handler = BotOnMessageReceived(botClient, update.Message);
                    break;
                default:
                    handler = UnknownUpdateHandlerAsync(botClient, update);
                    break;
            }



            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }

        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Неизвестный тип обновления: {update.Type}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Метод обработки сообщений пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            bool selectId = false;
            switch (message.Type)
            {
                case MessageType.Photo:
                    selectId = true;
                    break;
                case MessageType.Voice:
                    selectId = true;
                    break;
                case MessageType.Document:
                    selectId = true;
                    break;
                default:
                    break;
            }

            if (message.Type == MessageType.Text)
            {
                Task<Message> action;
                switch (message.EntityValues.First())
                {
                    case "/start":
                        action = ActionBot.Usage(botClient, message);
                        break;
                    case "/SendFile":
                        action = ActionBot.SendFile(botClient, message);
                        break;
                    case "/Files":
                        action = ActionBot.GetListFile(botClient, message);
                        break;
                    case "/News":
                        action = ActionBot.GetNews(botClient, message);
                        break;
                    default:
                        action = ActionBot.Other(botClient, message);
                        break;
                }

                var sentMessage = await action;
                Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
            }
            else if (selectId)
            {
                await ActionBot.DownloadFile(botClient, message);
                string fileName = "";
                switch (message.Type)
                {
                    case MessageType.Photo:
                        foreach (var item in message.Photo)
                        {
                            fileName += item.FileId;
                        }
                        break;
                    case MessageType.Voice:

                        fileName = message.Voice.FileId;
                        break;
                    case MessageType.Document:
                        fileName = message.Document.FileName;
                        break;
                    default:
                        break;
                }
                main.Dispatcher.Invoke(() =>
                {
                    BotMessageLog.Add(
                new MessageLog(
                    DateTime.Now.ToLongTimeString(),
                    $"Получен файл {fileName}",
                    message.Chat.FirstName,
                    message.Chat.Id));
                });
            }

        }


    }
}
