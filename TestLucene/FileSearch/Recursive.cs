
namespace TestLucene.FileSearch
{


    class Recursive
    {


        //Recursive File and Folder Listing VB.NET
        public static bool RecursiveSearch(string path)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path);
            //System.IO.FileSystemInfo fileObject = default(System.IO.FileSystemInfo);
            foreach (System.IO.FileSystemInfo fsiThisEntityInfo in dirInfo.GetFileSystemInfos())
            {

                if (fsiThisEntityInfo.Attributes == System.IO.FileAttributes.Directory)
                {
                    //Console.WriteLine("Searching directory " + fsiThisEntityInfo.FullName);
                    RecursiveSearch(fsiThisEntityInfo.FullName);
                }
                else
                {
                    //Console.WriteLine(fsiThisEntityInfo.FullName);
                }

            } // Next fiThisFileInfo

            return true;
        } // End Function RecursiveSearch


        public static System.Collections.Generic.List<string> RecursiveSearch2(string path)
        {
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();
            RecursiveSearch2(path, ls);

            return ls;
        }



        //Recursive File and Folder Listing VB.NET
        public static bool RecursiveSearch2(string path, System.Collections.Generic.List<string> ls)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path);
            //System.IO.FileSystemInfo fileObject = default(System.IO.FileSystemInfo);

            if (!dirInfo.IsDirectory())
                ls.Add(dirInfo.FullName);


            foreach (System.IO.FileSystemInfo fsiThisEntityInfo in dirInfo.GetFileSystemInfos())
            {

                if (fsiThisEntityInfo.Attributes == System.IO.FileAttributes.Directory)
                {
                    //Console.WriteLine("Searching directory " + fsiThisEntityInfo.FullName);
                    RecursiveSearch2(fsiThisEntityInfo.FullName, ls);
                }
                else
                {
                    ls.Add(fsiThisEntityInfo.FullName);
                    //Console.WriteLine(fsiThisEntityInfo.FullName);
                }

            } // Next fiThisFileInfo



            return true;
        } // End Function RecursiveSearch



    }


}
