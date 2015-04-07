using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CAccounts;

namespace StudClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Student student;
        public Configuration config;
        public Dictionary<string, int> GroupList;

        public void SaveConfig()
        {
            FileStream fs = new FileStream("config.bin", FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, config);
            fs.Close();
        }

        public void CreateConfig()
        {
            if (File.Exists("config.bin"))
            {
                FileStream fs = new FileStream("config.bin", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                config = (Configuration)formatter.Deserialize(fs);
                fs.Close();
            }
            else
                config = new Configuration();
        }
    }
}
