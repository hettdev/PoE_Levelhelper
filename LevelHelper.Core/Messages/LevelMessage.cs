using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelHelper.Core.Messages
{
    public class LevelMessage : IMessage
    {
        public string Level { get; private set; }
        public List<string> Messages { get; private set; } 

        public string Identifier()
        {
            return this.Level;
        }

        public LevelMessage(string level, string message)
        {
            Level = level;
            Messages = new List<string>();
            Messages.Add(message);
        }

        public LevelMessage(string level, List<string> messages)
        {
            Level = level;
            Messages = messages;
        }

        public void AddMessage(string message)
        {
            if(!Messages.Contains(message))
                Messages.Add(message);
        }

        public void AddMessages(List<string> messages)
        {
            foreach (string message in messages)
            {
                if (!this.Messages.Contains(message))
                    Messages.Add(message);
            }
        }

        public int CompareTo(object obj)
        {
            LevelMessage otherMessage = obj as LevelMessage;
            return Int32.Parse(Level).CompareTo(Int32.Parse(otherMessage.Level));
        }

        public List<string> GetMessages()
        {
            return this.Messages;
        }
    }
}
