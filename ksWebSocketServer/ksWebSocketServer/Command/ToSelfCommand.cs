using System.Collections.Generic;
using System.Text;
using System.Threading;
using ksWebSocketServer.Utils;

namespace ksWebSocketServer.Command
{
    class ToSelfCommand : ICommand
    {
        private readonly CommandData _cmdData;

        public ToSelfCommand(CommandData cmdData)
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
            cmdData.Sender.Send(msg);
            //var contentBytes = Encoding.UTF8.GetBytes(msg);
            //var bytes = ResponseDataHelper.GenerateResponseData(headerDict, contentBytes);
            //cmdData.Sender.Send(bytes);
        }
    }
}
