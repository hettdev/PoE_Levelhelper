using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelHelper.Core.Messages
{
    public interface IMessage : IComparable
    {
        void AddMessage(string message);
        void AddMessages(List<string> messages);
        List<string> GetMessages();
        string Identifier();
        string ToString(bool includeTrigger);
    }
}
