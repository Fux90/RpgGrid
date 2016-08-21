using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace NetUtils
{
    public static class CommandsExt
    {
        public static byte[] ToByteArray(this Connections.Commands cmd)
        {
            return new byte[] { (byte)cmd };
        }
    }

    public sealed class Connections
    {
        #region COMMUNICATION_SEMANTICS

        public const string MAP_RECEIVING = "mapRec";
        public const string MAP_SENDING = "mapSend";
        public const string PAWNS_AND_TEMPLATES_RECEIVING = "pawnsAndTemplatesRec";
        public const string PAWNS_AND_TEMPLATES_SENDING = "pawnsAndTemplatesSend";

        #endregion

        private class CommandBehaviour : Attribute
        {
            public Commands Command { get; private set; }

            public CommandBehaviour(Commands command)
            {
                Command = command;
            }
        }

        public enum Commands : byte
        {
            Done,
            Ping,
            Pong,
            InitialDataRequested,
            InitialDataReceived,
            MapReceived,
            CloseChannel,
            ClosedChannel,
        };

        private static Connections current;
        public static Connections Current
        {
            get
            {
                if(current == null)
                {
                    current = new Connections();
                }
                return current;
            }
        }

        private CommunicationModel model;
        public CommunicationModel Model
        {
            get
            {
                if(model == null)
                {
                    throw new Exception("MISSING MODEL");
                }

                return model;
            }

            set
            {
                model = value;
            }
        }

        // port -> socket
        private Dictionary<int, TcpListener> serverListeners;
        private Dictionary<int, TcpClient> serverSocks;

        private TcpClient clientTCP;

        public delegate void ReceivedMessageDelegate(TcpClient tcpClient, BackgroundWorker bwListener);
        private Dictionary<Commands, ReceivedMessageDelegate> behaviourByCommand;

        private Connections()
        {
            serverListeners = new Dictionary<int, TcpListener>();
            serverSocks = new Dictionary<int, TcpClient>();

            initBehaviours();
        }

        private void initBehaviours()
        {
            behaviourByCommand = new Dictionary<Commands, ReceivedMessageDelegate>();

            var behaviours = this.GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m.GetCustomAttributes(typeof(CommandBehaviour)).Count() == 1).ToArray();

            for (int i = 0; i < behaviours.Length; i++)
            {
                var b = behaviours[i];
                var cmd = ((CommandBehaviour)b.GetCustomAttribute(typeof(CommandBehaviour))).Command;
                var del = Delegate.CreateDelegate(typeof(ReceivedMessageDelegate), this, b);
                behaviourByCommand[cmd] = (ReceivedMessageDelegate)del;
            }
        }

        #region BEHAVIOURS

        [CommandBehaviour(Commands.Ping)]
        public void PingBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            MessageBox.Show("I was pinged");
            tcpClient.Client.Send(Commands.Pong.ToByteArray());
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.Pong)]
        public void PongBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            MessageBox.Show("I was ponged back");
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.CloseChannel)]
        public void CloseBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            MessageBox.Show("They want me to be closed");
            tcpClient.Client.Send(Commands.ClosedChannel.ToByteArray());
            tcpClient.Client.Shutdown(SocketShutdown.Both);
            tcpClient.Client.Close();
            tcpClient.Close();
        }

        [CommandBehaviour(Commands.ClosedChannel)]
        public void ClosedChannelBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            tcpClient.Client.Shutdown(SocketShutdown.Both);
            tcpClient.Client.Close();
            tcpClient.Close();
            if(serverSocks.ContainsValue(tcpClient))
            {
                var item = serverSocks.First(kvp => kvp.Value == tcpClient);
                serverSocks.Remove(item.Key);
            }
            MessageBox.Show("Closed!");
        }

        [CommandBehaviour(Commands.InitialDataRequested)]
        public void OnInitialDataRequestedBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            MessageBox.Show("I was asked for initial data");
            tcpClient.Client.Send(Commands.InitialDataReceived.ToByteArray());
            // Send initial data
            var checkpoint = new byte[1];

            // Map
            var mapData = Model.ProcessData(MAP_SENDING, null);
            tcpClient.Client.Send(mapData.Length);
            tcpClient.Client.Send(mapData.Buffer);

            tcpClient.Client.Receive(checkpoint);

            // Pawns and templates
            var pawnsTemplatesData = Model.ProcessData(PAWNS_AND_TEMPLATES_SENDING, null);
            //tcpClient.Client.Send(pawnsTemplatesData.Length);
            //tcpClient.Client.Send(pawnsTemplatesData.Buffer);
            //tcpClient.Client.Receive(checkpoint);

            // -----------------
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.InitialDataReceived)]
        public void OnInitialDataReceivedBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            // Wait for initial data - Can do synch
            Model.ShowProcessing();

            ReceiveMap(tcpClient);
            tcpClient.Client.Send(Commands.Done.ToByteArray());
            //ReceivePawnsAndTemplates(tcpClient);
            //tcpClient.Client.Send(Commands.Done.ToByteArray());

            Model.EndShowProcessing();

            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.MapReceived)]
        public void OnMapReceivedBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            Model.ShowProcessing();

            ReceiveMap(tcpClient);

            Model.EndShowProcessing();
            bwListener.RunWorkerAsync();
        }

        private void ReceiveMap(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(MAP_RECEIVING, buffer);
            }
        }

        private void ReceivePawnsAndTemplates(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(PAWNS_AND_TEMPLATES_RECEIVING, buffer);
            }
        }

        #endregion

        public string InvitePlayer(out Int32 sockID, out BackgroundWorker waitingPlayerBw, out Button closeConnectionButton)
        {
            var port = GeneratePort();
#if LOCAL_HOST_DEBUG
            var localAddr = IPAddress.Parse("127.0.0.1");
#else
            IPAddress localAddr = null;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddr = ip;
                }
            }
