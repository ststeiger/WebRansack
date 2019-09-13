
namespace TestLucene
{


    class RegexTests
    {

        public static void Test()
        {
            ReplaceGithubUser("TestUser", @"D:\username\Documents\Visual Studio 2017\Projects\xxxPdfSharpCore\.git\config");
        }


        public delegate string LineReplacementCallback_t(string abc);


        public static void ReplaceTextInFile(LineReplacementCallback_t callback)
        {
            System.IO.File.Copy("input.txt", "temp.txt"); // throws if temp.txt exists 

            using (System.IO.TextReader input = System.IO.File.OpenText("temp.txt"))
            {
                using (System.IO.TextWriter output = new System.IO.StreamWriter("input.txt", false))
                {
                    string line;
                    while (null != (line = input.ReadLine()))
                    {
                        // optionally modify line.
                        if (callback != null)
                            line = callback(line);

                        output.WriteLine(line);
                    } // Whend 

                } // End Using output 

            } // End Using input 

            // System.IO.File.Move("output.txt", "input.txt");
            System.IO.File.Delete("temp.txt");
        } // End Sub ReplaceTextInFile 


        public static void ReplaceTextInFile(string fileName, string newText)
        {
            ReplaceTextInFile(fileName, newText, true);
        } // End Sub ReplaceTextInFile 


        public static void ReplaceTextInFile(string fileName, string newText, bool withBackup)
        {
            if (withBackup)
                System.IO.File.Copy(fileName, fileName + ".bak", true);

            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName, enc, true))
            {
                reader.Peek(); // you need this!
                enc = reader.CurrentEncoding;
            } // End Using reader 

            using (System.IO.TextWriter output = new System.IO.StreamWriter(fileName, false, enc))
            {
                output.Write(newText);
                output.Flush();
            } // End Using output 

        } // End Sub ReplaceTextInFile 


        static void ReplaceGithubUser(string userName, string fileName)
        {
            string inputText = System.IO.File.ReadAllText(fileName);
            string attribute = System.Text.RegularExpressions.Regex.Escape("https://" + userName + ":")
                + ".+"
                + System.Text.RegularExpressions.Regex.Escape("@github.com");

            string pattern = @"^(\s*url\s*=\s*)(" + attribute + ")(.*)$";

            // System.Text.RegularExpressions.Regex.Unescape("test");

            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(inputText, pattern,
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase
                | System.Text.RegularExpressions.RegexOptions.Multiline
            );


            if (match.Success)
            {
                System.Console.WriteLine($"Match-Index: {match.Index}");
                System.Console.WriteLine($"Match-Length: {match.Length}");

                foreach (System.Text.RegularExpressions.Capture capture in match.Captures)
                {
                    System.Console.WriteLine("Index={0}, Value={1}", capture.Index, capture.Value);
                } // Next capture 

            } // End if (match.Success) 

#if false
            string crap = System.Text.RegularExpressions.Regex.Replace(inputText, pattern, "https://github.com",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase
                | System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Console.WriteLine(crap);
#endif 


            string outputText = System.Text.RegularExpressions.Regex.Replace(inputText, pattern,
                new System.Text.RegularExpressions.MatchEvaluator(
                    delegate (System.Text.RegularExpressions.Match pmatch)
                    {
                        string r = pmatch.Groups[1].Value + "https://github.com" + pmatch.Groups[3].Value;
                        return r;
                    }
                )
                , System.Text.RegularExpressions.RegexOptions.IgnoreCase
                | System.Text.RegularExpressions.RegexOptions.Multiline
            );

            System.Console.WriteLine(outputText);
            ReplaceTextInFile(fileName, outputText);
        } // End Sub ReplaceGithubUser 


        static void DisplayRegexResult(string input, string pattern)
        {
            System.Text.RegularExpressions.MatchCollection matches =
                System.Text.RegularExpressions.Regex.Matches(input, pattern);

            if (matches.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match thisMatch in matches)
                {

                    foreach (System.Text.RegularExpressions.Group thisGroup in thisMatch.Groups)
                    {
                        System.Console.WriteLine($"thisGroup: {thisGroup.Value}");

                        foreach (System.Text.RegularExpressions.Capture capture in thisGroup.Captures)
                        {
                            System.Console.WriteLine("Index={0}, Value={1}", capture.Index, capture.Value);
                        } // Next capture 

                    } // Next thisGroup

                    foreach (System.Text.RegularExpressions.Capture capture in thisMatch.Captures)
                    {
                        System.Console.WriteLine("Index={0}, Value={1}", capture.Index, capture.Value);
                    } // Next capture 

                } // Next thisMatch 

            } // End if (matches.Count > 0) 

        } // End Sub DisplayRegexResult 


    } // End Class RegexTests 


} // End Namespace TestLucene
