using System;

namespace PoE_Levelhelper
{
    public class InterpretEventArgs : EventArgs
    {
        private string lvl;
        private string charName;
        public string Level 
        {
            get { return lvl; }
        }

        public string CharName
        {
            get { return this.charName; }
        }

        public InterpretEventArgs(string level, string charName = null)
        {
            this.lvl = level;
            this.charName = charName;
        }
    }
}