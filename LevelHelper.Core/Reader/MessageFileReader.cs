using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using LevelHelper.Core.Messages;

namespace LevelHelper.Core.Reader
{
    class MessageFileReader
    {
        string dirPth;
        public Dictionary<string, IMessage> FlaskDict = new Dictionary<string, IMessage>();
        public Dictionary<string, IMessage> QuestDict = new Dictionary<string, IMessage>();
        public Dictionary<string, IMessage> CharacterDict = new Dictionary<string, IMessage>();
        
        public MessageFileReader()
        {
            dirPth = Directory.GetCurrentDirectory() + @"\Resources\MessageFiles\";
        }

        public void ReadAllMessageFiles(string charName)
        {
            List<string> dirs = new List<String>(Directory.EnumerateDirectories(dirPth));
            List<string> files = new List<String>(Directory.EnumerateFiles(dirPth));

            CreateFlaskAndQuestDict(files);

            files.Clear();
            foreach(string dir in dirs)
            {
                string charDirName = dir.Split('\\').Last();
                if(charDirName == charName)
                {
                    List<string> filesInCharDirName = new List<String>(Directory.EnumerateFiles(dirPth + charDirName));
                    files = files.Concat(filesInCharDirName).ToList();
                }
            }

            CreateCharacterSpecificDictionary(files);
        }
        
        public void CreateCharacterSpecificDictionary(List<string> files)
        {
            foreach(string file in files)
            {
                Dictionary<string, IMessage> newDict = CreateDictionaryFromFile(file, "levels", new string[] { "level", "msg" });
                if (newDict == null)
                {
                    newDict = CreateDictionaryFromFile(file, "areas", new string[] { "area", "msg" });
                }
                MergeDictionaries(newDict, ref CharacterDict);
            }
        }

        private void MergeDictionaries(Dictionary<string, IMessage> newDict, ref Dictionary<string, IMessage> originalDict)
        {
            foreach (KeyValuePair<string, IMessage> entry in newDict)
            {
                if (originalDict.ContainsKey(entry.Key))
                {
                    originalDict[entry.Key].AddMessages(entry.Value.GetMessages());
                }
                else
                {
                    originalDict.Add(entry.Key, entry.Value);
                }
            }
        }

        private void CreateFlaskAndQuestDict(List<string> files)
        {
            foreach(string file in files)
            {
                if (file.Contains("Flasks"))
                    CreateFlaskDictionary(file);
                else if (file.Contains("Quests"))
                    CreateQuestDictionary(file);
            }
        }

        private void CreateFlaskDictionary(string file)
        {
            Dictionary<string, IMessage> newDict = CreateDictionaryFromFile(file, "levels", new string[] {"level", "msg"});
            MergeDictionaries(newDict, ref FlaskDict);
        }

        private void CreateQuestDictionary(string file)
        {
            QuestDict = CreateDictionaryFromFile(file, "areas", new string[] { "area", "msg" });
        }

        private Dictionary<string, IMessage> CreateDictionaryFromFile(string file, string type, string[] valueNames )
        {
            Dictionary<string, IMessage> msgDict = new Dictionary<string, IMessage>();

            using (StreamReader sr = File.OpenText(file))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                JObject jo = (JObject)JToken.ReadFrom(reader);
                JToken arr = jo[type];

                if (arr != null)
                {
                    foreach (JToken child in arr.Children())
                    {
                        
                        string v1 = child[valueNames[0]].ToString();    // level or area
                        List<string> v2 = child[valueNames[1]].ToObject<List<string>>();    // msg

                        if (msgDict.ContainsKey(v1))
                        {
                            msgDict[v1].AddMessages(v2);
                        }
                        else
                        {
                            if (type == "levels")
                                msgDict.Add(v1, new LevelMessage(v1, v2));
                            else if (type == "areas")
                                msgDict.Add(v1, new AreaMessage(v1, v2));
                        }
                    }
                }
            }

            return msgDict.Count > 0 ? msgDict : null;
        }
    }
}