#endif
            var s = new TcpListener(localAddr, port);
            
            s.Start();
            var task = s.AcceptTcpClientAsync();
            
            sockID = port;
            serverListeners.Add(port, s);

            closeConnectionButton = new Button();
            var btn = closeConnectionButton;
            closeConnectionButton.Click += (sBtn, eBtn) =>
            {
                CloseByID(port);
                btn.Parent.Controls.Remove(btn);
            };
            closeConnectionButton.Enabled = false;

            waitingPlayerBw = AwaitPlayer(sockID, task, closeConnectionButton);
            var strB = new StringBuilder();
            strB.AppendFormat("{0}:{1}", localAddr.ToString(), port);
            return strB.ToString();
        }

        private int GeneratePort()
        {
            // TODO: Generate random port (one for each player)
            return 8888;
        }

        private BackgroundWorker AwaitPlayer(int sockID, Task<TcpClient> task, Button closeConnectionBtn)
        {
            var bwWaitForConnection = new BackgroundWorker();
            
            bwWaitForConnection.DoWork += (s, e) =>
            {
                var tcpClient = task.Result;
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                e.Result = new object[]
                {
                    sockID,
                    tcpClient,
                };
            };

            bwWaitForConnection.RunWorkerCompleted += (s, e) =>
            {
                if (!e.Cancelled)
                {
                    var results = (object[])e.Result;

                    var id = (int)results[0];
                    var tcpClient = (TcpClient)results[1];

                    serverSocks[id] = tcpClient;

                    serverListeners[id].Stop();
                    serverListeners.Remove(id);

                    StartListeningThread(tcpClient);

                    closeConnectionBtn.Enabled = true;
                }
            };

            return bwWaitForConnection;
        }

        public void AcceptInvite(IPAddress serverAddress, int serverPort)
        {
            clientTCP = new TcpClient();
            clientTCP.Connect(new IPEndPoint(serverAddress, serverPort));

            if(clientTCP.Connected)
            {
                StartListeningThread(clientTCP);
#if DEBUG
                //clientTCP.Client.Send(Commands.Ping.ToByteArray());
                clientTCP.Client.Send(Commands.InitialDataRequested.ToByteArray());
#endif
            }
            else
            {
                throw new Exception("Client is not connected");
            }
        }

        public void AcceptInvite(string serverAddress, string serverPort)
        {
            AcceptInvite(IPAddress.Parse(serverAddress), int.Parse(serverPort));
        }

        
        public void CloseByID(int sockID)
        {
            if (serverListeners.ContainsKey(sockID))
            {
                serverListeners[sockID].Stop();
                serverListeners.Remove(sockID);
            }
            else if(serverSocks.ContainsKey(sockID))
            {
                serverSocks[sockID].Client.Send(Commands.CloseChannel.ToByteArray());
            }
            else
            {
                throw new Exception(String.Format("No socket with specified ID [{0}]", sockID));
            }
        }

        private void StartListeningThread(TcpClient tcpClient)
        {
            var bwListening = new BackgroundWorker();

            bwListening.DoWork += (s, e) =>
            {
                var bufferCommand = new byte[1];
                tcpClient.Client.Receive(bufferCommand);

                e.Result = new object[]
                {
                    (Commands)bufferCommand[0],
                };
            };

            bwListening.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                else
                {
                    var results = (object[])e.Result;

                    var command = (Commands)results[0];

                    if(behaviourByCommand.ContainsKey(command))
                    {
                        behaviourByCommand[command](tcpClient, bwListening);
                    }
                    else
                    {
                        throw new Exception("Unknown command");
                    }
                }
            };

            bwListening.RunWorkerAsync();
        }

        #region COMMUNICATION_INTERFACE

        public bool PingServer()
        {
            if(clientTCP != null && clientTCP.Connected)
            {
                clientTCP.Client.Send(Commands.Ping.ToByteArray());
                return true;
            }

            return false;
        }
        
        #endregion
    }
}
