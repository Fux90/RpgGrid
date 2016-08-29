using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilsData
{
    public delegate byte[] StringSerializationMethod(string str);
    public delegate string StringDeserializationMethod(byte[] buffer);

    public static class Utils
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string generateUniqueName()
        {
            return compact(DateTime.Now);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static string compact(DateTime now)
        {
            return String.Format("{0}{1}{2}{3}{4}{5}",
                                    now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);
        }

         #region DESERIALIZATION

        public static string deserializeString(byte[] buffer, StringDeserializationMethod strConversion)
        {
            return strConversion(buffer);
        }

        public static int derializeInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

        public static Image deserializeImage(byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            return Image.FromStream(ms);
        }

        #endregion

        #region SERIALIZATION

        public static byte[] serializeString(string value, StringSerializationMethod strConvert)
        {
            return strConvert(value);
        }

        public static byte[] serializeInt32(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] serializeImage(Image value)
        {
            using (var ms = new MemoryStream())
            {
                value.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        #endregion
    }
}
