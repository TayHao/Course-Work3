using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace StudClient
{
    [Serializable]
    public class Configuration
    {
        public Configuration()
        {
            Port = 3456;
            IP = IPAddress.Parse("127.0.0.1");
            Login = "";
            Password = "";
        }

        public Configuration(int port, IPAddress ip, string login, string password)
        {
            Port = port;
            IP = ip;
            Login = login;
            Password = password;
        }

        public IPAddress IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
            }
        }
        public int Port
        {
            get { return _port; }
            set
            {
                if (value >= 1024 && value <= 49151)
                    _port = value;
                else
                    throw new WrongPort();
            }
        }
        // Data
        private int _port;
        private IPAddress _ip;
        public string Login;
        public string Password;
    }

    [Serializable()]
    public class WrongPort : Exception
    {        
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        //Стандартные конструкторы
        public WrongPort() : base("Неправильный номер порта.") { }
        public WrongPort(string message) : base(message) { }
        public WrongPort(string message, Exception innerException) : base(message, innerException) { }
    }
}
