
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Mono.Unix;
using Mono.Unix.Native;

namespace TestLucene.FileSearch
{


    public class IndexedSearch
    {


        // https://stackoverflow.com/questions/20980466/lucene-reading-all-field-names-that-are-stored
        private static string[] GetFields(Lucene.Net.Index.IndexReader r)
        {
            int num = r.NumDocs;

            if (num < 1)
                return new string[0];

            System.Collections.Generic.IEnumerable<Lucene.Net.Index.IIndexableField> fields = r.Document(0).Fields;

            int i = 0;
            string[] fieldList = new string[System.Linq.Enumerable.Count(fields)];
            foreach (Lucene.Net.Index.IIndexableField thisField in fields)
            {
                fieldList[i] = thisField.Name;
                ++i;
            } // Next thisField 

            return fieldList;
        } // End Function GetFields 


        // https://stackoverflow.com/questions/5483903/comparison-of-lucene-analyzers
        private static Lucene.Net.Analysis.Analyzer GetWrappedAnalyzer()
        {
            System.Collections.Generic.Dictionary<string, Lucene.Net.Analysis.Analyzer> fieldAnalyzers =
                new System.Collections.Generic.Dictionary<string, Lucene.Net.Analysis.Analyzer>(System.StringComparer.OrdinalIgnoreCase);

            fieldAnalyzers["full_name"] = new Lucene.Net.Analysis.Core.KeywordAnalyzer();
            fieldAnalyzers["directory_name"] = new Lucene.Net.Analysis.Core.KeywordAnalyzer();
            fieldAnalyzers["file_name"] = new Lucene.Net.Analysis.Core.KeywordAnalyzer();
            fieldAnalyzers["filename_no_extension"] = new Lucene.Net.Analysis.Core.KeywordAnalyzer();
            fieldAnalyzers["extension"] = new Lucene.Net.Analysis.Core.KeywordAnalyzer();

            Lucene.Net.Analysis.Miscellaneous.PerFieldAnalyzerWrapper wrapper =
                new Lucene.Net.Analysis.Miscellaneous.PerFieldAnalyzerWrapper(new Lucene.Net.Analysis.Core.KeywordAnalyzer(), fieldAnalyzers);

            return wrapper;
        } // End Function GetWrappedAnalyzer 


        private static void BuildIndex(string indexPath, System.Collections.Generic.IEnumerable<string> dataToIndex)
        {
            Lucene.Net.Util.LuceneVersion version = Lucene.Net.Util.LuceneVersion.LUCENE_48;

            Lucene.Net.Store.Directory luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);


            // Lucene.Net.Analysis.Analyzer analyzer = new Lucene.Net.Analysis.Core.WhitespaceAnalyzer(version);
            // Lucene.Net.Analysis.Analyzer analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(version);
            // Lucene.Net.Analysis.Analyzer analyzer = new Lucene.Net.Analysis.Core.KeywordAnalyzer();
            Lucene.Net.Analysis.Analyzer analyzer = GetWrappedAnalyzer();

            Lucene.Net.Index.IndexWriterConfig writerConfig = new Lucene.Net.Index.IndexWriterConfig(version, analyzer);
            writerConfig.OpenMode = Lucene.Net.Index.OpenMode.CREATE; // Overwrite, if exists

