
using Microsoft.AspNetCore.Builder;


namespace WebRansack
{


    public static class WebSocketsTableExtensions
    {


        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-2.1
        private static async System.Threading.Tasks.Task GetTableAsync(
              Microsoft.AspNetCore.Http.HttpContext context
            , System.Net.WebSockets.WebSocket webSocket)
        {
            WebSocketTextWriter wtw = new WebSocketTextWriter(webSocket);

            byte[] buffer = new byte[1024 * 4];
            System.Net.WebSockets.WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                new System.ArraySegment<byte>(buffer)
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
                await System.Threading.Tasks.Task.Delay(3000);
                
                wtw.WriteLine("Test 456");
                wtw.Transmit();
                await System.Threading.Tasks.Task.Delay(3000);


                using (Newtonsoft.Json.JsonTextWriter jsonWriter =
                    new Newtonsoft.Json.JsonTextWriter(wtw))
                {
                    
                }




                wtw.WriteLine("Echo: ");
                wtw.Flush();
                await System.Threading.Tasks.Task.Delay(3000);
                
                wtw.Write(@"The server received the following message: ");
                wtw.Flush();
                await System.Threading.Tasks.Task.Delay(3000);
                // wtw.Send(false, new byte[0]);

                await webSocket.SendAsync(
                    new System.ArraySegment<byte>(buffer, 0, result.Count)
                    , result.MessageType
                    , result.EndOfMessage
                    , System.Threading.CancellationToken.None
                );

                result = await webSocket.ReceiveAsync(
                    new System.ArraySegment<byte>(buffer)
                    , System.Threading.CancellationToken.None
                );
            } // Whend 

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
        } // End Task GetTableAsync 
        
        
        public static void UseTable(this Microsoft.AspNetCore.Builder.IApplicationBuilder app, string path)
        {
            Microsoft.AspNetCore.Http.PathString ps = new Microsoft.AspNetCore.Http.PathString(path);
            
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == ps)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await GetTableAsync(context, webSocket);
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
        } // End Sub UseEcho 


    } // End Class WebSocketsTableExtensions 


} // End Namespace WebRansack 
