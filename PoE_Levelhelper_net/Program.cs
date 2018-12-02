using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace PoE_Levelhelper
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string pth = @"C:\Program Files (x86)\Steam\steamapps\common\Path of Exile\logs\"; 
            string accName = "Addihash";

            LevelHelper helper = new LevelHelper(accName, pth);
            Console.ReadLine();
        }
        
    }
}
