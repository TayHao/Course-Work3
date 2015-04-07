using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CAccounts;
using ProjectType;
using DataBaseUtilities;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Message;

namespace StudClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Page
    {
        public Student student;
        public List<Project> ProjectCollection;
        public List<Event> EventCollection;
        public Project CurProject;
        public int CurrentEventID;
        public MainWindow()
        {
            student = (App.Current as App).student;
            InitializeComponent();
            UpdateProjectsList();
            Projects.SelectedIndex = 0;
            DataGridTextColumn col = new DataGridTextColumn();
            col.Header = "№";
            col.Width = new DataGridLength(30);
            col.Binding = new Binding("ID");
            EventsGrid.Columns.Add(col);
            col = new DataGridTextColumn();
            col.Header = "Назва";
            col.Width = new DataGridLength(250);
            col.Binding = new Binding("Name");
            EventsGrid.Columns.Add(col);
            col = new DataGridTextColumn();
            col.Header = "Дедлайн";
            col.Width = new DataGridLength(120);
            col.Binding = new Binding("Deadline");
            EventsGrid.Columns.Add(col);
            col = new DataGridTextColumn();
            col.Header = "Дата здачі";
            col.Width = new DataGridLength(120);
            col.Binding = new Binding("Date");
            EventsGrid.Columns.Add(col);
            col = new DataGridTextColumn();
            col.Header = "Оцінка";
            col.Width = new DataGridLength(70);
            col.Binding = new Binding("Points");
            EventsGrid.Columns.Add(col);
            col = new DataGridTextColumn();
            col.Header = "Штраф";
            col.Width = new DataGridLength(70);
            col.Binding = new Binding("Penalty");
            EventsGrid.Columns.Add(col);
            EventsTab.IsEnabled = false;
        }

        private void UpdateEventsGrid(int projectID)
        {
            Configuration config = (App.Current as App).config;
            TcpClient eClient = new TcpClient();
            try
            {
                eClient = new TcpClient(config.IP.ToString(), config.Port);
                using (NetworkStream writerStream = eClient.GetStream())
                {
                    MSG message = new MSG();
                    message.stat = STATUS.GET_EVENTS_BY_PROJECT;
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(writerStream, message);
                    formatter.Serialize(writerStream, projectID);
                    bool fl = (bool)formatter.Deserialize(writerStream);
                    if (fl)
                    {
                        EventCollection = (List<Event>) formatter.Deserialize(writerStream);
                        EventRow rw;
                        foreach (Event ev in EventCollection)
                        {
                            rw = new EventRow(ev.SerialNumber.ToString(), ev.Title,
                                ev.DeadLine == new DateTime() ? "Не встановлено" : ev.DeadLine.ToShortDateString(),
                                ev.AcceptDate == new DateTime() ? "Не здано" : ev.AcceptDate.ToShortDateString(),
                                ev.RealMark.ToString(), ((1 - ev.Penalty) * 100).ToString() + "%");
                            EventsGrid.Items.Add(rw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                eClient.Close();
            }
        }

        private void UpdateProjectsList()
        {
            Configuration config = (App.Current as App).config;
            TcpClient eClient = new TcpClient();
            try
            {
                eClient = new TcpClient(config.IP.ToString(), config.Port);
                using (NetworkStream writerStream = eClient.GetStream())
                {
                    MSG message = new MSG();
                    message.stat = STATUS.GET_PROJECT_BY_STUDENT;
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(writerStream, message);
                    formatter.Serialize(writerStream, student.StudentId);
                    bool fl = (bool)formatter.Deserialize(writerStream);
                    if (fl)
                        ProjectCollection = (List<Project>)formatter.Deserialize(writerStream);
                    else
                    {
                        ProjectCollection = new List<Project>();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                eClient.Close();
            }
            foreach (Project pr in ProjectCollection)
            {
                string type = pr.GetType().Name;
                switch (type)
                {
                    case "LabWorks":
                        type = "Лабораторна робота";
                        break;
                    case "RGR":
                        type = "РГР";
                        break;
                    case "DiplomaProject":
                        type = "Дипломний проект";
                        break;
                }
                Projects.Items.Add(type + ": " + pr.Theme);
            }
        }

        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }
        private void AccountMenu_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow sw = new AccountWindow();
            sw.ShowDialog();
        }

        private void QuitMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new System.Uri("StartPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Projects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventsGrid.Items.Clear();
            GeneralInfoTab.IsSelected = true;
            EventsTab.IsEnabled = false;
            UpdateEventsGrid(ProjectCollection[Projects.SelectedIndex].ID);
            ProjectDescription.Text = ProjectCollection[Projects.SelectedIndex].Description;
            CurProject = ProjectCollection[Projects.SelectedIndex];
            ProjectInfo.Text = "Поточна оцінка: " + (from i in EventCollection select i.RealMark).Sum().ToString();
            ProjectInfo.Text += "\nКінцевий термін здачі: " +
                                EventCollection[EventCollection.Count - 1].DeadLine.ToShortDateString();
            
        }
        private void Row_Click(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            ProjectTabs.SelectedIndex++;
            if (dg.SelectedIndex != -1)
            {
                CurrentEventID = dg.SelectedIndex;
                EventDescription.Text = EventCollection[CurrentEventID].Description;
                EventDescription.Text += String.Format("\nОцінка: {0}\nШтраф: {1}%\nКінцевий термін здачі: {2}\nДата здачі: {3}",
                    EventCollection[CurrentEventID].RealMark.ToString(), ((1 - EventCollection[CurrentEventID].Penalty) * 100).ToString(),
                    EventCollection[CurrentEventID].DeadLine == new DateTime() ? "Не встановлено" : EventCollection[CurrentEventID].DeadLine.ToShortDateString(),
                    EventCollection[CurrentEventID].AcceptDate == new DateTime() ? "Не здано" : EventCollection[CurrentEventID].AcceptDate.ToShortDateString());
                Theme.Text = EventCollection[CurrentEventID].Title;
                EventsTab.IsEnabled = true;
                Theme.Background = Brushes.White;
                int diff = DaysBetween(DateTime.Now, EventCollection[CurrentEventID].DeadLine);
                if (diff < 0)
                {
                    Theme.Background = new SolidColorBrush(Color.FromRgb(210,80,80));
                }
                else if (diff < 2)
                {
                    Theme.Background = new SolidColorBrush(Color.FromRgb(252, 231, 0));
                }
            }
        }
        private int DaysBetween(DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);
            return (int)span.TotalDays;
        }
    }
}
