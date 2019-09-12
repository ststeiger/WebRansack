
namespace TestLucene
{


    public static class FileSystemInfoExtensions
    {


        public static bool IsDirectory(this System.IO.FileSystemInfo fi)
        {
            return ((fi.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);
        }


        public static bool IsHidden(this System.IO.FileSystemInfo fi)
        {
            return ((fi.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden);
        }


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
        }


    } // End Class FileSystemInfoExtensions 


} // End Namespace TestLucene 
