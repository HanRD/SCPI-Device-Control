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
        private List<int> xaxis;
        private List<int> yaxis;
        private int deviceStatus=0;
        private bool start = false;
        public Form1()
        {
            xaxis = new List<int>();
            for (int i = 0; i < 600; i++)
            {
                xaxis.Add(i);
            }
            InitializeComponent();
            
            
        }

        private void synctrace() {
            while (true) {
                DoIntrumentIO();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread syncthread = new Thread(new ThreadStart(synctrace));
            syncthread.IsBackground = true;
            Control.CheckForIllegalCrossThreadCalls = false;
            syncthread.Start();
        }
        private void FindAvailableInstrument()
        {
            Ivi.Visa.Interop.ResourceManagerClass rm = new Ivi.Visa.Interop.ResourceManagerClass();
            Ivi.Visa.Interop.FormattedIO488Class ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            try
            {
                object[] idnItems;
                string[] availabledevice;
                availabledevice = (string[])rm.FindRsrc("USB?*INSTR");
            }
            catch (Exception e)
            {

                throw;
            }
        }
        private void DoIntrumentIO() {
            Ivi.Visa.Interop.ResourceManagerClass rm = new Ivi.Visa.Interop.ResourceManagerClass();
            Ivi.Visa.Interop.FormattedIO488Class ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            try
            {
                
                yaxis = new List<int>();
                
                string idnItems;
                string[] availabledevice;
                availabledevice = (string[])rm.FindRsrc("USB?*INSTR");
                
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open("USB0::6833::1416::DS1K00005888::0::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.SetBufferSize(Ivi.Visa.Interop.BufferMask.IO_OUT_BUF,1024);
                ioobj.WriteString(":WAVeform:DATA? CHANnel2", true);

                idnItems = ioobj.ReadString();
                char[] charlist = idnItems.ToCharArray();
                for (int i = 0; i < 600; i++) {
                    
                    yaxis.Add((int)charlist[i]);
                }
                chart1.Series[0].Points.DataBindXY(xaxis, yaxis);
                Console.WriteLine("Finished!");

                
            }
            catch (Exception e)
            {
                System.Console.WriteLine("An error occurred: " + e.Message);
            }
            finally {
                try {
                    Console.WriteLine("Session Ended");
                    ioobj.IO.Close();
                    
                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ioobj);
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
    }
}
