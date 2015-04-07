using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CAccounts;
using Message;
using ProjectType;
using DataBaseUtilities;
namespace ServerV1
{
    public partial class Form1 : Form
    {
        public TcpListener listener;
        public Thread thread_listener;
        public Configuration config;         
        private void listen()
        {
            try
            {
                listener.Start();
                while (true)
                {
                    ClientHandler handler = new ClientHandler(listener.AcceptTcpClient(), this);
                    Thread clientThread = new Thread(new ThreadStart(handler.RunClient));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }

            }
            catch (Exception exp)
            {

            }
        }

        public Form1()
        {
            if (File.Exists("config.bin"))
            {
                FileStream fs = new FileStream("config.bin", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                config = (Configuration) formatter.Deserialize(fs);
                fs.Close();
            }
            else
                config = new Configuration();
            listener = new TcpListener(config.get_ip(), config.get_port());
            InitializeComponent();
            bServerStop.Enabled = false;
            //DbUtilities.AddGroup("IS-31");
            //DbUtilities.AddGroup("IS-32");
            //DbUtilities.AddGroup("IS-33");
            //Student st = new Student("tayhao", "123456", "Ivan", "Ivanov", "Ivanovich", "1234", "IS-33", Faculties.FIOT);
            //DbUtilities.AddAccount(st);
            //Instructor instr = new Instructor("Kovaluk", "123456", "Tetyana", "Kovaluk", "Vladimirovna");
            //DbUtilities.AddAccount(instr);
            //Normokontroler nk = new Normokontroler("test", "123456", "Elena", "Klimenko", "Ivanovna");
            //DbUtilities.AddAccount(nk);
            //LabWorks lb1 = new LabWorks(instr.InstructorId, "Курс лабораторных работ с ООП",
            //    "Объе́ктно-ориенти́рованное программи́рование (ООП) — парадигма программирования, в которой основными концепциями являются понятия объектов и классов. В случае языков с прототипированием вместо классов используются объекты-прототипы.");
            //LabWorks lb2 = new LabWorks(instr.InstructorId, "Курс лабораторных работ с Теории Алгоритмов",
            //    "Тео́рия алгори́тмов — наука, находящаяся на стыке математики и информатики, изучающая общие свойства и закономерности алгоритмов и разнообразные формальные модели их представления. К задачам теории алгоритмов относятся формальное доказательство алгоритмической неразрешимости задач, асимптотический анализ сложности алгоритмов, классификация алгоритмов в соответствии с классами сложности, разработка критериев сравнительной оценки качества алгоритмов и т. п. Вместе с математической логикой теория алгоритмов образует теоретическую основу вычислительных наук.");
            //RGR rg = new RGR(instr.InstructorId, "Розрахунково-графична робота з курсу Павлова",
            //    "Развитие теории алгоритмов начинается с доказательства К. Гёделем теорем о неполноте формальных систем, включающих арифметику, первая из которых была доказана в 1931 г. Возникшее в связи с этими теоремами предположение о невозможности алгоритмического разрешения многих математических проблем (в частности, проблемы выводимости в исчислении предикатов) вызвало необходимость стандартизации понятия алгоритма. Первые стандартизованные варианты этого понятия были разработаны в 30-х годах XX века в работах А. Тьюринга, А. Чёрча и Э. Поста. Предложенные ими машина Тьюринга, машина Поста и лямбда-исчисление Чёрча оказались эквивалентными друг другу. Основываясь на работах Гёделя, С. Клини ввел понятие рекурсивной функции, также оказавшееся эквивалентным вышеперечисленным.");
            //DbUtilities.AddProject(lb1);
            //DbUtilities.AddProject(lb2);
            //DbUtilities.AddProject(rg);
            //Event lb11 = new Event(lb1.ID, 1, "Задание 1", DateTime.Now, "Сделать 2+2 и 3-1 для павлина");
            //Event lb12 = new Event(lb1.ID, 2, "Задание 2", DateTime.Now, "Сделать 2+2 и 3-1 для акваланга");
            //Event lb13 = new Event(lb1.ID, 3, "Задание 2", DateTime.Now, "Сделать 2+2 и 3-1 для жигуля");
            //Event lb21 = new Event(lb2.ID, 1, "Задание 1", DateTime.Now, "Сделать проект для Ковалюк 1");
            //Event lb22 = new Event(lb2.ID, 2, "Задание 2", DateTime.Now, "Сделать проект для Ковалюк 2");
            //Event lb23 = new Event(lb2.ID, 3, "Задание 3", DateTime.Now, "Сделать проект для Ковалюк 3");
            //Event rg1 = new Event(rg.ID, 1, "Задание 1", DateTime.Now, "Посчитать производную");
            //Event rg2 = new Event(rg.ID, 2, "Задание 2", DateTime.Now, "Посчитать интеграл");
            //Event rg3 = new Event(rg.ID, 3, "Задание 3", DateTime.Now, "Посчитать и убиться об стену");

            //DbUtilities.AddEvent(lb11);
            //DbUtilities.AddEvent(lb12);
            //DbUtilities.AddEvent(lb13);
            //DbUtilities.AddEvent(lb21);
            //DbUtilities.AddEvent(lb22);
            //DbUtilities.AddEvent(lb23);
            //DbUtilities.AddEvent(rg1);
            //DbUtilities.AddEvent(rg2);
            //DbUtilities.AddEvent(rg3);
            //DiplomaProject dp = new DiplomaProject(instr.InstructorId, "Ковалючный дипломный проект",
            //    "Ім'я Ковалюк Тетяни Володимирівни — ученого секретаря комісії з галузі знань «Інформатика та обчислювальна техніка» Науково-методичної ради Міністерства освіти і науки України та підкомісії з комп'ютерних наук, доцента, кандидата технічних наук добре відоме не лише в НТУУ «КПІ», а й в Україні.");
            //dp.InstroctorName = instr.Lastname + " " + instr.Firstname + " " + instr.Patronymic;
            //dp.NormokontrolerName = nk.Lastname + " " + nk.Firstname + " " + nk.Patronymic;
            //DbUtilities.AddProject(dp);
            //Event dp1 = new Event(dp.ID, 1, "Задание 1", DateTime.Now,
            //    "Сделать первую версию РГР, послушать бред по министерству образования");
            //Event dp2 = new Event(dp.ID, 2, "Задание 2", DateTime.Now,
            //    "Сделать вторую версию РГР, послушать бред про корею");
            //Event dp3 = new Event(dp.ID, 3, "Задание 3", DateTime.Now,
            //    "Сделать 3 версию РГР, пойти митинговать самсунг");
            //Event dp4 = new Event(dp.ID, 4, "Задание 4", DateTime.Now,
            //    "Сделать 4 версию РГР, сделать ковалюк министром освиты");
            //Event dp5 = new Event(dp.ID, 5, "Задание 5", DateTime.Now,
            //    "Сделать 5 версию РГР, сделать ковалюк президентом мира");
            //DbUtilities.AddEvent(dp1);
            //DbUtilities.AddEvent(dp2);
            //DbUtilities.AddEvent(dp3);
            //DbUtilities.AddEvent(dp4);
            //DbUtilities.AddEvent(dp5);
            //DbUtilities.AddStudentToProject(st.StudentId, lb1.ID);
            //DbUtilities.AddStudentToProject(st.StudentId, lb2.ID);
            //DbUtilities.AddStudentToProject(st.StudentId, rg.ID);
            //DbUtilities.AddStudentToProject(st.StudentId, dp.ID);
        }

        private void bServerStart_Click(object sender, EventArgs e)
        {
            thread_listener = new Thread(new ThreadStart(listen));
            thread_listener.Start();
            bServerStart.Enabled = false;
            bServerStop.Enabled = true;            
        }
        private void bServerStop_Click(object sender, EventArgs e)
        {
            listener.Stop();
            thread_listener.Abort();
            bServerStart.Enabled = true;
            bServerStop.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(thread_listener != null && thread_listener.IsAlive)
            {
                listener.Stop();
                thread_listener.Abort();
            }
            FileStream fs = new FileStream("config.bin", FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, config);
            fs.Close();
        }
    }    
    public class ClientHandler
    {
        public ClientHandler(TcpClient arg, Form1 f)
        {
            client = arg;
            form = f;
        }
        public void RunClient()
        {
           
            using(NetworkStream writerStream = client.GetStream())
            {
                    BinaryFormatter outFormatter = new BinaryFormatter();
                    MSG message;
                        message = (MSG)outFormatter.Deserialize(writerStream);
                        switch (message.stat)
                        {                        
                            case STATUS.LOGIN:
                                string login = (string)outFormatter.Deserialize(writerStream);
                                string password = (string)outFormatter.Deserialize(writerStream);
                                Account ac = DbUtilities.Login(login, password);
		                        bool fll = true;
		                        if (ac == null)
		                        {
			                        fll = false;
			                        outFormatter.Serialize(writerStream, fll);
		                        }
		                        else
		                        {
			                        outFormatter.Serialize(writerStream, fll);
									outFormatter.Serialize(writerStream, ac);
		                        }
                                break;
                            case STATUS.GET_GROUPS:
		                        Dictionary<string, int> dic = DbUtilities.GetGroups();
                                bool gg = true;
                                if (dic == null)
                                {
                                    gg = false;
                                    outFormatter.Serialize(writerStream, gg);
                                }
                                else
                                {
                                    outFormatter.Serialize(writerStream, gg);
                                    outFormatter.Serialize(writerStream, dic);
                                }
		                        break;
							case STATUS.ADD_STUDENT:
		                        Student st = (Student) outFormatter.Deserialize(writerStream);
		                        bool fl = true;
		                        try
		                        {
									DbUtilities.AddAccount(st);
		                        }
		                        catch (Exception)
		                        {
			                        fl = false;
		                        }
		                        outFormatter.Serialize(writerStream, fl);
		                        break;
							case STATUS.GET_PROJECT_BY_STUDENT:
		                        int id = (int)outFormatter.Deserialize(writerStream);
		                        List<Project> ProjectCoollection = DbUtilities.GetProjects(id);
		                        bool flpc = true;
		                        if (ProjectCoollection == null)
		                        {
			                        flpc = false;
									outFormatter.Serialize(writerStream, flpc);
		                        }
		                        else
		                        {
			                        outFormatter.Serialize(writerStream, flpc);
									outFormatter.Serialize(writerStream, ProjectCoollection);
		                        }
		                        break;
                            case STATUS.GET_EVENTS_BY_PROJECT:
                                int prid = (int) outFormatter.Deserialize(writerStream);
                                List<Event> evCollection = DbUtilities.GetEvents(prid);
                                bool flgebp = true;
                                if (evCollection == null)
                                {
                                    flgebp = false;
                                    outFormatter.Serialize(writerStream, flgebp);
                                }
                                else
                                {
                                    outFormatter.Serialize(writerStream, flgebp);
                                    outFormatter.Serialize(writerStream, evCollection);
                                }
                                break;
                        }
            }
            /*StreamReader readerStream = new StreamReader(client.GetStream());
            NetworkStream writerStream = client.GetStream();
            string returnData = readerStream.ReadLine();
            returnData += "\r\n";
                byte[] dataWrite = Encoding.ASCII.GetBytes(returnData);
                writerStream.Write(dataWrite, 0, dataWrite.Length);
           
            client.Close();*/
        }        
        // Data
        private TcpClient client;
        private Form1 form;
    }
}
