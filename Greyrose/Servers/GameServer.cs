using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WizPS
{
    partial class Server
    {

        public static class GS_Settings
        {
            public static int TCPPort = 12170;
            public static int UDPPort = 12171;
            public static IPAddress IP = IPAddress.Parse("0.0.0.0");
            public static string Key = "blob";
            public static bool SocketOpen = false;
            public static bool SessionOpen = false;
            public static ushort SessionID = 4513;
        }

        public static async Task GS()
        {
            Console.WriteLine("GAME SERVER STARTED!");
            TcpListener server = null;
            try
            {
                //Start a new server with the gameserver's ip/port
                server = new TcpListener(GS_Settings.IP, GS_Settings.TCPPort);

                //Start listening for client requests.
                server.Start();

                //Buffer for reading data. 4096 bytes should be fine
                Byte[] bytes = new Byte[4096];

                //Enter the listening loop.
                while (true)
                {
                    Console.Write("GS: Waiting for a connection...\n");
                    GS_Settings.SocketOpen = false;
                    GS_Settings.SessionOpen = false;

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("GS: Connected!\n");

                    //Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    Console.WriteLine("GS: AWAITING MESSAGE");

                    int i;

                    try
                    {
                        if (!GS_Settings.SocketOpen)
                            stream.ReadTimeout = 300; //300 ms timeout if the session isn't open
                        else
                            stream.ReadTimeout = 100000; //Timeout of 100000ms if the session is open
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            if (!GS_Settings.SocketOpen) //If a socket hasn't been opened
                            {
                                Program.Globals.SessionOpen = false; //Inform the rest of the program that a new session needs to be made
                                Program.Globals.Sessid = 0792; //0318
                            }
                            GS_Settings.SocketOpen = true; //Let the program know that the socket is open (so don't timeout anymore)
                            stream.ReadTimeout = 100000;
                            try
                            {
                                Tuple<byte[], int,int> temptuple = MessageParser.Parse(bytes,GS_Settings.TCPPort, GS_Settings.UDPPort, GS_Settings.Key,GS_Settings.SessionOpen,GS_Settings.SessionID);
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
                                        RealResponse._STR("2572,4376,-28,5.55"); //Location
                                        RealResponse._INT(0); //Slot
                                        RealResponse._INT(0); //PrepPhase
                                        RealResponse._INT(0); //Error
                                        RealResponse._STR("WizPS.Login"); //LoginServer (Not important, just displayed in title bar)
                                        byte[] packet2 = RealResponse.Finalise();
                                        stream.Write(packet2,0,packet2.Length);
                                    }
                                    if (SessionOpen == 1)
                                        GS_Settings.SessionOpen = true;
                                }
                            }
                            catch //If the data couldn't be parsed, print the data packet
                            {
                                string data = null;

                                for (int q = 0; q < bytes.Length; q++)
                                {
                                    data = data + bytes[q].ToString("X2");
                                }
                                Console.WriteLine("GS: Received: {0}\n", data);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("GS: NO DATA RECEIVED!\n");
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
                Console.WriteLine("\n\n\n====================\nGS: SERVER STOP\n====================\n\n\n");
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();

            //login.us.wizard101.com (165.193.63.4)
            //login server sends/receives on port 12000. Client changes ports randomly between range of 12000-12999
            return;
        }


    }
}
