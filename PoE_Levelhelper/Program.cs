using System;
using System.Threading;

namespace PoE_Levelhelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string pth = @"D:\SteamLibrary\steamapps\common\Path of Exile\logs\";
            FileScanner scanner = new FileScanner(pth);
            while (true)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
