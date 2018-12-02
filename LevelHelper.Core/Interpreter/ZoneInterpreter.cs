using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LevelHelper.Core.Interpreter
{
    public class ZoneInterpreter : IStringInterpreter
    {
        public InterpretEventArgs InterpretLine(string line)
        {
            string regExp = @"(.+)(You)(.)(have)(.)(entered)(.)(.+)(\.)";
            Match match = Regex.Match(line, regExp);


            string zone = match.Groups[match.Groups.Count-2].Value;

            return match.Groups.Count > 0 ? new InterpretEventArgs(zone: zone) : null;
        }
    }
}
