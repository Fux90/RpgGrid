
#define SHOW_SELECTION_CURSOR

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Reflection;
using UtilsData;

namespace RpgGridUserControls
{
    public partial class Grid : UserControl, ResizeablePawnContainer
    {
        private sealed class ExtraInfoFileCodes
        {
            public const string PixelsInFiveFeetKey = "PixelsInFiveFeet";
            public const string GridRegionKey = "GridRegion";

            public static float PixelsInFiveFeet { get; private set; }
            public static Rectangle GridRegion { get; private set; }

            public static void Parse_PixelsInFiveFeet(string line)
            {
                PixelsInFiveFeet = float.Parse(line, CultureInfo.InvariantCulture);
            }

            public static void Parse_GridRegion(string line)
            {
                var cleanLine = line.Trim();
                if(cleanLine[0] == '(')
                {
                    cleanLine = cleanLine.Substring(1, cleanLine.Length - 1);
                }
                var e = cleanLine.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
                var x = int.Parse(e[0]);
                var y = int.Parse(e[1]);
                var w = int.Parse(e[2]);
                var h = int.Parse(e[3]);
                GridRegion = new Rectangle(x,y,w,h);
            }

            private static void UseDefaultAndWrite(string extraInfoFilePath, Image img)
            {
                PixelsInFiveFeet = 10.0f;
                GridRegion = new Rectangle(new Point(), img.Size);

                var strB = new StringBuilder();
                strB.AppendLine(ExtraInfoFileCodes.PixelsInFiveFeetKey);
                strB.AppendLine(PixelsInFiveFeet.ToString());
                strB.AppendLine(ExtraInfoFileCodes.GridRegionKey);
                strB.AppendFormat("{0},{1},{2},{3}", 
                                    GridRegion.X,
                                    GridRegion.Y,
                                    GridRegion.Width,
                                    GridRegion.Height);
                strB.AppendLine();
                File.WriteAllText(extraInfoFilePath, strB.ToString());
            }

            private static void ReadInfoFrom(string extraInfoFilePath)
            {
                var lines = File.ReadAllLines(extraInfoFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    switch (lines[i])
                    {
                        case ExtraInfoFileCodes.PixelsInFiveFeetKey:
                            i += 1;
                            Parse_PixelsInFiveFeet(lines[i]);
                            break;
                        case ExtraInfoFileCodes.GridRegionKey:
                            i += 1;
                            Parse_GridRegion(lines[i]);
                            break;
                    }
                }
            }

            public static void Read(string imagePath, Image img)
            {
                var extraInfoFilePath = Path.ChangeExtension(imagePath, metricInfoExt);
                if (File.Exists(extraInfoFilePath))
                {
                    ReadInfoFrom(extraInfoFilePath);
                }
                else
                {
                    UseDefaultAndWrite(extraInfoFilePath, img);
                }
            }
        }

        #region EVENT ARGS

        public class PawnEventArgs : EventArgs
        {
            public GridPawn Pawn { get; private set; }
            public bool AlreadyContained { get; private set; }
            public bool Propagate { get; private set; }

            public PawnEventArgs(GridPawn pawn, bool alreadyContained, bool propagate)
            {
                Pawn = pawn;
                AlreadyContained = alreadyContained;
                Propagate = propagate;
            }

            public PawnEventArgs(GridPawn pawn, bool alreadyContained)
                : this(pawn, alreadyContained, true)
            {
            }
        }

        public class PawnAndLocationEventArgs : PawnEventArgs
        {
            public Point Location { get; private set; }

            public PawnAndLocationEventArgs(GridPawn pawn, bool alreadyContained, Point location)
                : base(pawn, alreadyContained)
            {
                Location = location;
            }
        }

        public class PawnAndColorEventArgs : PawnEventArgs
        {
            public Color Color { get; private set; }

            public PawnAndColorEventArgs(GridPawn pawn, bool alreadyContained, bool propagate, Color color)
                : base(pawn, alreadyContained, propagate)
            {
                Color = color;
            }

            public PawnAndColorEventArgs(GridPawn pawn, Color color)
                : this(pawn, true, false, color)
            {

            }
        }

        public class ImageEventArgs : EventArgs
        {
            public Image InvolvedImage { get; private set; }

            public ImageEventArgs(Image involvedImage)
            {
                InvolvedImage = involvedImage;
            }
        }

