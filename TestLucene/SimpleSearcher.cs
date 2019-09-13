
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;


namespace TestLucene
{


    // https://github.com/abrishu/lucenedemo/blob/master/src/com/pkg/luceneapp/SimpleSearcher.java
    public class SimpleSearcher
    {

        public SimpleSearcher()
        { }


        public static void Main2(string[] args) 
        {
            // File indexDir = new File("/Users/abhinavjha/Documents/index/");
            System.IO.DirectoryInfo path = new System.IO.DirectoryInfo("/Users/abhinavjha/Documents/index/");
            string query = "Car";
            int hits = 100;
            SimpleSearcher searcher = new SimpleSearcher();
            searcher.SearchIndex(path, query, hits, new SimpleAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48));
        } // End Sub Main2 


        public static void Main1(string[] args) 
        {
            System.IO.DirectoryInfo path = new System.IO.DirectoryInfo("/Users/abhinavjha/Documents/index/");
            string query = "App";
            int hits = 100;
            SimpleSearcher searcher = new SimpleSearcher();
            searcher.SearchIndexFromDB(path, query, hits, new SimpleAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48));
        } // End Sub Main1 


        private void SearchIndexFromDB(System.IO.DirectoryInfo indexDir, string queryStr, int maxHits, SimpleAnalyzer analyzer)
        {
            Directory directory = FSDirectory.Open(indexDir);
            IndexReader ireader=DirectoryReader.Open(directory);
            IndexSearcher searcher = new IndexSearcher(ireader);
            QueryParser parser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "appname", analyzer);
            Query query = parser.Parse(queryStr);

            TopDocs topDocs = searcher.Search(query, maxHits);

            
            ScoreDoc[] hits = topDocs.ScoreDocs;
            for (int i = 0; i<hits.Length; i++)
            {
                int docId = hits[i].Doc;
                Document d = searcher.Doc(docId);

                System.Console.WriteLine(d.Get("appname"));
            } // Next i 
        
            System.Console.WriteLine("Found " + hits.Length);
        } // End Sub SearchIndexFromDB 


        private void SearchIndex(System.IO.DirectoryInfo indexDir, string queryStr, int maxHits, SimpleAnalyzer analyzer)
        {
            Directory directory = FSDirectory.Open(indexDir);
            IndexReader ireader=DirectoryReader.Open(directory);
            IndexSearcher searcher = new IndexSearcher(ireader);

            QueryParser parser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "content", analyzer);
            Query query = parser.Parse(queryStr);

            TopDocs topDocs = searcher.Search(query, maxHits);

            ScoreDoc[] hits = topDocs.ScoreDocs;
            for (int i = 0; i<hits.Length; i++)
            {
                int docId = hits[i].Doc;
                Document d = searcher.Doc(docId);
                System.Console.WriteLine(d.Get("filename"));
            } // Next i 

            System.Console.WriteLine("Found " + hits.Length);
        } // End Sub SearchIndex 


    } // End Class SimpleSearcher 


} // End Namespace TestLucene 
