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
using System.Net.Sockets;
using System.ComponentModel;
using System.Windows.Threading;

namespace Savoprojekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polje[,] matrica = new Polje[10, 10];
        int[] podmornice = new int[6];
        int brojPreostalihPodmornica = 4;
        int brojPoteza = 0;
        int protivnikRezultat = 0;
        bool krajPartije = false;
        bool protivnikZavrsio = false;
        TcpListener host;
        TcpClient client;
        Socket sock;
        int port = 3577;
        BackgroundWorker messageReciever = new BackgroundWorker();
        bool isHost;

        public MainWindow(string ip, bool isHost)
        {
            this.isHost = isHost;
            InitializeComponent();
            kreirajMatricu();
            messageReciever.DoWork += MessageReciever_DoWork;
            kreirajPodmornice();
            if (isHost)
            {
                host = new TcpListener(System.Net.IPAddress.Any, port);
                host.Start();
                sock = host.AcceptSocket();
                messageReciever.RunWorkerAsync();
            }
            else
            {
                try
                {
                    client = new TcpClient(ip, port);
                    sock = client.Client;
                    messageReciever.RunWorkerAsync();
                }
                catch
                {
                    MessageBox.Show("Error");
                    Close();
                }

            }


        }
        void posaljiRezultat()
        {
            krajPartije = true;
            byte[] num = { (byte)brojPoteza };
            sock.Send(num);
            this.Dispatcher.Invoke(() =>
            {
                if (protivnikZavrsio == true)
                {
                    if (protivnikRezultat < brojPoteza)
                    {
                        MessageBox.Show("You lost! \n Resullt: " + brojPoteza + "/" + protivnikRezultat);
                    }
                    else if (protivnikRezultat > brojPoteza)
                    {
                        MessageBox.Show("You won! \n Resullt: " + brojPoteza + "/" + protivnikRezultat);
                    }
                    else
                    {
                        MessageBox.Show("It's a tie! \n Resullt: " + brojPoteza + "/" + protivnikRezultat);
                    }
                }
            });
        }
        void primiRezultat()
        {
            try
            {
                byte[] buffer = new byte[2];
                sock.Receive(buffer);
                protivnikRezultat = buffer[0];
                this.Dispatcher.Invoke(() =>
                {
                    lab_protivnik.Content = "Opponent is done!";
                });
            }
            catch
            {

            }
        }
        private void MessageReciever_DoWork(object sender, DoWorkEventArgs e)
        {

                    primiRezultat();
            this.Dispatcher.Invoke(() =>
            {


                lab_protivnik.Content = "Opponent is done!";
                    protivnikZavrsio = true;
                    if (krajPartije == true)
                    {
                    if (protivnikRezultat < brojPoteza)
                    {
                        MessageBox.Show("You lost! \n Resullt: " + brojPoteza + "/" + protivnikRezultat);
                    }
                    else if (protivnikRezultat > brojPoteza)
                    {
                        MessageBox.Show("You won! \n Resullt: " + brojPoteza + "/" + protivnikRezultat);
                    }
                    else
                    {
                        MessageBox.Show("It's a tie! \n Resullt: " + brojPoteza + "/" + protivnikRezultat);
                    }
                }
                });
            }
        

        public bool isPozicijaMoguca(int x, int y, int o, int N)
        {
            if (o == 1 && y - N >= 0) 
            {
                for(int i = y; i > y - N; i--)
                {
                    if (matrica[x, i].isPodmornica !=-1) return false;
                }
                return true;
            }
            if (o == 2 && y + N < 10)
            {
                for (int i = y; i < y + N; i++)
                {
                    if (matrica[x, i].isPodmornica  != -1) return false;
                }
                return true;
            }
            if (o == 3 && x + N < 10)
            {
                for (int i = x; i < x + N; i++)
                {
                    if (matrica[i, y].isPodmornica != -1) return false;
                }
                return true;
            }
            if (o == 4 && x - N >= 0)
            {
                for (int i = x; i > x - N; i--)
                {
                    if (matrica[i, y].isPodmornica != -1) return false;
                }
                return true;
            }
            return false;
        }

        public void kreirajPodmornicu(int x, int y, int o, int N)
        {
            if (o == 1)
            {
                for (int i = y; i > y - N; i--)
                {
                    podmornice[N] = N-1;
                    matrica[x, i].isPodmornica = N;
                    
                }
                
            }
            if (o == 2)
            {
                for (int i = y; i < y + N; i++)
                {
                    podmornice[N] = N-1;
                    matrica[x, i].isPodmornica = N;
                   
                }

            }
            if (o == 3) // dole
            {
                for (int i = x; i < x + N; i++)
                {
                    podmornice[N] = N-1;
                    matrica[i, y].isPodmornica = N;
                   
                }

            }
            if (o == 4) // gore
            {
                for (int i = x; i > x - N; i--)
                {
                    matrica[i, y].isPodmornica = N;
                    podmornice[N] = N-1;
                    
                }

            }
        }
        public void kreirajPodmornice()
        {
            bool k = true;
            Random random = new Random();
            for(int i = 5; i > 1; i--)
            {
                
                k = true;
                while (k == true)
                {
                    int pozicijaX = random.Next(0, 10);
                    int pozicijaY = random.Next(0, 10);
                    int orentacija = random.Next(1, 5);
                    
                    if (isPozicijaMoguca(pozicijaX, pozicijaY, orentacija, i) == true)
                    {

                        kreirajPodmornicu(pozicijaX, pozicijaY, orentacija, i);
                        k = false;
                    }
                }
               
                
            }
        }


        public void kreirajMatricu()
        {
            
            for (int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    Polje polje = new Polje();
                    Thickness thickness = polje.Margin;
                    thickness.Left = 1;
                    thickness.Right = 1;
                    thickness.Top = 1;
                    thickness.Bottom = 1;
                    polje.Margin = thickness;
                    polje.isPodmornica = -1;
                    polje.Click += Klik;
                    matrica[i,j] = polje;
                    Grid.SetRow(matrica[i, j], i);
                    Grid.SetColumn(matrica[i, j], j);
                    grid_matrica.Children.Add(matrica[i, j]);
                }
            }
        }
        
        
        private void Klik(object sender, RoutedEventArgs e)
        {
            Polje kliknutoDugme = new Polje();
            kliknutoDugme = (Polje)sender;
            if (kliknutoDugme.isKliknuto == false && krajPartije == false)
            {
                kliknutoDugme.isKliknuto = true;
                brojPoteza++;
                if (kliknutoDugme.isPodmornica == -1)
                {
                    
                    kliknutoDugme.Background = Brushes.Blue;
                }
                else
                {
                    kliknutoDugme.Background = Brushes.Red;
                    podmornice[kliknutoDugme.isPodmornica]--;
                    if (podmornice[kliknutoDugme.isPodmornica] == -1) 
                    { 
                        MessageBox.Show("A battleship is found");
                        brojPreostalihPodmornica--;
                        if(brojPreostalihPodmornica == 0)
                        {
                            krajIgre();
                        }

                    }

                }
            }
            
            
        }
        public void krajIgre()
        {
            MessageBox.Show("You found all of the battleships!\n" + "Moves made: " + brojPoteza.ToString());
            krajPartije = true;
            posaljiRezultat();
        }

        private void btn_Restart_Click(object sender, RoutedEventArgs e)
        {
            kreirajMatricu();
            kreirajPodmornice();
            brojPreostalihPodmornica = 4;
            brojPoteza = 0;
            krajPartije = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