        #endregion

        public CharacterPawnListener PawnListener { get; set; }
        public CharacterPawnController PawnController { get; set; }

        private const float gridLinesWidth = 1.0f;

        private Rectangle _vieportAtComplete;
        private float _xControlToPixel
        {
            get
            {
                return Viewport == null ? 0.0f : (float)Viewport.Width / (float)Width;
            }
        }

        private float _yControlToPixel
        {
            get
            {
                return Viewport == null ? 0.0f : (float)Viewport.Height / (float)Height;
            }
        }

        private Image image;
        public Image Image
        {
            get
            {
                return image;
            }

            private set
            {
                OnBackgroundImageChanged(EventArgs.Empty);
                ResizeToFitImage(value);
                ZoomFactor = 1.0f;
                _vieportAtComplete = value == null ? new Rectangle(): new Rectangle(new Point(), new Size(value.Width, value.Height));
                ComputeViewport(0, 0);
                image = value;
                InvalidatePawnsImage();
                this.Invalidate();
            }
        }

        private Image gridImage;
        private bool isGridImageToRedraw;
        private Image GridImage
        {
            get
            {
                if (isGridImageToRedraw)
                {
                    if (Image == null)
                    {
                        gridImage = null;
                    }
                    else
                    {
                        gridImage = (Image)Image.Clone();
                        var g = Graphics.FromImage(gridImage);
                        var p = new Pen(Brushes.YellowGreen, gridLinesWidth);
                        var numHorizontalLines = GridRegion.Height / PixelsInFiveFeet;
                        var numVerticalLines = GridRegion.Width / PixelsInFiveFeet;

                        var x1 = GridRegion.X;
                        var x2 = GridRegion.Width;
                        var yL = (float)GridRegion.Y;
                        for (int y = 0; y < numHorizontalLines; y++)
                        {
                            g.DrawLine(p, new PointF(x1, yL), new PointF(x2, yL));
                            yL += PixelsInFiveFeet;
                        }

                        var y1 = GridRegion.Y;
                        var y2 = GridRegion.Height;
                        var xL = (float)GridRegion.X;
                        for (int x = 0; x < numVerticalLines; x++)
                        {
                            g.DrawLine(p, new PointF(xL, y1), new PointF(xL, y2));
                            xL += PixelsInFiveFeet;
                        }

                        isGridImageToRedraw = false;
                    }
                }
                return gridImage;
            }
        }

        private void ResizeToFitImage(Image inputImage)
        {
            //TODO...   
        }

        public bool DrawGrid { get; set; }
        private Rectangle GridRegion { get; set; }
        
        // 1.5 m
        public float PixelsInFiveFeet
        {
            get;
            private set;
        }
        public const string metricInfoExt = "metricInfo";
        private string imagePath;
        public string ImagePath
        {
            get
            {
                return imagePath;
            }

            set
            {
                if(File.Exists(value))
                {
                    imagePath = value;
                    Image = Image.FromFile(imagePath);
                    ExtraInfoFileCodes.Read(imagePath, Image);
                    PixelsInFiveFeet = ExtraInfoFileCodes.PixelsInFiveFeet;
                    GridRegion = ExtraInfoFileCodes.GridRegion;
                    InvalidateGridImage();
                }
            }
        }

        private void InvalidateGridImage()
        {
            isGridImageToRedraw = true;
        }

        private void InvalidatePawnsImage()
        {
            isGridImageToRedraw = true;
        }

        public float PanningSensibilityFactor { get; set; }
        private bool isPanning;

        protected float InverseZoomFactor { get; set; }
        private float zoomFactor;
        protected float ZoomFactor
        {
            get
            {
                return zoomFactor;
            }

            set
            {
                zoomFactor = value;
                InverseZoomFactor = value != 0.0f ? 1.0f / value : 1.0f;
            }
        }

        protected Rectangle Viewport { get; set; }

        private Control PreviousFocused { get; set; }

        public event EventHandler<PawnEventArgs> PawnAdded;
        public event EventHandler<PawnAndLocationEventArgs> PawnDragDropped;
        public event EventHandler<PawnAndLocationEventArgs> TemplateDragDropped;
        public event EventHandler<PawnEventArgs> PawnRemoved;
        public event EventHandler<PawnAndLocationEventArgs> PawnMoved;

