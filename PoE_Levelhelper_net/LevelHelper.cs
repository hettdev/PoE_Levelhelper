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
        private bool _webCrawlFinished = false;

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
                Thread.Sleep(100);
//                Console.WriteLine("Game not running");
            }
            Console.WriteLine("Game Running");
            WebCrawl crawler = new WebCrawl(accountName, new WebCrawlInterpreter());
            crawler.InterpreterEvent += OnWebCrawl;
            crawler.Crawl();
            while (!_webCrawlFinished)
            {
                Thread.Sleep(10);
            }
            fileScanner.InterpreterEvent -= OnGameRunning;
            fileScanner.LineInterpreter = new LevelupInterpreter(_charName);
        }

        private void OnWebCrawl(object sender, InterpretEventArgs args)
        {
            _charName = args.CharName;
            _currentLevel = args.Level;
            _webCrawlFinished = true;

        }

        private void OnGameRunning(object sender, InterpretEventArgs args)
        {
            _gameRunning = args.GameRunning;
        }

        private void OnLevelUp(object sender, InterpretEventArgs args)
        {
            this._currentLevel = args.Level;
            Console.WriteLine("{0} is level {1}", _charName, _currentLevel);
        }
    }
}