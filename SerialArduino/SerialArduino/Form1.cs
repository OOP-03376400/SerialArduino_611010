using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using ZedGraph;

namespace SerialArduino
{
    public partial class Form1 : Form
    {
        // 1. declare delegate
        public delegate void AddDataDelegate(string str);

        // 2. define delegate
        public AddDataDelegate myDelegate;

        public Form1()
        {
            InitializeComponent();
        // 3. create delegate object
            myDelegate = new AddDataDelegate(AddData);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string str = sp.ReadExisting();
            Console.WriteLine(str);
            // 4. Invoke delegate
            textBox1.Invoke(myDelegate, str);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            listBox1.Items.Clear();
            listBox1.Items.AddRange(ports);
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen == true)
            {
                return;
            }
            else
            {
                serialPort1.Open();
                button2.Enabled = false;
                button3.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                return;
            }
            else
            {
                serialPort1.Close();
                button2.Enabled = true;
                button3.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.Text;
            Console.WriteLine(serialPort1.PortName);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.BaudRate =  Int32.Parse(comboBox2.Text);

        }

        public void AddData(string str)
        {
            textBox1.AppendText(str);   
        }

        GraphPane myPane;
        RollingPointPairList points;
        LineItem myCurve;

        private void Form1_Load(object sender, EventArgs e)
        {
            zedGraphControl1.IsShowHScrollBar = true;
            zedGraphControl1.IsAutoScrollRange = true;

            myPane = zedGraphControl1.GraphPane;

            myPane.YAxis.Scale.Min = -1.2;
            myPane.YAxis.Scale.Max = 1.2;
            myPane.YAxis.Scale.MajorStep = 1;
            myPane.YAxis.Scale.MinorStep = 1;

            myPane.XAxis.Scale.MajorStep = 1000;
            myPane.XAxis.Scale.MinorStep = 1000;
            myPane.XAxis.Scale.Format = "#";
            myPane.XAxis.Scale.Mag = 0;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 5000;

            points = new RollingPointPairList(15000);
            myCurve = myPane.AddCurve("Sine wave", points, Color.Blue, SymbolType.None);
        }

        private void graphUpdate_Tick(object sender, EventArgs e)
        {
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
    }
}
