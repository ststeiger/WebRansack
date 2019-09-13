
namespace TestLucene
{


    public static class FileSystemInfoExtensions
    {


        public static bool IsDirectory(this System.IO.FileSystemInfo fi)
        {
            return ((fi.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);
        } // End Function IsDirectory 


        public static bool IsHidden(this System.IO.FileSystemInfo fi)
        {
            return ((fi.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden);
        } // End Function IsHidden 


        public static bool CanRead(this System.IO.FileSystemInfo fsi)
        {
            bool ret = false;
            System.IO.FileInfo fi = (System.IO.FileInfo)fsi;

            try
            {
                using (System.IO.FileStream a = fi.OpenRead())
                {
                    ret = a.CanRead;
                }
            }
            catch (System.Exception)
            { }

            return ret;
        } // End Function CanRead 


    } // End Class FileSystemInfoExtensions 


} // End Namespace TestLucene 
