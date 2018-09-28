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
using WebSocketSharp;
using Newtonsoft.Json;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Ivi.Visa.Interop.ResourceManagerClass rm;
        Ivi.Visa.Interop.FormattedIO488Class ioobj;
        private List<int> xaxis;
        private List<int> yaxis;
        private int deviceStatus = 0;
        private bool start = false;
        [Serializable()]
        private class testObj {
            public string avatar { get; set; }
            public string[] group { get; set; }
            public string id { get; set; }
            public string sign { get; set; }
            public string status { get; set; }
            public string token { get; set; }
            public string type { get; set; }
            public string username { get; set; }
        }
        private string _selecteddeviceaddress;
        private List<string> selectabledevicelist=new List<string>();
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
            InitSocketListener();
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


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        WebSocket ws;
        string _url = "ws://kx-dev.woquxiu.cn:7272/?token=wDHRdV9yAUOXALTD3Z_f5Bv4M-Gd6s1nb5u9LHNkiJY";
        private void Form1_Load(object sender, EventArgs e)
        {
            ws = new WebSocket(_url);
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;

            ws.Connect();

        }
        private void Ws_OnOpen(object sender, EventArgs e) {
            Console.WriteLine("Connection opened");
        }
        private void Ws_OnMessage(object sender, MessageEventArgs e) {
            Console.WriteLine("Received Message,content:" + e.Data);
            if (e.Data.StartsWith("{\"type\":\"ping"))
            {
                ws.Send("{\"type\":\"pong\"}");
            }
        }
        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Connection error");
        }
        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Connection closed");
        }
        private void InitSocketListener()
        {
            testObj msgcontent = new testObj();
            msgcontent.avatar = "https://thirdqq.qlogo.cn/qqapp/101343857/A72CD6FE0C366BADFD6095BBE6A3D7D9/100";
            msgcontent.group = new string[] { "qun_1", "qun_2", "qun_3" };
            msgcontent.id = "usr_4";
            msgcontent.sign = "啊啊啊啊啊啊啊啊啊啊啊啊啊啊";
            msgcontent.status = "online";
            msgcontent.token = "wDHRdV9yAUOXALTD3Z_f5Bv4M-Gd6s1nb5u9LHNkiJY";
            msgcontent.type = "login";
            msgcontent.username = "李四";
            string msgstring = JsonConvert.SerializeObject(msgcontent);
            Console.WriteLine(msgstring);
            ws.Send(msgstring);
        }

        //初始化设备
        private void initInstrument() {

        }

        //配置测试参数
        private void configureParameters()
        {

        }

        //执行测试
        private void PerformReadingMeasurement(){

        }

        //执行波形测试
        private void PerformWaveformMeasurement() {

        }

        //搜索可用设备
        private List<string> SearchConnectedDevice(string port) {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            selectabledevicelist = new List<string>();
            string[] availabledevice;
            string[] deviceInfList;
            try
            {
                availabledevice = (string[])rm.FindRsrc(port+"?*INSTR");
                if (availabledevice.Length > 0)
                {
                    foreach (string addr in availabledevice) {
                    try
                    {
                            ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(addr, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");
                            ioobj.WriteString("*IDN?");
                            deviceInfList=(string[])ioobj.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                            string deviceinf = deviceInfList[0] + "," + deviceInfList[1];
                            selectabledevicelist.Add(deviceinf);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    }
                }
                else {
                    //备选设备列表置空,提示未搜索到设备

                }
                return selectabledevicelist;
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                {
                    try
                    {
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
            
        }
        private void SendSCPIcommand(string str) {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            try
            {
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(_selecteddeviceaddress, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.WriteString(str, true);

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
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

        private string SendSCPICommandAndReadString(string str) {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            string result;
            try
            {
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(_selecteddeviceaddress, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.WriteString(str, true);
                result = ioobj.ReadString();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                result = "error";
                return result;
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

        private void SendSCPICommandAndReadTrace(string str) {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            string result;
            try
            {
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(_selecteddeviceaddress, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.WriteString(str, true);
                result = ioobj.ReadString();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                result = "error";
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
    }
}