            using (Lucene.Net.Index.IndexWriter writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, writerConfig))
            {

                foreach (string thisValue in dataToIndex)
                {
                    Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();

                    string directory_name = System.IO.Path.GetDirectoryName(thisValue);
                    string file_name = System.IO.Path.GetFileName(thisValue);
                    string filename_no_extension = System.IO.Path.GetFileNameWithoutExtension(thisValue);
                    string extension = System.IO.Path.GetExtension(thisValue);


                    // StringField indexes but doesn't tokenize
                    doc.Add(new Lucene.Net.Documents.StringField("full_name", thisValue, Lucene.Net.Documents.Field.Store.YES));
                    doc.Add(new Lucene.Net.Documents.StringField("directory_name", directory_name, Lucene.Net.Documents.Field.Store.YES));
                    doc.Add(new Lucene.Net.Documents.StringField("file_name", file_name, Lucene.Net.Documents.Field.Store.YES));
                    doc.Add(new Lucene.Net.Documents.StringField("filename_no_extension", filename_no_extension, Lucene.Net.Documents.Field.Store.YES));
                    doc.Add(new Lucene.Net.Documents.StringField("extension", extension, Lucene.Net.Documents.Field.Store.YES));
                    // doc.Add( new Lucene.Net.Documents.TextField("favoritePhrase", thisValue, Lucene.Net.Documents.Field.Store.YES) );


                    writer.AddDocument(doc);
                } // Next thisValue 

                // writer.Optimize();
                writer.Flush(true, true);
            } // Dispose needs to be called, otherwise the index cannot be read ... 

        } // End Sub BuildIndex 


        public static string GetHomeDirectory()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                return System.Environment.GetEnvironmentVariable("HOME");

            // return System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"); // Z:
            // return System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile); // C:\Users
            // return System.Environment.GetFolderPath(System.Environment.SpecialFolder.History); // C:/...
            // return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            // return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            // return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            return System.IO.Path.GetFullPath(
                System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "..")
            );
        }


        public static void BuildIndex()
        {
            string path = @"D:\username\Desktop\DesktopArchiv";
            string indexPath = System.IO.Path.Combine(GetHomeDirectory(), ".Lucene", "TestIndex");

            System.Collections.Generic.IEnumerable<string> all_files = System.Linq.Enumerable.Empty<string>();
            System.Collections.Generic.IEnumerable<string> files =
                System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories);

            all_files = System.Linq.Enumerable.Concat(all_files, files);

            // foreach (string s in files) System.Console.WriteLine(s);

            BuildIndex(indexPath, files);
        } // End Sub BuildIndex 


        // https://lucenenet.apache.org/
        // https://www.codeproject.com/Articles/609980/Small-Lucene-NET-Demo-App
        // https://stackoverflow.com/questions/12600196/lucene-how-to-index-file-names
        private static void SearchPath(string phrase, string indexPath)
        {
            Lucene.Net.Util.LuceneVersion version = Lucene.Net.Util.LuceneVersion.LUCENE_48;
            Lucene.Net.Store.Directory luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);

            Lucene.Net.Index.IndexReader r = Lucene.Net.Index.DirectoryReader.Open(luceneIndexDirectory);

            Lucene.Net.Search.IndexSearcher searcher = new Lucene.Net.Search.IndexSearcher(r);
            Lucene.Net.Analysis.Analyzer analyzer = GetWrappedAnalyzer();

            Lucene.Net.QueryParsers.Classic.QueryParser parser = new Lucene.Net.QueryParsers.Classic.QueryParser(version, "file_name", analyzer);

            // https://stackoverflow.com/questions/15170097/how-to-search-across-all-the-fields
            // Lucene.Net.QueryParsers.Classic.MultiFieldQueryParser parser = new Lucene.Net.QueryParsers.Classic.MultiFieldQueryParser(version, GetFields(r), analyzer);


            Lucene.Net.Search.Query query = parser.Parse(Lucene.Net.QueryParsers.Classic.QueryParser.Escape(phrase));

            Lucene.Net.Search.ScoreDoc[] hits = searcher.Search(query, 10).ScoreDocs;
            foreach (Lucene.Net.Search.ScoreDoc hit in hits)
            {
                Lucene.Net.Documents.Document foundDoc = searcher.Doc(hit.Doc);
                System.Console.WriteLine(hit.Score);
                string full_name = foundDoc.Get("full_name");
                System.Console.WriteLine(full_name);
                // string favoritePhrase = foundDoc.Get("favoritePhrase");
                // System.Console.WriteLine(favoritePhrase);
            } // Next hit 

        } // End Sub SearchPath 


        public static void SearchPath(string phrase)
        {
            string indexPath = @"D:\lol\lucene";
            SearchPath(phrase, indexPath);
        } // End Sub SearchPath 


        public static void Test()
        {
            string ghc = GetHomeDirectory();
            System.Console.WriteLine(ghc);

            // IndexFiles();
            // SearchPath(@"15-03-_2018_13-49-43.png");

            foreach (System.IO.FileInfo thisFile in EnumerateAllDrivesFiles())
            {
                System.Console.WriteLine(thisFile.FullName);
            }

            System.Console.WriteLine("That's all !");
        } // End Sub Test 


        public static System.Collections.Generic.IEnumerable<string> EnumerateDrivesRoot()
        {
            System.IO.DriveInfo[] disks = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo thisDisk in disks)
            {
                if (thisDisk.DriveType != System.IO.DriveType.Fixed)
                    continue;

                if (thisDisk.TotalSize == 0)
                    continue;

                if (thisDisk.RootDirectory == null
                    || thisDisk.RootDirectory.FullName.StartsWith("/snap", System.StringComparison.InvariantCultureIgnoreCase)
                    || thisDisk.RootDirectory.FullName.StartsWith("/boot", System.StringComparison.InvariantCultureIgnoreCase))
                    continue;

                // System.Console.WriteLine(thisDisk.DriveType);
                // System.Console.WriteLine(thisDisk.DriveFormat);
                // System.Console.WriteLine(thisDisk.AvailableFreeSpace);
                // System.Console.WriteLine(thisDisk.TotalSize);
                // System.Console.WriteLine(thisDisk.TotalFreeSpace);
                // System.Console.WriteLine(thisDisk.RootDirectory);

                yield return thisDisk.RootDirectory.FullName;
            } // Next thisDisk 

        } // End Function GetDrivesRoot 
        
        
        private static bool IsSymLink(System.IO.FileSystemInfo pathInfo)
        {
            return pathInfo.Attributes.HasFlag(System.IO.FileAttributes.ReparsePoint);
        } // End Function IsSymLink 
        
        
        public static string GetSymlinkTarget(System.IO.FileSystemInfo di)
        {
            string target = null;
            
            if(!IsSymLink(di))
                throw new System.ArgumentException("\"" + di.FullName+"\" is not a symlink...");
            
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                Mono.Unix.Native.Syscall.readlink(di.FullName, sb);
                target = sb.ToString();
                sb.Clear();
                sb = null;
                if(target != null)
                    target = target.TrimEnd('\0');
            } // End if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            else
                target = CrapLord.WindowsNativeMethods.GetFinalPathName(di.FullName);

            if (string.IsNullOrWhiteSpace(target))
                return null;
            
            target = System.IO.Path.GetFullPath(target);
            return target;
        } // End Function GetSymlinkTarget 
        
        
        public static bool IsCyclicSymlink(System.IO.FileSystemInfo fi)
        {
            if (!IsSymLink(fi))
            {
                return false;
            }

            if (fi.FullName.StartsWith("/proc", StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (fi.FullName.StartsWith("/sys", StringComparison.OrdinalIgnoreCase))
                return true;
            
            string target = GetSymlinkTarget(fi);
            if (string.IsNullOrWhiteSpace(target))
                return true;
            
            return fi.FullName.StartsWith(target, System.StringComparison.OrdinalIgnoreCase);
        } // End Function IsCyclicSymlink 
        

        // https://stackoverflow.com/questions/45132081/file-permissions-on-linux-unix-with-net-core
        private static bool DirectoryHasPermission_Unix(System.IO.DirectoryInfo di, System.Security.AccessControl.FileSystemRights AccessRight)
        {
            int res = Mono.Unix.Native.Syscall.access(di.FullName, AccessModes.F_OK | AccessModes.R_OK);
            return res == 0;
            
            /*
            Mono.Unix.UnixFileInfo unixFileInfo = new Mono.Unix.UnixFileInfo("test.txt");
            // set file permission to 644
            unixFileInfo.FileAccessPermissions =
                Mono.Unix.FileAccessPermissions.UserRead | Mono.Unix.FileAccessPermissions.UserWrite
                | Mono.Unix.FileAccessPermissions.GroupRead
                | Mono.Unix.FileAccessPermissions.OtherRead;
            */
            
            // https://www.geeksforgeeks.org/access-control-listsacl-linux/
            // https://www.tecmint.com/secure-files-using-acls-in-linux/
            
            // grep -i acl /boot/config*
            // nm -D /lib/x86_64-linux-gnu/libacl.so.1 | grep "acl"
            
            // [on RedHat based systems]
            // yum install nfs4-acl-tools acl libacl
            // [on Debian based systems]
            // sudo apt-get install nfs4-acl-tools acl
            
            // cat /proc/mounts
            // df -h | grep " /$"
            // mount | grep -i root
            // ==>
            // mount | grep `df -h | grep " /$" | awk '{print $1}'`
            // tune2fs -l /dev/nvme0n1p2 | grep acl
            // Mono.Unix.Native.Passwd ent = Mono.Unix.Native.Syscall.getpwent();
            // Mono.Unix.Native.Syscall.getgrouplist()
            
            /*
            if (AccessRight == FileSystemRights.Read)
            {
                Mono.Unix.UnixDirectoryInfo unixDirectoryInfo = new Mono.Unix.UnixDirectoryInfo(di.FullName);
                return (unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserRead)
                        || unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupRead)
                        || unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherRead)
                    );
            }
            else if (AccessRight == FileSystemRights.Write)
            {
                Mono.Unix.UnixDirectoryInfo unixDirectoryInfo = new Mono.Unix.UnixDirectoryInfo(di.FullName);
                return (unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserWrite)
                        || unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupWrite)
                        || unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherWrite)
                    );
            }
            else if (AccessRight == FileSystemRights.ExecuteFile)
            {
                Mono.Unix.UnixDirectoryInfo unixDirectoryInfo = new Mono.Unix.UnixDirectoryInfo(di.FullName);
                return (unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserExecute)
                        || unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupExecute)
                        || unixDirectoryInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherExecute)
                    );
            }
            */
            
            throw new System.NotImplementedException("AccessRight for \"" + AccessRight.ToString() + "\"");
        } // End Function DirectoryHasPermission_Unix 
        
        
        /// <summary>
        /// Test a directory for create file access permissions
        /// </summary>
        /// <param name="DirectoryPath">Full path to directory </param>
        /// <param name="AccessRight">File System right tested</param>
        /// <returns>State [bool]</returns>
        public static bool DirectoryHasPermission(System.IO.DirectoryInfo di,
            System.Security.AccessControl.FileSystemRights AccessRight)
        {
            if (!di.Exists)
                return false;
            
            bool ret = false;
            
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                ret = DirectoryHasPermission_Unix(di, AccessRight);
            else
                ret = DirectoryHasPermission_Windows(di, AccessRight);
            
            return ret;
        } // End Function DirectoryHasPermission 
        
        
        /// <summary>
        /// Test a directory for create file access permissions
        /// </summary>
        /// <param name="DirectoryPath">Full path to directory </param>
        /// <param name="AccessRight">File System right tested</param>
        /// <returns>State [bool]</returns>
        public static bool DirectoryHasPermission_Windows(System.IO.DirectoryInfo di, System.Security.AccessControl.FileSystemRights AccessRight)
        {
            if (!di.Exists)
                return false;
            
            // Requires nuget: System.IO.FileSystem.AccessControl 
            try
            {
                System.Security.AccessControl.AuthorizationRuleCollection rules = System.IO.FileSystemAclExtensions.GetAccessControl(di).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                // https://stackoverflow.com/questions/1410127/c-sharp-test-if-user-has-write-access-to-a-folder
                // AccessControlType deny takes precedence over allow, so to be completely thorough rules that deny the access right should be checked as well, 
                foreach (System.Security.AccessControl.FileSystemAccessRule rule in rules)
                {
                    // if (identity.Groups.Contains(rule.IdentityReference))
                    if (identity.Groups.Contains(rule.IdentityReference) || identity.Owner.Equals(rule.IdentityReference))
                    {
                        if ((AccessRight & rule.FileSystemRights) > 0)
                        {
                            if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Deny)
                                return false;
                        }
                    }
                }


                foreach (System.Security.AccessControl.FileSystemAccessRule rule in rules)
                {
                    // if (identity.Groups.Contains(rule.IdentityReference))
                    if (identity.Groups.Contains(rule.IdentityReference) || identity.Owner.Equals(rule.IdentityReference))
                    {
                        if ((AccessRight & rule.FileSystemRights) == AccessRight)
                        {
                            if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Allow)
                                return true;
                        }
                    }
                }
            }
            catch(System.Exception) 
            { }
            
            return false;
        }
        
        
        public static System.Collections.Generic.IEnumerable<System.IO.FileInfo> IterativelyEnumerate(
              System.IO.DirectoryInfo initialPath,
              System.Func<System.IO.FileInfo, bool> selector
            )
        {
            // https://stackoverflow.com/questions/3932382/traversing-directories-without-using-recursion
            // http://hg.openjdk.java.net/jdk7/jdk7/jdk/file/00cd9dc3c2b5/src/share/classes/java/util/ArrayDeque.java
            System.Collections.Generic.Stack<System.IO.DirectoryInfo> stack = new System.Collections.Generic.Stack<System.IO.DirectoryInfo>();
            stack.Push(initialPath);

            int n = 0;

            string NtAccountName = System.Environment.UserDomainName + @"\" + System.Environment.UserName;

            while (stack.Count != 0)
            {
                n++;
                System.IO.DirectoryInfo di = stack.Pop();
                
                if(IsCyclicSymlink(di))
                    continue;
                
                System.IO.FileSystemInfo[] entries = null;
                try
                {
                    bool hasPerm = DirectoryHasPermission(di, System.Security.AccessControl.FileSystemRights.Read);

                    if (!hasPerm)
                    {
                        System.Console.WriteLine(di.FullName);
                        continue;
                    }

                    entries = di.GetFileSystemInfos();
                }
                catch (System.Exception ex)
                {
                    // "Access to the path 'C:\\Windows\\Temp' is denied."
                    // still one bug: 'C:\ProgramData\Microsoft\NetFramework\BreadcrumbStore'
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                    System.Console.WriteLine(di.FullName);
                    continue;
                }



                foreach (System.IO.FileSystemInfo f in entries)
                {
                    if (f.IsDirectory())
                    {
                        System.IO.DirectoryInfo dir = f as System.IO.DirectoryInfo;
                        if(dir != null)
                            stack.Push(dir);
                        else
                        {
                            Console.WriteLine(f.FullName);
                        }
                        
                        continue;
                    }
                    
                    // if (IsSymLink(f)) continue;
                    if(IsCyclicSymlink(f))
                        continue;
                    
                    // if (f.IsHidden()) continue;

                    n++;
                    System.IO.FileInfo fi = (System.IO.FileInfo)f;


                    if (selector != null && !selector(fi))
                        continue;

                    System.Console.WriteLine(fi.FullName);
                    yield return fi;
                } // Next f 

            } // Whend 

            System.Console.WriteLine(n);
        } // End Function IterativelyEnumerate 


        public static System.Collections.Generic.IEnumerable<System.IO.FileInfo> IterativelyEnumerate(string path, System.Func<System.IO.FileInfo, bool> selector)
        {
            return IterativelyEnumerate(new System.IO.DirectoryInfo(path), selector);
        }

        public static System.Collections.Generic.IEnumerable<System.IO.FileInfo> IterativelyEnumerate(System.IO.DirectoryInfo path)
        {
            return IterativelyEnumerate(path, null);
        }

        public static System.Collections.Generic.IEnumerable<System.IO.FileInfo> IterativelyEnumerate(string path)
        {
            return IterativelyEnumerate(new System.IO.DirectoryInfo(path));
        }



        public static System.Collections.Generic.IEnumerable<System.IO.FileInfo> EnumerateAllDrivesFiles(
            System.Collections.Generic.IEnumerable<string> rootDirectories, System.IO.SearchOption options)
        {
            foreach (string thisRootDirectory in rootDirectories)
            {
                //foreach (string thisFile in System.IO.Directory.EnumerateFiles(thisRootDirectory, "*.*", options))
                //{
                //    yield return thisFile;
                //} // Next thisFile 

                foreach (System.IO.FileInfo thisFile in IterativelyEnumerate(thisRootDirectory,
                    delegate (System.IO.FileInfo fi)
                    {
                        if (".ini".Equals(fi.Extension, StringComparison.InvariantCultureIgnoreCase))
                            return true;

                        return false;
                    })
                    )
                {
                    yield return thisFile;
                } // Next thisFile 

            } // Next thisRootDirectory 

            System.Console.WriteLine("Finished");
        } // End Sub GetAllDrivesFiles 


        public static System.Collections.Generic.IEnumerable<System.IO.FileInfo> EnumerateAllDrivesFiles()
        {
            System.Collections.Generic.IEnumerable<string> rootDirectories = EnumerateDrivesRoot();
            return EnumerateAllDrivesFiles(rootDirectories, System.IO.SearchOption.AllDirectories);
        } // End Function GetAllDrivesFiles 


    } // End Class IndexedSearch 


} // End Namespace TestLucene.FileSearch 
