using System;
using System.Collections.Generic;
using System.Threading;
using LevelHelper.Core.Interpreter;
using LevelHelper.Core.Reader;
using LevelHelper.Core.Messages;
using System.Text;

namespace LevelHelper.Core
{
    public class LevelHelper
    {
        public string CharacterName { get => _charName; }
        public string Level { get => _currentLevel; }
        public string Zone { get => _currentZone; }
        public string AccountName { get => _accountName; }
        public string CharacterClass { get => _charClass; }


        private Dictionary<int,string> levelMessageDictionary = new Dictionary<int, string>();

        private bool _initialized = false;
        private string _currentLevel;
        private string _currentZone;
        private List<IStringInterpreter> interpreter = new List<IStringInterpreter>();
        private LevelupInterpreter levelupInterpreter;
        private ZoneInterpreter zoneInterpreter;
        private GameRunningInterpreter gameRunningInterpreter;
        private FileScanner fileScanner;
        private MessageFileReader  msgFileReader = new MessageFileReader();
        private string _charName;
        private string _charClass;
        private bool _gameRunning = false;
        private bool _webCrawlFinished = false;
        private string _clientDirectory;
        private string _accountName;

        public event EventHandler<InterpretEventArgs> InitializedEvent;
        public event EventHandler<InterpretEventArgs> WebCrawlEvent;
        public event EventHandler<InterpretEventArgs> GameRunningEvent;
        public event EventHandler<InterpretEventArgs> LevelUpEvent;
        public event EventHandler<InterpretEventArgs> ZoneChangeEvent;

        public LevelHelper(string accountName, string installPath)
        {
            _clientDirectory = installPath;
            _accountName = accountName;
        }

        public void Initialize()
        {
            gameRunningInterpreter = new GameRunningInterpreter();
            interpreter.Add(gameRunningInterpreter);
            fileScanner = new FileScanner(_clientDirectory, interpreter);
            fileScanner.InterpreterEvent += OnGameRunning;
        }

        public void StartWebCrawling()
        {
            WebCrawl crawler = new WebCrawl(_accountName, new WebCrawlInterpreter());
            crawler.InterpreterEvent += OnWebCrawl;
            crawler.Crawl();
        }

        public void ReadMesageFiles()
        {
            msgFileReader.ReadAllMessageFiles(_charName);
        }

        public void StartLevelingMonitoring()
        {
            levelupInterpreter = new LevelupInterpreter(_charName);
            zoneInterpreter = new ZoneInterpreter();
            interpreter.Clear();
            interpreter.Add(levelupInterpreter);
            interpreter.Add(zoneInterpreter);
            fileScanner.LineInterpreters = interpreter;
            fileScanner.InterpreterEvent += OnLevelUp;
            fileScanner.InterpreterEvent += OnZoneChange;
            fileScanner.InterpreterEvent -= OnGameRunning;

            _initialized = true;
        }

        public void GetLevelMessages(string key, out LevelMessage flaskMessage, out LevelMessage characterMessage)
        {
            bool hasFlask = false;
            bool hasChar = false;
            flaskMessage = null;
            characterMessage = null;
            for(int i=Int32.Parse(key); i>0; --i)
            {
                if (!hasFlask && msgFileReader.FlaskDict.ContainsKey(i.ToString()))
                {
                    flaskMessage = msgFileReader.FlaskDict[i.ToString()] as LevelMessage;
                    hasFlask = true;
                }
                if(!hasChar && msgFileReader.CharacterDict.ContainsKey(i.ToString()))
                {
                    characterMessage = msgFileReader.CharacterDict[i.ToString()] as LevelMessage;
                    hasChar = true;
                }
                if (hasFlask && hasChar)
                    break;
            }
        }

        public void GetLevelMessages2(string key, out string flaskMessage, out List<string> characterMessages)
        {
            bool hasFlask = false;
            bool hasChar = false;
            flaskMessage = null;
            characterMessages = null;
            StringBuilder flaskStringBuilder = new StringBuilder();
            for (int i = Int32.Parse(key); i > 0; --i)
            {
                if (!hasFlask && msgFileReader.FlaskDict.ContainsKey(i.ToString()))
                {
                    foreach (string flskMsg in msgFileReader.FlaskDict[i.ToString()].GetMessages()) {
                        flaskStringBuilder.AppendLine(flskMsg);
                    }
                    flaskMessage = flaskStringBuilder.ToString();
                    hasFlask = true;
                }
                if (msgFileReader.CharacterDict.ContainsKey(i.ToString()))
                {
                    if (characterMessages == null)
                        characterMessages = msgFileReader.CharacterDict[i.ToString()].GetMessages();
                    else
                        characterMessages.AddRange(msgFileReader.CharacterDict[i.ToString()].GetMessages());
                }
            }
        }

        public void GetZoneMessages(string key, out AreaMessage questMessage, out AreaMessage characterMessage)
        {
            bool hasQuest = false;
            bool hasChar = false;
            questMessage = null;
            characterMessage = null;
            for (int i = Int32.Parse(key); i > 0; --i)
            {
                if (!hasQuest && msgFileReader.QuestDict.ContainsKey(key))
                {
                    questMessage = msgFileReader.QuestDict[key] as AreaMessage;
                    hasQuest = true;
                }
                if (!hasChar && msgFileReader.CharacterDict.ContainsKey(key))
                {
                    characterMessage = msgFileReader.CharacterDict[key] as AreaMessage;
                    hasChar = true;
                }
                if (hasQuest && hasChar)
                    break;
            }
        }

        protected virtual void OnRaiseInitializedEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = InitializedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseLevelEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = LevelUpEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseZoneEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = ZoneChangeEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseWebCrawlEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = WebCrawlEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseGameRunningEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = GameRunningEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        private void OnWebCrawl(object sender, InterpretEventArgs args)
        {
            _charName = args.CharName;
            _currentLevel = args.Level;
            _charClass = args.CharClass;
            _webCrawlFinished = true;
            Console.WriteLine("OnWebCrawl {0}, {1}, {2}", _charName, _currentLevel, _charClass);

            OnRaiseWebCrawlEvent(new InterpretEventArgs(level: _currentLevel, charName: _charName, charClass: _charClass));
        }
        
        private void OnGameRunning(object sender, InterpretEventArgs args)
        {
            _gameRunning = args.GameRunning;
            Console.WriteLine("OnGameRunning");


            OnRaiseGameRunningEvent(new InterpretEventArgs(gameRunning:_gameRunning));
            //            OnRaiseGameRunningEvent(args);
        }

        private void OnLevelUp(object sender, InterpretEventArgs args)
        {
            if (args == null || sender == null || args.Level==null)
                return;
            this._currentLevel = args.Level;
            Console.WriteLine("{0} is level {1}", _charName, _currentLevel);
            OnRaiseLevelEvent(args);
        }

        private void OnZoneChange(object sender, InterpretEventArgs args)
        {
            if (args == null || sender == null || args.Zone == null)
                return;
            this._currentZone = args.Zone;
            OnRaiseZoneEvent(args);
        }
    }
}