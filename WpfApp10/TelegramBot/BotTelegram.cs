//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Exceptions;
//using Telegram.Bot.Extensions.Polling;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;

//namespace HomeWorkConsoleApp9
//{
//    public class BotTelegram
//    {aa
//        static TelegramBotClient bot;

//        public async Task InitAsync(string token)
//        {
//            bot = new TelegramBotClient(token);
//            var me = await bot.GetMeAsync();
//            Console.Title = me.Username;

//            var cts = new CancellationTokenSource();

//            bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

//            cts.Cancel();
//        }

//        private void Command() {
//            while (true) {
//                Console.ReadLine();
//            }
//        }

//        /// <summary>
//        /// Обработка ошибок
//        /// </summary>
//        /// <param name="botClient"></param>
//        /// <param name="exception"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
//        {
//            var ErrorMessage = exception switch
//            {
//                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
//                _ => exception.ToString()
//            };

//            Console.WriteLine(ErrorMessage);
//            return Task.CompletedTask;
//        }


//        /// <summary>
//        /// Метод обработки событий
//        /// </summary>
//        /// <param name="botClient"></param>
//        /// <param name="update"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//        {


//        }

//        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
//        {
//            Console.WriteLine($"Неизвестный тип обновления: {update.Type}");
//            return Task.CompletedTask;
//        }

//        /// <summary>
//        /// Метод обработки сообщений пользователя
//        /// </summary>
//        /// <param name="botClient"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
//        {
            
            
//        }


//    }
//}
