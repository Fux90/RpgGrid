using NetUtils;
using ResourceManagement;
using RpgGridUserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        [ResponseMethods(Connections.INITIAL_DATA_RECEIVING)]
        private DataRes ReceiveInitialData(byte[] buffer)
        {
            var receivedString = GetString(buffer);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(receivedString));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.INITIAL_DATA_SENDING)]
        private DataRes SendInitialData(byte[] buffer)
        {
            var testMsg = "Hola this is a test";
            var outBuf = GetBytes(testMsg);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent initial msg, strLen {0}, bufferLen {1}", testMsg.Length, outBuf.Length)));
#endif
            return new DataRes(outBuf);
        }

        #endregion

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
    }
}
