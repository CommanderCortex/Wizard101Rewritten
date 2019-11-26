using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WizPS
{
    class MessageParser
    {
        /// <summary>
        /// Reads the input packet (as a byte stream), analyses it and sends it to the appropriate message handler
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Tuple<Byte[],int,int> Parse(byte[] bytes, int TCPPort, int UDPPort, string Key, bool SessionOpen, ushort SessionID)
        {

            Stream ByteStream = new MemoryStream(bytes);
            using (BinaryReader reader = new BinaryReader(ByteStream))
            {
                string header = reader.ReadByte().ToString("X2");
                header = header + reader.ReadByte().ToString("X2");

                if (header == "0DF0")
                {
                    /*
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("KI PACKET");
                    Console.ResetColor();
                    */
                }
                else //If the packet doesn't start with 0DF0 (F00D)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("UNKNOWN PACKET TYPE!"); //Inform the user that an unknown packet was received
                    Console.ResetColor();
                    return null; //Cancel this process because we don't know how to parse the packet
                }

                int length = reader.ReadUInt16();
                int isControl = reader.ReadByte();
                int opCode = reader.ReadByte();

                int unknown = reader.ReadUInt16(); //The extra 00 00 bytes that seem to always be 00 00. This just seeks the data, really. I don't care what the data is

                if (unknown != 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\n\n############### UNKNOWN DATA WAS NOT 00 00! OH MY GOD IT FUCKING CHANGED! PANIC! ###############"); //If the unknown data is not 00 00 like it *always* is, print a little panic message, lol
                    Console.ResetColor();
                }

                //Debug output, uncomment to display more packet information
                //Console.WriteLine("Length: {0}", length);
                //Console.WriteLine("isControl: {0}", isControl);
                //Console.WriteLine("opCode: {0}", opCode);
                //Console.WriteLine("undefined: {0}", reader.ReadByte());
                //Console.WriteLine("undefined: {0}", reader.ReadByte());

                if (isControl != 0) //If the message has a control code, parse the control code
                {
                    //Console.WriteLine("CONTROL MESSAGE!");
                    Tuple<byte[],int,int> temptuple = Handlers._0ControlMessages(reader, opCode, length, bytes);
                    return temptuple;
                }

                if (isControl == 0 && opCode == 0) //If the message is not a control message, and has no opcode. Parse it like a message
                {
                    uint svcid = reader.ReadByte();
                    Console.WriteLine("Service ID: {0}", svcid);

                    if (svcid == 1)
                    {
                        //Console.WriteLine("Base Message. Passing to handler _1BaseMessages");
                        return Tuple.Create(Handlers._1BaseMessages(reader,SessionOpen,SessionID),0,0);
                    }
                    else if (svcid == 2)
                    {
                        Console.WriteLine("Extended Base Message. No current handlers");
                    }
                    else if (svcid == 5)
                    {
                        //Console.WriteLine("Game Message. Passing to handler _5GameMessages");
                        return Handlers._5GameMessages(reader,TCPPort,UDPPort,Key);
                    }
                    else if (svcid == 7)
                    {
                        //Console.WriteLine("Login Message. Passing to handler _7LoginMessages");
                        return Handlers._7LoginMessages(reader,TCPPort,UDPPort,Key);
                    }
                    else if (svcid == 8)
                    {
                        //Console.WriteLine("Patch Message. Passing to handler _8PatchMessages");
                        return Tuple.Create(Handlers._8PatchMessages(reader),0,0);
                    }
                    else if (svcid == 52)
                    {
                        Console.WriteLine("Quest Message. No current handlers");
                    }
                    return null;
                }

                Console.WriteLine("UNHANDLED MESSAGE!");

                string hexbytes = null;

                for (int i = 0; i < length + 4; i++) //Convert bytes to their appropriate hex representation (not decimal bytes like the disgusting programming language decides it should be stored as)
                {
                    hexbytes = hexbytes + bytes[i].ToString("X2");
                }

                Console.WriteLine(hexbytes);

            }



            return null; //No response to send to the client
        }

    }
}
