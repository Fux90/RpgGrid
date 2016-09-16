//#define PREVENT_CLIENT_BROADCASTING
#define CLIENT_BROADCASTING_EXPECTS_ACK

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
using System.Runtime.CompilerServices;
using System.Drawing;
using System.IO;
using UtilsData;

namespace NetUtils
{
    public delegate void OnLeavingBehaviour();

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

        public const string CANCEL_MAP = "cancelMap";

        public const string MAP_NAME_SENDING = "mapNameSend";
        public const string MAP_EXTRAINFO_SENDING = "mapExtraSend";
        public const string MAP_SENDING = "mapSend";
        public const string MAP_NAME_RECEIVING = "mapNameReceive";
        public const string MAP_EXTRAINFO_RECEIVING = "mapExtraReceive";
        public const string MAP_RECEIVING = "mapRec";
        public const string PAWNS_SENDING = "pawnsSend";
        public const string PAWNS_RECEIVING = "pawnsRec";
        public const string TEMPLATES_SENDING = "templatesSend";
        public const string TEMPLATES_RECEIVING = "templatesRec";
        public const string CREATED_NEW_PAWN = "createdNewPawn";
        public const string PAWN_ADDED_TO_GRID = "pawnAddedToGrid";
        public const string PAWN_CLIENT_LOCATION = "pawnClientLocation";
        public const string CREATED_NEW_TEMPLATE = "createdNewTemplate";
        public const string TEMPLATE_ADDED_TO_GRID = "templateAddedToGrid";
        public const string MOVED_PAWN_IN_GRID = "pawnMoved";
        public const string PAWN_THAT_CHANGED = "pawnThatChanged";
        public const string PAWN_VALUE_TYPE_CHANGED = "pawnValueTypeChanged";
        public const string PAWN_VALUE_CHANGED = "pawnValue";
        public const string PAWN_ROTATION_90_DEGREES = "pawnRotated";
        public const string PAWN_BORDER_COLOR = "pawnBOrderColor";
        public const string PAWN_REMOVED = "pawnRemoved";
        public const string PAWN_DELETED = "pawnDeleted";

        public const string CLOSE_CLIENT = "closeClient";

        public const string MESSAGE = "message";
        public const string WARNING = "warning";
        public const string ERROR = "error";

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
            Reset,
            Ping,
            Pong,
            InitialDataRequested,
            InitialDataReceived,
            MapReceived,
            PawnsReceived,
            Templatesreceived,
            CreateNewPawn,
            CreateNewTemplate,
            AddPawnToGrid,
            AddPawnFromTemplateToGrid,
            MovePawnTo,
            PawnValueChanged,
            PawnRotated90Degrees,
            PawnRemoved, // Removed from grid and put into pawnManager
            PawnDeleted, // Removed permanently
            Broadcast,
            Yes,
            No,
            CloseChannel,
            ClosedChannel,
            DoneWithTicket,
            PawnChangedBorderColor,
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

        /// <summary>
        /// Not null if it's a client
        /// </summary>
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

        #region REACTIVE BEHAVIOURS

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
            if (this.clientTCP != null)
            {
                //MessageBox.Show("They want me to be closed");
                tcpClient.Client.Send(Commands.ClosedChannel.ToByteArray());
                tcpClient.Client.Shutdown(SocketShutdown.Both);
                tcpClient.Client.Close();
                tcpClient.Close();
            }
            else
            {
                var id = this.serverSocks.Where(pair => pair.Value == tcpClient).Select(pair => pair.Key).Single();
                // Find a way to press close button
                Model.ProcessData(CLOSE_CLIENT, Utils.serializeInt32(id));
            }
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
            //MessageBox.Show("I was asked for initial data");
            tcpClient.Client.Send(Commands.InitialDataReceived.ToByteArray());
            // Send initial data
            var checkpoint = new byte[1];

            // Map
            SendMap(tcpClient, checkpoint);

            // Pawns and templates
            SendPawnAndTemplates(tcpClient, checkpoint);

