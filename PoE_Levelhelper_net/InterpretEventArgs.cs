using System;

namespace PoE_Levelhelper
{
    public class InterpretEventArgs : EventArgs
    {
        private string lvl;
        private string charName;
        private bool gameRunning;
        public string Level 
        {
            get { return lvl; }
        }

        public string CharName
        {
            get { return this.charName; }
        }

        public bool GameRunning
        {
            get { return this.gameRunning; }
        }

        public InterpretEventArgs(string level = "0", string charName = null, bool gameRunning = false)
        {
            this.lvl = level;
            this.charName = charName;
            this.gameRunning = gameRunning;
        }
        
    }
}