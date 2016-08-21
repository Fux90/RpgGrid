using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public static class Utils
    {
        private readonly static Pen circlePen = new Pen(Brushes.Black, 2.0f);

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static Image ApplyCircleMask(Bitmap inputImage)
        //{
        //    if (inputImage != null)
        //    {
        //        var img = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format32bppArgb);
        //        var alphaChannel = new Bitmap(inputImage.Width, inputImage.Height);
        //        var g = Graphics.FromImage(alphaChannel);
        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        //        var rect = new Rectangle(new Point(), new Size(alphaChannel.Width, alphaChannel.Height));

        //        g.FillRectangle(Brushes.White, rect);
        //        g.FillEllipse(Brushes.Black, rect);

        //        for (int r = 0; r < img.Height; r++)
        //        {
        //            for (int c = 0; c < img.Width; c++)
        //            {
        //                if (alphaChannel.GetPixel(c, r).R == (byte)0)
        //                {
        //                    img.SetPixel(c, r, Color.FromArgb(255, inputImage.GetPixel(c, r)));
        //                }
        //            }
        //        }

        //        var gOut = Graphics.FromImage(img);
        //        gOut.DrawEllipse(circlePen, rect);

        //        return img;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public delegate void ManageResultMethod(Bitmap image);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ApplyCircleMask(Bitmap inputImage, ManageResultMethod manageResult)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                var arguments = (object[])e.Argument;

                var inImage = (Bitmap)((Bitmap)arguments[0]).Clone();

                if (inImage != null)
                {
                    try
                    {
                        var img = new Bitmap(inImage.Width, inImage.Height, PixelFormat.Format32bppArgb);
                    
                        var alphaChannel = new Bitmap(inImage.Width, inImage.Height);
                        var g = Graphics.FromImage(alphaChannel);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        var rect = new Rectangle(new Point(), new Size(alphaChannel.Width, alphaChannel.Height));
                    
                        g.FillRectangle(Brushes.White, rect);
                        g.FillEllipse(Brushes.Black, rect);

                        for (int r = 0; r < img.Height; r++)
                        {
                            for (int c = 0; c < img.Width; c++)
                            {
                                if (alphaChannel.GetPixel(c, r).R == (byte)0)
                                {
                                    img.SetPixel(c, r, Color.FromArgb(255, inImage.GetPixel(c, r)));
                                }
                            }
                        }

                        var gOut = Graphics.FromImage(img);
                        var p = new Pen(Brushes.Black, 2.0f);
                        
                        gOut.DrawEllipse(p, rect);

                        manageResult(img);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        manageResult(null);
                    }
                }
                else
                {
                    manageResult(null);
                }
            };

            var localImage = inputImage;
            bw.RunWorkerAsync(new object[] 
            {
                localImage
            });
        }

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
    }
}
