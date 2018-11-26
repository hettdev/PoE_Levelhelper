using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;


namespace PoE_Levelhelper
{
    class FileScanner
    {
        public string Path { get; set; }
        public string ClientFile { get; set; }
        private FileSystemWatcher watcher = new FileSystemWatcher();
        protected long fileLength = 0;
        public IStringInterpreter LineInterpreter;
        public event EventHandler<InterpretEventArgs> InterpreterEvent;
        public FileScanner(string directoryPath, IStringInterpreter lineInterpreter)
        {
            
            LineInterpreter = lineInterpreter;
            Path = directoryPath;
            ClientFile = Path + "Client.txt";
            using (FileStream fs = File.OpenRead(ClientFile))
            {
                fileLength = fs.Length;
            }
            watcher.Path = Path;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess;
            watcher.Filter = "Client.txt";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("created watcher for {0}", watcher.Path + watcher.Filter);
        }

        protected virtual void OnRaiseLevelEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = InterpreterEvent;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {   
            using (FileStream fs = File.OpenRead(ClientFile))
            {
                fs.Seek(fileLength, SeekOrigin.Begin);
                int size = (int)(fs.Length - fileLength);
                if(size > 0){
                    byte[] data = new byte[size];
                    fs.Read(data, 0, size);
                    UTF8Encoding enc = new UTF8Encoding(true);
                    string addedLine = enc.GetString(data); 
                    Console.WriteLine(addedLine);
                    string interpreted = LineInterpreter.InterpretLine(addedLine);
                    OnRaiseLevelEvent(new InterpretEventArgs(interpreted));
                    fileLength = fs.Length;
                }
            }
        }
    }
}
