
namespace WebRansack
{


    public class SearchResult
    {
        public string File;
        public string SearchTerm;
        public string Line;
        public int LineNumber;
        public int CharPos;
        
        
        public SearchResult(string filePath, string line, int lineNumber, int charPos)
        {
            this.File = filePath;
            this.Line = line;
            this.LineNumber = lineNumber;
            this.CharPos = charPos;
        } // End Constructor 
        
        
    } // End Class SearchResult 



    public class SearchArguments
    {
        public string FileName;
        public string ContainingText;
        public string LookIn;
        public bool Subfolders;
    }


    public class FileSearch
    {
        
        
        // /root/github/RedmineMailService/RedmineMailService/Redmine/API.cs (132):   , SecretManager.GetSecret<string>("RedmineSuperUser")
        // /root/github/RedmineMailService/RedmineMailService/Redmine/API.cs (133):   , SecretManager.GetSecret<string>("RedmineSuperUserPassword")
        // /root/github/SchemaPorter/SchemaPorter/FileSearch.cs (55):                    if (line.IndexOf("RedmineSuperUser") != -1)
        // /root/github/CorMine/RedmineClient/RedmineFactory.cs (43):                 , TestPlotly.SecretManager.GetSecret<string>("RedmineSuperUser")
        // /root/github/CorMine/RedmineClient/RedmineFactory.cs (44):                 , TestPlotly.SecretManager.GetSecret<string>("RedmineSuperUserPassword")
        // /root/github/CorMine/CorMine/AppCode/RedmineFactory.cs (42):               , SecretManager.GetSecret<string>("RedmineSuperUser")
        // /root/github/CorMine/CorMine/AppCode/RedmineFactory.cs (43):               , SecretManager.GetSecret<string>("RedmineSuperUserPassword")
        
        
        // SchemaPorter.FileSearch.Test();
        public static void Test()
        {
            string path = @"/root/github";
            path = @"D:\username\Documents\Visual Studio 2017\Projects";
            string searchTerm = @"RedmineSuperUser";
            string pattern = "*.cs";

            SearchArguments searchArguments = new SearchArguments()
            {
                LookIn = path,
                FileName = pattern,
                ContainingText = searchTerm
            };

            System.Collections.Generic.List<SearchResult> ls = SearchContent(searchArguments);
            
            for (int j = 0; j < ls.Count; ++j)
            {
                System.Console.WriteLine(ls[j].File + " ("+ls[j].LineNumber.ToString()+"):\t" + ls[j].Line);
            } // Next j 
            
        } // End Sub Test 
        

        public static System.Collections.Generic.List<SearchResult> SearchContent(
            SearchArguments searchArguments)
        {
            System.Collections.Generic.List<SearchResult> searchResults = new System.Collections.Generic.List<SearchResult>();

            string[] filez = System.IO.Directory.GetFiles(searchArguments.LookIn, searchArguments.FileName, System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < filez.Length; ++i)
            {

                using (System.IO.StreamReader reader = new System.IO.StreamReader(filez[i]))
                {
                    for (int lineNumber = 1; !reader.EndOfStream; ++lineNumber)
                    {
                        string line = reader.ReadLine();
                        int pos = line.IndexOf(searchArguments.ContainingText, System.StringComparison.OrdinalIgnoreCase);

                        if (pos != -1)
                        {
                            searchResults.Add(new SearchResult(filez[i], line, lineNumber, pos));
                        } // End if (pos != -1)

                    } // Whend 

                } // End Using reader

            } // Next i 

            return searchResults;
        } // End Function SearchContent 


        public static System.Collections.Generic.IEnumerable<SearchResult> SearchContent2(SearchArguments searchArguments)
        {
            foreach (string file in System.IO.Directory.EnumerateFiles(searchArguments.LookIn, searchArguments.FileName, System.IO.SearchOption.AllDirectories))
            {

                using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                {
                    for (int lineNumber = 1; !reader.EndOfStream; ++lineNumber)
                    {
                        string line = reader.ReadLine();
                        int pos = line.IndexOf(searchArguments.ContainingText, System.StringComparison.OrdinalIgnoreCase);

                        if (pos != -1)
                        {
                            yield return new SearchResult(file, line, lineNumber, pos);
                        } // End if (pos != -1)

                    } // Whend 

                } // End Using reader

            } // Next file 

        } // End Function SearchContent 


        public static bool ContainsGroup(string file)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
            {
                bool hasAction = false;
                bool hasInput = false;
                bool hasResult = false;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!hasAction)
                    {
                        if (line.StartsWith("ACTION:"))
                            hasAction = true;
                    }
                    else if (!hasInput)
                    {
                        if (line.StartsWith("INPUT:"))
                            hasInput = true;
                    }
                    else if (!hasResult)
                    {
                        if (line.StartsWith("RESULT:"))
                            hasResult = true;
                    }

                    if (hasAction && hasInput && hasResult)
                        return true;
                } // Whend 
                
                return false;
            } // End Using reader
            
            
        } // End Sub ContainsGroup 
        
        
    } // End Class FileSearch 
    
    
} // End Namespace WebRansack 
