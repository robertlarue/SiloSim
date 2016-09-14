using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SiloSim
{
    class Scale
    {
        public static string scaleData = "";
        public static volatile int scaleWeight = 24000;
        public static int fillIncrement = 20;
        public static int tareWeight = 24000;
        public static int grossWeight = 80000;
        public static int netWeight = 56000;
        public static int port = 10001;
        public static TcpListener scaleTcpServer;
        public static List<TcpClient> scaleClients = new List<TcpClient>();
        public static bool scaleInMotion = false;
        public static Queue<int> prevScaleWeights = new Queue<int>(10);

        public static void ListenClientThread(object obj)
        {
            int port = (int)obj;
            IPEndPoint scaleEndpoint = new IPEndPoint(IPAddress.Any, port);
            scaleTcpServer = new TcpListener(scaleEndpoint);
            TcpClient scaleClient = default(TcpClient);
            try
            {
                scaleTcpServer.Start();
                while (true)
                {
                    scaleClient = scaleTcpServer.AcceptTcpClient();
                    scaleHandleClient scaleHc = new scaleHandleClient();
                    scaleHc.startClient(scaleClient);
                }
            }
            catch //(Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public static void AcceptClientThread(object obj)
        {
            TcpClient client = (TcpClient)obj;
            scaleClients.Add(client);
            while (client.Connected)
            {
                if ((client.Client.Available == 0) && client.Client.Poll(1000, SelectMode.SelectRead))
                {
                    try
                    {
                        Console.WriteLine("Scale client disconnected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                        scaleClients.Remove(client);
                        client.Client.Disconnect(false);
                    }
                    catch
                    {
                        Console.WriteLine("Scale client disconnected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                        scaleClients.Remove(client);
                        client.Close();
                    }
                }
                else
                {
                    WriteScale(client);
                }
            }
        }
        static void WriteScale(object obj)
        {
            var client = (TcpClient)obj;

            try
            {
                string scaleZero = "  ";
                if (scaleWeight == 0)
                {
                    scaleZero = "CZ";
                }
                string motion = "  ";
                if (scaleInMotion)
                {
                    motion = "MO";
                }
                scaleData = string.Format("\r{0,7} LB G {1,2} {2,2}\r", scaleWeight.ToString("D2") , scaleZero, motion);
                client.SendTimeout = 50;
                client.SendBufferSize = 17;
                NetworkStream stream = client.GetStream();
                stream.WriteTimeout = 50;
                StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = false };
                writer.Write(scaleData);
                writer.Flush();
                stream.Flush();
                //writer.Write(scaleData);
                //writer.Flush();
                //stream.Flush();
                Thread.Sleep(50);
            }
            catch //(IOException ex)
            {
                //Console.WriteLine("IOException: " + ex.Message);
            }
        }
    }
    public class scaleHandleClient
    {
        public void startClient(TcpClient client)
        {
            Thread ctThread = new Thread(Scale.AcceptClientThread);
            Console.WriteLine("Scale client connected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
            ctThread.Start(client);
        }
    }
}