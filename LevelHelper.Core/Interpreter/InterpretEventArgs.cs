using System;

namespace LevelHelper.Core.Interpreter
{
    public class InterpretEventArgs : EventArgs
    {
        private string lvl;
        private string charName;
        private string charClass;
        private bool gameRunning;
        private string zone;
        public string Level 
        {
            get { return lvl; }
        }

        public string Zone
        {
            get { return zone; }
        }

        public string CharName
        {
            get { return this.charName; }
        }

        public string CharClass
        {
            get { return this.charClass; }
        }

        public bool GameRunning
        {
            get { return this.gameRunning; }
        }

        public InterpretEventArgs(string level = null, string charName = null, bool gameRunning = false, string charClass = null, string zone = null)
        {
            this.lvl = level;
            this.charName = charName;
            this.gameRunning = gameRunning;
            this.charClass = charClass;
            this.zone = zone;
        }
        
    }
}