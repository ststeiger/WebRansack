
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


        public static void BuildIndex()
        {
            string path = @"D:\username\Desktop\DesktopArchiv";
            string indexPath = @"D:\lol\lucene";
            System.Collections.Generic.IEnumerable<string> files = System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories);

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
            // IndexFiles();
            SearchPath(@"15-03-_2018_13-49-43.png");
        } // End Sub Test 


    } // End Class IndexedSearch 


} // End Namespace TestLucene.FileSearch 
