using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using CourseProject.ViewModel;

namespace CourseProject.ProgramContent.Server
{
    class Client : IDisposable
    {
        private Dictionary<int, TcpClient> threads;
        private ChatRoomViewModel chatRoomVm;

        public Client(string ip, int remotePort, int uniqueID, ChatRoomViewModel chatRoomVm)
        {
            this.chatRoomVm = chatRoomVm;
            threads = new Dictionary<int, TcpClient>()
            {
                { uniqueID + 0, new TcpClient() },
                { uniqueID + 1, new TcpClient() },
                { uniqueID + 2, new TcpClient() },
            };
            ConnectToServer(ip, remotePort);
        }

        private void ConnectToServer(string ip, int remotePort)
        {
            foreach (var value in threads)
            {
                value.Value.Connect(IPAddress.Parse(ip), remotePort);
                byte[] data = BitConverter.GetBytes(value.Key);
                value.Value.GetStream().Write(data, 0, data.Length);
                Task.Run(() => GetData(value.Value));
            }
        }

        public void SendData(byte[] data, byte operation, int uniqueID)
        {
            try
            {
                data = CollectionConversion.AddToEndArray(CollectionConversion.AddToEndArray(data, new byte[] { operation }), BitConverter.GetBytes(uniqueID));
                threads[uniqueID].GetStream().Write(data, 0, data.Length);
            }
            catch
            {

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
                        DataConversion(ms.ToArray());
                    }
                }
            }
            catch
            {
                client.Close();
                chatRoomVm.DataProcessing(0, 3, null);
            }
        }

        public void Dispose()
        {
            foreach (var value in threads)
            {
                if (value.Value.Connected)
                {
                    value.Value.GetStream().Close();
                    value.Value.Close();
                }
            }
            threads.Clear();
        }

        private void DataConversion(byte[] source)
        {
            var (userID, operation, data) = CollectionConversion.GetSenderInformation(source);
            chatRoomVm.DataProcessing(BitConverter.ToInt32(userID, 0), operation, data);
        }
    }
}
