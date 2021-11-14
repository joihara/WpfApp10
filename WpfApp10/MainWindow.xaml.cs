using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Telegram.Bot.Types;
using WpfApp10.TelegramBot;

namespace WpfApp10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StructBot[] Bots { set; get; }
        private string SelectedToken { set; get; }

        private readonly BotTelegram botTelegram = new BotTelegram();
        private readonly Utils utils = new Utils();

        public MainWindow()
        {
            InitializeComponent();


            Bots = utils.ReadBot();
            UpdateListBots();
        }

        #region SelectBot

        /// <summary>
        /// Обновление списка с ботами
        /// </summary>
        private void UpdateListBots()
        {
            Bots = utils.ReadBot();
            List<string> BotSelector = new List<string>();
            foreach (var item in Bots)
            {
                BotSelector.Add(item.BotName);
            }

            ListSelectBot.ItemsSource = BotSelector;
        }

        /// <summary>
        /// Обработчик выбора и списка ботов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSelectBot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectOnListBox = ((ListBox)sender).SelectedItem;
            var structBotSelect = Bots.First(s => s.BotName.Equals(selectOnListBox));
            SelectedToken = structBotSelect.BotName;
            RunBot.IsEnabled = true;
        }

        /// <summary>
        /// Обработка нажатия кнопки добавления бота в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBot_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBot.Text;
            var token = TokenBot.Text;

            var checkName = Bots.Any(n => n.BotName.Equals(name));
            var checkToken = Bots.Any(t => t.Token.Equals(token));
            if (checkName)
            {
                MessageBox.Show($"Бот с таким именем уже существует:\n " +
                    $"Имя: {name}\n " +
                    $"Токен: {token}");
            }
            else if (checkToken)
            {
                MessageBox.Show($"Бот с таким токеном уже существует:" +
                    $"Имя: {name}\n " +
                    $"Токен: {token}");
            }
            else {
                CreateBot.IsEnabled = false;
                RunBot.IsEnabled = false;
                utils.AddBot(name, token);
                MessageBox.Show("Бот добавлен");
                UpdateListBots();
            }
        }

        /// <summary>
        /// Обработчик ввода название для бота
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameBot_TextChanged(object sender, TextChangedEventArgs e)
        {
            var name = ((TextBox)sender).Text;
            var checkName = Bots.Any(n => n.BotName.Equals(name));
            CreateBot.IsEnabled = !checkName && TokenBot.Text != "" && name != "";
        }

        /// <summary>
        /// Обработчик ввода токена для бота
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TokenBot_TextChanged(object sender, TextChangedEventArgs e)
        {
            var token = ((TextBox)sender).Text;
            var checkToken = Bots.Any(t => t.Token.Equals(token));
            CreateBot.IsEnabled = !checkToken && NameBot.Text != "" && token != "";
        }

        /// <summary>
        /// Обработчик нажатия кнопки запуска выбранного бота
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RunBot_Click(object sender, RoutedEventArgs e)
        {
            var token = Bots.First(b => b.BotName == SelectedToken).Token;

            await botTelegram.InitAsync(token, this);
            BotEnter.Visibility = Visibility.Collapsed;
            BotSelect.Visibility = Visibility.Visible;
        }
        #endregion
        #region WorkBot
        private async void btnMsgSendClick(object sender, RoutedEventArgs e)
        {
            var text = txtMsgSend.Text;
            var id = TargetSend.Text;
            if (id != "")
            {
                await botTelegram.bot.SendTextMessageAsync(id, text);
                botTelegram.BotMessageLog.Add(new MessageLog(
                    DateTime.Now.ToLongTimeString(),
                    text,
                    "Console",
                    long.Parse(id)));
        }
}

        #endregion

        private void txtMsgSend_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnMsgSend.IsEnabled = true;
        }

        /// <summary>
        /// Кнопка сохранения истории сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveHistory_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "JavaScript Object Notation(*.json)|*.json",
                RestoreDirectory = true
            };
            saveFileDialog1.ShowDialog();
            var file = saveFileDialog1.FileName;

            string json = JsonConvert.SerializeObject(botTelegram.BotMessageLog.ToArray());

            //write string to file
            System.IO.File.WriteAllText(file, json);
        }

        /// <summary>
        /// Открытие папки с загруженными файлами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            Process.Start($@"{path}\Files");
        }
    }
}
