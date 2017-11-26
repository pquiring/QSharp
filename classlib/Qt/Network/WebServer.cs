using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    public interface WebServerHandler {
        void Process(WebRequest request, WebReply reply);
    }
    public class WebServer {
        private TcpServer tcp;
        private SslServer ssl;
        private WebServerHandler handler;

        public void SetHandler(WebServerHandler handler) {
            this.handler = handler;
        }
        public bool Listen(int port = 80, int sslport = 443) {
            if (handler == null) return false;
            if (port != -1) {
                tcp = new TcpServer();
                tcp.OnPending((TcpServer server) => {OnTcpConnect(server);});
                tcp.Listen(port);
            }
            if (sslport != -1) {
                ssl = new SslServer();
                ssl.OnPending((SslServer server) => {OnSslConnect(server);});
                ssl.Listen(sslport);
            }
            return true;
        }

        private void OnTcpConnect(TcpServer server) {
            TcpSocket socket = server.Accept();
            WebThread thread = new WebThread(handler, socket);
            thread.Start();
        }
        private void OnSslConnect(SslServer server) {
            SslSocket socket = server.Accept();
            WebThread thread = new WebThread(handler, socket);
            thread.Start();
        }
    }
    public class WebThread : Thread {
        public WebThread(WebServerHandler handler, TcpSocket socket) {
            String req = new String();
            byte[] ch = new byte[1];
            while (true) {
                int read = socket.Read(ch, 0, 1);
                if (read == 1) req.Append((char)ch[0]);
                if (req.EndsWith("\r\n\r\n")) {
                    String[] lns = req.Split("\r\n");
                    String[] pts = lns[0].Split(' ');  //method url HTTP/version
                    String method = pts[0];
                    String url = pts[1];
                    WebRequest request = new WebRequest(new Url(url));
                    request.SetMethod(WebRequest.GetMethod(method));
                    WebReply reply = new WebServerReply();
                    ByteArray post = socket.ReadAll();
                    request.SetData(post);
                    handler.Process(request, reply);
                    String headerarray = reply.GetHeaders();
                    byte[] headerdata = headerarray.ToByteArray();
                    socket.Write(headerdata, 0, headerdata.Length);
                    ByteArray outarraydata = reply.GetData();
                    byte[] outdata = outarraydata.ToArray();
                    socket.Write(outdata, 0, outdata.Length);
                    req.Length = 0;
                }
            }
        }
        [CPPExtends("QNetworkReply")]
        [CPPClass(
            "public: qint64 readData(char *data, qint64 maxlen) {return 0;}" +
            "public: void abort() {}"
        )]
        private class WebServerReply : WebReply {
        }
    }
}
