using System;
using System.Net.Http;
using System.IO;

namespace PoE_Levelhelper
{
    class WebCrawl
    {
        private string _urlBase
        {
            get { return "https://www.pathofexile.com/account/view-profile/"; }
        }
        private string _urlEnd
        {
            get { return "/characters"; }
        }

        private string _url;
        public IStringInterpreter Interpreter;
        public event EventHandler<InterpretEventArgs> InterpreterEvent;
        public WebCrawl(string accountName, IStringInterpreter interpreter)
        {
            _url = _urlBase + accountName + _urlEnd;
            Interpreter = interpreter;
        }

        public void Crawl(string fileToSave = null)
        {
            HttpClient client = new HttpClient();
            string  answer = client.GetStringAsync(_url).Result;
            if(fileToSave != null)
            {
                System.IO.File.WriteAllText(fileToSave,answer);
            }
            Interpreter.InterpretLine(answer);
        }

        public string Interpret()
        {
            throw new NotImplementedException();
            return null;
        }
    }
}