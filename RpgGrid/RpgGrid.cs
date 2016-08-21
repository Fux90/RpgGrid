using NetUtils;
using ResourceManagement;
using RpgGridUserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGrid
{
    public class RpgGrid : CommunicationModel
    {
#if DEBUG
        public event EventHandler<VerboseDebugArgs> VerboseDebugging;
#endif

        #region CONTROLS
        public Form ViewContainer { get; private set; }

        public ResourceManager ResourceManager{ get; private set; }
        private Grid mainGrid;
        public Grid MainGrid
        {
            get
            {
                return mainGrid;
            }

            private set
            {
                mainGrid = value;
                mainGrid.PawnRemoved += (s, e) =>
                {
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Removed pawn: {0}", e.Pawn.Name)));
#endif
                };
                mainGrid.PawnAdded += (s, e) =>
                {
                    // TODO: Call method to tell pawn is added to grid
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs("TODO: Call method to tell pawn is added to grid"));
#endif
                };
                mainGrid.PawnMoved += (s, e) =>
                {
                    // TODO: Call method to tell pawn is moved
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs("TODO: Call method to tell pawn is moved"));
#endif
                };
                mainGrid.BackgroundImageChanged += (s, e) =>
                {
                    for (int i = 0; i < mainGrid.Controls.Count; i++)
                    {
                        var removed = mainGrid.Remove((GridPawn)mainGrid.Controls[i]);
                        MainPawnManager.LoadPawn(removed);
                    }
                };
            }
        }
        public PawnManager MainPawnManager { get; private set; }

        #endregion

        #region PATHS

        private string mapsFolder;
        public string MapsFolder
        {
            get
            {
                return CreateIfNotExistAndGetFolder(ref mapsFolder);
            }

            set
            {
                mapsFolder = value;
            }
        }
        
        private string CreateDefaultMapFolder()
        {
            return Path.Combine("Data", "Maps");
        }

        private string CreateIfNotExistAndGetFolder(ref string folder)
        {
            if (folder == null)
            {
                folder = CreateDefaultMapFolder();
            }
            
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        #endregion

        private BinaryFormatter binaryFormatter;
        private BinaryFormatter BinaryFormatter
        {
            get
            {
                if(binaryFormatter == null)
                {
                    binaryFormatter = new BinaryFormatter();
                }
                return binaryFormatter;
            }
        }

        public RpgGrid( Form viewContainer,
                        Grid mainGrid,
                        PawnManager mainPawnManager)
            : base()
        {
            ResourceManager = ResourceManager.Current;

            ViewContainer = viewContainer;
            MainGrid = mainGrid;
            MainPawnManager = mainPawnManager;
        }

        public override void ShowProcessing()
        {
            ViewContainer.Cursor = Cursors.WaitCursor;
        }

        public override void EndShowProcessing()
        {
            ViewContainer.Cursor = Cursors.Default;
        }

        #region RESPONSE METHODS

        protected override void InitReceiveProcessingMethods()
        {
            var processingDict = this.Processing;
            var behaviours = this.GetType()
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(m => m.GetCustomAttributes(typeof(ResponseMethods)).Count() == 1).ToArray();

            for (int i = 0; i < behaviours.Length; i++)
            {
                var b = behaviours[i];
                var cmd = ((ResponseMethods)b.GetCustomAttribute(typeof(ResponseMethods))).Semantic;
                var del = Delegate.CreateDelegate(typeof(CommunicationDataProcessing), this, b);
                processingDict[cmd] = (CommunicationDataProcessing)del;
            }
        }

        [ResponseMethods(Connections.MAP_NAME_SENDING)]
        private DataRes SendMapName(byte[] buffer)
        {
            var name = Path.GetFileName(MainGrid.ImagePath);            
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent map name: {0}", name)));
#endif
            return new DataRes(GetBytesFromString(name));
        }

        [ResponseMethods(Connections.MAP_EXTRAINFO_SENDING)]
        private DataRes SendMapExtraInfo(byte[] buffer)
        {
            var content = File.ReadAllText(Path.ChangeExtension(MainGrid.ImagePath, Grid.metricInfoExt));
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs("Sending map extra info: " + Environment. NewLine + content));
#endif
            return new DataRes(GetBytesFromString(content));
        }

        [ResponseMethods(Connections.MAP_SENDING)]
        private DataRes SendMapData(byte[] buffer)
        {
            using (var ms = new MemoryStream())
            {
                MainGrid.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent map: {0}x{1}", MainGrid.Image.Width, MainGrid.Image.Height)));
#endif
                return new DataRes(ms.ToArray());
            }
        }

        string tmpMapName;
        [ResponseMethods(Connections.MAP_NAME_RECEIVING)]
        private DataRes ReceiveMapName(byte[] buffer)
        {
            tmpMapName = GetStringFromByteArray(buffer);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received map name: {0}", tmpMapName)));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.MAP_EXTRAINFO_RECEIVING)]
        private DataRes ReceiveMapExtraInfo(byte[] buffer)
        {
            if (tmpMapName == null)
            {
                throw new Exception("Not received MapName");
            }
            else
            {
                var outpath = Path.Combine(MapsFolder, Path.ChangeExtension(tmpMapName, Grid.metricInfoExt));
                var content = GetStringFromByteArray(buffer);

                File.WriteAllText(outpath, content);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received extra info: {0}", outpath)));
#endif
            }
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.MAP_RECEIVING)]
        private DataRes ReceiveMapData(byte[] buffer)
        {
            if (tmpMapName == null)
            {
                throw new Exception("Not received MapName");
            }
            else
            {
                var outpath = Path.Combine(MapsFolder, Path.ChangeExtension(tmpMapName, "png"));

                using (var ms = new MemoryStream(buffer))
                {
                    using (var fs = new FileStream(outpath, FileMode.OpenOrCreate))
                    {
                        ms.WriteTo(fs);
                        fs.Flush();
                        fs.Close();
                    }

                    ms.Close();
                    ms.Flush();
                }

                MainGrid.ImagePath = outpath;
                tmpMapName = null;
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received map: {0}x{1}", MainGrid.Image.Width, MainGrid.Image.Height)));
#endif
            }
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.PAWNS_RECEIVING)]
        private DataRes ReceivePawns(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                var pawns = (GridPawn[])BinaryFormatter.Deserialize(ms);
                MainPawnManager.LoadPawns(pawns);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received pawns: {0} bytes, {1} items", buffer.Length, pawns.Length)));
#endif
                return DataRes.Empty;
            }
        }
        
        [ResponseMethods(Connections.PAWNS_SENDING)]
        private DataRes SendPawns(byte[] buffer)
        {
            var pawns = this.MainPawnManager.GetPawns();

            using (var ms = new MemoryStream())
            {
                BinaryFormatter.Serialize(ms, pawns);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent pawns: {0} items, {1} bytes", pawns.Length, ms.Length)));
#endif
                return new DataRes(ms.ToArray());
            }
        }

        [ResponseMethods(Connections.TEMPLATES_RECEIVING)]
        private DataRes ReceiveTemplates(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                var templates = (CharacterPawnTemplate[])BinaryFormatter.Deserialize(ms);
                MainPawnManager.LoadPawnTemplates(templates);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received templates: {0} bytes, {1} items", buffer.Length, templates.Length)));
#endif
                return DataRes.Empty;
            }
        }



        [ResponseMethods(Connections.TEMPLATES_SENDING)]
        private DataRes SendTemplates(byte[] buffer)
        {
            var templates = this.MainPawnManager.GetPawnTemplates();

            using (var ms = new MemoryStream())
            {
                BinaryFormatter.Serialize(ms, templates);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent templates: {0} items, {1} bytes", templates.Length, ms.Length)));
#endif
                return new DataRes(ms.ToArray());
            }
        }

        #endregion

        #region VERBOSE_DEBUGGING
#if DEBUG
        public class VerboseDebugArgs : EventArgs
        {
            public string Message { get; private set; }
            public VerboseDebugArgs(string message)
            {
                Message = message;
            }

            public override string ToString()
            {
                return String.Format("[DEBUG] - {0}", Message);
            }
        }

        protected void OnVerboseDebugging(VerboseDebugArgs verboseArgs)
        {
            var tmp = VerboseDebugging;
            if(tmp != null)
            {
                tmp(this, verboseArgs);
            }
        }
#endif
        #endregion
    }
}
