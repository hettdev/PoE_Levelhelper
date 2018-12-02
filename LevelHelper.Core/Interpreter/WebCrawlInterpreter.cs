using System;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace LevelHelper.Core.Interpreter
{
    class WebCrawlInterpreter : IStringInterpreter
    {

        public InterpretEventArgs InterpretLine(string line)
        {
            line = line.Replace("\"", "");
            string regExp = @"(.+)(C\()(.+)(\))";
            Match match = Regex.Match(line, regExp, RegexOptions.Multiline);
            string[] attributes = match.Groups[3].Value.Split(',');
            string character = "";
            string level = "";
            string charClass = "";
            foreach (string s in attributes)
            {
                if (s.ToLower().Contains("level"))
                {
                    level = s.Split(':')[1];
                    break;
                }
                else if(s.ToLower().Contains("name"))
                {
                    character = s.Split(':')[1];
                }
                else if (s.ToLower().Contains("class"))
                {
                    charClass = s.Split(':')[1];
                }
            }
            return new InterpretEventArgs(charName: character, level: level, charClass: charClass);
        }
    }
}
