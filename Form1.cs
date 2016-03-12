﻿using System;
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

        }

        //Jika Sesi Port Serial Terbuka
        //class ini aktif

        // Deklarasi Variable Global
        string RAWData; //RawData = Data Dari Arduino
        string[] Data; //RAWData yang jadi Array biar bisa di split
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RAWData = serialPort1.ReadExisting();
            Data = Regex.Split(RAWData, " "); //Memisah RAWData berdasarkan Spasi, diperlukan library using System.Text.RegularExpressions;. 

            this.Invoke(new EventHandler(tampildata));
        }

        // Buat Class Tampil Data

        private void tampildata(object sender, EventArgs e)
        {
            richTextBox1.Text = RAWData;
        }
    }
}