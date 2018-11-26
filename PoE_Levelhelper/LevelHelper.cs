using System;
using System.Collections.Generic;
using System.Threading;

namespace PoE_Levelhelper
{
    public class LevelHelper
    {
        private Dictionary<int,string> levelMessageDictionary = new Dictionary<int, string>();

        private bool _initialized = false;
        private string _currentLevel;
        private LevelupInterpreter levelupInterpreter;
        private GameRunningInterpreter gameRunningInterpreter;
        private FileScanner fileScanner;
        private string logDir = ""; // DEBUG
        private string _charName;
        private bool _gameRunning = false;
        public LevelHelper(string accountName, string installPath)
        {
            string clientDirectory = installPath + logDir;

            gameRunningInterpreter = new GameRunningInterpreter();
            fileScanner = new FileScanner(clientDirectory, gameRunningInterpreter); 
            fileScanner.InterpreterEvent += OnGameRunning;
            fileScanner.InterpreterEvent += OnLevelUp;
            // do nohing before game runs
            while (!_gameRunning)
            {
                Thread.Sleep(10);
            }
            fileScanner.InterpreterEvent -= OnGameRunning;
            levelupInterpreter = new LevelupInterpreter(_charName);
            fileScanner.LineInterpreter = levelupInterpreter;
            
            WebCrawl crawler = new WebCrawl(accountName, new WebCrawlInterpreter());
            crawler.Crawl();
            string currentLevelStr = crawler.Interpret();
            _currentLevel = currentLevelStr;
        }

        private void OnGameRunning(object sender, InterpretEventArgs args)
        {
            _gameRunning = true;
            this._currentLevel = args.Level;
            this._charName = args.CharName;
        }

        private void OnLevelUp(object sender, InterpretEventArgs args)
        {
            this._currentLevel = args.Level;
        }
    }
}