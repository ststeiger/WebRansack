
namespace TestLucene
{
    
    
    // http://www.thebestcsharpprogrammerintheworld.com/2017/09/27/getting-started-with-lucene-net-2-9-2-using-c/
    // http://www.thebestcsharpprogrammerintheworld.com/2017/10/12/how-to-create-and-search-a-lucene-net-index-in-4-simple-steps-using-c-step-1/
    // https://www.codeproject.com/Articles/609980/Small-Lucene-NET-Demo-App
    class Program
    {
        
        // https://antoinevastel.com/bot%20detection/2018/01/17/detect-chrome-headless-v2.html
        // https://www.urbandictionary.com/define.php?term=LART
        // http://texrights.org/2017/02/20/article-about-microsoft-cult-tactics/
        // http://texrights.org/2019/09/12/e-in-epo-for-extortion/
        static void Main(string[] args)
        {
            TestFSE();
            
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 
        
        
        static void TestFSE()
        {
            string path = @"C:\Users\Administrator\Downloads\Lucene_VS2012_Demo_App\Lucene\SimpleLuceneSearch";
            
            // System.IO.Directory.GetFileSystemEntries(path);
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            
            System.IO.FileSystemInfo[] fsis = di.GetFileSystemInfos();
            
            foreach (System.IO.FileSystemInfo fsi in fsis)
            {
                System.Console.WriteLine(fsi.Name);
                System.Console.WriteLine(fsi.FullName);
                
                fsi.IsHidden();
                
                if (fsi.IsDirectory())
                {
                    System.IO.DirectoryInfo dii = (System.IO.DirectoryInfo)fsi;
                }
                else
                {
                    System.IO.FileInfo fi = (System.IO.FileInfo)fsi;
                    fsi.CanRead();
                }
                
            } // Next fsi 
            
        } // End Sub Main 
        
        
    } // End Class Program 
    
    
} // End Namespace TestLucene 