        public event EventHandler<PawnEventArgs> PawnRotated90Degrees;
        public event EventHandler<PawnAndColorEventArgs> PawnBorderColorChanged;

        public event EventHandler<ImageEventArgs> GridImageChangedByDialog;

        public Grid()
        {
            InitializeComponent();
            ZoomFactor = 1.0f;
            ComputeViewport(0, 0);

            this.DoubleBuffered = true;
            this.AllowDrop = true;

#if DEBUG
            PanningSensibilityFactor = 50.0f;
            DrawGrid = true;
#endif
        }

        /// <summary>
        /// Returns false if pawn was not present
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public bool AddIfNotPresent(GridPawn ctrl)
        {
            if (!Controls.Contains(ctrl))
            {
                Controls.Add(ctrl);
                OnPawnAdded(new PawnEventArgs(ctrl, false));

                ctrl.SetSizeAtNoZoom(PixelsInFiveFeet);
                SetPawnSize(ctrl);
                AssignEvents(ctrl);
                this.Invalidate();

                return false;
            }

            return true;
        }

        private void AssignEvents(GridPawn ctrl)
        {
            ctrl.Rotate90Degrees += Ctrl_Rotate90Degrees;
            ctrl.MouseUp += Pawn_MouseUp;
            if(ctrl.GetType() == typeof(CharacterPawn))
            {
                var cP = (CharacterPawn)ctrl;
                cP.AssignedBorderColor += CP_AssignedBorderColor;
            }
        }

        private void RemoveEvents(GridPawn ctrl)
        {
            ctrl.Rotate90Degrees -= Ctrl_Rotate90Degrees;
            ctrl.MouseUp -= Pawn_MouseUp;
            if (ctrl.GetType() == typeof(CharacterPawn))
            {
                var cP = (CharacterPawn)ctrl;
                cP.AssignedBorderColor -= CP_AssignedBorderColor;
            }
        }

        public GridPawn GetByUniqueID(string ID)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var gridPawn = (GridPawn)Controls[i];

                if (gridPawn.UniqueID == ID)
                {
                    return gridPawn;
                }
            }

            return null;
        }

        public GridPawn Remove(GridPawn ctrl, bool propagate = true)
        {
            RemoveEvents(ctrl);
            Controls.Remove(ctrl);
            OnPawnRemoved(new PawnEventArgs(ctrl, true, propagate));

            return ctrl;
        }

        private void CP_AssignedBorderColor(object sender, CharacterPawn.AssignedBorderColorEventArgs e)
        {
            OnPawnBorderColorChanged(new PawnAndColorEventArgs((GridPawn)sender, true, true, e.Color));
        }

        private void Ctrl_Rotate90Degrees(object sender, EventArgs e)
        {
            var pawn = (GridPawn)sender;

            if (!pawn.IsSquared)
            {
                pawn.Width = (int)Math.Round(pawn.Width * _yControlToPixel);
                pawn.Height = (int)Math.Round(pawn.Height * _xControlToPixel);

                SetPawnSize(pawn);
            }

            this.Invalidate();

            var propagate = true;
            if(e.GetType() == typeof(PawnRotationEventArgs))
            {
                propagate = ((PawnRotationEventArgs)e).Propagate;
            }

            OnPawnRotated90Degrees(new PawnEventArgs(pawn, true, propagate));
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if(typeof(GridPawn).IsAssignableFrom(e.Control.GetType()))
            {
                OnPawnRemoved(new PawnEventArgs((GridPawn)e.Control, true, true));
            }

            base.OnControlRemoved(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if ((Utils.IsOfType<GridPawn>(e) || Utils.IsOfType<CharacterPawnTemplate>(e))
                && e.AllowedEffect == (DragDropEffects.Move | DragDropEffects.Copy))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            Type t;
            var pt = new Point(drgevent.X, drgevent.Y);

            if (Utils.IsOfType<GridPawn>(drgevent, out t))
            {
                var ctrl = (GridPawn)drgevent.Data.GetData(t);
                //var ptClient = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                //var contained = DragDropAdding(ctrl, ptClient);
                var contained = DragDropAdding(ctrl, pt);
                OnPawnDragDropped(new PawnAndLocationEventArgs(ctrl, contained, ctrl.PositionAtNoZoom));
            }
            else if(Utils.IsOfType<CharacterPawnTemplate>(drgevent, out t))
            {
                var ctrl = ((CharacterPawnTemplate)drgevent.Data.GetData(t)).Build();
                //var ptClient = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                //var contained = DragDropAdding(ctrl, ptClient);
                var contained = DragDropAdding(ctrl, pt);
                OnTemplateDragDropped(new PawnAndLocationEventArgs(ctrl, contained, ctrl.PositionAtNoZoom));
            }
        }

        public bool DragDropAdding(GridPawn ctrl, Point? pt, bool receivedFromOutside = false)
        {
            var res = AddIfNotPresent(ctrl);

            if (pt == null)
            {
                SetLocation(ctrl, propagateEvent: false);
            }
            else
            {
                var ptClient = this.PointToClient((Point)pt);
                SetLocation(ctrl, ptClient.X, ptClient.Y, propagateEvent: !receivedFromOutside);
            }

            InvalidatePawnsImage();

            return res;
        }

        protected void OnGridImageChangedByDialog(ImageEventArgs iE)
        {
            var tmp = GridImageChangedByDialog;
            if(tmp != null)
            {
                tmp(this, iE);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Focus();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if(PreviousFocused != null)
            {
                PreviousFocused.Focus();
            }
            base.OnMouseLeave(e);
        }

        private Point? Offset { get; set; }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Middle)
            {
                Offset = new Point()
                {
                    X = e.X,
                    Y = e.Y
                };

                HideAllControls();
            }
        }

