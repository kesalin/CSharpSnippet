using System;
using Fleck;
using ksWebSocketServer.Utils;

namespace ksWebSocketServer.Command
{
    public enum CommandStatus
    {
        Ready = 0,
        InProcess = 1,
        Canceled = 2,
        Complete = 3
    }

    public enum CommandType
    {
        ToSelf,
        Broardcast,
    }

    // Customize this
    public class RequestData
    {
        public string Message { set; get; }
    }

    public class CommandData
    {
        public IWebSocketConnection Sender { set; get; }
        public CommandType CommandType { set; get; }
        public CommandStatus Status { set; get; }

        public RequestData Request { set; get; }

        public CommandData()
        {
            Request = new RequestData();
            CommandType = CommandType.ToSelf;
            Status = CommandStatus.Ready;
        }

        public static CommandData Parse(string cmd, out string errorInfo)
        {
            var cmdData = new CommandData();
            errorInfo = null;

            var parts = cmd.Split(ServerDefines.CommandSplitChar);
            foreach (var part in parts)
            {
                var index = part.IndexOf('=');
                if (index <= 0 || index + 1 >= part.Length)
                {
                    errorInfo = string.Format("Invalid command parameter:{0}", part);
                    return null;
                }

                var key = part.Substring(0, index).Trim().ToUpper();
                var value = part.Substring(index + 1).Trim();

                switch (key)
                {
                    case ServerDefines.CommandType:
                        if (value.Equals(ServerDefines.ToSelf, StringComparison.OrdinalIgnoreCase))
                        {
                            cmdData.CommandType = CommandType.ToSelf;
                        }

                        else if (value.Equals(ServerDefines.Broardcast, StringComparison.OrdinalIgnoreCase))
                        {
                            cmdData.CommandType = CommandType.Broardcast;
                        }
                        else
                        {
                            errorInfo = string.Format("Invalid command type:{0}", part);
                            return null;
                        }
                            
                        break;

                    case ServerDefines.Message:
                        cmdData.Request.Message = value;
                        break;

                    default:
                        errorInfo = string.Format("Unknown command type.");
                        return null;
                }
            }

            return cmdData;
        }
    }
}
