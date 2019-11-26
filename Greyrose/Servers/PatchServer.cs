using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace WizPS
{
    partial class Server
    {

        private static class PS_Settings
        {
            public static int TCPPort = 12500;
            public static int UDPPort = 0;
            public static string Key = "blob";
            public static IPAddress IP = IPAddress.Parse("0.0.0.0");
            public static bool SocketOpen = false;
            public static bool SessionOpen = false;
            public static ushort SessionID = 4513;

        }

        public static async Task PS()
        {
            TcpListener server = null;
            try
            {

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(PS_Settings.IP, PS_Settings.TCPPort);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data. 4096 bytes should be fine
                Byte[] bytes = new Byte[4096];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Patch: Waiting for a connection...\n");
                    PS_Settings.SocketOpen = false;
                    PS_Settings.SessionOpen = false;

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Patch: Connected!\n");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {

                        try
                        {
                            Tuple<byte[], int,int> temptuple = MessageParser.Parse(bytes,PS_Settings.TCPPort, PS_Settings.UDPPort, PS_Settings.Key, PS_Settings.SessionOpen, PS_Settings.SessionID);
                            if (temptuple == null)
                                continue;
                            byte[] response = temptuple.Item1;
                            int requestlogin = temptuple.Item2;
                            int SessionOpen = temptuple.Item3;
                            if (response != null)
                            {
                                stream.Write(response, 0, response.Length);
                                if (requestlogin == 1)
                                {
                                    KIPacket RealResponse = new KIPacket(); //Initialise new packet
                                    RealResponse.Header(0x00, 0x00, 0x07, 0x03); //Create header with SVCID 7 and MSGID 3 (MSG_CHARACTERSELECTED)
                                    RealResponse._STR("127.0.0.1"); //IP
                                    RealResponse._INT(PS_Settings.TCPPort); //TCPPORT
                                    RealResponse._INT(PS_Settings.UDPPort); //UDPPORT
                                    RealResponse._STR(PS_Settings.Key); //KEY
                                    RealResponse._GID(4295088136144); //UserID
                                    RealResponse._GID(191965934135706025); //CharID
                                    RealResponse._GID(123004564835992122); //ZoneID
                                    RealResponse._STR("WizardCity/WC_Ravenwood"); //ZoneName
                                    RealResponse._STR("2572,4376,-28,5.55"); //Location
                                    RealResponse._INT(0); //Slot
                                    RealResponse._INT(0); //PrepPhase
                                    RealResponse._INT(0); //Error
                                    RealResponse._STR("WizPS.Patch"); //LoginServer
                                    byte[] packet2 = RealResponse.Finalise();
                                    stream.Write(packet2, 0, packet2.Length);
                                }
                                if (SessionOpen == 1)
                                    PS_Settings.SessionOpen = true;
                            }
                        }
                        catch //If the data couldn't be parsed, write the data packet
                        {
                            for (int q = 0; q < bytes.Length; q++)
                            {
                                data = data + bytes[q].ToString("X2");
                            }
                            Console.WriteLine("Patch: Received: {0}\n", data);
                        }

                    }
                    Console.WriteLine("Patch: NO DATA RECEIVED!\n");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Patch: SocketException: {0}\n", e);
            }
            finally
            {
                // Stop listening for new clients.
                Console.WriteLine("Patch: SERVER STOP\n");
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
            return;
        }

    }
}
