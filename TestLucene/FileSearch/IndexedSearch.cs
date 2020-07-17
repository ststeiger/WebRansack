
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
        

        private static bool IsSymLink(string path)
        {
            // get the file attributes for file or directory
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);

            return attr.HasFlag(System.IO.FileAttributes.ReparsePoint);
        } // End Function IsSymLink 



        public static bool IsDirectory(string path)
        {
            if (!System.IO.File.Exists(path) && !System.IO.Directory.Exists(path))
                return false;
            
            // get the file attributes for file or directory
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);
            
            if (attr.HasFlag(System.IO.FileAttributes.Directory))
                return true;

            return false;
        }


        public static bool IsCyclicSymlink(System.IO.FileSystemInfo fi)
        {
            string target = null;
            
            try
            {
                if (!IsSymLink(fi))
                {
                    return false;
                }
                
                target = CrapLord.AbstractNativeMethods.GetSymlinkTarget(fi);
                
                // Invalid symlinks 
                if (string.IsNullOrWhiteSpace(target))
                    return true;

                // A link to a file will never be cyclic - or can it ? 
                // this allows /etc/nginx/sites-enabled/foo => /etc/nginx/sites-available/foo 
                // /opt/microsoft/powershell/7/libssl.so.1.0.0 does not exist...
                if (!IsDirectory(target))
                    return false;

                // if (fi.FullName.StartsWith("/proc", System.StringComparison.OrdinalIgnoreCase)) return true;
                // if (fi.FullName.StartsWith("/sys", System.StringComparison.OrdinalIgnoreCase)) return true;

                // a symlink to the root directory will always be cyclic 
                if (string.Equals("/", target))
                    return true;

                // a symlink to the root directory if root is not / 
                if (string.Equals(System.IO.Path.GetPathRoot(fi.FullName), target))
                    return true;

                // a self-referencing symlink - possible ? 
                if (string.Equals(fi.FullName, target))
                    return true;

                // a symlink that goes somewhere into the parent directory of symlink 
                // e.g. if /foo/bar/foobar/symlink goes to /foo/bar
                if (fi.FullName.StartsWith(target, System.StringComparison.OrdinalIgnoreCase))
                    return true;


                // gets the parent directoy of symlink /home/username/ for /home/username/symlink
                System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(fi.FullName);

                // allow a symlink to another subdirectory of parent directory 
                // eg /opt/java/current to /opt/java/v8/
                // home/mygithub/username ==> home/github/username
                // is this potentially dangerous ? 
                if (target.StartsWith(parent.FullName) && !string.Equals(target, parent.FullName))
                    return false;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(fi.FullName);
                System.Console.WriteLine(target);
                System.Console.WriteLine(e);
                throw;
            }
            
            return true;
        } // End Function IsCyclicSymlink 
        

        // https://stackoverflow.com/questions/45132081/file-permissions-on-linux-unix-with-net-core
        private static bool DirectoryHasPermission_Unix(System.IO.DirectoryInfo di, System.Security.AccessControl.FileSystemRights AccessRight)
        {
            int res = CrapLord.MonoSux.access(di.FullName, CrapLord.MonoSux.AccessModes.F_OK | CrapLord.MonoSux.AccessModes.R_OK);
            return res == 0;
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
                System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();

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
                            System.Console.WriteLine(f.FullName);
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
                        if (".ini".Equals(fi.Extension, System.StringComparison.InvariantCultureIgnoreCase))
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
