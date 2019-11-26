using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace WizPS
{
    class MessageCrafter
    {

        public static void BYT(BinaryWriter message, sbyte data)
        {
            message.Write(data);
        }

        public static void UBYT(BinaryWriter message, byte data)
        {
            message.Write(data);
        }

        public static void SHRT(BinaryWriter message, Int16 data)
        {
            message.Write(data);
        }

        public static void USHRT(BinaryWriter message, ushort data)
        {
            message.Write(data);
        }

        public static void INT(BinaryWriter message, Int32 data)
        {
            message.Write(data);
        }

        public static void UINT(BinaryWriter message, UInt32 data)
        {
            message.Write(data);
        }

        public static void STR(BinaryWriter message, string data)
        {
            ushort length = (ushort)data.Length; //Grab the string length
            message.Write(length); //Write the string length as a ushort (how the game stores string length)
            message.Write(data); //Write the string
        }

        public static void WSTR(BinaryWriter message, string data)
        {
            ushort length = (ushort)data.Length; //Grab the string length
            message.Write(length); //Write the string length as a ushort (how the game stores string length)
            message.Write(data); //Write the string
        }

        public static void FLT(BinaryWriter message, float data)
        {
            message.Write(data); //Write the float
        }

        public static void DBL(BinaryWriter message, double data)
        {
            message.Write(data); //Write the double
        }

        public static void GID(BinaryWriter message, UInt64 data)
        {
            message.Write(data); //Write the UInt64
        }
    }

    /// <summary>
    /// Easy method of crafting DML packets.
    /// Start by declaring a new KIPacket.
    /// Call KIPacket.Header to create a header.
    /// Add your own data by calling _FLT, _STR, etc...
    /// Calculate lengths and finish the packet by calling KIPacket.Finalise
    /// </summary>
    public class KIPacket
    {
        private static int outerlen = 0;
        private static ushort innerlen = 0;
        private static byte[] tempbyte = new byte[4096];
        private static Stream ByteStream = new MemoryStream(tempbyte);
        private static BinaryWriter stream = new BinaryWriter(ByteStream);
        
        /// <summary>
        /// Generates a message header using the defined input.
        /// </summary>
        /// <param name="control">Control code</param>
        /// <param name="opcode">Op code</param>
        /// <param name="svcid">Service ID</param>
        /// <param name="msgid">Message ID</param>
        public void Header(uint control, uint opcode, uint svcid, uint msgid)
        {
            _UBYT(0x0D); //Add '0D' byte
            _UBYT(0xF0); //Add 'F0' byte
            _USHRT(0); //Add 00 00 for length (because we don't currently know the packet length)
            _UBYT(control); //Add specified control flag
            _UBYT(opcode); //Add specified opcode
            _USHRT(0); //Add 00 00 for unknown bytes (bytes are always 00 during gameplay, so it seems unimportant)
            _UBYT(svcid); //Add specified Service ID
            _UBYT(msgid); //Add specified Message ID
            _USHRT(0); //Add inner-message length placeholder
            outerlen -= 5;
            innerlen -= 10;
        }

        //Data handlers
        public void _BYT(sbyte data) { stream.Write(data); innerlen += 1; outerlen += 1; }
        public void _UBYT(uint data) { stream.Write((byte)data); innerlen += 1; outerlen += 1; }
        public void _SHRT(Int16 data) { stream.Write(data); innerlen += 2; outerlen += 2; }
        public void _USHRT(ushort data) { stream.Write(data); innerlen += 2; outerlen += 2; }
        public void _INT(Int32 data) { stream.Write(data); innerlen += 4; outerlen += 4; }
        public void _UINT(UInt32 data) { stream.Write(data); innerlen += 4; outerlen += 4; }
        public void _STR(string data) { stream.Write((UInt16)data.Length); stream.Write(data.ToCharArray()); innerlen += (ushort)(data.Length + 2); outerlen += (ushort)(data.Length + 2); }
        public void _WSTR(string data) { stream.Write((UInt16)data.Length); stream.Write(data.ToCharArray()); innerlen += (ushort)(data.Length + 2); outerlen += (ushort)(data.Length + 2); }
        public void _FLT(float data) { stream.Write(data); innerlen += 4; outerlen += 4; }
        public void _DBL(double data) { stream.Write(data); innerlen += 8; outerlen += 8; }
        public void _GID(UInt64 data) { stream.Write(data); innerlen += 8; outerlen += 8; }
        public void _HEXSTRING(string data){ string tempstr = string.Join("", data.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries)); byte[] temp = ConvertHexStringToByteArray(tempstr); stream.Write(temp); innerlen += (ushort)temp.Length; outerlen += (ushort)temp.Length; }

        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
                hexString = "0" + hexString;

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        //Marks the packet as complete
        //Corrects packet lengths (lengths are initially filled as 0000 because they are unknown)

        /// <summary>
        /// Fills correct packet/message lengths and returns packet as a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] Finalise()
        {
            _BYT(0); //Add padding byte
            innerlen += 1; //Increment inner-length counter because a padding byte was added
            outerlen += 1; //Increment inner-length counter because a padding byte was added
            long pktlen = stream.BaseStream.Position; //Store the entire packet length
            byte[] output = new byte[pktlen]; //Make a new array the exact size of the output packet
            stream.Seek(2, 0); //Move the writer position to the outer-length bytes
            stream.Write(outerlen); //Overwrite the null bytes with the correct outer-length
            stream.Seek(10, 0); //Set the StreamWriter index position to the message-length bytes
            stream.Write(innerlen); //Overwrite the null bytes with the correct inner-length
            stream.Close(); //Close the stream because we've finished writing to it
            Array.Copy(tempbyte, output, pktlen); //Copy the packet data from the temporary array to the new exact-size array
            return output;
        }

        public byte[] RawFinalise()
        {
            _BYT(0); //Add padding byte
            innerlen += 1; //Increment inner-length counter because a padding byte was added
            outerlen += 1; //Increment inner-length counter because a padding byte was added
            long pktlen = stream.BaseStream.Position; //Store the entire packet length
            byte[] output = new byte[pktlen]; //Make a new array the exact size of the output packet
            stream.Close(); //Close the stream because we've finished writing to it
            Array.Copy(tempbyte, output, pktlen); //Copy the packet data from the temporary array to the new exact-size array
            return output;
        }

        //When a new KIPacket class is initialised
        public KIPacket()
        {
            tempbyte = new byte[4096]; //Reset the byte array
            ByteStream = new MemoryStream(tempbyte); //Reset the memorystream
            stream = new BinaryWriter(ByteStream); //Reset the binarywriter
            outerlen = 0; //Reset the outer message length (First length after f00d)
            innerlen = 0; //Reset the inner message length (Length after MSGID)
        }

    }

}
