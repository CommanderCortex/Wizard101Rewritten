using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WizPS
{
    partial class Handlers
    {

        public static Tuple<byte[],int,int> _0ControlMessages(BinaryReader data, int opCode, int length, byte[] bytes)
        {
            if (opCode == 0)
            {
                Console.WriteLine("OPCODE: SESSION OFFER");
                UInt16 sessionid = DataHandler.USHRT(data);
                UInt32 undefined = DataHandler.UINT(data);
                Int32 timestamp = DataHandler.INT(data);
                UInt32 milliseconds = DataHandler.UINT(data);
                Console.WriteLine("Session ID: {0}", sessionid);
                Console.WriteLine("undefined: {0}", undefined);
                Console.WriteLine("Timestamp: {0}", timestamp);
                Console.WriteLine("Milliseconds: {0}", milliseconds);
            }
            else if (opCode == 3)
            {
                Console.WriteLine("OPCODE: KEEP ALIVE");
                UInt16 sessionid = DataHandler.USHRT(data);
                UInt16 milliseconds = DataHandler.USHRT(data);
                UInt16 minutes = DataHandler.USHRT(data);
                Console.WriteLine("Sessiond ID: {0}", sessionid); //ID of the session
                Console.WriteLine("Milliseconds: {0}", milliseconds); //How long the session has been open in MS
                Console.WriteLine("Minutes: {0}", minutes); //Minutes since the session was established

                byte[] b = new byte[length + 4];
                Array.Copy(bytes, 0, b, 0, length + 4);

                b[5] = 0x04; //Change the opcode to keepalive response, keeping the payload the same (which is what the server does)
                return Tuple.Create(b,0,0); //Return the same payload, with a different svcid (04 for response, instead of 03 for request)



            }
            else if (opCode == 4)
            {
                Console.WriteLine("OPCODE: KEEP ALIVE RESPONSE");
                uint undefined = DataHandler.USHRT(data);
                uint timestamp = DataHandler.UINT(data);
                Console.WriteLine("Unknown: {0}", undefined); //Unsure, always appears to be 0
                Console.WriteLine("Timestamp: {0}", timestamp); //MS since the server started
            }
            else if (opCode == 5)
            {
                Console.WriteLine("OPCODE: SESSION ACCEPT");
                uint undefined = DataHandler.USHRT(data); //Hasn't been observed to be anything above 0.
                uint undefined2 = DataHandler.UINT(data); //Hasn't been observed to be anything above 0. Potentially the upper 4 bytes of a timestamp.
                uint timestamp = DataHandler.UINT(data);  //The UNIX timestamp(seconds since 01 / 01 / 1970) at the time this message was sent.
                uint milliseconds = DataHandler.UINT(data); //The number of milliseconds we were in to the current second when this message was sent.
                uint sessionId = DataHandler.USHRT(data);    //The ID of the session that the client is accepting (from the SESSION_OFFER).
                Console.WriteLine("Unknown1: {0}", undefined);
                Console.WriteLine("Unknown2: {0}", undefined2);
                Console.WriteLine("Timestamp: {0}", timestamp);
                Console.WriteLine("Milliseconds: {0}", milliseconds);
                Console.WriteLine("Session ID: {0}", sessionId);
                byte[] bytarr = null;
                return Tuple.Create(bytarr, 0, 1);
            }
            else
            {
                Console.WriteLine("UNSUPPORTED OPCODE!");
            }

            return null;
        }

    }
}
