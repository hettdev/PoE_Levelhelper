using System;
using System.Text.RegularExpressions;

namespace PoE_Levelhelper
{
    class GameRunningInterpreter : IStringInterpreter
    {
        private string commonLine = "Connect time to instance server was";

        public InterpretEventArgs InterpretLine(string line)
        {
            return line.Contains(commonLine)? new InterpretEventArgs(gameRunning: true) : new InterpretEventArgs(gameRunning: false);
        }
    }
}