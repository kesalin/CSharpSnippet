using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Fleck;
using ksWebSocketServer.Command;
using ksWebSocketServer.Utils;

namespace ksWebSocketServer.Server
{
    public class RequestManager
    {
        // Singleton
        //
        private RequestManager() { }
        private static readonly RequestManager _instance = new RequestManager();
        public static RequestManager Instance()
        {
            return _instance;
        }

        private Thread _processRequestThread;
        private volatile bool _isExit;

        public void Start()
        {
            _isExit = false;
            if (_processRequestThread == null)
            {
                _processRequestThread = new Thread(ProcessRequestLoop) { Priority = ThreadPriority.Lowest };
                _processRequestThread.Start();
            }
        }

        public void Stop()
        {
            _isExit = true;
            _processRequestThread = null;
        }

        #region Request management

        private readonly object _lockObj = new object();
        private readonly Dictionary<IWebSocketConnection, Queue<CommandData>> _requestDict = new Dictionary<IWebSocketConnection, Queue<CommandData>>();

        public void AddRequest(IWebSocketConnection socket, string cmd)
        {
            if (_isExit)
                return;

            Logger.Instance().InfoWithFormat(" >> Received Command {0} from {1} : {2}.",
                cmd,
                socket.ConnectionInfo.ClientIpAddress,
                socket.ConnectionInfo.Id);

            lock (_lockObj)
            {
                string errorInfo;
                var cmdData = CommandData.Parse(cmd, out errorInfo);
                if (cmdData == null)
                {
                    if (errorInfo == null)
                    {
                        errorInfo = string.Format("Invalid command.");
                    }

                    SendErrorResponse(socket, errorInfo);
                    return;
                }

                cmdData.Sender = socket;

                Queue<CommandData> cmdQueue;
                bool succeed = _requestDict.TryGetValue(socket, out cmdQueue);

                if (succeed && cmdQueue != null)
                {
                    var cmdAlreadyExists = Enumerable.Contains(cmdQueue, cmdData);
                    if (cmdAlreadyExists)
                    {
                        Logger.Instance().Info(" >> Received same command, ignore it.");
                    }
                    else
                    {
                        cmdQueue.Enqueue(cmdData);
                        _requestDict[socket] = cmdQueue;
                    }
                }
                else
                {
                    cmdQueue = new Queue<CommandData>();
                    cmdQueue.Enqueue(cmdData);
                    _requestDict.Add(socket, cmdQueue);
                }
            }
        }

        public void CancelRequest(IWebSocketConnection socket)
        {
            if (_requestDict.ContainsKey(socket))
            {
                lock (_lockObj)
                {
                    if (_requestDict.ContainsKey(socket))
                    {
                        _requestDict.Remove(socket);
                    }
                }
            }
        }

        private static void SendErrorResponse(IWebSocketConnection socket, string errorInfo)
        {
            Logger.Instance().ErrorWithFormat("{0}:{1}, {2}", socket.ConnectionInfo.ClientIpAddress, socket.ConnectionInfo, errorInfo);

            var header = string.Format("{0}={1}", ServerDefines.CommandType, ServerDefines.Error);
            var headerBytes = Encoding.UTF8.GetBytes(header);
            var bodyBytes = string.IsNullOrEmpty(errorInfo) ? null : Encoding.UTF8.GetBytes(errorInfo);
            var bytes = ResponseDataHelper.GenerateResponseData(headerBytes, bodyBytes);
            socket.Send(bytes);
        }

        public void BroardcastMessage(byte[] message)
        {
            if (_requestDict.Count > 0)
            {
                lock (_lockObj)
                {
                    if (_requestDict.Count > 0)
                    {
                        foreach (var clientConnection in _requestDict.Keys)
                        {
                            clientConnection.Send(message);
                        }
                    }
                }
            }
        }

        public void BroardcastMessage(string message)
        {
            if (_requestDict.Count > 0)
            {
                lock (_lockObj)
                {
                    if (_requestDict.Count > 0)
                    {
                        foreach (var clientConnection in _requestDict.Keys)
                        {
                            clientConnection.Send(message);
                        }
                    }
                }
            }
        }

        #endregion

        #region Process request loop

        private CommandData GetNextCommandData()
        {
            CommandData cmdData = null;
            if (_requestDict.Count > 0)
            {
                lock (_lockObj)
                {
                    if (_requestDict.Count > 0)
                    {
                        foreach (var pair in _requestDict)
                        {
                            var key = pair.Key;
                            var quene = pair.Value;
                            if (quene.Count > 0)
                            {
                                cmdData = quene.Dequeue();
                            }

                            if (quene.Count == 0)
                            {
                                _requestDict.Remove(key);
                            }

                            if (cmdData != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return cmdData;
        }

        private void ProcessCommand(CommandData cmdData)
        {
            Logger.Instance().InfoWithFormat(" >> Process Command {0} from {1}:{2}",
                cmdData.CommandType,
                cmdData.Sender.ConnectionInfo.ClientIpAddress,
                cmdData.Sender.ConnectionInfo.Id);

            var cmdObj = CommandFactory.CreateCommand(cmdData);
            cmdObj.Excute();
        }

        public void ProcessRequestLoop()
        {
            while (!_isExit)
            {
                var cmdData = GetNextCommandData();
                if (cmdData != null)
                {
                    ProcessCommand(cmdData);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        #endregion
    }
}
