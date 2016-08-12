using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    [Serializable]
    public abstract class GridPawn : UserControl, ISerializable
    {
        private const string NameSerializationKey = "name";

        private const string ImageSerializationName = "img";
        private const string SizeAtNoZoomSerializationName = "ctrlSize";
        private const string PositionAtNoZoomSerializationName = "pos";

        private const string ModSizeSerializationName = "rpgSize";
        private const string IsSquaredSerializationName = "sqr";

        private float SquarePixelSize { get; set; }

        public enum RpgSize
        {
            //...
            Medium,
            Large,
            Large_Long,
            //...
        }

        public GridPawn()
        {

        }

        public GridPawn(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString(NameSerializationKey);

            Image = (Image)info.GetValue(ImageSerializationName, typeof(Image));
            SizeAtNoZoom = (SizeF)info.GetValue(SizeAtNoZoomSerializationName, typeof(SizeF));
            PositionAtNoZoom = (Point)info.GetValue(PositionAtNoZoomSerializationName, typeof(Point));

            ModSize = (RpgSize)info.GetValue(ModSizeSerializationName, typeof(RpgSize));
            IsSquared = info.GetBoolean(IsSquaredSerializationName);
        }

        public event EventHandler Rotate90Degrees;

        public new string Name { get; set; }
        public abstract Image Image { get; set; }
        public SizeF SizeAtNoZoom { get; private set; }
        public Point PositionAtNoZoom { get; private set; }
        //public abstract Point PositionAtNoZoomNoMargin { get; }

        private RpgSize modSize;
        public RpgSize ModSize
        {
            get
            {
                return modSize;
            }

            set
            {
                modSize = value;
                ResetSizeAtNoZoom();
                this.Invalidate();
            }
        }

        protected bool IsSquared { get; private set; }

        public bool MouseIsOver { get; private set; }

        public void SetPositionAtNoZoom(Point positionInPixels)
        {
            PositionAtNoZoom = positionInPixels;
        }

        private void ResetSizeAtNoZoom()
        {
            SetSizeAtNoZoom(-1.0f);
            if(Parent != null && typeof(ResizeablePawnContainer).IsAssignableFrom(Parent.GetType()))
            {
                ((ResizeablePawnContainer)Parent).SetPawnSize(this);
            }
        }

        public void SetSizeAtNoZoom(float squareSizeInPixels)
        {
            var dim1 = squareSizeInPixels;
            var dim2 = squareSizeInPixels;
            
            if(squareSizeInPixels == -1.0f)
            {
                dim1 = SquarePixelSize;
                dim2 = SquarePixelSize;
            }
            else
            {
                SquarePixelSize = squareSizeInPixels;
            }

            IsSquared = true;

            switch (ModSize)
            {
                case RpgSize.Large:
                    dim1 *= 2;
                    dim2 = dim1;
                    break;
                case RpgSize.Large_Long:
                    dim1 *= 2;
                    IsSquared = false;
                    break;
                case RpgSize.Medium:
                default:
                    // dim *= 1;
                    break;
            }

            SizeAtNoZoom = new SizeF(dim1, dim2);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.DoDragDrop(this, DragDropEffects.Copy | DragDropEffects.Move);
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.PerformRotate90Degrees();
                this.Parent.Invalidate();                
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            setMouseOver(true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            setMouseOver(false);
        }

        private void setMouseOver(bool value)
        {
            MouseIsOver = value;
            this.Invalidate();
        }

        public virtual void PerformRotate90Degrees()
        {
            if (!IsSquared)
            {
                SizeAtNoZoom = new SizeF()
                {
                    Width = SizeAtNoZoom.Height,
                    Height = SizeAtNoZoom.Width
                };

                Size = new Size()
                {
                    Width = Size.Height,
                    Height = Size.Width
                };

                OnRotate90Degrees(EventArgs.Empty);
            }
        }

        private void OnRotate90Degrees(EventArgs e)
        {
            var tmp = Rotate90Degrees;
            if(tmp != null)
            {
                tmp(this, e);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationKey, Name, typeof(string));
            info.AddValue(ImageSerializationName,Image,typeof(Image));
            info.AddValue(SizeAtNoZoomSerializationName,SizeAtNoZoom,typeof(SizeF));
            info.AddValue(PositionAtNoZoomSerializationName,PositionAtNoZoom,typeof(Point));

            info.AddValue(ModSizeSerializationName,ModSize,typeof(RpgSize));
            info.AddValue(IsSquaredSerializationName,IsSquared,typeof(bool));
        }
    }
}
