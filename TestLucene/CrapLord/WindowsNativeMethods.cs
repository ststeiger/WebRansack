
namespace TestLucene.CrapLord
{
    
    
    
    
    public static class WindowsNativeMethods
    {
        private static readonly System.IntPtr INVALID_HANDLE_VALUE = new System.IntPtr(-1);

        private const uint FILE_READ_EA = 0x0008;
        private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;

        [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern uint GetFinalPathNameByHandle(System.IntPtr hFile,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)] System.Text.StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool CloseHandle(System.IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern System.IntPtr CreateFile(
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)] string filename,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] uint access,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] System.IO.FileShare share,
            System.IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or System.IntPtr.Zero
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] System.IO.FileMode creationDisposition,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] uint flagsAndAttributes,
            System.IntPtr templateFile);

        public static string GetFinalPathName(string path)
        {
            System.IntPtr h = CreateFile(path,
                FILE_READ_EA,
                System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete,
                System.IntPtr.Zero,
                System.IO.FileMode.Open,
                FILE_FLAG_BACKUP_SEMANTICS,
                System.IntPtr.Zero);
            
            if (h == INVALID_HANDLE_VALUE)
                throw new System.ComponentModel.Win32Exception();

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
                uint res = GetFinalPathNameByHandle(h, sb, 1024, 0);
                if (res == 0)
                    throw new System.ComponentModel.Win32Exception();
                
                return sb.ToString();
            }
            finally
            {
                CloseHandle(h);
            }
        }
    }
}