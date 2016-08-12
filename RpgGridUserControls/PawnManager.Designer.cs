namespace RpgGridUserControls
{
    partial class PawnManager
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
        private void InitializeComponent()
        {
            this.grpMain = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pagInstancePawns = new System.Windows.Forms.TabPage();
            this.pagTemplatePawns = new System.Windows.Forms.TabPage();
            this.pawnContainer1 = new RpgGridUserControls.PawnContainer();
            this.templateContainer1 = new RpgGridUserControls.TemplateContainer();
            this.grpMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.pagInstancePawns.SuspendLayout();
            this.pagTemplatePawns.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.tabControl1);
            this.grpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMain.Location = new System.Drawing.Point(0, 0);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(209, 269);
            this.grpMain.TabIndex = 0;
            this.grpMain.TabStop = false;
            this.grpMain.Text = "Pawns";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pagInstancePawns);
            this.tabControl1.Controls.Add(this.pagTemplatePawns);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(203, 250);
            this.tabControl1.TabIndex = 0;
            // 
            // pagInstancePawns
            // 
            this.pagInstancePawns.Controls.Add(this.pawnContainer1);
            this.pagInstancePawns.Location = new System.Drawing.Point(4, 22);
            this.pagInstancePawns.Name = "pagInstancePawns";
            this.pagInstancePawns.Padding = new System.Windows.Forms.Padding(3);
            this.pagInstancePawns.Size = new System.Drawing.Size(195, 224);
            this.pagInstancePawns.TabIndex = 0;
            this.pagInstancePawns.Text = "Pawns";
            this.pagInstancePawns.UseVisualStyleBackColor = true;
            // 
            // pagTemplatePawns
            // 
            this.pagTemplatePawns.Controls.Add(this.templateContainer1);
            this.pagTemplatePawns.Location = new System.Drawing.Point(4, 22);
            this.pagTemplatePawns.Name = "pagTemplatePawns";
            this.pagTemplatePawns.Padding = new System.Windows.Forms.Padding(3);
            this.pagTemplatePawns.Size = new System.Drawing.Size(195, 224);
            this.pagTemplatePawns.TabIndex = 1;
            this.pagTemplatePawns.Text = "Templates";
            this.pagTemplatePawns.UseVisualStyleBackColor = true;
            // 
            // pawnContainer1
            // 
            this.pawnContainer1.CellHeight = 30F;
            this.pawnContainer1.CellWidth = 30F;
            this.pawnContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnContainer1.Location = new System.Drawing.Point(3, 3);
            this.pawnContainer1.Name = "pawnContainer1";
            this.pawnContainer1.Size = new System.Drawing.Size(189, 218);
            this.pawnContainer1.TabIndex = 0;
            // 
            // templateContainer1
            // 
            this.templateContainer1.CellHeight = 30F;
            this.templateContainer1.CellWidth = 30F;
            this.templateContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateContainer1.Location = new System.Drawing.Point(3, 3);
            this.templateContainer1.Name = "templateContainer1";
            this.templateContainer1.Size = new System.Drawing.Size(189, 218);
            this.templateContainer1.TabIndex = 0;
            // 
            // PawnManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMain);
            this.Name = "PawnManager";
            this.Size = new System.Drawing.Size(209, 269);
            this.grpMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.pagInstancePawns.ResumeLayout(false);
            this.pagTemplatePawns.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pagInstancePawns;
        private System.Windows.Forms.TabPage pagTemplatePawns;
        private PawnContainer pawnContainer1;
        private TemplateContainer templateContainer1;
    }
}
