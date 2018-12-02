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
        private bool running = false;

        public FileScanner(string directoryPath, IStringInterpreter lineInterpreter)
        {

            LineInterpreter = lineInterpreter;
            Path = directoryPath;
            ClientFile = Path + "Client.txt";

            FileInfo fi = new FileInfo(ClientFile);
            fileLength = fi.Length;

            new Thread(() => CustomWatcher(ClientFile)).Start();

            //watcher.Path = Path;

            //watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess;
            //watcher.Filter = "Client.txt";
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.EnableRaisingEvents = true;
            //Console.WriteLine("created watcher for {0}", watcher.Path + watcher.Filter);
        }

        private void CustomWatcher(string path)
        {
            running = true;
            while (running)
            {
                FileInfo fi = new FileInfo(ClientFile);
                //                Console.WriteLine(fi.Length);
                if (fi.Length > fileLength)
                {
                    FileInfo newFI = fi.CopyTo("ClientCopy", true);
                    using (FileStream fs = newFI.OpenRead())
                    {
                        fs.Seek(fileLength, SeekOrigin.Begin);
                        int size = (int)(fs.Length - fileLength);
                        if (size > 0)
                        {
                            byte[] data = new byte[size];
                            fs.Read(data, 0, size);
                            UTF8Encoding enc = new UTF8Encoding(true);
                            string addedLine = enc.GetString(data);
                            Console.WriteLine(addedLine);

                            InterpretEventArgs interpreted = LineInterpreter.InterpretLine(addedLine);
                            OnRaiseLevelEvent(interpreted);

                        }
                        else
                        {
                            Console.WriteLine("{0} nothing changed in {1}", DateTime.Now, path);
                        }
                    }
                }

                fileLength = fi.Length;
                Thread.Sleep(100);
            }
        }

        protected virtual void OnRaiseLevelEvent(InterpretEventArgs e)
        {
            EventHandler<InterpretEventArgs> handler = InterpreterEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("change");
            /*
            using (FileStream fs = new FileStream(ClientFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Seek(fileLength, SeekOrigin.Begin);
                int size = (int)(fs.Length - fileLength);
                if(size > 0){
                    byte[] data = new byte[size];
                    fs.Read(data, 0, size);
                    UTF8Encoding enc = new UTF8Encoding(true);
                    string addedLine = enc.GetString(data); 
                    Console.WriteLine(addedLine);
                    InterpretEventArgs interpreted = LineInterpreter.InterpretLine(addedLine);
                    OnRaiseLevelEvent(interpreted);
                    fileLength = fs.Length;
                }
            }
            */
        }
    }
}
