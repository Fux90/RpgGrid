using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UtilsData
{
    [Serializable]
    public class TaggedObject<T, S> : ISerializable
        //where T : ISerializable
        //where S : ISerializable
    {
        private const string objSerializationKey = "obj";
        private const string tagSerializationKey = "tag";

        public T Obj { get; private set; }
        public S Tag { get; private set; }

        public TaggedObject(T obj, S tag)
        {
            Obj = obj;
            Tag = tag;
        }

        public TaggedObject(SerializationInfo info, StreamingContext context)
        {
            Obj = (T)info.GetValue(objSerializationKey, typeof(T));
            Tag = (S)info.GetValue(tagSerializationKey, typeof(S));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(objSerializationKey, Obj, typeof(T));
            info.AddValue(tagSerializationKey, Tag, typeof(S));
        }
    }
}
