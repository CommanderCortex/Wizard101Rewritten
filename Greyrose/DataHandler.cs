using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WizPS
{
    class DataHandler
    {

        public static uint MSGID(BinaryReader data)
        {
            uint msgid = data.ReadByte();
            return msgid;
        }

        public static int BYT(BinaryReader data)
        {
            return (int)data.ReadSByte();
        }

        public static uint UBYT(BinaryReader data)
        {
            return (uint)data.ReadByte();
        }

        public static Int16 SHRT(BinaryReader data)
        {
            return data.ReadInt16();
        }

        public static UInt16 USHRT(BinaryReader data)
        {
            return data.ReadUInt16();
        }

        public static Int32 INT(BinaryReader data)
        {
            return data.ReadInt32();
        }

        public static UInt32 UINT(BinaryReader data)
        {
            return data.ReadUInt32();
        }

        public static string STR(BinaryReader data)
        {
            uint length = data.ReadUInt16();
            string temp = null;
            for (int i = 0; i < length; i++)
            {
                temp = temp + data.ReadChar();
            }

            return temp;
        }

        public static string WSTR(BinaryReader data)
        {
            uint length = data.ReadUInt16();
            string temp = null;
            for (int i = 0; i < length; i++)
            {
                temp = temp + data.ReadChar();
            }

            return temp;
        }

        public static float FLT(BinaryReader data)
        {
            return data.ReadSingle();
        }

        public static double DBL(BinaryReader data)
        {
            return data.ReadDouble();
        }

        public static UInt64 GID(BinaryReader data)
        {
            return data.ReadUInt64();
        }
    }
}
