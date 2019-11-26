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

        public static byte[] _8PatchMessages(BinaryReader data)
        {
            uint msgid = DataHandler.MSGID(data);
            uint msglen = DataHandler.USHRT(data);

            if (msgid == 1)
            {
                Console.WriteLine("MSG: Latest File List"); //Patch Client sends this message to request the latest file list.  Patch Server sends this message to tell the client where to get it.
                uint LatestVersion = DataHandler.UINT(data);
                string ListFileName = DataHandler.STR(data);
                uint ListFileType = DataHandler.UINT(data);
                uint ListFileSize = DataHandler.UINT(data);
                uint ListFileCRC = DataHandler.UINT(data);
                string ListFileURL = DataHandler.STR(data);
                string URLPrefix = DataHandler.STR(data);
                string URLSuffix = DataHandler.STR(data);
                Console.WriteLine("Latest Version: {0}", LatestVersion);
                Console.WriteLine("File Name: {0}", ListFileName);
                Console.WriteLine("File Type: {0}", ListFileType);
                Console.WriteLine("File Size: {0}", ListFileSize);
                Console.WriteLine("File CRC: {0}", ListFileCRC);
                Console.WriteLine("File URL: {0}", ListFileURL);
                Console.WriteLine("URL Prefix: {0}", URLPrefix);
                Console.WriteLine("URL Suffix: {0}", URLSuffix);
            }
            else if (msgid == 2)
            {
                Console.WriteLine("MSG: Latest File List V2"); //Patch Client sends this message to request the latest file list.  Patch Server sends this message to tell the client where to get it.


                uint LatestVersion = DataHandler.UINT(data);
                string ListFileName = DataHandler.STR(data);
                uint ListFileType = DataHandler.UINT(data);
                uint ListFileTime = DataHandler.UINT(data);
                uint ListFileSize = DataHandler.UINT(data);
                uint ListFileCRC = DataHandler.UINT(data);
                string ListFileURL = DataHandler.STR(data);
                string URLPrefix = DataHandler.STR(data);
                string URLSuffix = DataHandler.STR(data);
                string Locale = DataHandler.STR(data);

                Console.WriteLine("Latest Version: {0}", LatestVersion);
                Console.WriteLine("File Name: {0}", ListFileName);
                Console.WriteLine("File Type: {0}", ListFileType);
                Console.WriteLine("File Time: {0}", ListFileTime);
                Console.WriteLine("File Size: {0}", ListFileSize);
                Console.WriteLine("File CRC: {0}", ListFileCRC);
                Console.WriteLine("File URL: {0}", ListFileURL);
                Console.WriteLine("URL Prefix: {0}", URLPrefix);
                Console.WriteLine("URL Suffix: {0}", URLSuffix);
                Console.WriteLine("Locale: {0}", Locale);
            }
            else if (msgid == 3)
            {
                Console.WriteLine("MSG: Next Version");
            }

            return null;
        }
    }
}
