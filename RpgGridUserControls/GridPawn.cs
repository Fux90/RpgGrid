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
        public const string TemplateGeneratePrefix = "T_";
        public const string NonTemplateGeneratePrefix = "";

        private const string defaultName = "NPC";

        private const string NameSerializationKey = "name";
        private const string ImageSerializationName = "img";
        private const string SizeAtNoZoomSerializationName = "ctrlSize";
        private const string PositionAtNoZoomSerializationName = "pos";
        private const string ModSizeSerializationName = "rpgSize";
        private const string IsSquaredSerializationName = "sqr";

        private const string UniqueIDSerializationName = "uID";

        private float SquarePixelSize { get; set; }

        public enum RpgSize
        {
            //...
            Medium,
            Large,
            Large_Long,
            //...
        }

        private string uniqueID;
        public string UniqueID
        {
            get
            {
                if(uniqueID == null)
                {
                    uniqueID = generateUniqueID();
                }

                return uniqueID;
            }

            private set
            {
                uniqueID = value;
            }
        }

        public event EventHandler DescriptionInvalidate;

        public GridPawn()
            : this(false)
        {
            UniqueID = generateUniqueID();
        }

        public GridPawn(bool generatedFromTemplate)
        {
            UniqueID = String.Format(   "{0}{1}", 
                                        generatedFromTemplate ? TemplateGeneratePrefix : NonTemplateGeneratePrefix,
                                        generateUniqueID());
        }

        public GridPawn(SerializationInfo info, StreamingContext context)
            : this()
        {
            UniqueID = info.GetString(UniqueIDSerializationName);
            Name = info.GetString(NameSerializationKey);

            Image = (Image)info.GetValue(ImageSerializationName, typeof(Image));
            SizeAtNoZoom = (SizeF)info.GetValue(SizeAtNoZoomSerializationName, typeof(SizeF));
            PositionAtNoZoom = (Point)info.GetValue(PositionAtNoZoomSerializationName, typeof(Point));

            ModSize = (RpgSize)info.GetValue(ModSizeSerializationName, typeof(RpgSize));
            IsSquared = info.GetBoolean(IsSquaredSerializationName);
        }

        private string generateUniqueID()
        {
            return UtilsData.Utils.generateUniqueName();
        }

        protected void OnDescriptionInvalidate(EventArgs e)
        {
            CreateToolTip();
            var tmp = DescriptionInvalidate;
            if(tmp != null)
            {
                tmp(this, EventArgs.Empty);
            }
        }

        private void CreateToolTip()
        {
            // Create the ToolTip and associate with the Form container.
            var toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this, this.ToString());
        }
        
        public void InvalidateTooltipDescription()
        {
            OnDescriptionInvalidate(EventArgs.Empty);
        }

        public event EventHandler Rotate90Degrees;

        private string name;
        public new string Name
        {
            get
            {
                return name == null ? defaultName : name;
            }
            set
            {
                name = value;
                InvalidateTooltipDescription();
            }
        }

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
                InvalidateTooltipDescription();
            }
        }

        public bool IsSquared { get; private set; }

        public bool MouseIsOver { get; private set; }

        public void SetPositionAtNoZoom(Point positionInPixels)
        {
            PositionAtNoZoom = positionInPixels;
        }

        private void ResetSizeAtNoZoom()
        {
            SetSizeAtNoZoom(-1.0f);
            if (ParentIsGrid())
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

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                InvokeOnClick(this, e);
            }
            else
            { 
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

        public virtual void PerformRotate90Degrees(bool hasToBePropagated = true)
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
            }

            OnRotate90Degrees(hasToBePropagated ? EventArgs.Empty : new PawnRotationEventArgs());
        }

        private void OnRotate90Degrees(EventArgs e)
        {
            var tmp = Rotate90Degrees;
            if(tmp != null)
            {
                tmp(this, e);
            }
        }

        private bool ParentIsGrid()
        {
            return Parent != null && typeof(ResizeablePawnContainer).IsAssignableFrom(Parent.GetType());
        }

        public override string ToString()
        {
            var strB = new StringBuilder();

            strB.AppendLine(this.Name);
            strB.AppendLine(this.ModSize.ToString());

            return strB.ToString();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(UniqueIDSerializationName, UniqueID, typeof(string)); 

            info.AddValue(NameSerializationKey, Name, typeof(string));
            info.AddValue(ImageSerializationName,Image,typeof(Image));
            info.AddValue(SizeAtNoZoomSerializationName,SizeAtNoZoom,typeof(SizeF));
            info.AddValue(PositionAtNoZoomSerializationName,PositionAtNoZoom,typeof(Point));

            info.AddValue(ModSizeSerializationName,ModSize,typeof(RpgSize));
            info.AddValue(IsSquaredSerializationName,IsSquared,typeof(bool));
        }

        public bool IsTemplateGenerated()
        {
            return UniqueID.Substring(0, TemplateGeneratePrefix.Length) == TemplateGeneratePrefix;
        }

        /// <summary>
        /// Removes prefix given by template creation
        /// </summary>
        public void NormalizeUniqueID()
        {
            UniqueID = UniqueID.Substring(TemplateGeneratePrefix.Length);
        }
    }

    public class PawnRotationEventArgs : EventArgs
    {
        public bool Propagate { get; private set; }
        public float RotationAngle { get; private set; }

        public PawnRotationEventArgs(bool propagate, float rotationAngle)
        {
            Propagate = propagate;
            RotationAngle = rotationAngle;
        }

        public PawnRotationEventArgs(bool propagate)
            : this(propagate, 90.0f)
        {

        }

        public PawnRotationEventArgs()
            : this(false, 90.0f)
        {

        }
    }
}
