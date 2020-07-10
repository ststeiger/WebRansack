
using System;
using DocumentFormat.OpenXml.Drawing.Charts;

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
            
            foreach (string thisFile in EnumerateAllDrivesFiles())
            {
                System.Console.WriteLine(thisFile);
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



        public static void foo()
        {
            // https://stackoverflow.com/questions/3932382/traversing-directories-without-using-recursion
            // http://hg.openjdk.java.net/jdk7/jdk7/jdk/file/00cd9dc3c2b5/src/share/classes/java/util/ArrayDeque.java
            Wintellect.PowerCollections.Deque<System.IO.FileInfo> stack = new Wintellect.PowerCollections.Deque<System.IO.FileInfo>();
            stack.Add(new System.IO.FileInfo("<path>"));

            int n = 0;
            
            stack.

            while(!stack.isEmpty()){

                n++;
                System.IO.FileInfo file = stack.pop();

                System.Console.Error.WriteLine(file);

                System.IO.DirectoryInfo di;
                di.GetFileSystemInfos();
                
                System.IO.FileSystemInfo fi;
                

                System.IO.FileInfo[] files = file.listFiles();

                for(System.IO.FileInfo f: files){

                    if(f.isHidden()) continue;

                    if(f.isDirectory()){
                        stack.push(f);
                        continue;
                    }

                    n++;
                    System.Console.WriteLine((f);


                }

            }

            System.Console.WriteLine(n);
        }



        public static System.Collections.Generic.IEnumerable<string> EnumerateAllDrivesFiles(
            System.Collections.Generic.IEnumerable<string> rootDirectories, System.IO.SearchOption options)
        {
            foreach (string thisRootDirectory in rootDirectories)
            {
                System.Collections.Generic.IEnumerable<string> nu = null;

                try
                {
                    nu = System.IO.Directory.EnumerateFiles(thisRootDirectory, "*.*", options);
                }
                catch (System.Exception foo)
                {
                    System.Console.WriteLine(foo.Message);
                    System.Console.WriteLine(foo.StackTrace);
                    System.Console.WriteLine(thisRootDirectory);
                }
                
                foreach (string thisFile in nu)
                {
                    yield return thisFile;
                } // Next thisFile 
                
            } // Next thisRootDirectory 
            
        } // End Sub GetAllDrivesFiles 


        public static System.Collections.Generic.IEnumerable<string> EnumerateAllDrivesFiles()
        {
            System.Collections.Generic.IEnumerable<string> rootDirectories = EnumerateDrivesRoot();
            return EnumerateAllDrivesFiles(rootDirectories, System.IO.SearchOption.AllDirectories);
        } // End Function GetAllDrivesFiles 


    } // End Class IndexedSearch 


} // End Namespace TestLucene.FileSearch 
