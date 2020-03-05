
namespace WebRansack
{



    public class WebSocketTextWriter 
        : System.IO.TextWriter
    {

        protected System.Net.WebSockets.WebSocket m_webSocket;
        protected System.Text.StringBuilder m_sb;
        protected System.IFormatProvider m_formatProvider;

        protected byte[] m_emptyByteArray;


        public override System.IFormatProvider FormatProvider
        {
            get
            {
                return this.m_formatProvider;
            }
        }


        public WebSocketTextWriter()
        {
            this.m_sb = new System.Text.StringBuilder();
            this.m_formatProvider = System.Globalization.CultureInfo.InvariantCulture;
            this.m_emptyByteArray = new byte[0];
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


        public override void Write(string value)
        {
            this.m_sb.Append(value);
        }

        public override void Flush()
        {
            this.SendAsync(false).Wait();
        }


        public override async System.Threading.Tasks.Task FlushAsync()
        {
            await this.SendAsync(false);
        }


        //public void Transmit()
        //{
        //    this.SendAsync(true).Wait();
        //}


        public async System.Threading.Tasks.Task TransmitAsync()
        {
            await this.SendAsync(true);
        }


        public void Send(bool endTransmission)
        {
            this.SendAsync(endTransmission).Wait();
        }


        public async System.Threading.Tasks.Task SendAsync(bool endTransmission)
        {
            if (this.m_sb.Length < 1)
            {
                await this.SendAsync(endTransmission, this.m_emptyByteArray);
                return;
            }

            string answer = this.m_sb.ToString();
            this.m_sb.Clear();

            byte[] answerBuffer = this.Encoding.GetBytes(answer);

            await this.SendAsync(endTransmission, answerBuffer);
        }


        public void Send(bool endTransmission, byte[] buffer)
        {
            SendAsync(endTransmission, buffer).Wait();
        }


        public async System.Threading.Tasks.Task SendAsync(bool endTransmission, byte[] buffer)
        {
            await this.m_webSocket.SendAsync(
                  new System.ArraySegment<byte>(buffer, 0, buffer.Length)
                , System.Net.WebSockets.WebSocketMessageType.Text
                , endTransmission
                , System.Threading.CancellationToken.None
            );
        }


        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }


    } // End Class WebSocketTextWriter 


}
