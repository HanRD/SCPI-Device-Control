using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    class WebSocketCommunication
    {
        private Form1 form;
        private SCPIConf scpiobj=new SCPIConf();
        string device;

        [Serializable()]
        public class socketMSGobj
        {
            public string type = "chatmessage";
            public socketMSGdata data { get; set; }
        }

        [Serializable()]
        public class socketMSGdata {
            public mineobj mine { get; set; }
            public toobj to { get; set; }
        }

        [Serializable()]
        public class mineobj
        {
            public string avatar { get; set; }
            public string content { get; set; }
            public string id { get; set; }
            public bool mine = true;
            public string username  { get; set; }
    }
        [Serializable()]
        public class toobj
        {
            public string avatar { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string sign { get; set; }
            public string status { get; set; }
            public string type = "friend";
            public string username { get; set; }
        }

        [Serializable()]
        public class IMaccountObj
        {
            public string avatar { get; set; }
            public string[] group { get; set; }
            public string id { get; set; }
            public string sign { get; set; }
            public string status { get; set; }
            public string token { get; set; }
            public string type { get; set; }
            public string username { get; set; }
        }

        [Serializable()]
        public class MessageObj {
            public string type { get; set; }
            public MessageData data { get; set; }
        }

        [Serializable()]
        public class MessageData {
            public string username { get; set; }
            public string avatar { get; set; }
            public string id { get; set; }
            public string type { get; set; }
            public MessageContent content { get; set; }
            public string cid { get; set; }
            public string fromid { get; set; }
            public string timestamp { get; set; }
        }

        [Serializable()]
        public class MessageContent {
            public string frequency { get; set; }
            public string frequency_unit { get; set; }
            public string reading { get; set; }
            public string reading_unit { get; set; }
            public string cal_mode { get; set; }
            public string offset { get; set; }
        }
        WebSocket ws;
        string _url = "ws://kx-dev.woquxiu.cn:7272/?token=wDHRdV9yAUOXALTD3Z_f5Bv4M-Gd6s1nb5u9LHNkiJY";
        public void InitWebsocketSession()
        {
            ws = new WebSocket(_url);
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;

            ws.Connect();
            InitSocketListener();

        }
        private void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Connection opened");
        }
        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received Message,content:" + e.Data);
            if (e.Data.StartsWith("{\"type\":\"ping"))
            {
                ws.Send("{\"type\":\"pong\"}");
            }
            else if (e.Data.StartsWith("{\"type\":\"equip")) {
            var data=e.Data;//message data
            MessageObj datacontent = JsonConvert.DeserializeObject<MessageObj>(data);
            if (datacontent.type=="equip")
            {
                handleMsg(datacontent);
            }
            else {
            } }

        }
        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Connection error");
        }
        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Connection closed");
        }

        public void InitSocketListener()
        {
            IMaccountObj msgcontent = new IMaccountObj();
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
        private void handleMsg(MessageObj obj) {
            string command;
            
            string reading;
            switch (obj.data.type)
            {
                case "freqset":
                    command = "SENS1:FREQ " + obj.data.content.frequency + obj.data.content.frequency_unit;
                    scpiobj.SendSCPIcommand(command, device);
                    getFullData(device);
                    break;

                case "unitset":
                    command = "UNIT:POW ";
                    command += obj.data.content.reading_unit == "W" ? "DBM" : "W";
                    getFullData(device);
                    break;

                case "meas":
                    getFullData(device);
                    break;

                case "cal":
                    device = form.getSelectedDevice();
                    scpiobj.SendSCPIcommand("CAL1:ALL?", device);
                    break;
                default:
                    break;
            }
        }
        public void getFullData(string device) {
            string frequency = scpiobj.SendSCPICommandAndReadString("SENS1:FREQ?", device);
            string reading=scpiobj.SendSCPICommandAndReadString("FETC1?", device);
            string reading_unit=scpiobj.SendSCPICommandAndReadString("UNIT:POW?", device);
            MessageObj obj = new MessageObj();
            long currentTicks = DateTime.Now.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long currentMillis = (currentTicks - dtFrom.Ticks) / 10000;
            obj.type = "chatmessage";
            obj.data = new MessageData();
            obj.data.content = new MessageContent();
            obj.data.username = "李四";
            obj.data.avatar = "https://thirdqq.qlogo.cn/qqapp/101343857/A72CD6FE0C366BADFD6095BBE6A3D7D9/100";
            obj.data.id = "usr_4";
            obj.data.fromid = "usr_4";
            obj.data.content.frequency = formatReturnedFrequency(frequency)[0];
            obj.data.content.frequency_unit = formatReturnedFrequency(frequency)[1];


            if (reading_unit == "DBM\n") {
                obj.data.content.reading = Convert.ToSingle(reading).ToString("f2");
                obj.data.content.reading_unit = "dBm";
            }
            else {

                obj.data.content.reading = formatReturnedWaltPower(reading)[0];
                obj.data.content.reading_unit= formatReturnedWaltPower(reading)[1];
            }
            obj.data.cid = "defaultcid";
            obj.data.fromid = "defaultfromid";
            obj.data.timestamp = currentMillis.ToString();
            obj.data.type = "result";
            string msgobj = JsonConvert.SerializeObject(obj);
            Console.WriteLine(msgobj);
            sendSocketMsg(msgobj);
        }

        private void sendSocketMsg(string msg) {
            socketMSGobj msgobj = new socketMSGobj();
            msgobj.data = new socketMSGdata();
            msgobj.data.mine = new mineobj();msgobj.data.to = new toobj();
            msgobj.data.mine.avatar = "";
            msgobj.data.mine.content = msg;
            msgobj.data.mine.id = "usr_4";
            msgobj.data.mine.username = "李四";
            msgobj.data.to.avatar = "https://thirdqq.qlogo.cn/qqapp/101343857/6C636D2974C3FA9D7142330E3FA8F9FD/100";
            msgobj.data.to.id = "usr_1";
            msgobj.data.to.name = "刘一";
            msgobj.data.to.status = "offline";
            msgobj.data.to.type = "friend";
            msgobj.data.to.username = "刘一";
            msgobj.data.to.sign = "刘一的签名，系统生成";
            string message = JsonConvert.SerializeObject(msgobj);
            ws.Send(message);
        }

        private string[] formatReturnedFrequency(string reading) {
            string[] reading_arr = reading.Split('E');
            float val = Convert.ToSingle(reading_arr[0]);
            int digit = Convert.ToInt32(reading_arr[1]);
            int level = digit / 3;
            double res = digit % 3;
            string unit = level == 1 ? "kHz" : level == 2 ? "MHz" : level == 3 ? "GHz" : "";
            float frequency_Number = val * Convert.ToInt32(Math.Pow(10, res));
            string[] frequencyArr = new string[2];
            frequencyArr[0] = frequency_Number.ToString("f1");
            frequencyArr[1] = unit;
            return frequencyArr;
        }

        /// <summary>
        /// 处理单位为Walt的功率读数
        /// </summary>
        /// <param name="reading">SCPI指令返回的读数</param>
        /// <returns>String数组,0为读数1为单位</returns>
        private string[] formatReturnedWaltPower(string reading) {
            string[] reading_arr = reading.Split('E');
            float val = Convert.ToSingle(reading_arr[0]);
            int digit = Convert.ToInt32(reading_arr[1]);
            int level;
            double res = digit % 3;
            if (res == 0)
            {
                level = (digit / 3);
            }
            else
            {
                level = (digit / 3) - 1;
                res = res + 3;
            }
            string unit = level == -1 ? "mW" : level == -2 ? "uW" : level == -3 ? "nW" : level == -4 ? "pW" : "";
            float power_Number = val * Convert.ToInt32(Math.Pow(10, res));
            string[] waltPowerArr = new string[2];
            waltPowerArr[0] = power_Number.ToString("f1");
            waltPowerArr[1] = unit;
            return waltPowerArr;
        }

        public void setdeviceaddress(string addr)
        {
            device = addr;
        }
    }
}
