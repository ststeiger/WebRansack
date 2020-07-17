
namespace TestLucene.CrapLord
{


    public class LinuxNativeMethods
    {

        public static string ReadLink(string path)
        {
            // 4096 characters path Limux for ext4
            byte[] buffer = new byte[4096]; 

            MonoSux.readlink(path, buffer, (ulong)buffer.Length);
            string target = System.Text.Encoding.UTF8.GetString(buffer);
            
            if(target != null)
                target = target.TrimEnd('\0');
            
            return target;
        } // End Function ReadLink 


        public static string ReadLink_crap(string path)
        {
            string target = null;
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // Mono.Unix.Native.Syscall.readlink(path, sb);
            target = sb.ToString();
            sb.Clear();
            sb = null;
            if(target != null)
                target = target.TrimEnd('\0');
            
            return target;
        } // End Function ReadLink_crap 


    }


}
