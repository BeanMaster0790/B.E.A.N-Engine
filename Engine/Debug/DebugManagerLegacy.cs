//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace BeanMachine.Debug
//{
//    public class DebugManagerLegacy
//    {

//        public static DebugManagerLegacy Instance = new DebugManagerLegacy();

//        private List<object> _logs = new List<object>();

//        private List<string> _logsTime = new List<string>();

//        private Dictionary<string, object> _trackedObjects = new Dictionary<string, object>();

//        public void Log(object obj)
//        {
//            this._logs.Add(obj);
//            this._logsTime.Add(string.Format("{0} : {1} : {2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
//        }

//        public void Monitor(object obj, string name)
//        {
//            foreach (KeyValuePair<string, object> i in this._trackedObjects)
//            {
//                if (i.Key == name)
//                {
//                    this._trackedObjects.Remove(i.Key);
//                }
//            }

//            this._trackedObjects.Add(name, obj);
//        }

//        private string ParseDebugInfoToJson()
//        {
//            DebugFile file = new DebugFile();

//            file.LoggedValues = new List<string>();
//            file.LoggedValuesTimes = new List<string>();

//            file.MonitoredValues = new List<string>();
//            file.MonitoredValuesNames = new List<string>();

//            foreach (KeyValuePair<string, object> pair in this._trackedObjects)
//            {
//                file.MonitoredValues.Add(pair.Value.ToString());
//                file.MonitoredValuesNames.Add(pair.Key.ToString());
//            }

//            if (this._logs.Count > 1000)
//            {
//                this._logs.RemoveAt(0);
//                this._logsTime.RemoveAt(0);
//            }

//            foreach (object obj in this._logs.ToArray())
//            {

//                file.LoggedValues.Add(obj.ToString());
//                this._logs.Remove(obj);
//            }

//            foreach (string obj in this._logsTime.ToArray())
//            {
//                file.LoggedValuesTimes.Add(obj.ToString());
//                this._logsTime.Remove(obj);
//            }

//            Console.WriteLine(JsonConvert.SerializeObject(file));
//            return JsonConvert.SerializeObject(file);
//        }

//        public void SendDataToDebugConsole()
//        {
//            try
//            {
//                IPAddress ipaddress = IPAddress.Parse("127.0.0.1");

//                TcpListener mylist = new TcpListener(ipaddress, 8000);
//                mylist.Start();

//                Socket socket = mylist.AcceptSocket();

//                while (true)
//                {
//                    try
//                    {
//                        byte[] bytes = new byte[100000];
//                        int k = socket.Receive(bytes);

//                        string request = "";
//                        string responce = "";

//                        for (int i = 0; i < k; i++)
//                        {
//                            request += Convert.ToChar(bytes[i]);
//                        }

//                        Console.WriteLine(request);

//                        if (request == "DebugInfo")
//                        {
//                            responce = ParseDebugInfoToJson();
//                        }

//                        if (request == string.Empty)
//                        {
//                            throw new Exception("No Request");
//                        }

//                        ASCIIEncoding asencd = new ASCIIEncoding();
//                        socket.Send(asencd.GetBytes(responce));

//                    }
//                    catch (Exception)
//                    {
//                        mylist.Stop();
//                        socket.Close();

//                        SendDataToDebugConsole();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error.." + ex.StackTrace);
//            }

//        }

//    }
//}
