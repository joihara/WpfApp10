using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace WpfApp10.TelegramBot
{
    public class ActionBot
    {
        public static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            const string usage = "Используйте:\n" +
                "/start                     -   Получить подсказку о использовании бота\n" +
                "/SendFile (Название файла) -   Получить файл с выбранным название если он существует у бота\n" +
                "/Files                     -   Получить список всех файлов у бота\n" +
                "/News                      -   Получение новостей";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: usage,
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        public static async Task<Message> Other(ITelegramBotClient botClient, Message message)
        {
            const string usage = "Введите комманду:\n/start";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: usage,
                                                        replyMarkup: new ReplyKeyboardRemove());
        }
        /// <summary>
        /// Получение файлов от пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task DownloadFile(ITelegramBotClient botClient, Message message)
        {
            Directory.CreateDirectory("Files");

            switch (message.Type)
            {
                case MessageType.Photo:
                    foreach (var item in message.Photo)
                    {
                        GetMessageDownload(botClient, item.FileId, item.FileId);
                    }
                    await SendTextUser(botClient, message, "Фото загружены");
                    break;
                case MessageType.Voice:

                    GetMessageVoiceDownload(botClient, message.Voice.FileId);
                    await SendTextUser(botClient, message, "Голосовое сообщение загружено");
                    break;
                case MessageType.Document:

                    GetMessageDownload(botClient, message.Document.FileId, message.Document.FileName);
                    await SendTextUser(botClient, message, "Документ загружен");
                    break;
                default:
                    await SendTextUser(botClient, message, "Нечего не загружено");
                    break;
            }
        }

        public static async Task<Message> SendTextUser(ITelegramBotClient botClient, Message message, string text)
        {
            return await botClient.SendTextMessageAsync(message.Chat.Id, text);
        }

        public static async void GetMessageVoiceDownload(ITelegramBotClient botClient, string selectID)
        {
            var files = Directory.GetFiles(@"Files");
            int countVoice = files.Count(e => e.Contains("Voice"));
            string nameFile = $"Voice{countVoice}.ogg";

            var file = await botClient.GetFileAsync(selectID);
            FileStream fs = new FileStream($@"Files/{nameFile}", FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        public static async void GetMessageDownload(ITelegramBotClient botClient, string selectID, string nameFile)
        {
            var file = await botClient.GetFileAsync(selectID);
            FileStream fs = new FileStream($@"Files/{nameFile}", FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        public static async Task<Message> GetNews(ITelegramBotClient botClient, Message message)
        {
            string urlNews = "http://rss.dw.de/xml/rss-ru-news";
            var news = Utils.ParseRSS(urlNews);
            string outMessage = "";
            foreach (var item in news)
            {
                outMessage += $"{item.Summary}\n\n";
            }
            return await botClient.SendTextMessageAsync(message.Chat.Id, outMessage);
        }
    }
}