#if SHOW_SELECTION_CURSOR
        private PointF? selectedPoint;
        private SizeF? selectedSize;
#endif
        private Point? translation;

        protected override void OnMouseMove(MouseEventArgs e)
        {
#if SHOW_SELECTION_CURSOR
            
            var actualX = (int)Math.Round(Viewport.X + (e.X * _xControlToPixel));
            var actualY = (int)Math.Round(Viewport.Y + (e.Y * _yControlToPixel));

            if(actualX < GridRegion.X || actualX > GridRegion.Width || actualY < GridRegion.Y || actualY > GridRegion.Height)
            {
                selectedPoint = null;
            }
            else
            {
                var selX = (float)Math.Floor(actualX / PixelsInFiveFeet) * PixelsInFiveFeet;
                selX -= Viewport.X;
                selX += gridLinesWidth;
                selX /= _xControlToPixel;

                var selY = (float)Math.Floor(actualY / PixelsInFiveFeet) * PixelsInFiveFeet;
                selY -= Viewport.Y;
                selY += gridLinesWidth;
                selY /= _yControlToPixel;

                selX = ZeroIfIssues(selX);
                selY = ZeroIfIssues(selY);
                var w = IfIssuesSubstitute(PixelsInFiveFeet / _xControlToPixel, 1.0f);
                var h = IfIssuesSubstitute(PixelsInFiveFeet / _yControlToPixel, 1.0f);

                selectedPoint = new PointF(selX, selY);
                selectedSize = new SizeF(w, h);
            }

            this.Invalidate();
#endif
            if (e.Button == MouseButtons.Middle)
            {
                if (Offset != null)
                {
                    isPanning = true;

                    InvalidatePawnsImage();
                    translation = TranslateViewport((int)Math.Round((((Point)Offset).X - e.X) / PanningSensibilityFactor),
                                                        (int)Math.Round((((Point)Offset).Y - e.Y) / PanningSensibilityFactor));

#if DEBUG
                    
#endif
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Offset = null;
            isPanning = false;

            if (e.Button == MouseButtons.Middle)
            {
                ComputeControlsLocation();
                ShowAllControls();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ChangeImageDialog();
            }
        }

        private void ChangeImageDialog()
        {
            using (var oDlg = new OpenFileDialog())
            {
                oDlg.Title = "Change Map";
                oDlg.Multiselect = false;
                oDlg.Filter = "Jpg Files|*.jpg|Png Files|*.png|Bmp Files|*.bmp|All Files|*.*";

                if (oDlg.ShowDialog() == DialogResult.OK)
                {
                    this.ImagePath = oDlg.FileName;
                    OnGridImageChangedByDialog(new ImageEventArgs(Image));
                }
            }
        }

        private void HideAllControls()
        {
            SetAllControlsVisibility(false);
        }

        private void ShowAllControls()
        {
            SetAllControlsVisibility(true);
        }

        private void SetAllControlsVisibility(bool value)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].Visible = value;
            }

            this.Invalidate();
        }

        private static float ZeroIfIssues(float num)
        {
            return IfIssuesSubstitute(num, 0.0f);
        }

        private static float IfIssuesSubstitute(float num, float what)
        {
            num = float.IsNaN(num) || float.IsInfinity(num) ? what : num;
            return num;
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // > 0 --> ZoomIn
            // < 0 --> ZoomOut
            ComputeZoomFactor(e.Delta);
            ComputeViewport(e.X, e.Y);
            ComputeControlsSize();
            ComputeControlsLocation();
        }

        private void ComputeControlsLocation()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var ctrl = Controls[i];
                if (typeof(GridPawn).IsAssignableFrom(ctrl.GetType()))
                {
                    SetLocation((GridPawn)ctrl, propagateEvent: false);
                }
            }
            this.Invalidate();
        }

        private void SetLocation(GridPawn ctrl, int mouseX = -1, int mouseY = -1, bool propagateEvent = true)
        {
            Point point;

            var actualX = ctrl.PositionAtNoZoom.X;
            var actualY = ctrl.PositionAtNoZoom.Y;

            if (mouseX != -1 && mouseY != -1)
            {
                /*
                se imposto la posizione memorizzare la posizione REALE in pixel
                quando faccio zoom, non passo i parametri mouseX e mouseY
                e quindi i valori actualX e actualY sono memorizzati nel controllo
                */
                actualX = (int)Math.Round(Viewport.X + (mouseX * _xControlToPixel));
                actualY = (int)Math.Round(Viewport.Y + (mouseY * _yControlToPixel));

                ctrl.SetPositionAtNoZoom(new Point(actualX, actualY));
            }

            if (actualX < GridRegion.X || actualX > GridRegion.Width || actualY < GridRegion.Y || actualY > GridRegion.Height)
            {
                point = new Point(GridRegion.X, GridRegion.Y);
            }
            else
            {
                var selX = (float)Math.Floor(actualX / PixelsInFiveFeet) * PixelsInFiveFeet;
                selX -= Viewport.X;
                selX /= _xControlToPixel;

                var selY = (float)Math.Floor(actualY / PixelsInFiveFeet) * PixelsInFiveFeet;
                selY -= Viewport.Y;
                selY /= _yControlToPixel;
                
                point = new Point()
                {
                    X = (int)Math.Round(ZeroIfIssues(selX)),
                    Y = (int)Math.Round(ZeroIfIssues(selY))
                };
            }

            ctrl.Location = point;
            if (propagateEvent)
            {
                OnPawnMoved(new PawnAndLocationEventArgs(ctrl, true, ctrl.PositionAtNoZoom));
            }
        }

        private void ComputeControlsSize()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var ctrl = Controls[i];
                if (typeof(GridPawn).IsAssignableFrom(ctrl.GetType()))
                {
                    SetPawnSize((GridPawn)ctrl);
                }
            }
            this.Invalidate();
        }

        public void SetPawnSize(GridPawn pawn)
        {
            var w = (int)Math.Floor(pawn.SizeAtNoZoom.Width / _xControlToPixel);
            var h = (int)Math.Floor(pawn.SizeAtNoZoom.Height / _yControlToPixel);
            pawn.Size = new Size(w, h);
        }

        private void ComputeZoomFactor(int delta)
        {
            var currentZoom = ZoomFactor;
            var mod = Math.Abs(delta) / 1000.0f;

            if (delta >= 0)
            {
                mod *= -1;
            }
            
            ZoomFactor = Math.Max(Math.Min(currentZoom+mod, 1.0f), 0.0001f);
        }

        private void ComputeViewport(int mouseX, int mouseY)
        {
            var actualX = (int)Math.Round(Viewport.X + (mouseX * _xControlToPixel));
            var actualY = (int)Math.Round(Viewport.Y + (mouseY * _yControlToPixel));

            var w = (int)Math.Round(ZoomFactor * _vieportAtComplete.Width);
            var h = (int)Math.Round(ZoomFactor * _vieportAtComplete.Height);

            var w2 = w / 2;
            var h2 = h / 2;

            var newX = actualX - w2;
            var newY = actualY - h2;

            var x = newX;
            var y = newY;

            CropToRegion(ref x, ref y, w, h);

            Viewport = new Rectangle(x, y, w, h);

            InvalidateGridImage();
            this.Invalidate();
        }

        private void CropToRegion(ref int x, ref int y, int w, int h)
        {
            if (x < 0) x = 0;
            if (x + w > _vieportAtComplete.Width) x = _vieportAtComplete.Width - w;
            if (y < 0) y = 0;
            if (y + h > _vieportAtComplete.Height) y = _vieportAtComplete.Height - h;
        }

        private Point TranslateViewport(int mouseDeltaX, int mouseDeltaY)
        {
            var x = Viewport.X + mouseDeltaX;
            var y = Viewport.Y + mouseDeltaY;
            
            CropToRegion(ref x, ref y, Viewport.Width, Viewport.Height);

            var translation = new Point()
            {
                X = Viewport.X - x,
                Y = Viewport.Y - y,
            };

            Viewport = new Rectangle(x, y, Viewport.Width, Viewport.Height);

            InvalidateGridImage();
            this.Invalidate();

            return translation;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

#if SHOW_SELECTION_CURSOR
            if(selectedPoint != null)
            {
                var p = (PointF)selectedPoint;
                var s = (SizeF)selectedSize;

                g.DrawRectangle(Pens.Red, p.X, p.Y, s.Width, s.Height);
            }
#endif

            if (isPanning && translation != null)
            {
                var _translation = (PointF)translation;
                
                _translation.X = _translation.X / _xControlToPixel;
                _translation.Y = _translation.Y / _yControlToPixel;

                for (int i = 0; i < Controls.Count; i++)
                {
                    var ctrl = Controls[i];
                    if (typeof(GridPawn).IsAssignableFrom(ctrl.GetType()))
                    {
                        ctrl.Location = new Point()
                        {
                            X = (int)Math.Round(ctrl.Location.X + _translation.X),
                            Y = (int)Math.Round(ctrl.Location.Y + _translation.Y),
                        };

                        var pawn = (GridPawn)ctrl;
                        g.DrawImage(pawn.Image, new Rectangle(ctrl.Location, ctrl.Size));
                    }
                }

                translation = null;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;

            if (Image != null)
            {
                if(DrawGrid)
                {
                    //g.DrawImage(GridPawnImage, ClientRectangle, Viewport, GraphicsUnit.Pixel);
                    g.DrawImage(GridImage, ClientRectangle, Viewport, GraphicsUnit.Pixel);
                }
                else
                {
                    //g.DrawImage(Image, ClientRectangle, Viewport, GraphicsUnit.Pixel);
                    g.DrawImage(Image, ClientRectangle, Viewport, GraphicsUnit.Pixel);
                }
            }
        }

        protected void OnPawnAdded(PawnEventArgs e)
        {
            var tmp = PawnAdded;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        protected void OnPawnDragDropped(PawnAndLocationEventArgs e)
        {
            var tmp = PawnDragDropped;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        protected void OnTemplateDragDropped(PawnAndLocationEventArgs e)
        {
            var tmp = TemplateDragDropped;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        protected void OnPawnRemoved(PawnEventArgs e)
        {
            var tmp = PawnRemoved;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        protected void OnPawnRotated90Degrees(PawnEventArgs e)
        {
            var tmp = PawnRotated90Degrees;
            if(tmp != null)
            {
                tmp(this, e);
            }
        }

        protected void OnPawnBorderColorChanged(PawnAndColorEventArgs e)
        {
            var tmp = PawnBorderColorChanged;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        protected void OnPawnMoved(PawnAndLocationEventArgs e)
        {
            var tmp = PawnMoved;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        private void Grid_Load(object sender, EventArgs e)
        {
            PreviousFocused = FirstControlOfParent(this.Parent);
        }

        private Control FirstControlOfParent(Control parent)
        {
            if (parent.Controls.Count > 1)
            {
                foreach (Control ctrl in parent.Controls)
                {
                    if(ctrl != this)
                    {
                        return ctrl;
                    }
                }
            }
            else
            {
                if (parent.Parent != null)
                {
                    return FirstControlOfParent(parent.Parent);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        private void Pawn_MouseUp(object sender, MouseEventArgs mE)
        {
            if (mE.Button == MouseButtons.Middle)
            {
                if(PawnListener != null)
                {
                    PawnListener.RegisterCharacterPawn((CharacterPawn)sender);
                }
                if (PawnController != null)
                {
                    PawnController.RegisterCharacterPawn((CharacterPawn)sender);
                }
            }
        }
    }
}
