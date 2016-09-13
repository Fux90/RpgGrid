using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RpgGridUserControls
{
#if TEST_NO_TEMPLATE
    public partial class PawnContainer
#else
    public partial class PawnContainer : UserControl
#endif
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        public void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cmdPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pawnSaver1 = new RpgGridUserControls.PawnSaver();
            this.pawnNewButton1 = new RpgGridUserControls.PawnNewButton();
            this.mainPanel.SuspendLayout();
            this.cmdPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.cmdPanel, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 2;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.mainPanel.Size = new System.Drawing.Size(149, 221);
            this.mainPanel.TabIndex = 0;
            // 
            // cmdPanel
            // 
            this.cmdPanel.ColumnCount = 3;
            this.cmdPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.cmdPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.cmdPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.cmdPanel.Controls.Add(this.pawnNewButton1, 0, 0);
            this.cmdPanel.Controls.Add(this.pawnSaver1, 1, 0);
            this.cmdPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdPanel.Location = new System.Drawing.Point(3, 159);
            this.cmdPanel.Name = "cmdPanel";
            this.cmdPanel.RowCount = 1;
            this.cmdPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cmdPanel.Size = new System.Drawing.Size(143, 59);
            this.cmdPanel.TabIndex = 0;
            // 
            // pawnSaver1
            // 
            this.pawnSaver1.AllowDrop = true;
            this.pawnSaver1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnSaver1.Location = new System.Drawing.Point(50, 3);
            this.pawnSaver1.Name = "pawnSaver1";
            this.pawnSaver1.Size = new System.Drawing.Size(41, 53);
            this.pawnSaver1.TabIndex = 0;
            // 
            // pawnNewButton1
            // 
            this.pawnNewButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnNewButton1.Location = new System.Drawing.Point(3, 3);
            this.pawnNewButton1.Name = "pawnNewButton1";
            this.pawnNewButton1.Size = new System.Drawing.Size(41, 53);
            this.pawnNewButton1.TabIndex = 1;
            // 
            // PawnContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Name = "PawnContainer";
            this.Size = new System.Drawing.Size(149, 221);
            this.mainPanel.ResumeLayout(false);
            this.cmdPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainPanel;
        private TableLayoutPanel cmdPanel;
        private PawnSaver pawnSaver1;
        private PawnNewButton pawnNewButton1;
    }
}
