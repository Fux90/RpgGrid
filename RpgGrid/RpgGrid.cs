using NetUtils;
using ResourceManagement;
using RpgGridUserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public Form ViewContainer { get; private set; }

        public ResourceManager ResourceManager{ get; private set; }
        public Grid MainGrid { get; private set; }
        public PawnManager MainPawnManager { get; private set; }

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

        [ResponseMethods(Connections.MAP_SENDING)]
        private DataRes SendMap(byte[] buffer)
        {
            using (var ms = new MemoryStream())
            {
                MainGrid.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return new DataRes(ms.ToArray());
            }
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent map: {0}x{1}", MainGrid.Image.Width, MainGrid.Image.Height)));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.MAP_RECEIVING)]
        private DataRes ReceiveMap(byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            MainGrid.Image = Image.FromStream(ms);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received map: {0}x{1}", MainGrid.Image.Width, MainGrid.Image.Height)));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.PAWNS_AND_TEMPLATES_RECEIVING)]
        private DataRes ReceivePawnsAndTemplates(byte[] buffer)
        {
            
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs("Receive pawn and templates"));
#endif
            return DataRes.Empty;
        }

        

        [ResponseMethods(Connections.PAWNS_AND_TEMPLATES_SENDING)]
        private DataRes SendPawnsAndTemplates(byte[] buffer)
        {

#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs("Receive pawn and templates"));
#endif
            return DataRes.Empty;
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
