using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net.Configuration;

namespace Client
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient();   //SOCKET TANIMLANDI
        NetworkStream serverStream = default(NetworkStream);  // SERVER ÜZERİNE YAYIMLANACAK STREAM
        string readdata = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket.Connect(textBox1.Text, Int32.Parse(textBox2.Text));  //BAĞLAN BUTONUNA BASILDIĞINDA SERVERA TCP BAĞLANTI İSTEĞİ GÖNDERİLDİ
                Thread ctThread = new Thread(getMessage);  // SERVERDAN CEVAP GELİRSE YAKALAMAK İÇİN CEVAP ALMA THREADİ OLUŞTURULUP BAŞLATILDI
                ctThread.Start();
                button1.Enabled = false;  // BAĞLANTI KURULDUKTAN SONRA BAĞLANTININ KURULDUĞUNA EMİN OLMAK İÇİN BUTON KAPATILDI
            }
            catch (Exception )  // SERVER İLE BAĞLANTI KURULMADA AKSAKLIK YAŞANIRSA...
            {
                MessageBox.Show("SERVER İLE BAĞLANTI KURULAMADI LÜTFEN İP ADRESİ VE PORTLARI KONTROL EDİNİZ VE SERVERİN BAŞLATILDIĞINDAN EMİN OLUNUZ", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void getMessage()  // SERVER İLE BAĞLANTI KURULDUKTAN SONRA SÜREKLİ ÇALIŞAN BU THREAD SERVERDAN CEVAP VAR MI DİYE KONTROL EDİYOR SÜREKLİ CEVAP GELİRSE BUNU SONUÇ TEXTBOXUNA BASIYOR
        {
            string returndata;

            while (true)
            {
                serverStream = clientSocket.GetStream();
                var buffsize = clientSocket.ReceiveBufferSize;
                byte[] instream = new byte[buffsize];

                serverStream.Read(instream, 0, buffsize);

                returndata = System.Text.Encoding.ASCII.GetString(instream);

                readdata = returndata;
                msg();

            }

        }

        private void msg()  //GELEN SONUCU TEXTBOXA BASMAK İÇİN KULLANILDI
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                textBox4.Text = readdata;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)  // BURADA SEÇİLEN GÖNDERME TÜRÜNE GÖRE YANİ 4 İŞLEM FAKTORİYEL GİBİ SERVERA SERVERİN HANGİ HESAPLAMAYI YAPMASI GEREKTİĞİNİ 
                                                                  // ANLAMASI İÇİN ÜZERİNE PARAMETRE EKLEYEREK VE BYTE OLARAK ENCODİNG YAPARAK SERVERA GÖNDERİLDİ
        {
            byte[] outstream = null;
            if (radioButton1.Checked)
            {
                outstream = Encoding.ASCII.GetBytes(textBox3.Text+"islem");
            }
            else if (radioButton2.Checked)
            {
                outstream = Encoding.ASCII.GetBytes(textBox3.Text+"faktor"); //ÜZERİNE EKLEMELER VE BYTE ENCODİNG
            }
            else if (radioButton3.Checked)
            {
                outstream = Encoding.ASCII.GetBytes(textBox3.Text+"fibo");
            }


            serverStream.Write(outstream, 0, outstream.Length); // SERVERA BYTE OLARAK ŞİFRELENMİŞ STREAM GÖNDERİLDİ
            serverStream.Flush();                               // RAME BASILDI
        }
    }
}
