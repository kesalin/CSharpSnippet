using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ksWebSocketServer.Command
{
    public class CommandFactory
    {
        private CommandFactory() { }

        public static ICommand CreateCommand(CommandData cmdData)
        {
            switch (cmdData.CommandType)
            {
                case CommandType.ToSelf:
                    return new ToSelfCommand(cmdData);

                case CommandType.Broardcast:
                    return new BroardcastCommand(cmdData);
            }

            return null;
        }
    }

    public interface ICommand
    {
        void Excute();
    }
}