            // -----------------
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.InitialDataReceived)]
        public void OnInitialDataReceivedBehaviour(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            // Wait for initial data - Can do synch
            Model.ShowProcessing();

            ReceiveMap(tcpClient);
            ReceivePawnsAndTemplates(tcpClient);

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

        [CommandBehaviour(Commands.CreateNewPawn)]
        public void OnCreatedNewPawn(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            Model.ShowProcessing();

            ReceiveNewPawn(tcpClient);

            Model.EndShowProcessing();
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.PawnsReceived)]
        public void OnPawnsReceived(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            Model.ShowProcessing();

            ReceivePawns(tcpClient);

            Model.EndShowProcessing();
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.Templatesreceived)]
        public void OnTemplatesReceived(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            Model.ShowProcessing();

            ReceiveTemplates(tcpClient);

            Model.EndShowProcessing();
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.AddPawnToGrid)]
        public void OnAddPawnToGrid(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            PawnAddedToGrid(tcpClient);
            PawnMovedInGrid(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.AddPawnFromTemplateToGrid)]
        public void OnAddPawnFromTemplateToGrid(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            PawnAddedFromTemplateToGrid(tcpClient);
            PawnMovedInGrid(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.MovePawnTo)]
        public void OnMovePawn(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            PawnMovedInGrid(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.PawnValueChanged)]
        public void OnPawnValueChanged(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            PawnValueChanged(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.PawnRotated90Degrees)]
        public void OnPawnRotated90Degrees(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            PawnRotated90Degrees(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.PawnChangedBorderColor)]
        public void OnPawnChangedBorderColor(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            PawnBorderColorChanged(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.PawnRemoved)]
        public void OnPawnRemoved(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            RemovePawn(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.PawnDeleted)]
        public void OnPawnDeleted(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            DeletePawn(tcpClient);
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.Broadcast)]
        public void Broadcast(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            ReceiveDataAndBroadcast(tcpClient);
            bwListener.RunWorkerAsync();
        }

        List<string> DoneTickets = new List<string>();

        [CommandBehaviour(Commands.Done)]
        public void Done(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            Model.ProcessData(ERROR, Model.GetBytesFromString("Received void ticket"));
            bwListener.RunWorkerAsync();
        }

        [CommandBehaviour(Commands.DoneWithTicket)]
        public void DoneWithTicket(TcpClient tcpClient, BackgroundWorker bwListener)
        {
            ReceiveTicketAck(tcpClient);
            bwListener.RunWorkerAsync();
        }

        private void ReceiveTicketAck(TcpClient tcpClient)
        {
            var sizeOfBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeOfBuf);
            var ticketBuffer = new byte[BitConverter.ToInt32(sizeOfBuf, 0)];

            if (ticketBuffer.Length > 0)
            {
                tcpClient.Client.Receive(ticketBuffer);

                var ticket = Model.GetStringFromByteArray(ticketBuffer);

                if (DoneTickets.Contains(ticket))
                {
                    DoneTickets.Remove(ticket);
                    Model.ProcessData(MESSAGE, Model.GetBytesFromString(String.Format("Acked ticket {0}", ticket)));
                }
                else
                {
                    Model.ProcessData(WARNING, Model.GetBytesFromString(String.Format("No ticket {0}", ticket)));
                }
            }
            else
            {
                Model.ProcessData(ERROR, Model.GetBytesFromString("Received void ticket"));
            }
        }

        private void SendMap(TcpClient tcpClient, byte[] checkpoint)
        {
            var mapName = Model.ProcessData(MAP_NAME_SENDING, null);
            tcpClient.Client.Send(mapName.Length);
            tcpClient.Client.Send(mapName.Buffer);

            tcpClient.Client.Receive(checkpoint);

            var mapExtraInfo = Model.ProcessData(MAP_EXTRAINFO_SENDING, null);
            tcpClient.Client.Send(mapExtraInfo.Length);
            tcpClient.Client.Send(mapExtraInfo.Buffer);

            tcpClient.Client.Receive(checkpoint);

            var mapData = Model.ProcessData(MAP_SENDING, null);
            
            tcpClient.Client.Send(mapData.Length);
            tcpClient.Client.Send(mapData.Buffer);
            
            tcpClient.Client.Receive(checkpoint);

            if ((Connections.Commands)checkpoint[0] == Commands.Reset)
            {
                SendMap(tcpClient, checkpoint);
                MessageBox.Show("Resent map to client");
            }
        }

        private void ReceiveMap(TcpClient tcpClient)
        {
            ReceiveMapName(tcpClient);
            ReceiveMapExtraInfo(tcpClient);
            ReceiveMapData(tcpClient);
        }

        private void ReceiveMapName(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(MAP_NAME_RECEIVING, buffer);
            }

            tcpClient.Client.Send(Commands.Done.ToByteArray());
        }

        private void ReceiveMapExtraInfo(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(MAP_EXTRAINFO_RECEIVING, buffer);
            }

            tcpClient.Client.Send(Commands.Done.ToByteArray());
        }

        private void ReceiveMapData(TcpClient tcpClient)
        {
            //var img = Image.FromStream(tcpClient.GetStream());
            //MessageBox.Show("image " + img.Size);

            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                try
                {
                    Model.ProcessData(MAP_RECEIVING, buffer);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("TODO: request again for map");
                    tcpClient.Client.Send(Commands.Reset.ToByteArray());
                    Model.ProcessData(CANCEL_MAP, null);
                }
            }

            tcpClient.Client.Send(Commands.Done.ToByteArray());
        }

        private void SendPawnAndTemplates(TcpClient tcpClient, byte[] checkpoint)
        {
            SendPawns(tcpClient, checkpoint);
            SendTemplates(tcpClient, checkpoint);
        }

        private void ReceiveNewPawn(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                Model.ShowProcessing();

                var clientStream = tcpClient.GetStream();
                var pawn = Utils.BinaryFormatter.Deserialize(clientStream);

                //tcpClient.Client.Receive(buffer);
                var ms = new MemoryStream();
                Utils.BinaryFormatter.Serialize(ms, pawn);
                var imgBuffer = ms.ToArray();
                Model.ProcessData(CREATED_NEW_PAWN, imgBuffer);

                Model.EndShowProcessing();
            }
        }

        private void SendPawns(TcpClient tcpClient, byte[] checkpoint)
        {
            var pawnsData = Model.ProcessData(PAWNS_SENDING, null);
            tcpClient.Client.Send(pawnsData.Length);
            tcpClient.Client.Send(pawnsData.Buffer);
            tcpClient.Client.Receive(checkpoint);
        }

        private void SendTemplates(TcpClient tcpClient, byte[] checkpoint)
        {
            var templatesData = Model.ProcessData(TEMPLATES_SENDING, null);
            tcpClient.Client.Send(templatesData.Length);
            tcpClient.Client.Send(templatesData.Buffer);
            tcpClient.Client.Receive(checkpoint);
        }

        private void ReceivePawnsAndTemplates(TcpClient tcpClient)
        {
            ReceivePawns(tcpClient);
            ReceiveTemplates(tcpClient);
        }

        private void ReceivePawns(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(PAWNS_RECEIVING, buffer);
            }

            tcpClient.Client.Send(Commands.Done.ToByteArray());
        }

        private void ReceiveTemplates(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(TEMPLATES_RECEIVING, buffer);
            }

            tcpClient.Client.Send(Commands.Done.ToByteArray());
        }

        private void PawnAddedToGrid(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(PAWN_ADDED_TO_GRID, buffer);
            }
        }

        private void PawnMovedInGrid(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(PAWN_CLIENT_LOCATION, buffer);
            }
        }

        private void PawnAddedFromTemplateToGrid(TcpClient tcpClient)
        {
            byte[] sizeBuf = new byte[sizeof(int)];
            tcpClient.Client.Receive(sizeBuf);
            byte[] buffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
            if (buffer.Length > 0)
            {
                tcpClient.Client.Receive(buffer);
                Model.ProcessData(TEMPLATE_ADDED_TO_GRID, buffer);
            }
        }

        private void PawnValueChanged(TcpClient tcpClient)
        {
            var client = tcpClient.Client;
            // 1 - Which one
            var lenWhichBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichBuffer);
            var whichOneBuffer = new byte[BitConverter.ToInt32(lenWhichBuffer, 0)];
            client.Receive(whichOneBuffer);
            Model.ProcessData(PAWN_THAT_CHANGED, whichOneBuffer);
            // 2 - Which value content
            var lenWhichContentTypeBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichContentTypeBuffer);
            var whichContentTypeBuffer = new byte[BitConverter.ToInt32(lenWhichContentTypeBuffer, 0)];
            client.Receive(whichContentTypeBuffer);
            Model.ProcessData(PAWN_VALUE_TYPE_CHANGED, whichContentTypeBuffer);
            // 3 - Which value
            var lenWhichContentBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichContentBuffer);
            var whichContentBuffer = new byte[BitConverter.ToInt32(lenWhichContentBuffer, 0)];
            client.Receive(whichContentBuffer);
            Model.ProcessData(PAWN_VALUE_CHANGED, whichContentBuffer);
        }

        private void PawnRotated90Degrees(TcpClient tcpClient)
        {
            var client = tcpClient.Client;

            var lenWhichBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichBuffer);
            var whichOneBuffer = new byte[BitConverter.ToInt32(lenWhichBuffer, 0)];
            client.Receive(whichOneBuffer);
            Model.ProcessData(PAWN_ROTATION_90_DEGREES, whichOneBuffer);
        }

        private void PawnBorderColorChanged(TcpClient tcpClient)
        {
            var client = tcpClient.Client;

            var lenWhichBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichBuffer);
            var whichOneBuffer = new byte[BitConverter.ToInt32(lenWhichBuffer, 0)];
            client.Receive(whichOneBuffer);
            Model.ProcessData(PAWN_THAT_CHANGED, whichOneBuffer);

            var lenWhichColorBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichColorBuffer);
            var whichColorBuffer = new byte[BitConverter.ToInt32(lenWhichColorBuffer, 0)];
            client.Receive(whichColorBuffer);
            Model.ProcessData(PAWN_BORDER_COLOR, whichColorBuffer);
        }

        private void RemovePawn(TcpClient tcpClient)
        {
            var client = tcpClient.Client;

            var lenWhichBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichBuffer);
            var whichOneBuffer = new byte[BitConverter.ToInt32(lenWhichBuffer, 0)];
            client.Receive(whichOneBuffer);
            Model.ProcessData(PAWN_REMOVED, whichOneBuffer);
        }

        private void DeletePawn(TcpClient tcpClient)
        {
            var client = tcpClient.Client;

            var lenWhichBuffer = new byte[sizeof(int)];
            client.Receive(lenWhichBuffer);
            var whichOneBuffer = new byte[BitConverter.ToInt32(lenWhichBuffer, 0)];
            client.Receive(whichOneBuffer);
            Model.ProcessData(PAWN_DELETED, whichOneBuffer);
        }

        private void ReceiveDataAndBroadcast(TcpClient tcpClient)
        {
            var client = tcpClient.Client;

            // 1 - Command to be broadcasted
            var commandToBroadcast = new byte[1];
            client.Receive(commandToBroadcast);
#if DEBUG
            Model.ProcessData(MESSAGE, Model.GetBytesFromString(((Commands)commandToBroadcast[0]).ToString()));
#endif
            // 2 - How many data are sent
            var howManyDataBuffer = new byte[sizeof(int)];
            client.Receive(howManyDataBuffer);
            var howManyData = BitConverter.ToInt32(howManyDataBuffer, 0);
#if DEBUG
            Model.ProcessData(MESSAGE, Model.GetBytesFromString(String.Format("Sent {0} DataRes", howManyData)));
#endif
            // 3 - Actual data
            var lstData = new DataRes[howManyData];
            for (int i = 0; i < lstData.Length; i++)
            {
                byte[] sizeBuf = new byte[sizeof(int)];
                client.Receive(sizeBuf);
                byte[] receivedBuffer = new byte[BitConverter.ToInt32(sizeBuf, 0)];
                if (receivedBuffer.Length > 0)
                {
                    client.Receive(receivedBuffer);
                }

                lstData[i] = new DataRes(receivedBuffer);
            }

            // 4 - Do broadcast
            Broadcast((Commands)commandToBroadcast[0], lstData, tcpClient);

#if CLIENT_BROADCASTING_EXPECTS_ACK
            // 5 - Send ack
            byte[] sizeTicketBuf = new byte[sizeof(int)];
            client.Receive(sizeTicketBuf);
            byte[] ticketBuffer = new byte[BitConverter.ToInt32(sizeTicketBuf, 0)];
            if (ticketBuffer.Length > 0)
            {
                client.Receive(ticketBuffer);
                tcpClient.Client.Send(Commands.DoneWithTicket.ToByteArray());
                var ticketData = new DataRes(ticketBuffer);
                tcpClient.Client.Send(ticketData.Length);
                tcpClient.Client.Send(ticketData.Buffer);
            }
#if DEBUG
            Model.ProcessData(MESSAGE, Model.GetBytesFromString("Sent ack"));
#endif
#endif
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

#if DEBUG
        private static int currFreePort = 8888;
#endif
        private int GeneratePort()
        {
            // TODO: Generate random port (one for each player)
            return currFreePort++;
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

        public void AcceptInvite(IPAddress serverAddress, int serverPort, out OnLeavingBehaviour whatToDoOnLeaveClick)
        {
            clientTCP = new TcpClient();
            clientTCP.Connect(new IPEndPoint(serverAddress, serverPort));
            
            if(clientTCP.Connected)
            {
                whatToDoOnLeaveClick = () =>
                {
                    clientTCP.Client.Send(Commands.CloseChannel.ToByteArray());
                };

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

        public void AcceptInvite(string serverAddress, string serverPort, out OnLeavingBehaviour whatToDoOnLeaveClick)
        {
            AcceptInvite(IPAddress.Parse(serverAddress), int.Parse(serverPort), out whatToDoOnLeaveClick);
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

        /// <summary>
        /// External invoked broadcasting method
        /// </summary>
        /// <param name="commandToBroadcast">Command to broadcast</param>
        /// <param name="semantic">Single semantic to perform</param>
        /// <returns></returns>
        public bool Broadcast(Commands commandToBroadcast, string semantic, object data = null)
        {
            return Broadcast(commandToBroadcast, new string[] { semantic });
        }

        /// <summary>
        /// External invoked broadcasting method
        /// </summary>
        /// <param name="commandToBroadcast">Command to broadcast</param>
        /// <param name="semantics">List of semantics to broadcast</param>
        /// /// <param name="serverHasToKnow">If message has been sent by client, tells if server needs to receive informations</param>
        /// <returns></returns>
        public bool Broadcast(Commands commandToBroadcast, string[] semantics, bool serverHasToKnow = true)
        {
            var lstData = new DataRes[semantics.Length];

            for (int i = 0; i < semantics.Length; i++)
            {
                lstData[i] = Model.ProcessData(semantics[i], null);
            }

            if (clientTCP != null) // If I'm a Client
            {
                var tcpClient = clientTCP.Client;

                // A - Tell server, if need to [Send data twice, good for now]
                if (serverHasToKnow)
                {
                    tcpClient.Send(commandToBroadcast.ToByteArray());
                    SendDataTo(lstData, tcpClient);
                }
                // B - Ask server to broadcast
#if PREVENT_CLIENT_BROADCASTING
                Model.ProcessData(ERROR, Model.GetBytesFromString("FINISH BROADCASTING"));
#else
                // 0 - Create ticket
                var ticket = GenerateTicket();
                // 1 - Ask to broadcast message
                tcpClient.Send(Commands.Broadcast.ToByteArray());
                // 2 - Command to be broadcasted
                tcpClient.Send(commandToBroadcast.ToByteArray());
#if DEBUG
                Model.ProcessData(MESSAGE, Model.GetBytesFromString(String.Format("Wanted to broadcast {0}", commandToBroadcast)));
#endif
                // 3 - How many data are sent
                tcpClient.Send(BitConverter.GetBytes(lstData.Length));
#if DEBUG
                Model.ProcessData(MESSAGE, Model.GetBytesFromString(String.Format("Sent {0} DataRes", lstData.Length)));
#endif
                // 4 - Actual data
                SendDataTo(lstData, tcpClient);
#if CLIENT_BROADCASTING_EXPECTS_ACK
                // 5 - Waiting for ack (message is broadcasted)
                var ticketData = new DataRes(Model.GetBytesFromString(ticket));
                tcpClient.Send(ticketData.Length);
                tcpClient.Send(ticketData.Buffer);
#if DEBUG
                Model.ProcessData(MESSAGE, Model.GetBytesFromString(String.Format("Waiting for ack on ticket {0}", ticket)));
#endif
#endif
#endif
            }
            else // I'm a server
            {
                Broadcast(commandToBroadcast, lstData);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private string GenerateTicket()
        {
            var ticket = UtilsData.Utils.generateUniqueName();
            DoneTickets.Add(ticket);
            return ticket;
        }

        private static void SendDataTo(DataRes[] lstData, Socket tcpClient)
        {
            for (int i = 0; i < lstData.Length; i++)
            {
                var curData = lstData[i];

                tcpClient.Send(curData.Length);
                tcpClient.Send(curData.Buffer);
            }
        }

        /// <summary>
        /// Actual broadcasting method
        /// </summary>
        /// <param name="commandToBroadcast">Command that has to be broadcasted</param>
        /// <param name="lstData">Actual data to broadcast</param>
        private void Broadcast(Commands commandToBroadcast, DataRes[] lstData, TcpClient clientToSkip = null)
        {
            foreach (var sockID in serverSocks.Keys)
            {
                var localID = sockID;
                if (clientToSkip != null && serverSocks[localID] == clientToSkip)
                {
                    Model.ProcessData(MESSAGE, Model.GetBytesFromString("Skipped client"));
                    continue;
                }

                var bw = new BackgroundWorker();
                

                bw.DoWork += (s, e) =>
                {
                    var tcpClient = serverSocks[localID].Client;
                    tcpClient.Send(commandToBroadcast.ToByteArray());
                    // Not used SendDataTo() on purpose
                    for (int i = 0; i < lstData.Length; i++)
                    {
                        var curData = lstData[i];

                        tcpClient.Send(curData.Length);
                        tcpClient.Send(curData.Buffer);
                    }
                };

                bw.RunWorkerAsync();
            }
        }

#endregion
    }
}
