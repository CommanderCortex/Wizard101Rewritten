using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace WizPS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "W101PS";
            worker(); //Start the async worker
        }

        public static class Globals
        {
            public static bool SessionOpen = false;
            public static bool SocketOpen = false;
            public static bool Transfer = false;
            public static ushort Sessid = 4513;
        }

        static void worker()
        {
            var task1 = Task.Run(Server.LS);
            var task2 = Task.Run(Server.PS);
            var task3 = Task.Run(Server.GS);
            Task.WhenAll(task1,task2,task3);
            Console.WriteLine("ALL CONNECTIONS CLOSED!");
            Thread.Sleep(1000);
            Console.ReadLine();
        }


        //Slightly better than putting a Console.ForegrounColor call before the line is written for each handler
        public static void ColorTitle(string input)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(input);
            Console.ResetColor();
            return;
        }

    }
}
