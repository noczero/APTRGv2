using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Library Serial Communication
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace APTRGv2
{
    public partial class Form1 : Form
    {
        //Variable Global


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //comboBox1 = PortName
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames()); //Mendeteksi Otomatia Port COM yang Aktif
        }

        //button2 = Connect
        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            button2.Enabled = false;

            button1.Enabled = true; //Mengaktifkan tombol send
            button4.Enabled = true; //Mengaktifkan tombol stop


            serialPort1.BaudRate = Int32.Parse(comboBox2.Text);
            serialPort1.PortName = comboBox1.Text;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataBits = 8;

            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);           
            serialPort1.Open(); //Membuka Sesi Port Serial untuk Komunikasi ke Arduino.
        }

        
       
        //button4 = Stop
        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            button2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;

            button4.Enabled = false; //Mematikan tombol Stop
            
            serialPort1.Close(); //Menutup Sesi Port Serial untuk Komunikasi ke Arduino.
          //  Close();
        }

        //Jika Sesi Port Serial Terbuka
        //class ini aktif

        // Deklarasi Variable Global
        string RAWData; //RawData = Data Dari Arduino
        string[] Data; //RAWData yang jadi Array biar bisa di split
                
        // Buat Class Tampil Data
        //Deklarasi Data
        string header, waktu, ketinggian, temperature, kelembaban, tekanan, arahangin, kec_angin, lintang, bujur;

        //Membuat class langusng dari tanda Event Properties DOble click
        private void serialPort1_DataReceived_1(object sender, SerialDataReceivedEventArgs e)
        {
            RAWData = serialPort1.ReadLine();
            
            Data = Regex.Split(RAWData, " "); //Memisah RAWData berdasarkan Spasi, diperlukan library using System.Text.RegularExpressions;. 

            this.Invoke(new EventHandler(tampildata)); //callback fungsi tampildata
            this.Invoke(new EventHandler(writedata)); //callback fungsi writedata
        }

        private void tampildata(object sender, EventArgs e)
        {
            //Array 
            header = Data[0];
            waktu = Data[1];
            ketinggian = Data[2];
            temperature = Data[3];
            kelembaban = Data[4];
            tekanan = Data[5];
            arahangin = Data[6];
            kec_angin = Data[7];
            lintang = Data[8];
            bujur = Data[9];

            richTextBox1.AppendText(RAWData);
            richTextBox1.ScrollToCaret(); //Scroll Biar di bawah
            textBox1.Text = waktu;
            textBox2.Text = ketinggian;
            textBox3.Text = temperature;
            textBox4.Text = kelembaban;
            textBox5.Text = tekanan;
            textBox6.Text = arahangin;
            textBox7.Text = kec_angin;
            textBox8.Text = lintang;
            textBox9.Text = bujur;

        }

        // Update Box

        
        //button5 = Browse
        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFD = new SaveFileDialog();
            saveFD.Filter = "Text Files|*.txt|All Files|*.*";
            saveFD.Title = "Select a File to Store the Serial Data";
            if (saveFD.ShowDialog() == DialogResult.OK)
            {
                pathbox.Text = saveFD.FileName;
            }
        }

        // Class Save Data
        bool Autosavestate;
        
        private void writedata(object sender, EventArgs e)
        {
            if (Autosavestate)
            {
                if (!File.Exists(pathbox.Text))
                {
                    File.Create(pathbox.Text);
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(pathbox.Text))
                    {
                        sw.Write(RAWData + "\n"); //RAWData di save
                    }
                }
            }
        }
        
        // Auto Save
        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (pathbox.Text.Length != 0)
                {
                    if (Autosavestate)
                    {
                        Autosavestate = false;
                        button3.Text = "Autosave";
                    }
                    else
                    {
                        Autosavestate = true;
                        button3.Text = "Stop Autosave";
                    }

                }
                else
                    MessageBox.Show("File Path Empty");
            }
            else
                MessageBox.Show("Serial Port not Connected");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("1");
        }
    }
}
