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
            string pth = Directory.GetCurrentDirectory();
            string accName = "Addihash";

            WebCrawl wc = new WebCrawl(accName, new WebCrawlInterpreter());
            wc.Crawl();
            
            
        }
    }
}
