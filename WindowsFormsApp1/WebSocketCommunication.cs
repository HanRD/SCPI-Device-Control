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
        [Serializable()]
        private class IMaccountObj
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
        WebSocket ws;
        string _url = "ws://kx-dev.woquxiu.cn:7272/?token=wDHRdV9yAUOXALTD3Z_f5Bv4M-Gd6s1nb5u9LHNkiJY";
        private void InitWebsocketSession(object sender, EventArgs e)
        {
            ws = new WebSocket(_url);
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;

            ws.Connect();

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
    }
}
