using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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


        private static BinaryFormatter binaryFormatter;
        public static BinaryFormatter BinaryFormatter
        {
            get
            {
                if (binaryFormatter == null)
                {
                    binaryFormatter = new BinaryFormatter();
                }
                return binaryFormatter;
            }
        }

        public static bool IsOfType<T>(DragEventArgs e, out Type type)
        {
            Type parent = typeof(T);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            //var types = Assembly.GetExecutingAssembly().GetTypes(); // Maybe select some other assembly here, depending on what you need
            var inheritingTypes = types.Where(t => parent.IsAssignableFrom(t));

            foreach (var item in inheritingTypes)
            {
                if (e.Data.GetDataPresent(item))
                {
                    type = item;
                    return true;
                }
            }

            type = null;
            return false;
        }

        public static bool IsOfType<T>(DragEventArgs e)
        {
            Type dummy;
            return IsOfType<T>(e, out dummy);
        }

        public static void ShowImage(Image img)
        {
            var frm = new Form();
            frm.MinimizeBox = false;
            frm.MaximizeBox = false;

            var pic = new PictureBox();
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.Dock = DockStyle.Fill;
            pic.Image = img;

            frm.Controls.Add(pic);

            frm.ShowDialog();
        }

        #region DESERIALIZATION

        public static string deserializeString(byte[] buffer, StringDeserializationMethod strConversion)
        {
            return strConversion(buffer);
        }

        public static int deserializeInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

        public static Image deserializeImage(byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            return Image.FromStream(ms);
        }

        public static Image deserializeImageRaw(byte[] buffer)
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
                value.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] serializeImageRaw(Image value)
        {
            var ms = new MemoryStream();
            {
                value.Save(ms, value.RawFormat);
                return ms.ToArray();
            }
        }

        public static string ConvertImageToBase64String(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                var imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image ConvertBase64StringToImage(string base64String)
        {
            var strB = new StringBuilder();
            try
            {
                var imageBytes = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(imageBytes);
                var image = Image.FromStream(ms);

                strB.AppendLine(imageBytes != null ? imageBytes.ToString() : "No imageBytes");
                strB.AppendLine(ms != null ? ms.ToString() : "No ms");

                return image;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

                
                MessageBox.Show(strB.ToString());

                return new Bitmap(128, 128);
            }
        }

        #endregion
    }
}
