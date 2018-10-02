
using Microsoft.AspNetCore.Builder;


namespace WebRansack
{


    public static class OpenFolderOrFileExtension
    {

        
        public static void UseOpenFolderOrFileExtensions(
             this Microsoft.AspNetCore.Builder.IApplicationBuilder app
            ,string path)
        {

            app.Use(async (context, next) =>
            {

                if (context.Request.Path.Equals(new Microsoft.AspNetCore.Http.PathString(path), System.StringComparison.InvariantCultureIgnoreCase))
                {
                    //System.Diagnostics.Process.Start("explorer.exe", "file:///D:/");

                    System.Uri uri = new System.Uri(@"D:\temp\SQL\COR_Basic_Demo_V4_sts.bak");
                    if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                        uri = new System.Uri(@"/root/Downloads/built-compare.zip");

                    string fileUri = uri.AbsoluteUri;

                    // turns a Uri back into a local filepath too for anyone that needs this.
                    // string path = new System.Uri("file:///C:/whatever.txt").LocalPath;

                    System.IO.FileAttributes attr = System.IO.File.GetAttributes(uri.LocalPath);

                    if (attr.HasFlag(System.IO.FileAttributes.Directory))
                        System.Console.WriteLine("Its a directory");
                    else
                        System.Console.WriteLine("Its a file");



                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                    {
                        //  open -a Finder myTextFile.txt.
                        using (System.Diagnostics.Process.Start("open", "-a Finder \"" + fileUri + "\"")) { }
                    }
                    else if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                    {
                        // nautilus <path_to_file>
                        // activate window
                        // gnome-open PATH

                        // open zip file in archiver
                        // xdg-open file


                        // open explorer window with file
                        // nautilus filename

                        // https://github.com/mono/dbus-sharp
                        // https://developers.redhat.com/blog/2017/09/18/connecting-net-core-d-bus/
                        // https://unix.stackexchange.com/questions/202214/how-can-i-open-thunar-so-that-it-selects-specific-file

                        //  open -a Finder myTextFile.txt.

                        try
                        {
                            using (System.Diagnostics.Process.Start("nautilus1", "\"" + fileUri + "\"")) { }
                        }
                        catch (System.Exception e)
                        {
                            System.Console.WriteLine(e);
                        }

                    }
                    else
                    {
                        // explorer.exe /select,"C:\Folder\subfolder\file.txt"

                        using (System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + fileUri + "\"")) { }
                    }



                    //if (context.WebSockets.IsWebSocketRequest)
                    //{
                    //    System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    //    await Echo(context, webSocket);
                    //}
                    //else
                    //{
                    //    context.Response.StatusCode = 400;
                    //}
                }
                else
                {
                    await next();
                }

            });

        } // End Sub UseOpenFolderOrFileExtensions(this Microsoft.AspNetCore.Builder.IApplicationBuilder app) 


    } // End Class OpenFolderOrFileExtension 


} // End Namespace WebRansack 
