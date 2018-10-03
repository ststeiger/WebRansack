
using Microsoft.AspNetCore.Builder;


namespace WebRansack
{


    public static class WebSocketsRansackExtensions
    {


        private static async System.Threading.Tasks.Task<T> DeserializeJSON<T>(
              System.Net.WebSockets.WebSocket webSocket
            , Newtonsoft.Json.JsonSerializer serializer)
        {
            T searchArguments = default(T);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] buffer = new byte[1024 * 4];
                System.Net.WebSockets.WebSocketReceiveResult result = null;

                do
                {
                    result = await webSocket.ReceiveAsync(
                         new System.ArraySegment<byte>(buffer)
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
                            searchArguments = serializer.Deserialize<T>(jsonReader);
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine(ex.Message);
                        }

                    } // End Using jsonReader 

                } // End Using tr 

            } // End Using ms 

            return searchArguments;
        } // End Task DeserializeJSON 


        private static async System.Threading.Tasks.Task<T> DeserializeJSON<T>(
            System.Net.WebSockets.WebSocket webSocket)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            T result = await DeserializeJSON<T>(webSocket, serializer);
            serializer = null;

            return result;
        } // End Task DeserializeJSON 


        private static async System.Threading.Tasks.Task Ransack(
            Microsoft.AspNetCore.Http.HttpContext context
            , System.Net.WebSockets.WebSocket webSocket)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            SearchArguments searchArguments = await DeserializeJSON<SearchArguments>(webSocket, serializer);


            string[] fieldNames = LinqHelper.GetFieldAndPropertyNames<SearchResult>();
            Getter_t<SearchResult>[] getters = LinqHelper.GetGetters<SearchResult>(fieldNames);


            using (WebSocketTextWriter wtw = new WebSocketTextWriter(webSocket))
            {

#if true                

                using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(wtw))
                {
                    System.Threading.Tasks.Task wsa = jsonWriter.WriteStartArrayAsync();
                    
                    // jsonWriter.WriteStartArray();
                    
                    int j = 0;
                    foreach (SearchResult thisSearchResult in FileSearch.SearchContent2(searchArguments))
                    {
                        await wsa;


                        // serializer.Serialize(jsonWriter, thisSearchResult);


                        // jsonWriter.WriteStartObject();
                        await jsonWriter.WriteStartObjectAsync();
                        
                        
                                                                        
                        // jsonWriter.WritePropertyName("CharPos");
                        // jsonWriter.WriteValue(thisSearchResult.CharPos);

                        // jsonWriter.WritePropertyName("File");
                        // jsonWriter.WriteValue(thisSearchResult.File);

                        // jsonWriter.WritePropertyName("Line");
                        // jsonWriter.WriteValue(thisSearchResult.Line);

                        // jsonWriter.WritePropertyName("LineNumber");
                        // jsonWriter.WriteValue(thisSearchResult.LineNumber);

                        // jsonWriter.WritePropertyName("SearchTerm");
                        // jsonWriter.WriteValue(thisSearchResult.SearchTerm);

                        /*
                        
                        await jsonWriter.WritePropertyNameAsync("CharPos");
                        await jsonWriter.WriteValueAsync(thisSearchResult.CharPos);
                            
                        await jsonWriter.WritePropertyNameAsync("CharPos");
                        await jsonWriter.WriteValueAsync(thisSearchResult.CharPos);

                        await jsonWriter.WritePropertyNameAsync("File");
                        await jsonWriter.WriteValueAsync(thisSearchResult.File);

                        await jsonWriter.WritePropertyNameAsync("Line");
                        await jsonWriter.WriteValueAsync(thisSearchResult.Line);

                        await jsonWriter.WritePropertyNameAsync("LineNumber");
                        await jsonWriter.WriteValueAsync(thisSearchResult.LineNumber);

                        await jsonWriter.WritePropertyNameAsync("SearchTerm");
                        await jsonWriter.WriteValueAsync(thisSearchResult.SearchTerm);
                        */
                        
                        
                        
                        for (int i = 0; i < getters.Length; ++i)
                        {
                            System.Threading.Tasks.Task wpnt = jsonWriter.WritePropertyNameAsync(fieldNames[i]);
                            object value = getters[i](thisSearchResult);
                            // if (value == System.DBNull.Value) value = null;

                            await wpnt;
                            await jsonWriter.WriteValueAsync(value);
                        } // Next i 
                        


                        // await awso;

                        
                        
                        // jsonWriter.WriteEndObject();
                        System.Threading.Tasks.Task weo = jsonWriter.WriteEndObjectAsync();
                        // await weo;

                        
                        if (j > 0 && j % 200 == 0)
                        {
                            j++;
                            await weo;
                            await jsonWriter.WriteEndArrayAsync();
                            
                            await jsonWriter.FlushAsync();
                            // await wtw.FlushAsync();
                            await wtw.SendAsync(true);
                            
                            await jsonWriter.WriteStartArrayAsync();
                        } // Next j 
                        else
                        {
                            j++;
                            await weo;
                        }
                        
                    } // Next thisSearchResult 

                    await jsonWriter.WriteEndArrayAsync();
                    //jsonWriter.WriteEndArray();
                    
                    await jsonWriter.FlushAsync();
                    // jsonWriter.Flush();
                } // End Using jsonWriter 

#else
                // System.Collections.Generic.List<SearchResult> ls = FileSearch.SearchContent(searchArguments);
                System.Collections.Generic.IEnumerable<SearchResult> ls = FileSearch.SearchContent2(searchArguments);

                using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(wtw))
                {
                    serializer.Serialize(jsonWriter, ls);
                    await jsonWriter.FlushAsync();
                } // End Using jsonWriter 
#endif

                await wtw.SendAsync(true);
            } // End Using wtw 
            
            await webSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Normal closure; the connection successfully completed whatever purpose for which it was created.", System.Threading.CancellationToken.None);
            
            serializer = null;
        } // End Task Ransack 


        public static void UseRansack(
             this Microsoft.AspNetCore.Builder.IApplicationBuilder app
            ,string path)
        {

            app.Use(async (context, next) =>
            {
                
                if (context.Request.Path.Equals(new Microsoft.AspNetCore.Http.PathString(path), System.StringComparison.InvariantCultureIgnoreCase))
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

        } // End Sub UseRansack 


    } // End Class WebSocketsRansackExtensions 


} // End Namespace WebRansack 
