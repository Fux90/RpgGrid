﻿using System;
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
            Ping,
            Pong,
            SendInitialData,
            CloseChannel,
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

        #endregion

        public string InvitePlayer(out Int32 sockID, out BackgroundWorker waitingPlayerBw)
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
            waitingPlayerBw = AwaitPlayer(sockID, task);
            var strB = new StringBuilder();
            strB.AppendFormat("{0}:{1}", localAddr.ToString(), port);
            return strB.ToString();
        }

        private BackgroundWorker AwaitPlayer(int sockID, Task<TcpClient> task)
        {
            var bwWaitForConnection = new BackgroundWorker();
            
            bwWaitForConnection.DoWork += (s, e) =>
            {
                var tcpClient = task.Result;

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
                    serverListeners.Remove(id);

                    StartListeningThread(tcpClient);
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
                clientTCP.Client.Send(Commands.Ping.ToByteArray());
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

        private int GeneratePort()
        {
            return 8888;
        }

        public void CloseByID(int sockID)
        {
            serverListeners[sockID].Stop();
            serverListeners.Remove(sockID);
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

                    //switch (command)
                    //{
                    //    case Commands.Ping:
                    //        MessageBox.Show("I was pinged");
                    //        tcpClient.Client.Send(Commands.Pong.ToByteArray());
                    //        bwListening.RunWorkerAsync();
                    //        break;
                    //    case Commands.Pong:
                    //        MessageBox.Show("I was ponged back");
                    //        bwListening.RunWorkerAsync();
                    //        break;
                    //    case Commands.SendInitialData:
                    //        break;
                    //    case Commands.CloseChannel:
                    //        break;
                    //    default:
                    //        throw new Exception("Unknown command");
                    //}
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

        public bool PingServer()
        {
            if(clientTCP.Connected)
            {
                clientTCP.Client.Send(Commands.Ping.ToByteArray());
                return true;
            }

            return false;
        }

        public bool PingClient(int clientID)
        {
            throw new NotImplementedException();
        }
    }
}
