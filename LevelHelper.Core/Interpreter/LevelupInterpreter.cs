using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LevelHelper.Core.Interpreter
{
    public class LevelupInterpreter : IStringInterpreter
    {
        public string CharacterName {get; set; }

        public LevelupInterpreter(string characterName)
        {
            CharacterName = characterName;
        }

        public InterpretEventArgs InterpretLine(string line)
        {
            string level = "";
            if(!line.Contains(CharacterName))
                return null;

            string regExp = @"(.+)(" + CharacterName + @")(.+)(level)(.)([0-9])";
            Match match = Regex.Match(line, regExp);

            level = match.Groups[match.Groups.Count - 1].Value;

            Console.WriteLine("LevelupInterpreter: level={0}", level);
            return match.Groups.Count > 0 ? new InterpretEventArgs(level: level) : null;
        }
    }
}