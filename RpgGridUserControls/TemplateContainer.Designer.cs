using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RpgGridUserControls
{
#if TEST_NO_TEMPLATE
    public partial class TemplateContainer
#else
    public partial class TemplateContainer : UserControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pawnTemplateSaver1 = new RpgGridUserControls.PawnTemplateSaver();
            this.pawnTemplateNewButton1 = new RpgGridUserControls.PawnTemplateNewButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(149, 221);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.pawnTemplateSaver1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pawnTemplateNewButton1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 159);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(143, 59);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // pawnTemplateSaver1
            // 
            this.pawnTemplateSaver1.AllowDrop = true;
            this.pawnTemplateSaver1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnTemplateSaver1.Location = new System.Drawing.Point(50, 3);
            this.pawnTemplateSaver1.Name = "pawnTemplateSaver1";
            this.pawnTemplateSaver1.Size = new System.Drawing.Size(41, 53);
            this.pawnTemplateSaver1.TabIndex = 0;
            // 
            // pawnTemplateNewButton1
            // 
            this.pawnTemplateNewButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnTemplateNewButton1.Location = new System.Drawing.Point(3, 3);
            this.pawnTemplateNewButton1.Name = "pawnTemplateNewButton1";
            this.pawnTemplateNewButton1.Size = new System.Drawing.Size(41, 53);
            this.pawnTemplateNewButton1.TabIndex = 1;
            // 
            // TemplateContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TemplateContainer";
            this.Size = new System.Drawing.Size(149, 221);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private PawnTemplateSaver pawnTemplateSaver1;
        private PawnTemplateNewButton pawnTemplateNewButton1;
    }
}
