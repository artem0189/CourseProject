using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseProject.Model;
using CourseProject.ProgramContent;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace CourseProject.ProgramContent.Server
{
    class Server : IDisposable
    {
        private bool isRunning;
        private TcpListener server;
        private List<UserModel> clientInfo = new List<UserModel>();
        private Dictionary<TcpClient, int> clients = new Dictionary<TcpClient, int>();
        public Server(string ip, int localPort)
        {
            server = new TcpListener(IPAddress.Parse(ip), localPort);
            server.Start();
            isRunning = true;
            Task.Run(() => GetConnection(server));
        }

        private void GetConnection(TcpListener server)
        {
            try
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    byte[] data = new byte[4];
                    int bytes = client.GetStream().Read(data, 0, data.Length);
                    if (BitConverter.ToInt32(data, 0) != 3)
                    {
                        clients.Add(client, BitConverter.ToInt32(data, 0));
                        Task.Run(() => GetData(client));
                    }
                    else
                    {
                        client.Close();
                    }
                }
            }
            catch
            {
                server.Stop();
            }
        }

        private void SendData(TcpClient client, byte[] data)
        {
            if (clients.ContainsKey(client))
            {
                byte threadType = (byte)(clients[client] & 3);
                if (threadType == 0)
                {
                    DataProcess(client, data);
                }
                foreach (var value in clients)
                {
                    if (value.Key != client && (value.Value & 3) == threadType)
                    {
                        if (value.Key.Connected)
                        {
                            NetworkStream stream = value.Key.GetStream();
                            stream.Write(data, 0, data.Length);
                        }
                    }
                }
            }
        }

        private void GetData(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int bytes = 0;
                        byte[] data = new byte[65536];
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            ms.Write(data, 0, bytes);
                        }
                        while (stream.DataAvailable);
                        SendData(client, ms.ToArray());
                    }
                }
            }
            catch
            {
                if (isRunning)
                {
                    if (client.Connected)
                    {
                        clientInfo.Remove(clientInfo.Find(obj => obj.ID == (int)(clients[client] & 0xFFFFFFFC)));
                        SendData(client, CollectionConversion.AddToEndArray(CollectionConversion.AddToEndArray(ConvertClass.ObjectToByteArray(clientInfo), new byte[] { 4 }), new byte[4]));
                        clients.Remove(client);
                        client.Close();
                    }
                }
            }
        }

        private void DataProcess(TcpClient client, byte[] source)
        {
            var (userID, operation, data) = CollectionConversion.GetSenderInformation(source);
            switch (operation)
            {
                case 0:
                    var listData = CollectionConversion.AddToEndArray(CollectionConversion.AddToEndArray(ConvertClass.ObjectToByteArray(clientInfo), new byte[]{ 1 }), userID);
                    client.GetStream().Write(listData, 0, listData.Length);
                    clientInfo.Add(ConvertClass.ByteArrayToObject(data) as UserModel);
                    break;
                case 2:
                    UserModel user = clientInfo.Find(obj => obj.ID == BitConverter.ToInt32(userID, 0));
                    PropertyInfo property = ConvertClass.ByteArrayToObject(data) as PropertyInfo;
                    property.SetValue(user, !(bool)property.GetValue(user));
                    break;
            }
        }

        private byte[] UnionArrays(byte[] first, byte[] second)
        {
            Array.Resize(ref first, first.Length + second.Length);
            for (int i = 0; i < second.Length; i++)
            {
                first[first.Length - second.Length + i] = second[i];
            }
            return first;
        }

        public void Dispose()
        {
            isRunning = false;
            foreach (var value in clients)
            {
                if (value.Key.Connected)
                {
                    value.Key.GetStream().Close();
                    value.Key.Close();
                }
            }
            clients.Clear();
            server.Stop();
        }
    }
}
