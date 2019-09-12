
namespace TestLucene
{


    class DirectoryMonitor
    {


        public static void MonitorDirectory(string path)
        {
            System.IO.FileSystemWatcher fileSystemWatcher = new System.IO.FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.IncludeSubdirectories = true;

            fileSystemWatcher.NotifyFilter = 
                  System.IO.NotifyFilters.LastAccess 
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName 
                | System.IO.NotifyFilters.DirectoryName
            ;

            // FileName = 1, // The name of the file.
            // DirectoryName = 2,// The name of the directory.
            // Attributes = 4, // The attributes of the file or folder.
            // Size = 8, // The size of the file or folder.
            // LastWrite = 16, // The date the file or folder last had anything written to it.
            // LastAccess = 32, // The date the file or folder was last opened.
            // CreationTime = 64,// The time the file or folder was created.
            // Security = 256 // The security settings of the file or folder.

            fileSystemWatcher.Filter = "*.*";

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;

            fileSystemWatcher.Disposed += FileSystemWatcher_Disposed;

            fileSystemWatcher.EnableRaisingEvents = true;
        } // End Sub MonitorDirectory 


        private static void FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Console.WriteLine("File created: {0}", e.Name);
        } // End FileSystemWatcher_Created 


        private static void FileSystemWatcher_Renamed(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Console.WriteLine("File renamed: {0}", e.Name);
        } // End FileSystemWatcher_Renamed 


        private static void FileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Console.WriteLine("File changed: {0}", e.Name);
        } // End FileSystemWatcher_Changed 


        private static void FileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Console.WriteLine("File deleted: {0}", e.Name);
        } // End FileSystemWatcher_Deleted 


        public static void FileSystemWatcher_Disposed(object sender, System.EventArgs e)
        {
            System.IO.FileSystemWatcher fileSystemWatcher = (System.IO.FileSystemWatcher) sender;

            if (fileSystemWatcher != null)
            {
                fileSystemWatcher.Created -= FileSystemWatcher_Created;
                fileSystemWatcher.Renamed -= FileSystemWatcher_Renamed;
                fileSystemWatcher.Changed -= FileSystemWatcher_Changed;
                fileSystemWatcher.Deleted -= FileSystemWatcher_Deleted;
            } // End if (fileSystemWatcher != null) 

            System.Console.WriteLine("called on dispose");
        } // End FileSystemWatcher_Disposed 


        public void Dispose()
        { 
            // avoiding resource leak 
            // watcher.Changed -= FileSystemWatcher_Changed; 
            // this.watcher.Dispose(); 
        } // End Sub Dispose 


    } // End Class DirectoryMonitor 


} // End Namespace TestLucene 
