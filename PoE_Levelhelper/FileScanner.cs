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

        public FileScanner(string directoryPath)
        {
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
            watcher.Renamed += new RenamedEventHandler(OnRename);
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("created watcher for {0}", watcher.Path + watcher.Filter);
        }

        private void OnRename(object source, RenamedEventArgs e)
        {
            Console.WriteLine("OnRenamed");
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("OnChanged");
            
            using (FileStream fs = File.OpenRead(ClientFile))
            {
                fs.Seek(fileLength, SeekOrigin.Begin);
                int size = (int)(fs.Length);
                byte[] data = new byte[size];
                fs.Read(data, 0, size);
                UTF8Encoding enc = new UTF8Encoding(true);
                Console.WriteLine(enc.GetString(data));
                fileLength = fs.Length;
            }
        }
    }
}
