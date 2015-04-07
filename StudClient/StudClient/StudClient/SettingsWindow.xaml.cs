using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudClient
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Configuration cn = (App.Current as App).config;
        public SettingsWindow()
        {
            InitializeComponent();
            IPBox.Text = cn.IP.ToString();
            PortBox.Text = cn.Port.ToString();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cn.IP = IPAddress.Parse(IPBox.Text);
                cn.Port = int.Parse(PortBox.Text);
                (App.Current as App).SaveConfig();
                this.Close();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("IP-адреса чи порт не встановлено");
            }
            catch (FormatException)
            {
                MessageBox.Show("Невірна IP-адреса");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
