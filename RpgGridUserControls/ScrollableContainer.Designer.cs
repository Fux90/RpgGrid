using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public partial class ScrollableContainer<T>
    {
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        protected System.Windows.Forms.VScrollBar vScrollBar1;
        protected System.Windows.Forms.Panel pnlMain;

        protected virtual void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.Load += ScrollableContainer_Load;
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.vScrollBar1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlMain, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(149, 221);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vScrollBar1.Location = new System.Drawing.Point(129, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(20, 221);
            this.vScrollBar1.TabIndex = 0;
            this.vScrollBar1.ValueChanged += vScrollBar1_ValueChanged;
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(3, 3);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(123, 215);
            this.pnlMain.TabIndex = 1;
            this.pnlMain.ControlAdded += pnlMain_ControlAdded;
            this.pnlMain.ControlRemoved += pnlMain_ControlRemoved;
            this.pnlMain.Paint += pnlMain_Paint;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
