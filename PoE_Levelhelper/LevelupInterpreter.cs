using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PoE_Levelhelper
{
    public class LevelupInterpreter : IStringInterpreter
    {
        public string CharacterName {get; set; }

        public LevelupInterpreter(string characterName)
        {
            CharacterName = characterName;
        }

        public string InterpretLine(string line)
        {
            string level = "";
            if(!line.Contains(CharacterName))
                return null;
            
            string charNameRegex = @"^.*?(?=" + CharacterName + @")";

            level = Regex.Replace(line, charNameRegex, "");
            level = Regex.Replace(level, @"^.*?(?=\))", "");
            level = Regex.Replace(level, @"[^0-9]", "");
            return level;
        }
    }
}