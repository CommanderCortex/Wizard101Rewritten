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
        public static byte[] _1BaseMessages(BinaryReader data, bool SessionOpen, ushort SessionID)
        {
            uint msgid = DataHandler.MSGID(data);
            uint msglen = DataHandler.USHRT(data);

            if (msgid == 1)
            {
                Console.WriteLine("MSG: Ping");
                //Send ping response


                if (!SessionOpen)
                {
                    uint unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    KIPacket packet = new KIPacket();
                    packet._UBYT(0x0d);
                    packet._UBYT(0xf0);
                    packet._UBYT(0x13);
                    packet._UBYT(0x00);
                    packet._UBYT(0x01);
                    packet._UBYT(0x00);
                    packet._UBYT(0x00);
                    packet._UBYT(0x00);
                    packet._USHRT(SessionID);
                    packet._INT(0); //00 00 00 00 Unknown data. Always appears to be 0 though
                    packet._UINT(unixTimestamp); //Send the current time
                    packet._INT(800); //We're 800 MS into the session (accuracy isn't important)

                    SessionOpen = true; //Remember that the session has been initiated
                    return packet.RawFinalise();
                }

                //If the session is already open, send a normal ping resonse
                KIPacket resp = new KIPacket();
                resp._UBYT(0x0D); //Add '0D' byte
                resp._UBYT(0xF0); //Add 'F0' byte
                resp._USHRT(09); //Add 00 09 for length
                resp._UBYT(0); //Add 00 control flag
                resp._UBYT(0); //Add 00 opcode
                resp._USHRT(0); //Add 00 00 for unknown bytes (bytes are always 00 during gameplay, so it seems unimportant)
                resp._UBYT(1); //SVCID 1 (basemessage)
                resp._UBYT(2); //MSGID 2 (PING_RESP)
                resp._USHRT(4); //Add inner-message length placeholder

                return resp.RawFinalise();
            }
            else if (msgid == 2)
            {
                Console.WriteLine("MSG: Ping response"); //The client responded to the server's ping request (we don't ping the client though, so this shouldn't be called anyway)
            }

            return null;
        }
    }
}
