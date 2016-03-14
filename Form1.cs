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

//Library Graph
using ZedGraph;


namespace APTRGv2
{
    public partial class Form1 : Form
    {
        //Variable Global Static
        // Untuk Line

        static LineItem curve_ketinggian, curve_temperature, curve_kelembaban, curve_tekanan, curve_arahangin, curve_kecangin, curve_lintang, curve_bujur;
        
        static RollingPointPairList  list_ketinggian, list_temperature, list_kelembaban, list_tekanan, list_arahangin, list_kecangin, list_lintang, list_bujur;

        static double xTimeStamp = 0;

        //int xTimeStamp = 1;
        // Deklarasi Variable Global
        string RAWData; //RawData = Data Dari Arduino
        string[] Data; //RAWData yang jadi Array biar bisa di split

        // Buat Class Tampil Data
        //Deklarasi Data
        string header, waktu, ketinggian, temperature, kelembaban, tekanan, arahangin, kec_angin, lintang, bujur;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            // ZedGraphControl1 = Temperature

            GraphPane Graph_Temperature = zedGraphControl1.GraphPane; // Buat Graph untuk Temperature
            Graph_Temperature.Title.Text = "Temperature";
            Graph_Temperature.XAxis.Title.Text = "Celcius";
            Graph_Temperature.YAxis.Title.Text = "Ketinggian";

            // ZedGraphControl2 = Kelembaban

            GraphPane Graph_Kelembaban = zedGraphControl2.GraphPane; // Buat Graph untuk Kelembaban
            Graph_Kelembaban.Title.Text = "Kelembaban";
            Graph_Kelembaban.XAxis.Title.Text = "%";
            Graph_Kelembaban.YAxis.Title.Text = "Ketinggian";

            // ZedGraphControl3 = Tekanan

            GraphPane Graph_Tekanan = zedGraphControl3.GraphPane; // Buat Graph untuk Tekanan
            Graph_Tekanan.Title.Text = "Tekanan";
            Graph_Tekanan.XAxis.Title.Text = "Pascal";
            Graph_Tekanan.YAxis.Title.Text = "Ketinggian";

            // ZedGraphControl4 = Kecepatan Angin

            GraphPane Graph_Kecepatan = zedGraphControl4.GraphPane; // Buat Graph untuk Kecepatan
            Graph_Kecepatan.Title.Text = "Kecepatan";
            Graph_Kecepatan.XAxis.Title.Text = "KM / Jam";
            Graph_Kecepatan.YAxis.Title.Text = "Ketinggian";

            // Graphic Waktu
            // ZedGraphCOntrol5 = terhadap waktu

            GraphPane Graph_Waktu = zedGraphControl5.GraphPane; // Buat Graph_Waktu
            Graph_Waktu.Title.Text = "Graph Based on Time";
            Graph_Waktu.XAxis.Title.Text = "Value";
            Graph_Waktu.YAxis.Title.Text = "Time";
            //Atur Skala Sumbu X
            Graph_Waktu.XAxis.Scale.Min = 0;
            Graph_Waktu.XAxis.Scale.Max = 30;
            Graph_Waktu.XAxis.Scale.MinorStep = 1;
            Graph_Waktu.XAxis.Scale.MajorStep = 5;
            //Atur Tampilan Stlye
            Graph_Waktu.XAxis.MajorGrid.IsVisible = true; //BUat grid
            Graph_Waktu.YAxis.MajorGrid.IsVisible = true;
            Graph_Waktu.XAxis.MajorGrid.Color = Color.DarkGreen;
            Graph_Waktu.YAxis.MajorGrid.Color = Color.DarkGreen;
            //Atur Warna Graph
            Graph_Waktu.Chart.Fill = new Fill(Color.AliceBlue);
            Graph_Waktu.Fill = new Fill(Color.AntiqueWhite);

            //Atur Spasi
            foreach (ZedGraph.LineItem li in Graph_Waktu.CurveList)
            {
                li.Line.Width = 5;
            }

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
            textBox2.Text = ketinggian + " mdpl";
            textBox3.Text = temperature + " °C";
            textBox4.Text = kelembaban + " %";
            textBox5.Text = tekanan + " Pa";
            textBox6.Text = arahangin + " derajat" ;
            textBox7.Text = kec_angin + " m/s";
            textBox8.Text = lintang + " °";
            textBox9.Text = bujur + " °";

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
