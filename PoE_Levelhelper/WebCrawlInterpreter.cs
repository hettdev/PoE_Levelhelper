using System;
using System.Text.RegularExpressions;

namespace PoE_Levelhelper
{
    class WebCrawlInterpreter : IStringInterpreter
    {
        public string InterpretLine(string line)
        {
            // new C({"name":"
            line = line.Replace("\"", "");
            string regExp = @"^.*?(?=C\(\{name:)";
            line = line.Split("C({name:")[1];
            line = line.Split("});")[0];
            Match match = Regex.Match(line, regExp);
            line = Regex.Replace(line, regExp, "");

            return line;
        }
    }
}
