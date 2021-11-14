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
            string text = $"{DateTime.Now.ToLongTimeString()}: " +
                $"{update.Message.Chat.FirstName} " +
                $"{update.Message.Chat.Id} " +
                $"{update.Message.Text}";

            if (update.Message.Text == null) return;

            var messageText = update.Message.Text;

            main.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(
                new MessageLog(
                    DateTime.Now.ToLongTimeString(), messageText, update.Message.Chat.FirstName, update.Message.Chat.Id));
            });

            Task handler = null;
            switch (update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    handler = BotOnMessageReceived(botClient, update.Message);
                    break;
                case UpdateType.InlineQuery:
                    break;
                case UpdateType.ChosenInlineResult:
                    break;
                case UpdateType.CallbackQuery:
                    break;
                case UpdateType.EditedMessage:
                    break;
                case UpdateType.ChannelPost:
                    break;
                case UpdateType.EditedChannelPost:
                    break;
                case UpdateType.ShippingQuery:
                    break;
                case UpdateType.PreCheckoutQuery:
                    break;
                case UpdateType.Poll:
                    break;
                case UpdateType.PollAnswer:
                    break;
                case UpdateType.MyChatMember:
                    break;
                case UpdateType.ChatMember:
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
        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            bool selectId = false;
            switch (message.Type)
            {
                case MessageType.Unknown:
                    break;
                case MessageType.Text:
                    break;
                case MessageType.Photo:
                    selectId = true;
                    break;
                case MessageType.Audio:
                    break;
                case MessageType.Video:
                    break;
                case MessageType.Voice:
                    selectId = true;
                    break;
                case MessageType.Document:
                    selectId = true;
                    break;
                case MessageType.Sticker:
                    break;
                case MessageType.Location:
                    break;
                case MessageType.Contact:
                    break;
                case MessageType.Venue:
                    break;
                case MessageType.Game:
                    break;
                case MessageType.VideoNote:
                    break;
                case MessageType.Invoice:
                    break;
                case MessageType.SuccessfulPayment:
                    break;
                case MessageType.WebsiteConnected:
                    break;
                case MessageType.ChatMembersAdded:
                    break;
                case MessageType.ChatMemberLeft:
                    break;
                case MessageType.ChatTitleChanged:
                    break;
                case MessageType.ChatPhotoChanged:
                    break;
                case MessageType.MessagePinned:
                    break;
                case MessageType.ChatPhotoDeleted:
                    break;
                case MessageType.GroupCreated:
                    break;
                case MessageType.SupergroupCreated:
                    break;
                case MessageType.ChannelCreated:
                    break;
                case MessageType.MigratedToSupergroup:
                    break;
                case MessageType.MigratedFromGroup:
                    break;
                case MessageType.Poll:
                    break;
                case MessageType.Dice:
                    break;
                case MessageType.MessageAutoDeleteTimerChanged:
                    break;
                case MessageType.ProximityAlertTriggered:
                    break;
                case MessageType.VoiceChatScheduled:
                    break;
                case MessageType.VoiceChatStarted:
                    break;
                case MessageType.VoiceChatEnded:
                    break;
                case MessageType.VoiceChatParticipantsInvited:
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
                    //case "/SendFile":
                    //    action = ActionBot.SendFile(botClient, message);
                    //    break;
                    //case "/Files":
                    //    action = ActionBot.Usage(botClient, message);
                    //    break;
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
            }

        }


    }
}
