using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;
        double result;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;//giriş
            server.StringEncoder = Encoding.UTF8;  // SERVERİN FORMATINA DÖNÜŞTÜRÜLDÜ
            server.DataReceived += Server_DataReceived;
        }
        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                string[] parameters;
                txtStatus.AppendText(Environment.NewLine);
                txtStatus.AppendText(Environment.NewLine); // LOGGİNG İŞLEMİ BURADA YAPILDI
                txtStatus.AppendText("Gelen İstek : ");
                txtStatus.AppendText(e.MessageString);
                string recievedMessage = e.MessageString ;    // SERVERİN DİNLEDİĞİ İP VE PORTA BİR MESAJ GÖNDERİLİRSE BURAYA DÜŞÜYOR VE RECİEVED MESSAGE DEĞİŞKENİNE ATILIYOR
                parameters= recievedMessage.Split('#');  //BU ŞEKİLDE GELEN MESAJIN PARÇALANMASI SAĞLANDI
                
                
                if (parameters[parameters.Length - 1] == "islem")  // MESAJ HATALI DEĞİLSE HESAPLA FONKSİYONUNA GÖNDERİLDİ
                {
                    if (parameters.Length !=4)   // EĞER GELEN PARAMETRE DEĞERLERİ HATALIYSA...
                    {
                        MessageBox.Show("Gelen mesajın parametreleri hatalı. Lütfen kullanım şekile uygun olarak tekrar deneyiniz. Doğru Radiobuttonu seçtiğinizden emin olunuz", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        result = calculation(parameters);
                        e.ReplyLine(string.Format("{0}#", result));  // İŞLEM SONUCU CLİENTA YANİ İSTEK ATAN KİŞİYE GÖNDERİLDİ
                    }
                    
                }
                if (parameters[parameters.Length - 1] == "faktor")  // MESAJ HATALI DEĞİLSE FAKTOR FONKSİYONUNA GÖNDERİLDİ
                {
                    if (parameters.Length != 2)   // EĞER GELEN PARAMETRE DEĞERLERİ HATALIYSA...
                    {
                        MessageBox.Show("Gelen mesajın parametreleri hatalı. Lütfen kullanım şekile uygun olarak tekrar deneyiniz. Doğru Radiobuttonu seçtiğinizden emin olunuz", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Thread facThread = new Thread(() => factorial(parameters));
                        facThread.Start();           //THREAD OLUŞTURUP FONKSİYONUMUZU YENİ THREADDE BAŞLATTIKTAN SONRA FONKSİYONUN İŞİNİ BİTİRMESİNİ BEKLEMEK İÇİN JOİN KULLANIYORUZ
                        facThread.Join();
                        e.ReplyLine(string.Format("{0}#", result));  // İŞLEM SONUCU CLİENTA YANİ İSTEK ATAN KİŞİYE GÖNDERİLDİ
                    }
                    
                }
                if (parameters[parameters.Length - 1] == "fibo")  // MESAJ HATALI DEĞİLSE FİBO FONKSİYONUNA GÖNDERİLDİ
                {
                    if (parameters.Length != 2)   // EĞER GELEN PARAMETRE DEĞERLERİ HATALIYSA...
                    {
                        MessageBox.Show("Gelen mesajın parametreleri hatalı. Lütfen kullanım şekile uygun olarak tekrar deneyiniz. Doğru Radiobuttonu seçtiğinizden emin olunuz", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Thread fiboThread = new Thread(() => fib(parameters));
                        fiboThread.Start();         //THREAD OLUŞTURUP FONKSİYONUMUZU YENİ THREADDE BAŞLATTIKTAN SONRA FONKSİYONUN İŞİNİ BİTİRMESİNİ BEKLEMEK İÇİN JOİN KULLANIYORUZ
                        fiboThread.Join();
                        e.ReplyLine(string.Format("{0}#", result));  // İŞLEM SONUCU CLİENTA YANİ İSTEK ATAN KİŞİYE GÖNDERİLDİ
                    }
                    
                }
            });
        }

        private void factorial(string[] parameters)    // GELEN DEĞER İÇİN BASİT FAKTORİYEL HESAPLAMASI YAPILIP CEVABA YAZILDI
        {

            int n = int.Parse(parameters[0]);
            int res = 1;
            while (n != 1)
            {
                res = res * n;
                n = n - 1;
            }
            result = res;
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            
            System.Net.IPAddress ip = IPAddress.Parse(txtHost.Text);
            server.Start(ip, Convert.ToInt32(txtPort.Text));          //SERVER TEXTBOXLARDA VERİLEN İP ADRES VE PORT ÜZERİNDE DİNLEMEYE BAŞLADI
            txtStatus.Text += "Server "+txtHost.Text+" ipsi üzerinde "+txtPort.Text+" portunda " +"dinlemeye başladı...";
        }

        private void btnStop_Click_1(object sender, EventArgs e)
        {
            if (server.IsStarted)
            {
                server.Stop();// STOP BUTONUNA BASILDIĞINDA SERVERİN DİNLEMEYİ DURDURMASI SAĞLANDI
                txtStatus.Text += "Server dinlemeyi kesti...";
            }
        }

        private void fib(string[] parameters)   // GELEN DEĞER İÇİN BASİT FİBONACCİ HESAPLAMASI YAPILIP CEVABA YAZILDI
        {
            int n = int.Parse(parameters[0]);
            
            int[] f = new int[n + 2];
            int i;

            
            f[0] = 0;
            f[1] = 1;

            for (i = 2; i <= n; i++)
            {
                
                f[i] = f[i - 1] + f[i - 2];
            }

            result=f[n];
        }

        public double calculation(string[] parameters)
        {
            // BURADA BASİT 4 İŞLEM YAPILDI EĞER HATALIYSA -999.99 GÖNDERİLDİ
            if (char.Parse(parameters[(parameters.Length-2)]) == '+')
            {
                return (double.Parse(parameters[0]) + double.Parse(parameters[1]));
            }
            else if (char.Parse(parameters[parameters.Length - 2]) == '*')
            {
                return (double.Parse(parameters[0]) * double.Parse(parameters[1]));
            }
            else if (char.Parse(parameters[parameters.Length - 2]) == '/')
            {
                return (double.Parse(parameters[0]) / double.Parse(parameters[1]));
            }
            else if (char.Parse(parameters[parameters.Length - 2]) == '-')
            {
                return (double.Parse(parameters[0]) - double.Parse(parameters[1]));
            }
            return -999.99;
        }

    }
}
