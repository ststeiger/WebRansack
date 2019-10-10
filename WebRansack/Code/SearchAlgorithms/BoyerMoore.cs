
namespace WebRansack.SearchAlgorithms
{


    // https://github.com/harvey007y/FindTextInFiles
    // https://stackoverflow.com/questions/37979457/fastest-way-to-search-string-in-large-text-file
    // https://www.codeproject.com/Articles/34451/Multithreaded-File-Folder-Finder
    // https://stackoverflow.com/questions/48543469/file-search-optimisation-in-c-sharp-using-parallel
    // https://www.thomashutter.com/facebook-der-unterschied-zwischen-edgerank-und-graphrank/
    // https://stackoverflow.com/questions/48543469/file-search-optimisation-in-c-sharp-using-parallel
    // https://codereview.stackexchange.com/questions/124038/performing-parallel-processing-on-a-file

    
    // https://www.wipfli.com/insights/blogs/connect-microsoft-dynamics-365-blog/improving-processing-performance-parallelforeach
    // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl
    // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl

    // System.Threading.Tasks.Parallel, System.Linq.Parallel, Microsoft.TPL.DataFlow
    // chrome --kiosk "https://www.example.com"
    public class BoyerMoore
    {


        public static int[] SearchString(string str, string pat)
        {
            System.Collections.Generic.List<int> retVal = 
                new System.Collections.Generic.List<int>();

            int m = pat.Length;
            int n = str.Length;

            int[] badChar = new int[256];

            BadCharHeuristic(pat, m, ref badChar);

            int s = 0;
            while (s <= (n - m))
            {
                int j = m - 1;

                while (j >= 0 && pat[j] == str[s + j])
                    --j;

                if (j < 0)
                {
                    retVal.Add(s);
                    s += (s + m < n) ? m - badChar[str[s + m]] : 1;
                }
                else
                {
                    s += System.Math.Max(1, j - badChar[str[s + j]]);
                }
            } // Whend 

            return retVal.ToArray();
        } // End Function SearchString 


        private static void BadCharHeuristic(string str, int size, ref int[] badChar)
        {
            int i;

            for (i = 0; i < 256; i++)
                badChar[i] = -1;

            for (i = 0; i < size; i++)
                badChar[(int)str[i]] = i;
        } // End Sub BadCharHeuristic 


    } // End Class BoyerMoore 


} // End Namespace WebRansack.SearchAlgorithms 
