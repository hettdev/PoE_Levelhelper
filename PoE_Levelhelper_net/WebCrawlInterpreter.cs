using System;
using System.Text.RegularExpressions;

namespace PoE_Levelhelper
{
    class WebCrawlInterpreter : IStringInterpreter
    {

        public InterpretEventArgs InterpretLine(string line)
        {
            // new C({"name":"
            line = line.Replace("\"", "");
            string regExp = @"^.*?(?=C\(\{name:)";

            line = Regex.Split(line, "C({name:")[1];
            line = Regex.Split(line, "});")[0];
            Match match = Regex.Match(line, regExp);
            line = Regex.Replace(line, regExp, "");
            string[] attributes = line.Split(',');
            string character = attributes[0];
            string level = "";
            foreach (string s in attributes)
            {
                if (s.ToLower().Contains("level"))
                {
                    level = s.Split(':')[1];
                    break;
                }
            }
            return new InterpretEventArgs(charName: character, level: level);
        }
    }
}
