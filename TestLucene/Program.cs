
using SkiaSharp;

namespace TestLucene
{


    // http://www.thebestcsharpprogrammerintheworld.com/2017/09/27/getting-started-with-lucene-net-2-9-2-using-c/
    // http://www.thebestcsharpprogrammerintheworld.com/2017/10/12/how-to-create-and-search-a-lucene-net-index-in-4-simple-steps-using-c-step-1/
    // https://www.codeproject.com/Articles/609980/Small-Lucene-NET-Demo-App
    class Program
    {

        public static void TestSkia()
        {
            int width = 500;
            int height = 500;

            

            SKRect svgBounds = SKRect.Create(0, 0, 100, 100);

            using (SKFileWStream stream = new SKFileWStream(@"D:\mytestfile.svg")) // there are a few types of streams
            {
                using (SKCanvas canvas = SKSvgCanvas.Create(svgBounds, stream))

                // SKBitmap bitmap = new SKBitmap(width, height);
                // using (SKCanvas canvas = new SKCanvas(bitmap))
                {

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Typeface = SKTypeface.FromFamilyName(null, SKTypefaceStyle.Bold);
                        paint.TextSize = 10;

                        // paint.Style = SKPaintStyle.Stroke;
                        // paint.StrokeWidth = 1;
                        // paint.Color = SKColors.Red;

                        using (SKPath textPath = paint.GetTextPath("CODE", 0, 0))
                        {
                            // Set transform to center and enlarge clip path to window height
                            SKRect bounds;
                            textPath.GetTightBounds(out bounds);

                            // canvas.Translate(width / 2, height/ 2);
                            // canvas.Scale(width / bounds.Width, height / bounds.Height);
                            // canvas.Translate(-bounds.MidX, -bounds.MidY);

                            canvas.Translate(-bounds.Left, -bounds.Top);

                            // Set the clip path
                            // canvas.ClipPath(textPath);
                            canvas.DrawPath(textPath, paint);
                        } // End Using textPath 

                    } // End Using paint 

                } // End Using canvas 

            } // End Using stream 

            // string foo = bitmap.ToString();
            //System.Console.WriteLine(foo);
            System.Console.WriteLine("text");
        }



        // https://antoinevastel.com/bot%20detection/2018/01/17/detect-chrome-headless-v2.html
        // https://www.urbandictionary.com/define.php?term=LART
        // http://texrights.org/2017/02/20/article-about-microsoft-cult-tactics/
        // http://texrights.org/2019/09/12/e-in-epo-for-extortion/
        static void Main(string[] args)
        {
            TestSkia();

            // System.Console.WriteLine(new System.Globalization.CultureInfo("ru-ru").TextInfo.ListSeparator);

            // FileSearch.IndexedSearch.Test();
            
            // TestJsonGeneration().Wait();
            
            // TestFSE();
            // SimpleFileIndexer.Main1();
            // SimpleFileIndexer.Main2();

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


        public static void TestCompleteness()
        {
            string path = @"D:\username\Desktop\DesktopArchiv";

            // FileSearch.Iterative.IterativeSearch1("");
            System.Collections.Generic.List<string> ls = FileSearch.Recursive.RecursiveSearch2(path);
            string[] arr = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);

            System.Console.WriteLine(ls.Count);
            System.Console.WriteLine(arr.Length);
        } // End Sub TestCompleteness 


        public static async System.Threading.Tasks.Task TestJsonGeneration2()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {

                using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

                    // await jsonWriter.WriteStartObjectAsync();


                    jsonWriter.WriteRaw("{");


                    

                    jsonWriter.WriteRaw(System.Environment.NewLine);

                    await jsonWriter.WritePropertyNameAsync("key");
                    await jsonWriter.WriteValueAsync("value");


                    await jsonWriter.WritePropertyNameAsync("key 2");
                    await jsonWriter.WriteValueAsync("value 2");
                    jsonWriter.WriteRaw(System.Environment.NewLine);
                    jsonWriter.WriteRaw("}");
                    jsonWriter.WriteRaw(System.Environment.NewLine);



                    jsonWriter.WriteRaw(",bar:{");
                    jsonWriter.WriteRaw(System.Environment.NewLine);

                    await jsonWriter.WritePropertyNameAsync("key");
                    await jsonWriter.WriteValueAsync("value");


                    await jsonWriter.WritePropertyNameAsync("key 2");
                    await jsonWriter.WriteValueAsync("value 2");
                    jsonWriter.WriteRaw(System.Environment.NewLine);
                    jsonWriter.WriteRaw("}");
                    jsonWriter.WriteRaw(System.Environment.NewLine);

                    // await jsonWriter.WriteEndObjectAsync();

                } // End Using jsonWriter 

            } // End Using sw

            string txt = sb.ToString();
            System.Console.WriteLine(txt);
        } // End Sub TestJsonGeneration2 


        public static async System.Threading.Tasks.Task TestJsonGeneration()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {

                using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;


                    await jsonWriter.WriteStartArrayAsync();

                    await jsonWriter.WriteStartObjectAsync();

                    await jsonWriter.WritePropertyNameAsync("key 1");
                    await jsonWriter.WriteValueAsync("value 1");

                    await jsonWriter.WritePropertyNameAsync("key 2");
                    await jsonWriter.WriteValueAsync("value 2");

                    await jsonWriter.WriteEndObjectAsync();



                    await jsonWriter.WriteStartObjectAsync();

                    await jsonWriter.WritePropertyNameAsync("key 1");
                    await jsonWriter.WriteValueAsync("value 1");

                    await jsonWriter.WritePropertyNameAsync("key 2");
                    await jsonWriter.WriteValueAsync("value 2");

                    await jsonWriter.WriteEndObjectAsync();
                    await jsonWriter.WriteEndArrayAsync();


                } // End Using jsonWriter 

            } // End Using sw

            string txt = sb.ToString();
            System.Console.WriteLine(txt);
        } // End Sub TestJsonGeneration 


        public static async System.Threading.Tasks.Task TestJsonGeneration1()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {

                using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

                    await jsonWriter.WriteStartObjectAsync();

                    await jsonWriter.WritePropertyNameAsync("tables");
                    await jsonWriter.WriteStartArrayAsync();
                    await jsonWriter.WriteEndArrayAsync();

                    await jsonWriter.WriteEndObjectAsync();

                } // End Using jsonWriter 

            } // End Using sw

            string txt = sb.ToString();
            System.Console.WriteLine(txt);
        } // End Sub TestJsonGeneration1 


        static void TestFSE()
        {
            string path = @"D:\Users\username\Downloads\Lucene_VS2012_Demo_App\Lucene\SimpleLuceneSearch";

            // System.IO.Directory.GetFileSystemEntries(path);
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            System.IO.FileSystemInfo[] fsis = di.GetFileSystemInfos();

            foreach (System.IO.FileSystemInfo fsi in fsis)
            {
                System.Console.WriteLine(fsi.Name);
                System.Console.WriteLine(fsi.FullName);

                fsi.IsHidden();

                if (fsi.IsDirectory())
                {
                    System.IO.DirectoryInfo dii = (System.IO.DirectoryInfo)fsi;
                }
                else
                {
                    System.IO.FileInfo fi = (System.IO.FileInfo)fsi;
                    fsi.CanRead();
                }

            } // Next fsi 

        } // End Sub TestFSE 


    } // End Class Program 


} // End Namespace TestLucene 
