using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public abstract class GridPawn : UserControl
    {
        private SizeF squareSize;

        public enum RpgSize
        {
            //...
            Medium,
            Large,
            Large_Long,
            //...
        }

        public abstract Image Image { get; set; }
        public SizeF SizeAtNoZoom { get; private set; }
        public Point PositionAtNoZoom { get; private set; }
        public abstract Point PositionAtNoZoomNoMargin { get; }

        public RpgSize ModSize { get; set; }
        protected bool IsSquared { get; private set; }

        public void SetPositionAtNoZoom(Point positionInPixels)
        {
            PositionAtNoZoom = positionInPixels;
        }

        public void SetSizeAtNoZoom(float squareSizeInPixels)
        {
            var dim1 = squareSizeInPixels;
            var dim2 = squareSizeInPixels;
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

        public event EventHandler Rotate90Degrees;

        private void OnRotate90Degrees(EventArgs e)
        {
            var tmp = Rotate90Degrees;
            if(tmp != null)
            {
                tmp(this, e);
            }
        }
    }
}
