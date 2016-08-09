using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public abstract partial class ScrollableContainer<T> : UserControl
        where T : Control
    {
        public const float baseDim = 30.0f;

        public int Columns
        {
            get
            {
                return (int)Math.Floor((float)pnlMain.ClientRectangle.Width / CellWidth);
            }
        }

        public int Rows
        {
            get
            {
                return (int)Math.Ceiling(((float)pnlMain.Controls.Count / (float)Columns));
            }
        }

        protected int VisibleRows
        {
            get
            {
                return (int)Math.Ceiling((float)pnlMain.ClientRectangle.Height / CellHeight);
            }
        }

        public float CellWidth { get; set; }
        public float CellHeight { get; set; }

        public Size CellSize
        {
            get
            {
                return new Size((int)CellWidth, (int)CellHeight);
            }
        }

        public ScrollableContainer()
        {
            InitializeComponent();
            InitializeScrollbar();
            InitCellDimensions();
        }

        private void InitCellDimensions()
        {
            CellWidth = SetToIfZero(CellWidth, baseDim);
            CellHeight = SetToIfZero(CellHeight, baseDim);
        }

        private float SetToIfZero(float value, float baseDim)
        {
            return value == .0f ? baseDim : value;
        }

        private void InitializeScrollbar()
        {
            pnlMain.VerticalScroll.Maximum = 150;
        }

        public void Add(T ctrl)
        {
            if (!pnlMain.Controls.Contains(ctrl))
            {
                pnlMain.Controls.Add(ctrl);
                pnlMain.Invalidate();
                UpdateChildrens(this);
            }
        }

        private static void UpdateChildrens(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                ctrl.Invalidate();
                if (ctrl.HasChildren)
                {
                    UpdateChildrens(ctrl);
                }
            }
        }

        protected bool IsOfType(DragEventArgs e, out Type type)
        {
            Type parent = typeof(T);
            Type[] types = Assembly.GetExecutingAssembly().GetTypes(); // Maybe select some other assembly here, depending on what you need
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

        protected bool IsOfType(DragEventArgs e)
        {
            Type dummy;
            return IsOfType(e, out dummy);
        }

        private void drawGrid(Graphics g, Pen pen)
        {
            horizontalLines(g, pen, VisibleRows);
            verticalLines(g, pen, Columns);
        }

        private void horizontalLines(Graphics g, Pen pen, int rows)
        {
            var ptA = new PointF(0, CellHeight);
            var ptB = new PointF(pnlMain.Width, CellHeight);

            for (int i = 0; i < rows; i++)
            {
                g.DrawLine(pen, ptA, ptB);

                ptA.Y += CellHeight;
                ptB.Y += CellHeight;
            }
        }

        private void verticalLines(Graphics g, Pen pen, int columns)
        {
            var ptA = new PointF(CellWidth, 0);
            var ptB = new PointF(CellWidth, pnlMain.Height);

            for (int i = 0; i < columns; i++)
            {
                g.DrawLine(pen, ptA, ptB);

                ptA.X += CellWidth;
                ptB.X += CellWidth;
            }
        }

        private void SetScrollOptions()
        {
            var r = this.Rows;
            var vR = this.VisibleRows;
            if (r < vR)
            {
                HideScrollbar();
                this.vScrollBar1.Value = 0;
                this.vScrollBar1.Maximum = 0;
            }
            else
            {
                ShowScrollbar();
                this.vScrollBar1.Maximum = Math.Max(0, (int)Math.Ceiling((r - vR + 1) * CellHeight));
            }
        }

        private void ShowScrollbar()
        {
            setScrollColumnSize(20);
        }

        private void HideScrollbar()
        {
            setScrollColumnSize(0);
        }

        private void setScrollColumnSize(int size)
        {
            tableLayoutPanel1.ColumnStyles[1].Width = size;
            tableLayoutPanel1.Invalidate();
        }

        private void pnlMain_ControlAdded(object sender, ControlEventArgs e)
        {
            SetScrollOptions();
        }

        private void pnlMain_ControlRemoved(object sender, ControlEventArgs e)
        {
            SetScrollOptions();
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (DesignMode)
            {
                var dashedPen = new Pen(Color.LightGray, 1.0f);
                dashedPen.DashPattern = new float[] { 3.0f, 1.0f };

                drawGrid(g, dashedPen);
            }

            var _size = CellSize;

            var rows = Rows;
            var cols = Columns;
            var currCol = 0;
            var currY = 0.0f;
            var currX = 0.0f;

            currY -= vScrollBar1.Value;

            foreach (Control ctrl in pnlMain.Controls)
            {
                if (currCol == cols)
                {
                    currX = 0;
                    currY += CellHeight;
                    currCol = 0;
                }

                ctrl.Size = _size;
                ctrl.Location = new Point((int)currX, (int)currY);
                currX += CellWidth;
                currCol++;
            }
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            pnlMain.Invalidate();
        }

        private void ScrollableContainer_Load(object sender, EventArgs e)
        {
            var r = this.Rows;
            var vR = this.VisibleRows;
            if (r < vR)
            {
                HideScrollbar();
            }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            InvalidateChildrens();
        }

        private void InvalidateChildrens()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                InvalidateChildrens(Controls[i]);
            }
        }

        private void InvalidateChildrens(Control ctrl)
        {
            ctrl.Invalidate();
            for (int i = 0; i < ctrl.Controls.Count; i++)
            {
                InvalidateChildrens(ctrl.Controls[i]);
            }
        }
    }
}
