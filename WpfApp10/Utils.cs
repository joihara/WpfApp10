using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;


public class Utils{

    #region Информация о боте
    /// <summary>
    /// Название файла для сохранения списка ботов
    /// </summary>
    private readonly string filename = "bots.db";

    /// <summary>
    /// Добавление нового бота в файл
    /// </summary>
    /// <param name="name"></param>
    /// <param name="token"></param>
    public void AddBot(string name, string token)
    {
            List<StructBot> bots = new List<StructBot>();

            var reader = ReadBot();

            if (reader != null)
            {
                bots = reader.ToList();
            }

            StructBot bot = new StructBot()
            {
                BotName = name,
                Token = token
            };

            bots.Add(bot);
            FileStream fs = FileWait();
            XmlSerializer x = new XmlSerializer(typeof(StructBot[]));
            TextWriter writer = new StreamWriter(fs);
            x.Serialize(writer, bots.ToArray());

    }

    /// <summary>
    /// Ожидание файла если он занят
    /// </summary>
    /// <returns></returns>
    public FileStream FileWait(string file)
    {
        FileStream fs;
        while (true)
        {
            fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            if (fs.CanRead)
            {
                Thread.Sleep(1000);
                break;
            }
        }
        return fs;
    }

    /// <summary>
    /// Ожидание файла если он занят
    /// </summary>
    /// <returns></returns>
    public FileStream FileWait()
    {
        return FileWait(filename);
    }

    /// <summary>
    /// Чтение списка с ботами
    /// </summary>
    /// <returns></returns>
    public StructBot[] ReadBot() {
            StructBot[] outer;
                
            XmlSerializer x = new XmlSerializer(typeof(StructBot[]));
            FileStream fs = FileWait();

            TextReader reader = new StreamReader(fs);
            try
            {
                outer = (StructBot[])x.Deserialize(reader);
            }
            catch (InvalidOperationException) {
                outer = Array.Empty<StructBot>();
            }
            return outer;
    }
    #endregion

        /// <summary>
        /// Получение новостной ленты
        /// </summary>
        public static StructRss[] ParseRSS(string Url)
        {
            StructRss[] MangaRss;
            try
            {
                XmlTextReader reader = new XmlTextReader(Url);
                var formatter = new Rss20FeedFormatter();
                formatter.ReadFrom(reader);
                List<StructRss> mrs = new List<StructRss>();
                var DataContext = formatter.Feed.Items;

                foreach (var send in DataContext)
                {
                    mrs.Add(new StructRss(send));
                }

                MangaRss = mrs.ToArray();
                mrs.Clear();
            }
            catch (Exception)
            {
                MangaRss = Array.Empty<StructRss>();
            }
            return MangaRss;
        }
    }
