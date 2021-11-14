using System;

namespace WpfApp10.TelegramBot
{
    [Serializable]
    public struct MessageLog
    {

        public MessageLog(string v, string messageText, string firstName, long id) : this()
        {
            Time = v;
            Text = messageText;
            FirstName = firstName;
            Id = id;
        }

        public string Time { set; get; }
        public string FirstName { set; get; }
        public long Id { set; get; }
        public string Text { set; get; }
    }
}