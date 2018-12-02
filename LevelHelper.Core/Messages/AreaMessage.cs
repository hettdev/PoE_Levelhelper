using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelHelper.Core.Messages
{
    public class AreaMessage : IMessage

    {
        public string Area { get; private set; }
        public List<string> Messages { get; private set; }

        public AreaMessage(string area, string message)
        {
            this.Area = area;
            this.Messages = new List<string>();
            this.Messages.Add(message);
        }

        public AreaMessage(string area, List<string> messages)
        {
            this.Area = area;
            this.Messages = messages;
        }

        public void AddMessage(string message)
        {
            if (!Messages.Contains(message))
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
            AreaMessage otherMessage = obj as AreaMessage;
            return this.Area.CompareTo(otherMessage.Area);
        }

        public string Identifier()
        {
            return this.Area;
        }

        public List<string> GetMessages()
        {
            return this.Messages;
        }
    }
}
