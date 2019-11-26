using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace WizPS
{
    partial class Handlers
    {

        public static Tuple<byte[],int,int> _7LoginMessages(BinaryReader data, int TCPPort, int UDPPort, string Key)
        {
            uint msgid = DataHandler.MSGID(data);
            uint msglen = DataHandler.USHRT(data);


            if (msgid == 1)
            {
                Console.WriteLine("MSG_CHARACTERINFO");
            }
            else if (msgid == 2)
            {
                Console.WriteLine("MSG_CHARACTERLIST");
            }
            else if (msgid == 3)
            {
                Console.WriteLine("MSG_CHARACTERSELECTED");

            }
            else if (msgid == 4)
            {
                Console.WriteLine("MSG_CREATECHARACTER");
            }
            else if (msgid == 5)
            {
                Console.WriteLine("MSG_CREATECHARACTERRESPONSE");
            }
            else if (msgid == 6)
            {
                Console.WriteLine("MSG_DELETECHARACTER");
            }
            else if (msgid == 7)
            {
                Console.WriteLine("MSG_DELETECHARACTERRESPONSE");
            }
            else if (msgid == 8)
            {
                Console.WriteLine("MSG_REQUESTCHARACTERLIST");
                //Return the characterlist
                //This is just a debug charlist ripped straight from a packet capture.
                //It includes more than one packet (startcharlist, char, char, char, char, endcharlist)

                KIPacket TempResponse = new KIPacket(); //Init a new packet
                TempResponse.Header(0, 0, 7, 12); //Set header to svcid:7, msgid:12 (MSG_STARTCHARACTERLIST)
                TempResponse._STR("WizPS.Login"); //LoginServer string (not important, just used for window title)
                TempResponse._INT(0); //Purchased character slots (0 character slots purchased)
                byte[] PKT1 = TempResponse.Finalise(); //Save KIPacket as the first packet to send

                
                TempResponse = new KIPacket(); //Reset the KIPacket (resets length counters)
                TempResponse.Header(0, 0, 7, 1); //MSG_CHARACTERINFO (character 1)
                TempResponse._HEXSTRING("82 00 4C 8F 6E 11 01 00 00 00 00 00 00 E9 30 20 01 00 00 AB 02 D0 DF 33 07 E8 03 00 00 00 07 8D D0 72 24 80 C0 58 20 81 80 40 01 00 00 00 88 BE C1 04 00 00 00 00 00 00 C3 CA F5 40 03 00 00 00 44 64 73 43 84 12 00 00 00 00 00 00 00 00 00 00 00 00 44 64 73 43 C4 EF 01 00 00 00 00 00 00 00 00 00 00 00 44 64 73 43 4B 54 01 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 49 1C 01 00 30 33 0B 00");
                byte[] PKT2 = TempResponse.Finalise();


                TempResponse = new KIPacket();
                TempResponse.Header(0, 0, 7, 1); //MSG_CHARACTERINFO (character 2)
                TempResponse._HEXSTRING("85 00 4C 8F 6E 11 01 00 00 00 00 00 00 F1 30 20 01 00 00 AB 02 D0 DF 33 07 E8 03 00 00 00 07 8D D0 72 14 00 80 4C 0C 31 80 18 01 00 00 00 88 BE C1 04 00 00 00 00 00 00 C3 CA F5 40 02 00 00 00 44 64 73 43 84 12 00 00 00 00 00 00 00 00 00 00 00 00 44 64 73 43 C3 EF 01 00 00 00 00 00 00 00 00 00 00 00 15 00 57 69 7A 61 72 64 5A 6F 6E 65 5F 54 68 65 43 6F 6D 6D 6F 6E 73 02 00 00 00 06 C1 23 00 0B 1A 4D 00");
                byte[] PKT3 = TempResponse.Finalise();


                TempResponse = new KIPacket(); 
                TempResponse.Header(0, 0, 7, 1); //MSG_CHARACTERINFO (character 3)
                TempResponse._HEXSTRING("A8 00 4C 8F 6E 11 01 00 00 00 00 00 00 A9 81 22 01 00 00 AA 02 D0 DF 33 07 E8 03 00 00 00 07 8D D0 72 38 00 80 2D 80 00 40 00 01 00 00 00 88 BE C1 04 00 00 00 02 00 00 C3 CA F5 40 04 00 00 00 44 64 73 43 84 12 00 00 00 00 00 00 00 00 00 00 00 00 44 64 73 43 C6 EF 01 00 00 00 00 00 00 00 00 00 00 00 44 64 73 43 F1 2E 01 00 61 00 00 00 00 00 00 00 00 00 44 64 73 43 CC 12 00 00 61 00 00 00 00 00 00 00 00 00 14 00 57 69 7A 61 72 64 5A 6F 6E 65 5F 52 61 76 65 6E 77 6F 6F 64 04 00 00 00 0D 5B 25 00 20 3F 3F 00");
                byte[] PKT4 = TempResponse.Finalise();

                TempResponse = new KIPacket();
                TempResponse.Header(0, 0, 7, 2); //MSG_CHARACTERLIST (signifies the end of a character list)
                TempResponse._UINT(0); //Error (0 means no error)
                byte[] PKT5 = TempResponse.Finalise();


                //byte [] response = PKT1.Concat(PKT2).ToArray().Concat(PKT3).ToArray().Concat(PKT4).ToArray().Concat(PKT5).ToArray(); //Combine all of the packets into one single packet (the game accepts multile packets concated into one)
                byte[] response = PKT1.Concat(PKT4).ToArray().Concat(PKT5).ToArray();
                Console.WriteLine("CHARACTER LIST SENT!");
                return Tuple.Create(response,0,0);
            }
            else if (msgid == 9)
            {
                Console.WriteLine("MSG_REQUESTSERVERLIST");
            }
            else if (msgid == 10)
            {
                Console.WriteLine("MSG_SELECTCHARACTER");


                //Craft packet to inform client that the server is processing the request (make the game show the 'verifying character' dialogue)

                KIPacket TempResponse = new KIPacket(); //Initialise new packet
                TempResponse.Header(0x00, 0x00, 0x07, 0x03); //Create header with SVCID 7 and MSGID 3 (MSG_CHARACTERSELECTED)
                TempResponse._STR(""); //IP
                TempResponse._INT(0); //TCPPORT
                TempResponse._INT(0); //UDPPORT
                TempResponse._STR(""); //KEY
                TempResponse._GID(0); //UserID
                TempResponse._GID(0); //CharID
                TempResponse._GID(0); //ZoneID
                TempResponse._STR(""); //ZoneName
                TempResponse._STR(""); //Location
                TempResponse._INT(0); //Slot
                TempResponse._INT(1); //PrepPhase
                TempResponse._INT(0); //Error
                TempResponse._STR("WizPS.Login"); //LoginServer
                byte[] packet1 = TempResponse.Finalise();

                return Tuple.Create(packet1,1,0); //Return temporary packet, informing the client that the server is processing the transfer. Return 1, so that the caller knows to initiate a transfer
                //After this, the login server will be neglected. So switch all data to the gameserver now

                /*
                Message structure:
                <IP TYPE="STR"></IP>
                <TCPPort TYPE="INT"></TCPPort>
                <UDPPort TYPE="INT"></UDPPort>
                <Key TYPE="STR"></Key>
                <UserID TYPE="GID"></UserID>
                <CharID TYPE="GID"></CharID>
                <ZoneID TYPE="GID"></ZoneID>
                <ZoneName TYPE="STR"></ZoneName>
                <Location TYPE="STR"></Location>
                <Slot TYPE="INT"></Slot>
                <PrepPhase TYPE="INT"></PrepPhase>
                <Error TYPE="INT"></Error>
                <LoginServer TYPE="STR"></LoginServer>
                */



            }
            else if (msgid == 11)
            {
                Console.WriteLine("MSG_SERVERLIST");
            }
            else if (msgid == 12)
            {
                Console.WriteLine("MSG_STARTCHARACTERLIST");
            }
            else if (msgid == 13)
            {
                Console.WriteLine("MSG_USER_AUTHEN");
            }
            else if (msgid == 14)
            {
                Console.WriteLine("MSG_USER_AUTHEN_RSP");
            }
            else if (msgid == 15)
            {
                Console.WriteLine("MSG_USER_VALIDATE");
                UInt64 UserID = DataHandler.GID(data);
                string PassKey3 = DataHandler.STR(data);
                UInt64 MachineID = DataHandler.GID(data);
                string Locale = DataHandler.STR(data);
                string PatchClientID = DataHandler.STR(data);
                
                Console.WriteLine("User ID: {0}", UserID);
                Console.WriteLine("PassKey3: {0}", PassKey3);
                Console.WriteLine("Machine ID: {0}", MachineID);
                Console.WriteLine("Locale: {0}", Locale);
                Console.WriteLine("Patch Client ID: {0}", PatchClientID);

                byte[] response1 = { 0x0d, 0xf0, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x10, 0x1c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xd0, 0xdf, 0x33, 0x07, 0xe8, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //MSG_USER_VALIDATE_RSP, informs the client that they have authenticated successfully (for debug purposes, will always admit the user. For an actual server, client keys will be verified.)
                byte[] response2 = { 0x0d, 0xf0, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x14, 0x0c, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //MSG_USER_ADMIT_IND, informs the client that they can join the game (allows them to click the 'play' button from the character-select screen)
                byte[] response = response1.Concat(response2).ToArray();
                return Tuple.Create(response,0,0);
            }
            else if (msgid == 16)
            {
                Console.WriteLine("MSG_USER_VALIDATE_RSP");
            }
            else if (msgid == 17)
            {
                Console.WriteLine("MSG_DISCONNECT_LOGIN_AFK");
            }
            else if (msgid == 18)
            {
                Console.WriteLine("MSG_LOGIN_NOT_AFK");
            }
            else if (msgid == 19)
            {
                Console.WriteLine("MSG_LOGINSERVERSHUTDOWN");
            }
            else if (msgid == 20)
            {
                Console.WriteLine("MSG_USER_ADMIT_IND");
            }
            else if (msgid == 21)
            {
                Console.WriteLine("MSG_WEBCHARACTERINFO");
            }
            else if (msgid == 22)
            {
                Console.WriteLine("MSG_USER_AUTHEN_V2");
            }
            else if (msgid == 23)
            {
                Console.WriteLine("MSG_SAVECHARACTER");
            }
            else if (msgid == 24)
            {
                Console.WriteLine("MSG_WEB_AUTHEN");
            }
            else if (msgid == 25)
            {
                Console.WriteLine("MSG_WEB_VALIDATE");
            }
            else if (msgid == 26)
            {
                Console.WriteLine("MSG_CHANGECHARACTERNAME");
            }
            else if (msgid == 27)
            {
                Console.WriteLine("MSG_USER_AUTHEN_V3");
            }
            else
            {
                Console.WriteLine("UNSUPPORTED MESSAGE! Make sure you are running revision r667549.Wizard_1_390");
            }

            return null;
        }
    }
}
