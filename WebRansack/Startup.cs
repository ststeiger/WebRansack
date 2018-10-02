
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace WebRansack
{


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }


        public class WebSocketTextWriter : System.IO.TextWriter
        {

            System.Net.WebSockets.WebSocket m_webSocket;
            System.Text.StringBuilder m_sb;


            public WebSocketTextWriter()
            {
                this.m_sb = new System.Text.StringBuilder();
            }

            public WebSocketTextWriter(System.Net.WebSockets.WebSocket webSocket)
                : this()
            {
                this.m_webSocket = webSocket;
            }


            public override void Write(char value)
            {
                this.m_sb.Append(value);
            }


            public override void Flush()
            {
                this.SendAsync(false).Wait();
            }


            public override async Task FlushAsync()
            {
                await this.SendAsync(false);
            }


            public void Transmit()
            {
                this.SendAsync(true).Wait();
            }


            public async Task TransmitAsync()
            {
                await this.SendAsync(true);
            }


            public void Send(bool endTransmission)
            {
                this.SendAsync(endTransmission).Wait();
            }


            public async Task SendAsync(bool endTransmission)
            {
                string answer = this.m_sb.ToString();
                this.m_sb.Clear();

                byte[] answerBuffer = this.Encoding.GetBytes(answer);

                await this.SendAsync(endTransmission, answerBuffer);
            }


            public void Send(bool endTransmission, byte[] buffer)
            {
                SendAsync(endTransmission, buffer).Wait();
            }


            public async Task SendAsync(bool endTransmission, byte[] buffer)
            {
                await this.m_webSocket.SendAsync(
                      new ArraySegment<byte>(buffer, 0, buffer.Length)
                    , System.Net.WebSockets.WebSocketMessageType.Text
                    , endTransmission
                    , System.Threading.CancellationToken.None
                );
            }


            public override System.Text.Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }


        }




        private async Task Ransack(Microsoft.AspNetCore.Http.HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            SearchArguments searchArguments = null;
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] buffer = new byte[1024 * 4];
                System.Net.WebSockets.WebSocketReceiveResult result = null;

                do
                {
                    result = await webSocket.ReceiveAsync(
                         new ArraySegment<byte>(buffer)
                       , System.Threading.CancellationToken.None
                    );

                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                // string json = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                // searchArguments = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchArguments>(json);

                ms.Position = 0;


                using (System.IO.TextReader tr = new System.IO.StreamReader(ms, System.Text.Encoding.UTF8))
                {
                    using (Newtonsoft.Json.JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(tr))
                    {
                        try
                        {
                            searchArguments = serializer.Deserialize<SearchArguments>(jsonReader);
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine(ex.Message);
                        }

                    } // End Using jsonReader 

                } // End Using tr 

            } // End Using ms 

            // Write result
            using (WebSocketTextWriter wtw = new WebSocketTextWriter(webSocket))
            {
                System.Collections.Generic.List<SearchResult> ls = FileSearch.SearchContent(searchArguments);

                using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(wtw))
                {
                    serializer.Serialize(jsonWriter, ls);
                    jsonWriter.Flush();
                }

                wtw.Send(true);
            } // End Using wtw 

            serializer = null;
        }
        


        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-2.1
        private async Task Echo(Microsoft.AspNetCore.Http.HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            WebSocketTextWriter wtw = new WebSocketTextWriter(webSocket);

            byte[] buffer = new byte[1024 * 4];
            System.Net.WebSockets.WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer)
                , System.Threading.CancellationToken.None
            );


            while (!result.CloseStatus.HasValue)
            {
                //string answer = @"The server received the following message: ";
                //byte[] answerBuffer = System.Text.Encoding.UTF8.GetBytes(answer);

                //await webSocket.SendAsync(
                //      new ArraySegment<byte>(answerBuffer, 0, answerBuffer.Length)
                //    , System.Net.WebSockets.WebSocketMessageType.Text
                //    , false, System.Threading.CancellationToken.None
                //);

                wtw.WriteLine("Test 123");
                wtw.Transmit();
                wtw.WriteLine("Test 456");
                wtw.Transmit();

                wtw.WriteLine("Echo: ");
                wtw.Flush();

                wtw.Write(@"The server received the following message: ");
                wtw.Flush();

                // wtw.Send(false, new byte[0]);

                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, result.Count)
                    , result.MessageType
                    , result.EndOfMessage
                    , System.Threading.CancellationToken.None
                );

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            WebSocketOptions webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/openfolder")
                {
                    //System.Diagnostics.Process.Start("explorer.exe", "file:///D:/");

                    System.Uri uri = new System.Uri(@"D:\temp\SQL\COR_Basic_Demo_V4_sts.bak");
                    string fileUri = uri.AbsoluteUri;

                    // turns a Uri back into a local filepath too for anyone that needs this.
                    // string path = new System.Uri("file:///C:/whatever.txt").LocalPath;

                    System.IO.FileAttributes attr = System.IO.File.GetAttributes(uri.LocalPath);

                    if (attr.HasFlag(System.IO.FileAttributes.Directory))
                        System.Console.WriteLine("Its a directory");
                    else
                        System.Console.WriteLine("Its a file");



                    if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        // nautilus <path_to_file>
                        // activate window
                        // gnome-open PATH
                        // xdg-open file

                        // https://github.com/mono/dbus-sharp
                        // https://developers.redhat.com/blog/2017/09/18/connecting-net-core-d-bus/
                        // https://unix.stackexchange.com/questions/202214/how-can-i-open-thunar-so-that-it-selects-specific-file

                        using (System.Diagnostics.Process.Start("open", fileUri)) { }
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


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ransack")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Ransack(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


    }


}
