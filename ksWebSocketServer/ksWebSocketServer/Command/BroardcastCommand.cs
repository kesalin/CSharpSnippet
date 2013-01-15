using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ksWebSocketServer.Server;
using ksWebSocketServer.Utils;

namespace ksWebSocketServer.Command
{
    class BroardcastCommand : ICommand
    {
        private readonly CommandData _cmdData;

        public BroardcastCommand(CommandData cmdData)
        {
            _cmdData = cmdData;
        }

        public void Excute()
        {
            // Multi-thread for complicated task.
            //ThreadPool.QueueUserWorkItem(ExcuteImpl, _cmdData);
            ExcuteImpl(_cmdData);
        }

        private static void ExcuteImpl(object content)
        {
            var cmdData = content as CommandData;
            if (null == cmdData)
                return;

            var headerDict = new Dictionary<string, string>
                                 {
                                     {ServerDefines.CommandType, ServerDefines.ToSelf}
                                 };

            var msg = string.Format("Server received:{0}", cmdData.Request.Message);
            RequestManager.Instance().BroardcastMessage(msg);

            //var contentBytes = Encoding.UTF8.GetBytes(msg);
            //var bytes = ResponseDataHelper.GenerateResponseData(headerDict, contentBytes);
            //RequestManager.Instance().BroardcastMessage(bytes);
        }
    }
}
