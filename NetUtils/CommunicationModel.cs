using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils
{
    public delegate DataRes CommunicationDataProcessing(byte[] buffer);

    public class DataRes
    {
        public byte[] Buffer { get; private set; }
        public byte[] Length { get; private set; }

        public static DataRes Empty
        {
            get
            {
                return new DataRes(null);
            }
        }

        public DataRes(byte[] buffer)
        {
            if(buffer == null)
            {
                Length = BitConverter.GetBytes(0);
            }
            else
            {
                Length = BitConverter.GetBytes(buffer.Length);
            }

            Buffer = buffer;
        }
    }

    public abstract class CommunicationModel
    {
        protected class ResponseMethods : Attribute
        {
            public string Semantic { get; private set; }

            public ResponseMethods(string semantic)
            {
                Semantic = semantic;
            }
        }

        Dictionary<string, CommunicationDataProcessing> processing;
        protected Dictionary<string, CommunicationDataProcessing> Processing
        {
            get
            {
                if(processing == null)
                {
                    processing = new Dictionary<string, CommunicationDataProcessing>();
                }

                return processing;
            }
        }

        public CommunicationModel()
        {
            Connections.Current.Model = this;
            InitReceiveProcessingMethods();
        }

        public DataRes ProcessData(string semantics, byte[] buffer)
        {
            return Processing[semantics](buffer);
        }

        protected abstract void InitReceiveProcessingMethods();

        protected byte[] GetBytesFromString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        protected string GetStringFromByteArray(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public abstract void ShowProcessing();
        public abstract void EndShowProcessing();
    }
}
