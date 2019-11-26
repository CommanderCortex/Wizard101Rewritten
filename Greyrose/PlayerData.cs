//Where player data is stored in memory
//
//In the future, it will obviously support more than one player, and will have database integration to make the data non-volatile
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizPS
{
    class PlayerData
    {
        public struct PlayerStruct
        {
            public float X;
            public float Y;
            public float Z;
            public float Rot;
            public Int64 GID;
            public string Zone_Name;
            public Int64 Zone_ID;

            public ushort Marker_X;
            public ushort Marker_Y;
            public ushort Marker_Z;
            public byte Marker_Rot;
        }
        
        //Temporary.
        //I just to test saving data between player sessions, and add marker support
        public static PlayerStruct player1; //Init a player struct with the name 'player1'


    }
}
