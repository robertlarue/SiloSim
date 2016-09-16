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
    class PLC
    {
        public static BitArray inputs = new BitArray(new bool[] {false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false });
        public static BitArray outputs = new BitArray(new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false });
        public static int[] binInventory = { 0, 0, 0, 0, 0 };
        public static int port = 11001;
        public static TcpListener plcTcpServer;
        public static List<TcpClient> plcClients = new List<TcpClient>();

        delegate void IncrementBinInventoryCallback(int[] binInventory);

        public static void ListenClientThread(object obj)
        {
            int port = (int)obj;
            IPEndPoint plcEndpoint = new IPEndPoint(IPAddress.Any, port);
            plcTcpServer = new TcpListener(plcEndpoint);
            TcpClient plcClient = default(TcpClient);
            try {
                plcTcpServer.Start();
                while (true)
                {
                    plcClient = plcTcpServer.AcceptTcpClient();
                    plcHandleClinet plcHc = new plcHandleClinet();
                    plcHc.startClient(plcClient);
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
            plcClients.Add(client);
            while (client.Connected)
            {
                if ((client.Client.Available == 0) && client.Client.Poll(1000, SelectMode.SelectRead))
                {
                    try
                    {
                        Console.WriteLine("PLC client disconnected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                        plcClients.Remove(client);
                        client.Client.Disconnect(false);
                    }
                    catch
                    {
                        Console.WriteLine("PLC client disconnected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                        plcClients.Remove(client);
                        client.Close();
                    }
                }
                else
                {
                    try {
                        byte[] buffer = new byte[64];
                        client.Client.Receive(buffer, 64, SocketFlags.None);
                        ParseReceive(buffer, client);
                    }
                    catch //(SocketException sockEx)
                    {
                        //Console.WriteLine(sockEx.Message);
                    }
                }
            }
        }
        static void ParseReceive(byte[] buffer,TcpClient client)
        {
            try {
                string receiveString = ByteArrayToString(buffer);
                if (receiveString.Substring(0, 4) == "1006" || receiveString.Substring(0, 4) == "0000")
                {
                    Console.WriteLine("Received: 1006");
                    return;
                }

                Console.WriteLine("Received: " + receiveString.Substring(0, receiveString.IndexOf("1003") + 8));
                //string testString = receiveString.Substring(4, receiveString.IndexOf("1003") - 4);
                //Console.WriteLine("CRC test bytes = " + testString + "03");
                //byte[] testBytes = StringToByteArray(testString + "03");
                //byte[] crcBytes = BitConverter.GetBytes(Crc16.ComputeChecksum(testBytes));
                //string crctest = BitConverter.ToString(crcBytes).Replace("-", "").ToLower();
                //Console.WriteLine("crc check = " + crctest);
                string message = receiveString.Substring(0, 4);
                string destination = receiveString.Substring(4, 2);
                string source = receiveString.Substring(6, 2);
                string command = receiveString.Substring(8, 2);
                string sts = receiveString.Substring(10, 2);
                string tns = receiveString.Substring(12, 4);
                string dataFunction = receiveString.Substring(16, 2);
                string dataPayload = receiveString.Substring(18, receiveString.IndexOf("1003") - 18);

                switch (message) {
                    case "1002":
                        //different command types
                        switch (command)
                        {
                            //settings commands
                            case "06":
                                switch (dataFunction)
                                {
                                    case "00":
                                        SendResponse(client, sts + tns + dataPayload, "00", "01", "46");
                                        break;
                                    case "01":
                                        string size = receiveString.Substring(receiveString.IndexOf("1003") - 2, 2);
                                        SendResponse(client, sts + tns + "0000000000000000000000000000000000000000000000000000", "00", "01", "46");
                                        break;
                                    case "03":
                                        SendResponse(client, sts + tns + "00ee349c23313736332d4c4543202020000026006e7a30fc00", "00", "01", "46");
                                        break;
                                }
                                break;
                            //read-write commands
                            case "0f":
                                switch (dataFunction)
                                {
                                    //client read
                                    //see 1170-6.5.15 section 7-17 protected typed logical read with three address fields
                                    case "a2":
                                        switch (dataPayload)
                                        {
                                            //read inputs
                                            //04 07 89 01 00
                                            //2 bytes, file number = 7, type = integer(89), element = 1, subelement = 0
                                            case "0407890100":
                                                string inputString = ConvertInputsToHex();
                                                SendResponse(client, sts + tns + inputString, dataPayload: dataPayload);
                                                break;

                                            //read outputs
                                            //14 07 89 03 00
                                            //14 bytes, file number = 7, type = integer(89), element = 5, subelement = 0
                                            case "0407890300":
                                                string outputString = ConvertOutputsToHex();
                                                SendResponse(client, sts + tns + outputString, dataPayload: dataPayload);
                                                break;

                                            //read inventory
                                            //14 07 89 06 00
                                            //14 bytes, file number = 7, type = integer(89), element = 6, subelement = 0
                                            case "1407890600":
                                                string inventoryString = ConvertInventoryToHex();
                                                SendResponse(client, sts + tns + inventoryString, dataPayload: dataPayload);
                                                break;

                                            //read counters reset
                                            //14 07 89 05 00
                                            //14 bytes, file number = 7, type = integer(89), element = 5, subelement = 0
                                            case "1407890500":
                                                SendResponse(client, sts + tns + "0000000000000000000000000000");
                                                break;
                                        }
                                        break;

                                    //client write 
                                    //see 1770-6.5.15 section 7-18 protected typed logical write with three address fields 
                                    case "aa":
                                        string address = dataPayload.Substring(0, 10);
                                        switch (address)
                                        {
                                            //binary write
                                            //02 03 85 00 00
                                            //2 bytes, file number = 3, type = bit(85), element = 0, subelement = 0
                                            case "0203850000":
                                                string outputString = dataPayload.Substring(10, dataPayload.Length - 10);
                                                SendResponse(client, sts + tns + "");
                                                break;

                                            //integer write (set outputs)
                                            //02 07 89 00 00
                                            //2 bytes, file number = 7, type = integer(89), element = 0, subelement = 0
                                            case "0207890000":
                                                outputString = dataPayload.Substring(10, dataPayload.Length - 10);
                                                SetOutputs(outputString);
                                                SendResponse(client, sts + tns + "");
                                                break;

                                            //integer write (set outputs)
                                            //02 07 89 06 00
                                            //2 bytes, file number = 7, type = integer(89), element = 5, subelement = 0
                                            case "0207890500":
                                                binInventory[0] = 0;
                                                binInventory[1] = 0;
                                                binInventory[2] = 0;
                                                binInventory[3] = 0;
                                                binInventory[4] = 0;
                                                SendResponse(client, sts + tns + "");
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                }
                
            }
            catch //(Exception ex)
            {
                //Console.WriteLine("source: " + ex.Source + " message: " + ex.Message);
            }
        }
        static void SetOutputs(string outputString)
        {
            byte[] outputBytes = StringToByteArray(outputString);
            BitArray outputBits = new BitArray(outputBytes);
            for (int i = 0; i < 14; i++)
            {
                Console.WriteLine("Output " + i + " = " + outputBits[i]);
                outputs[i] = outputBits[i];
            }
        }
        //static void SetInventory(string outputString)
        //{
        //    int bin1level = Convert.ToUInt16(outputString.Substring(0, 2), 16);
        //    int bin2level = Convert.ToUInt16(outputString.Substring(2, 2), 16);
        //    int bin3level = Convert.ToUInt16(outputString.Substring(4, 2), 16);
        //    int bin4level = Convert.ToUInt16(outputString.Substring(6, 2), 16);
        //    //int bin5level = Convert.ToUInt16(outputString.Substring(8, 2), 16);
        //    Console.WriteLine("bin1 = " + bin1level);
        //    Console.WriteLine("bin2 = " + bin2level);
        //    Console.WriteLine("bin3 = " + bin3level);
        //    Console.WriteLine("bin4 = " + bin4level);
        //    //Console.WriteLine("bin5 = " + bin5level);
        //}
        //static void SendStatus(TcpClient client)
        //{
        //    //byte[] buffer = StringToByteArray("1006100200014600121700ee349c23313736332d4c4543202020000026006e7a30fc0010035532");
        //    //client.Client.Send(buffer);
        //}
        //static void SendInputs(TcpClient client)
        //{
        //    //byte[] buffer = StringToByteArray("1006100200014f00a16e006000001003c7b3");
        //    //client.Client.Send(buffer);
        //}
        static void SendResponse(TcpClient client, string response, string src = "00", string dst = "01", string cmd = "4f", string dataPayload = "" )
        {
            string beginSend = "10061002";
            string data = src + dst + cmd + response;
            byte[] crcComputeBytes = StringToByteArray(data + "03");
            byte[] crcBytes = BitConverter.GetBytes(Crc16.ComputeChecksum(crcComputeBytes));
            string crc = BitConverter.ToString(crcBytes).Replace("-", "").ToLower();
            if (dataPayload != "" && dataPayload.Length > 3)
            {
                if (dataPayload.Substring(2, 2) == "07")
                {
                    string modifiedResponse = QuirkFix(response.Substring(6, response.Length - 6));
                    data = src + dst + cmd + response.Substring(0, 6) + modifiedResponse; //weird quirk
                }
            }
            string sendString = beginSend + data + "1003" + crc;
            Console.WriteLine("Sent: " + sendString);
            byte[] buffer = StringToByteArray(sendString);
            client.Client.Send(buffer);
        }
        static string ConvertInputsToHex()
        {
            byte[] inputData = new byte[4];
            inputs.CopyTo(inputData, 0);
            string inputString = BitConverter.ToString(inputData).Replace("-", "").ToLower();
            return inputString;
        }

        static string ConvertOutputsToHex()
        {
            byte[] outputData = new byte[4];
            outputs.CopyTo(outputData, 0);
            string outputString = BitConverter.ToString(outputData).Replace("-", "").ToLower();
            return outputString;
        }

        static string ConvertInventoryToHex()
        {
            int place1 = binInventory[4] % 16;
            int place2 = binInventory[4] % 256;
            int target0 = binInventory[0];
            target0 = ((target0 << 8) | (target0 >> 8)) & 0xFFFF;
            string hexString0 = target0.ToString("X4");
            int target1 = binInventory[1];
            target1 = ((target1 << 8) | (target1 >> 8)) & 0xFFFF;
            string hexString1 = target1.ToString("X4");
            int target2 = binInventory[2];
            target2 = ((target2 << 8) | (target2 >> 8)) & 0xFFFF;
            string hexString2 = target2.ToString("X4");
            int target3 = binInventory[3];
            target3 = ((target3 << 8) | (target3 >> 8)) & 0xFFFF;
            string hexString3 = target3.ToString("X4");
            int target4 = binInventory[4];
            target4 = ((target4 << 8) | (target4 >> 8)) & 0xFFFF;
            string hexString4 = target4.ToString("X4");
            string inventoryString = hexString0 + "0000"+ hexString1 + "0000" + hexString2 + "0000" + hexString3 + "0000" + hexString4 + "0000";                               
            return inventoryString;
        }

        static string QuirkFix(string response)
        {
            string quirkString = response;
            if (response.Length % 2 == 0)
            {
                quirkString = "";
                for (int i = 0; i < (response.Length / 2); i++)
                {
                    string byteString = response.Substring(i * 2, 2);
                    if (byteString == "10")
                    {
                        quirkString = quirkString + byteString;
                    }
                    quirkString = quirkString + byteString;
                }
            }
            return quirkString;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        private static UInt16[] aCRC16Table = {
    0x0,
    0xc0c1,
    0xc181,
    0x140,
    0xc301,
    0x3c0,
    0x280,
    0xc241,
    0xc601,
    0x6c0,
    0x780,
    0xc741,
    0x500,
    0xc5c1,
    0xc481,
    0x440,
    0xcc01,
    0xcc0,
    0xd80,
    0xcd41,
    0xf00,
    0xcfc1,
    0xce81,
    0xe40,
    0xa00,
    0xcac1,
    0xcb81,
    0xb40,
    0xc901,
    0x9c0,
    0x880,
    0xc841,
    0xd801,
    0x18c0,
    0x1980,
    0xd941,
    0x1b00,
    0xdbc1,
    0xda81,
    0x1a40,
    0x1e00,
    0xdec1,
    0xdf81,
    0x1f40,
    0xdd01,
    0x1dc0,
    0x1c80,
    0xdc41,
    0x1400,
    0xd4c1,
    0xd581,
    0x1540,
    0xd701,
    0x17c0,
    0x1680,
    0xd641,
    0xd201,
    0x12c0,
    0x1380,
    0xd341,
    0x1100,
    0xd1c1,
    0xd081,
    0x1040,
    0xf001,
    0x30c0,
    0x3180,
    0xf141,
    0x3300,
    0xf3c1,
    0xf281,
    0x3240,
    0x3600,
    0xf6c1,
    0xf781,
    0x3740,
    0xf501,
    0x35c0,
    0x3480,
    0xf441,
    0x3c00,
    0xfcc1,
    0xfd81,
    0x3d40,
    0xff01,
    0x3fc0,
    0x3e80,
    0xfe41,
    0xfa01,
    0x3ac0,
    0x3b80,
    0xfb41,
    0x3900,
    0xf9c1,
    0xf881,
    0x3840,
    0x2800,
    0xe8c1,
    0xe981,
    0x2940,
    0xeb01,
    0x2bc0,
    0x2a80,
    0xea41,
    0xee01,
    0x2ec0,
    0x2f80,
    0xef41,
    0x2d00,
    0xedc1,
    0xec81,
    0x2c40,
    0xe401,
    0x24c0,
    0x2580,
    0xe541,
    0x2700,
    0xe7c1,
    0xe681,
    0x2640,
    0x2200,
    0xe2c1,
    0xe381,
    0x2340,
    0xe101,
    0x21c0,
    0x2080,
    0xe041,
    0xa001,
    0x60c0,
    0x6180,
    0xa141,
    0x6300,
    0xa3c1,
    0xa281,
    0x6240,
    0x6600,
    0xa6c1,
    0xa781,
    0x6740,
    0xa501,
    0x65c0,
    0x6480,
    0xa441,
    0x6c00,
    0xacc1,
    0xad81,
    0x6d40,
    0xaf01,
    0x6fc0,
    0x6e80,
    0xae41,
    0xaa01,
    0x6ac0,
    0x6b80,
    0xab41,
    0x6900,
    0xa9c1,
    0xa881,
    0x6840,
    0x7800,
    0xb8c1,
    0xb981,
    0x7940,
    0xbb01,
    0x7bc0,
    0x7a80,
    0xba41,
    0xbe01,
    0x7ec0,
    0x7f80,
    0xbf41,
    0x7d00,
    0xbdc1,
    0xbc81,
    0x7c40,
    0xb401,
    0x74c0,
    0x7580,
    0xb541,
    0x7700,
    0xb7c1,
    0xb681,
    0x7640,
    0x7200,
    0xb2c1,
    0xb381,
    0x7340,
    0xb101,
    0x71c0,
    0x7080,
    0xb041,
    0x5000,
    0x90c1,
    0x9181,
    0x5140,
    0x9301,
    0x53c0,
    0x5280,
    0x9241,
    0x9601,
    0x56c0,
    0x5780,
    0x9741,
    0x5500,
    0x95c1,
    0x9481,
    0x5440,
    0x9c01,
    0x5cc0,
    0x5d80,
    0x9d41,
    0x5f00,
    0x9fc1,
    0x9e81,
    0x5e40,
    0x5a00,
    0x9ac1,
    0x9b81,
    0x5b40,
    0x9901,
    0x59c0,
    0x5880,
    0x9841,
    0x8801,
    0x48c0,
    0x4980,
    0x8941,
    0x4b00,
    0x8bc1,
    0x8a81,
    0x4a40,
    0x4e00,
    0x8ec1,
    0x8f81,
    0x4f40,
    0x8d01,
    0x4dc0,
    0x4c80,
    0x8c41,
    0x4400,
    0x84c1,
    0x8581,
    0x4540,
    0x8701,
    0x47c0,
    0x4680,
    0x8641,
    0x8201,
    0x42c0,
    0x4380,
    0x8341,
    0x4100,
    0x81c1,
    0x8081,
    0x4040

};
        //*********************************
        //* This CRC uses a table lookup
        //* algorithm for faster computing
        //*********************************
        private static string CalculateCRC16(byte[] DataInput)
        {
            int iCRC = 0x00;
            int bytT = 0x00;


            for (int i = 0; i <= DataInput.Length - 1; i++)
            {
                byte crcConst = 0xff;
                bytT = (iCRC & crcConst) ^ DataInput[i];
                iCRC = (iCRC >> 8) ^ aCRC16Table[bytT];
            }

            //***must do one more with ETX char
            bytT = (iCRC & 0xff) ^ 3;
            iCRC = (iCRC >> 8) ^ aCRC16Table[bytT];

            byte[] bytes = BitConverter.GetBytes((short)iCRC);
            Array.Reverse(bytes);
            string crcString = ByteArrayToString(bytes);
            return crcString;
        }

    }
    public static class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)((crc) ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
    public class plcHandleClinet
    {
        public void startClient(TcpClient client)
        {
            Thread ctThread = new Thread(PLC.AcceptClientThread);
            Console.WriteLine("PLC client connected from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
            ctThread.Start(client);
        }
    }
}
