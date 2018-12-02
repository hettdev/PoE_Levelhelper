using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using LevelHelper.Core.Reader;
using LevelHelper.Core.Interpreter;
using LevelHelper.Core;

namespace LevelHelper
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string pth = @"C:\Program Files (x86)\Steam\steamapps\common\Path of Exile\logs\"; 
            string accName = "Addihash";

            List<IStringInterpreter> intps = new List<IStringInterpreter>();
            intps.Add(new LevelupInterpreter("Alitessa"));
            FileScanner scanner = new FileScanner(pth, intps);


            Console.ReadLine();
        }
        
    }
}
