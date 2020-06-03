using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace CourseProject.ProgramContent.Server
{
    class ConnectionTesting
    {
        public static bool TryConnectToServer(string ip, int port)
        {
            bool result = true;

            TcpClient testConnection = new TcpClient();
            try
            {
                testConnection.Connect(ip, port);
                testConnection.GetStream().Write(BitConverter.GetBytes(3), 0, 4);
                testConnection.Close();
            }
            catch
            {
                testConnection.Close();
                result = false;
            }

            return result;
        }

        public static bool TryCreateServer(string ip, int port)
        {
            bool result = false;

            //if ("127.0.0.1" == ip)
            //{
                result = true;
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                foreach (var tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        result = false;
                        break;
                    }
                }
            //}

            return result;
        }
    }
}
