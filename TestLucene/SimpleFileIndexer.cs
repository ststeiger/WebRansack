
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;



namespace TestLucene
{


    // https://github.com/abrishu/lucenedemo/blob/master/src/com/pkg/luceneapp/SimpleFileIndexer.java
    class SimpleFileIndexer
    {

        public SimpleFileIndexer()
        { } // End Constructor 


        public static void Main1()
        {
            SimpleFileIndexer sfi = new SimpleFileIndexer();
            SimpleAnalyzer san = new SimpleAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);
            Directory directory = FSDirectory.Open(new System.IO.DirectoryInfo("/Users/abhinavjha/Documents/index"));
            IndexWriterConfig writerConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, san);
            IndexWriter indexWriter = new IndexWriter(directory, writerConfig);

            sfi.CreateDocument(sfi.PopulateDatabase(), indexWriter);
        } // End Sub Main 


        public static void Main2()
        {
            // File indexDir = new File("/Users/abhinavjha/Documents/index");
            System.IO.DirectoryInfo dataDir = new System.IO.DirectoryInfo("/Users/abhinavjha/Documents/workspace");
            string suffix = "java";
            SimpleFileIndexer sfi = new SimpleFileIndexer();
            SimpleAnalyzer san = new SimpleAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);
            Directory directory = FSDirectory.Open(new System.IO.DirectoryInfo("/Users/abhinavjha/Documents/index"));
            IndexWriterConfig writerConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, san);
            IndexWriter indexWriter = new IndexWriter(directory, writerConfig);

            sfi.IndexTheDataDirectory(indexWriter, dataDir, suffix);

            System.Console.WriteLine(indexWriter.MaxDoc);
            // indexWriter.Close();
            indexWriter.Dispose();
        }


        private void IndexTheDataDirectory(IndexWriter indexWriter, System.IO.DirectoryInfo dataDir, string suffix)
        {
            if (!dataDir.Exists)
                return;

            System.IO.FileInfo[] files = dataDir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                System.IO.FileSystemInfo f = files[i];

                if (f.IsDirectory())
                {
                    IndexTheDataDirectory(indexWriter, (System.IO.DirectoryInfo) f, suffix);
                }
                else
                {
                    IndexFileWithIndexWriter(indexWriter, f, suffix);
                }
            } // Next i 

        } // End Sub IndexTheDataDirectory 


        private System.Data.DataTable PopulateDatabase()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            
            dt.Columns.Add("application_id", typeof(int));
            dt.Columns.Add("application_name", typeof(string));
            dt.Columns.Add("notes", typeof(string));

            for (int i = 0; i < 10; ++i)
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["application_id"] = i;
                dr["application_name"] ="Application #" + i.ToString();
                dr["notes"] = "Note numer " + i.ToString();

                dt.Rows.Add(dr);
            } // Next i 

            /*
            string sql = "SELECT * FROM application";
            using (System.Data.Common.DbDataAdapter da = null)
            {
                da.Fill(dt);
            }
            
            try 
            {
                Class.forName("com.mysql.jdbc.Driver"); 
                Connection con = DriverManager.getConnection("jdbc:mysql://localhost/RiskAssessment", "root", "root"); 
                PreparedStatement pstmt = con.prepareStatement("SELECT * FROM application"); 
                rs = pstmt.executeQuery(); 
            } 
            catch (ClassNotFoundException e)  
            {
                // TODO Auto-generated catch block 
                e.printStackTrace(); 
		    }
            */

            return dt;
        } // End Function PopulateDatabase 


        // index particular file and check if its type matches the suffix
        private void IndexFileWithIndexWriter(IndexWriter indexWriter, System.IO.FileSystemInfo f, string suffix)
        {
            if (f.IsHidden() || f.IsDirectory() || !f.CanRead() || !f.Exists)
            {
                return;
            }

            if (suffix != null && !f.Name.EndsWith(suffix))
            {
                return;
            }

            System.Console.WriteLine("Indexing file " + f.FullName);
            CreateDocument((System.IO.FileInfo)f, indexWriter);
        } // End Sub IndexFileWithIndexWriter 
       

        private void CreateDocument(System.Data.DataTable dt, IndexWriter indexWriter)
        {
            //while (rs.Next())
            foreach (System.Data.DataRow rs in dt.Rows)
            {
                System.Console.WriteLine("Creating document");
                Document dbDocument = new Document();
                dbDocument.Add(new Field("id", System.Convert.ToString(rs["application_id"]), TextField.TYPE_STORED));
                dbDocument.Add(new Field("appname", System.Convert.ToString(rs["application_name"]), TextField.TYPE_STORED));
                dbDocument.Add(new Field("notes", System.Convert.ToString(rs["notes"]), TextField.TYPE_STORED));
                indexWriter.AddDocument(dbDocument);
            } // rs.Close();

            // indexWriter.Close();
            //indexWriter.Dispose(); // the same ? 
        } // End Sub CreateDocument 


        private void CreateDocument(System.IO.FileInfo sjp, IndexWriter indexWriter)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(sjp.OpenRead()))
            {
                Document dictionary = new Document();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                dictionary.Add(new Field("filename", sjp.FullName, TextField.TYPE_STORED));

                string readLine = null;
                while ((readLine = reader.ReadLine()) != null)
                {
                    readLine = readLine.Trim();
                    System.Console.WriteLine(readLine);
                    sb.Append(readLine);
                } // Whend 

                dictionary.Add(new Field("content", sb.ToString(), TextField.TYPE_STORED));
                indexWriter.AddDocument(dictionary);

                reader.Close();
            } // End Using reader 

        } // End Sub CreateDocument 


    } // End Class SimpleFileIndexer 


} // End Namespace TestLucene 
