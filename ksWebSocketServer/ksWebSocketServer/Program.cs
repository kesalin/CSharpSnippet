using System;
using System.ServiceProcess;

namespace ksWebSocketServer
{
    class Program
    {
        public static void RunAsConsole()
        {
            var imageServerService = new WebSocketServerService();
            imageServerService.StartImpl();

            var input = Console.ReadLine();
            while (input != "exit")
            {
                input = Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && args[0].Equals("/console", StringComparison.OrdinalIgnoreCase))
            {
                RunAsConsole();
            }
            else
            {
                ServiceBase.Run(new WebSocketServerService());
            }
        }
    }
}
