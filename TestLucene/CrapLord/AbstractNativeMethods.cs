namespace TestLucene.CrapLord
{
    
    
    public class AbstractNativeMethods
    {

        private static string InternalGetSymlinkTarget(System.IO.FileSystemInfo fi)
        {
            string target = null;
            
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                target = CrapLord.LinuxNativeMethods.ReadLink(fi.FullName);
            } // End if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            else
                target = CrapLord.WindowsNativeMethods.GetFinalPathName(fi.FullName);
            
            if (string.IsNullOrWhiteSpace(target))
                return null;
            
            return target;
        }

        private static bool IsSymLink(System.IO.FileSystemInfo pathInfo)
        {
            return pathInfo.Attributes.HasFlag(System.IO.FileAttributes.ReparsePoint);
        } // End Function IsSymLink 
        
        
        // CrapLord.AbstractNativeMethods.GetSymlinkTarget
        public static string GetSymlinkTarget(System.IO.FileSystemInfo fi)
        {
            if(!IsSymLink(fi))
                throw new System.ArgumentException("\"" + fi.FullName+"\" is not a symlink...");
            
            string target = InternalGetSymlinkTarget(fi);
            if (target == null)
                return null;
            
            if (!target.StartsWith("/"))
            {
                System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(fi.FullName);
                target = System.IO.Path.Combine(parent.FullName, target);
            }
            
            target = System.IO.Path.GetFullPath(target);
            return target;
        } // End Function GetSymlinkTarget 
        
        
    }
    
    
}