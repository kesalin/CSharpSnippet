using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using ksWebSocketServer.Server;
using ksWebSocketServer.Utils;

namespace ksWebSocketServer
{
    public class WebSocketServerService : ServiceBase
    {private WebSocketServer _webSocketServer;

    public WebSocketServerService()
        {
            ServiceName = "WebSocketServerService";
        }

        protected override void OnStart(string[] args)
        {
            StartImpl();
        }

        protected override void OnStop()
        {
            StopImpl();
        }

        // Web Socket Server
        //
        public void StartWebSocketServer()
        {
            if (_webSocketServer == null)
            {
                try
                {
                    _webSocketServer = new WebSocketServer();
                    _webSocketServer.Start();
                }
                catch (Exception ex)
                {
                    Logger.Instance().ErrorWithFormat("Failed to start WebSocketServer. {0}", ex.Message);
                }
            }
        }

        public void StopWebSocketServer()
        {
            if (_webSocketServer != null)
            {
                try
                {
                    _webSocketServer.Stop();
                    _webSocketServer = null;
                }
                catch (Exception ex)
                {
                    Logger.Instance().ErrorWithFormat("Failed to stop WebSocketServer. {0}", ex.Message);
                }
            }
        }

        // Start & Stop
        //
        public void StartImpl()
        {
            StartWebSocketServer();
        }

        public void StopImpl()
        {
            StopWebSocketServer();
        }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private readonly ServiceProcessInstaller _process;
        private readonly ServiceInstaller _service;

        public ProjectInstaller()
        {
            _process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
            _service = new ServiceInstaller
            {
                ServiceName = "WebSocketServerService",
                DisplayName = "WebSocket Server Service"
            };
            Installers.Add(_process);
            Installers.Add(_service);
        }
    }
}
