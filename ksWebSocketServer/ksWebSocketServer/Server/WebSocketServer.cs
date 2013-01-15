using System.Collections.Generic;
using System.Text;
using Fleck;
using ksWebSocketServer.Utils;

namespace ksWebSocketServer.Server
{
    public class WebSocketServer
    {
        private Fleck.WebSocketServer _server;

        private readonly object _lockObj = new object();
        private readonly List<IWebSocketConnection> _clientSocketList = new List<IWebSocketConnection>();

        public void Start()
        {
            FleckLog.Level = LogLevel.Debug;

            if (_server == null)
            {
                Logger.Instance().Info(" >> Starting Web Socket Server...");

                // start request manager
                //
                RequestManager.Instance().Start();

                var serverLocation = string.Format("ws://localhost:{0}", 8181);
                _server = new Fleck.WebSocketServer(serverLocation);
                _server.Start(ServerConfig);

                Logger.Instance().InfoWithFormat(" >> Running Web Socket Server {0}.", _server.Location);
            }
        }

        public void Stop()
        {
            Logger.Instance().InfoWithFormat(" >> Stop Web Socket Server {0}.", _server.Location);

            RequestManager.Instance().Stop();

            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }

        public void ServerConfig(IWebSocketConnection socket)
        {
            socket.OnOpen = () => OnOpen(socket);
            socket.OnClose = () => OnClose(socket);
            socket.OnMessage = message => OnMessage(socket, message);
            socket.OnBinary = bytes => OnBinary(socket, bytes);
        }

        public void OnOpen(IWebSocketConnection socket)
        {
            Logger.Instance().InfoWithFormat(" >> client {0} - {1} connected.", socket.ConnectionInfo.ClientIpAddress, socket.ConnectionInfo.Id);
            lock (_lockObj)
            {
                _clientSocketList.Add(socket);
            }
        }

        public void OnClose(IWebSocketConnection socket)
        {
            Logger.Instance().InfoWithFormat(" >> client {0} - {1} disconnected.", socket.ConnectionInfo.ClientIpAddress, socket.ConnectionInfo.Id);

            RequestManager.Instance().CancelRequest(socket);

            // remove client
            //
            lock (_lockObj)
            {
                _clientSocketList.Remove(socket);
            }
        }

        public void OnMessage(IWebSocketConnection socket, string message)
        {
            Logger.Instance().InfoWithFormat(" >> received message: {0} from {1} - {2}",
                message,
                socket.ConnectionInfo.ClientIpAddress,
                socket.ConnectionInfo.Id);

            RequestManager.Instance().AddRequest(socket, message);
        }

        public void OnBinary(IWebSocketConnection socket, byte[] bytes)
        {
            Logger.Instance().InfoWithFormat(">> received {0} bytes from {1} - {2}",
                bytes.Length,
                socket.ConnectionInfo.ClientIpAddress,
                socket.ConnectionInfo.Id);

            var message = Encoding.UTF8.GetString(bytes);
            RequestManager.Instance().AddRequest(socket, message);
        }

    }
}
