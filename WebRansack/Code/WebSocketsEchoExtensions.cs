
using Microsoft.AspNetCore.Builder;


namespace WebRansack
{


    public static class WebSocketsEchoExtensions
    {


        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-2.1
        private static async System.Threading.Tasks.Task Echo(
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
                await wtw.TransmitAsync();
                wtw.WriteLine("Test 456");
                await wtw.TransmitAsync();

                wtw.WriteLine("Echo: ");
                await wtw.FlushAsync();

                wtw.Write(@"The server received the following message: ");
                await wtw.FlushAsync();

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
        } // End Task Echo 


        public static void UseEcho(this Microsoft.AspNetCore.Builder.IApplicationBuilder app)
        {
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
        } // End Sub UseEcho 


    } // End Class WebSocketsEchoExtensions 


} // End Namespace WebRansack 
