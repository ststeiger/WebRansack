
namespace TestLucene
{


    public static class FileSystemInfoExtensions
    {

        public static string Get32BitSystemDirectory()
        {
            // https://github.com/mholo65/WebTerm
            // https://github.com/GoogleChromeLabs/carlo
            // https://github.com/gkmo/CarloSharp
            return Get32BitSystemDirectory(false);
        }


        public static string Get32BitSystemDirectory(bool placeInEnvironmentVariable)
        {
            string sysDir = "";

            if (System.Environment.Is64BitOperatingSystem)
                sysDir = System.Environment.ExpandEnvironmentVariables("%windir%\\SysWOW64");
            else
                sysDir = System.Environment.ExpandEnvironmentVariables("%windir%\\System32");

            if (placeInEnvironmentVariable)
                System.Environment.SetEnvironmentVariable(
                    "SYSDIR32", sysDir, System.EnvironmentVariableTarget.User
                );

            return sysDir;
        }



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
