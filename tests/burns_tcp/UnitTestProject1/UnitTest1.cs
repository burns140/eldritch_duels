using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using tcpTest;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {

        

        [TestMethod]
        public void getCollectionTestArray() {
            Thread.Sleep(5000);
            //UserInfo info = new UserInfo(id, token, "getCollection");
            UserInfo info = new UserInfo("id", "token", "getCollection");
            string json = JsonConvert.SerializeObject(info);

            Int32 port = 8000;
            TcpClient client = new TcpClient("localhost", port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            data = new Byte[256];
            string responseData = string.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            Thread.Sleep(2500);
            Debug.WriteLine(responseData);
        }
    }
}
