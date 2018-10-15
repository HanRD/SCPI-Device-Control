using Agilent.CommandExpert.ScpiNet.AgE441x_A_09_01;
using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Ivi.Visa.Interop.ResourceManagerClass rm;
        Ivi.Visa.Interop.FormattedIO488Class ioobj;
        SCPIConf scpiconf = new SCPIConf();
        WebSocketCommunication socketconf = new WebSocketCommunication();
        private List<int> xaxis;
        private List<int> yaxis;
        private Dictionary<string, string> deviceDict;
        private string _selecteddevice;
        
        public Form1()
        {
            xaxis = new List<int>();
            for (int i = 0; i < 600; i++)
            {
                xaxis.Add(i);
            }
            socketconf.InitWebsocketSession();
            InitializeComponent();
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

        }


        public string getSelectedDevice() {
            return _selecteddevice;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            //InitSocketListener();
        }
        private void FindAvailableInstrument()
        {
            Ivi.Visa.Interop.ResourceManagerClass rm = new Ivi.Visa.Interop.ResourceManagerClass();
            try
            {
                object[] idnItems;
                string[] availabledevice;
                deviceDict = new Dictionary<string, string>();
                comboBox1.Items.Clear();
                List<string> ConnectedDeviceList = new List<string>();
                availabledevice = (string[])rm.FindRsrc("USB?*INSTR");
                if (availabledevice.Length > 0)
                {
                    foreach (string item in availabledevice)
                    {
                        string devicedetail = scpiconf.SendSCPICommandAndReadString("*IDN?",item);
                        if (devicedetail != "error")
                        {
                            Console.Out.WriteLine(devicedetail);
                            string[] detailtips= devicedetail.Split(',');
                            string devicedesc = detailtips[0] + detailtips[1];
                            if (!deviceDict.ContainsKey(devicedesc))
                            {
                                comboBox1.Items.Add(devicedesc);
                                deviceDict.Add(devicedesc, item);
                            }
                            else {

                            }
                        }
                    }
                    MessageBox.Show("搜索到"+deviceDict.Count+"个可用设备", "搜索到设备");
                }
                else {
                    MessageBox.Show("没有搜索到可用设备,请确认设备连接状态", "未搜索到设备");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("没有搜索到可用设备,请确认设备连接状态", "未搜索到设备");
                Console.WriteLine("An error has occured:"+e.Data);
                throw;
            }
            finally
            {
                try
                {
                    Console.WriteLine("Session Ended");
                    ioobj.IO.Close();
                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rm);
                }
                catch { }
            }
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FindAvailableInstrument();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selecteddevice = deviceDict[(string)comboBox1.SelectedItem];
            socketconf.setdeviceaddress(_selecteddevice);
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            Console.WriteLine(deviceDict[(string)comboBox1.SelectedItem]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            scpiconf.SendSCPIcommand(textBox1.Text, _selecteddevice);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text=scpiconf.SendSCPICommandAndReadString(textBox1.Text, _selecteddevice);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            socketconf.getFullData(_selecteddevice);
        }
    }
       
}
