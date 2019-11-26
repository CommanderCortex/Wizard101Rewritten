using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WizPS
{
    partial class Server
    {
        private static class LS_Settings
        {
            public static int TCPPort = 12000;
            public static int UDPPort = 0;
            public static string Key = "blob";
            public static IPAddress IP = IPAddress.Parse("0.0.0.0");
            public static bool SocketOpen = false;
            public static bool SessionOpen = false;
            public static ushort SessionID = 4513;

        }


        public static async Task LS()
        {
            TcpListener server = null;
            try
            {
                // TcpListener server = new TcpListener(port);
                server = new TcpListener(LS_Settings.IP, LS_Settings.TCPPort);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data. 4096 bytes should be fine
                Byte[] bytes = new Byte[4096];

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Login: Waiting for a connection...\n");
                    LS_Settings.SocketOpen = false;
                    LS_Settings.SessionOpen = false;

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Login: Connected!\n");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    Console.WriteLine("LOGIN: AWAITING MESSAGE");

                    int i;

                    try
                    {
                        if (!LS_Settings.SocketOpen)
                            stream.ReadTimeout = 300; //300 ms timeout if the session isn't open
                        else
                            stream.ReadTimeout = 100000; //Timeout of 100000ms if the session is open
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            LS_Settings.SocketOpen = true; //Let the program know that the socket is open (so don't timeout anymore)
                            stream.ReadTimeout = 100000;
                            try
                            {
                                Tuple<byte[], int,int> temptuple = MessageParser.Parse(bytes, LS_Settings.TCPPort, LS_Settings.UDPPort, LS_Settings.Key,LS_Settings.SessionOpen,LS_Settings.SessionID);
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
                                        RealResponse._INT(GS_Settings.TCPPort); //TCPPORT
                                        RealResponse._INT(GS_Settings.UDPPort); //UDPPORT
                                        RealResponse._STR(GS_Settings.Key); //KEY
                                        RealResponse._GID(4295088136144); //UserID
                                        RealResponse._GID(191965934135706025); //CharID
                                        RealResponse._GID(123004564835992122); //ZoneID
                                        RealResponse._STR("WizardCity/WC_Ravenwood"); //ZoneName
                                        RealResponse._STR("2572,4376,-28,5.55"); //Location (X,Y,Z,ROT)
                                        RealResponse._INT(0); //Slot
                                        RealResponse._INT(0); //PrepPhase
                                        RealResponse._INT(0); //Error
                                        RealResponse._STR("WizPS.Login"); //LoginServer
                                        byte[] packet2 = RealResponse.Finalise();
                                        stream.Write(packet2, 0, packet2.Length);
                                    }
                                    if (SessionOpen == 1)
                                        LS_Settings.SessionOpen = true;
                                }
                            }
                            catch //If the data couldn't be parsed, print the data packet
                            {
                                string data = null;

                                for (int q = 0; q < bytes.Length; q++)
                                {
                                    data = data + bytes[q].ToString("X2");
                                }
                                Console.WriteLine("Login: Received: {0}\n", data);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Login: NO DATA RECEIVED!\n");
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Login: SocketException: {0}\n", e);
            }
            finally
            {
                // Stop listening for new clients.
                Console.WriteLine("\n\n\n====================\nLogin: SERVER STOP\n====================\n\n\n");
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();

            return;
        }

    }
}
