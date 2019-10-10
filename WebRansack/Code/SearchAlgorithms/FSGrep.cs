
namespace WebRansack.SearchAlgorithms
{


    public class FSGrep
    {


        public FSGrep()
        {
            this.Recursive = true;
        }


        public string RootPath { get; set; }
        public bool Recursive { get; set; }
        public string FileSearchMask { get; set; }
        public string FileSearchLinePattern { get; set; }


        public System.Collections.Generic.IEnumerable<string> GetFileNames()
        {
            if (!System.IO.Directory.Exists(this.RootPath))
                throw new System.ArgumentException("GetFileNames() -- Can't find RootPath!");

            if (string.IsNullOrWhiteSpace(this.FileSearchMask))
                throw new System.ArgumentException("GetFileNames() -- FileSearchPattern is empty; use *.*!");

            System.IO.SearchOption searchOptions = System.IO.SearchOption.AllDirectories;
            if (!Recursive)
                searchOptions = System.IO.SearchOption.TopDirectoryOnly;

            if (FileSearchMask.Contains(','))
            {
                string[] masks = FileSearchMask.Split(',');
                System.Collections.Generic.IEnumerable<string> results = 
                    System.IO.Directory.EnumerateFiles(this.RootPath, masks[0], searchOptions);

                if (masks.Length > 1)
                {
                    for (int index = 1; index < masks.Length; index++)
                    {
                        results = System.Linq.Enumerable.Concat(results, System.IO.Directory.EnumerateFiles(this.RootPath, masks[index], searchOptions));
                    }
                }
                return results;
            }
            else
            {
                return System.IO.Directory.EnumerateFiles(this.RootPath, this.FileSearchMask, searchOptions);
            }
        }


        public System.Collections.Generic.IEnumerable<Result> GetMatchingFiles()
        {
            foreach (string filePath in GetFileNames())
            {
                int lineNumber = 0;
                foreach (string line in System.IO.File.ReadAllLines(filePath))
                {
                    if (System.Text.RegularExpressions.Regex.Match(line, this.FileSearchLinePattern).Success)
                        yield return new Result() { FilePath = filePath, FileName = System.IO.Path.GetFileName(filePath), LineNumber = lineNumber, Line = line };

                    lineNumber++;
                }
            }
        }


        public class Result
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public string Line { get; set; }
            public int LineNumber { get; set; }

            public override string ToString()
            {
                return string.Format("--file {0}:{1}", FilePath, LineNumber);
            }

        } // End Class Result 


    } // End Class FSGrep 


} // End Namespace WebRansack.Code.SearchAlgorithms 
