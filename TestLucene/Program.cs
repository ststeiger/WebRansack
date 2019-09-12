
namespace TestLucene
{


    // http://www.thebestcsharpprogrammerintheworld.com/2017/09/27/getting-started-with-lucene-net-2-9-2-using-c/
    // http://www.thebestcsharpprogrammerintheworld.com/2017/10/12/how-to-create-and-search-a-lucene-net-index-in-4-simple-steps-using-c-step-1/
    // https://www.codeproject.com/Articles/609980/Small-Lucene-NET-Demo-App
    class Program
    {


        static void Main(string[] args)
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


            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace TestLucene 
