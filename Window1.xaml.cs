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
using System.Windows.Shapes;
using System.Net.Sockets;

namespace Savoprojekat
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btn_PridruziSe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = tb_ip.Text;
                MainWindow Igra = new MainWindow(ip, false);
                Igra.Show();
                this.Close();
                
            }
            catch
            {
                MessageBox.Show("Wrong IP address!");
            }
        }

        private void btn_kreirajIgru_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Igra = new MainWindow(null, true);
            Igra.Show();
            this.Close();
        }
    }
}
