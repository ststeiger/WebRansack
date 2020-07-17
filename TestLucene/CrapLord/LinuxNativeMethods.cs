
namespace TestLucene.CrapLord
{
    public class LinuxNativeMethods
    {

        public static string ReadLink(string path)
        {
            //4096 characters
            byte[] buffer = new byte[4096]; // 4096: Maximum path length
            Mono.Unix.Native.Syscall.readlink(path, buffer);
            string target = System.Text.Encoding.UTF8.GetString(buffer);
            
            if(target != null)
                target = target.TrimEnd('\0');
            
            return target;
        }
        
        
        public static string ReadLink_crap(string path)
        {
            string target = null;
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Mono.Unix.Native.Syscall.readlink(path, sb);
            target = sb.ToString();
            sb.Clear();
            sb = null;
            if(target != null)
                target = target.TrimEnd('\0');
            
            return target;
        }


    }
}