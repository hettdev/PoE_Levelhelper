using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using LevelHelper.Core.Interpreter;


namespace LevelHelper.Core.Reader
{
    class FileScanner
    {
        public string Path { get; set; }
        public string ClientFile { get; set; }
        private FileSystemWatcher watcher = new FileSystemWatcher();
        protected long fileLength = 0;
        public List<IStringInterpreter> LineInterpreters;
        public event EventHandler<InterpretEventArgs> InterpreterEvent;
        private bool running = false;

        public FileScanner(string directoryPath, List<IStringInterpreter> lineInterpreters)
        {

            LineInterpreters = lineInterpreters;
            Path = directoryPath;
            ClientFile = Path + "Client.txt";

            FileInfo fi = new FileInfo(ClientFile);
            fileLength = fi.Length;

            new Thread(() => CustomWatcher(ClientFile)).Start();
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
                            //                            Console.WriteLine(addedLine);

                            for(int i=LineInterpreters.Count-1; i>=0; --i)
                            {
                                InterpretEventArgs interpreted = LineInterpreters[i].InterpretLine(addedLine);
                                if (interpreted != null)
                                    OnRaiseLevelEvent(interpreted);
                            }

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
    }
}
