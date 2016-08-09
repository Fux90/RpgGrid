namespace RpgGrid
{
    partial class Form1
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

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.characterPawn1 = new RpgGridUserControls.CharacterPawn();
            this.grid1 = new RpgGridUserControls.Grid();
            this.button1 = new System.Windows.Forms.Button();
            this.chkToggleGrid = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pagMap = new System.Windows.Forms.TabPage();
            this.pagInvitation = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.pagMap.SuspendLayout();
            this.pagInvitation.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // characterPawn1
            // 
            this.characterPawn1.BackColor = System.Drawing.Color.Transparent;
            this.characterPawn1.CurrentPf = 0;
            this.characterPawn1.Image = null;
            this.characterPawn1.Location = new System.Drawing.Point(3, 3);
            this.characterPawn1.MaxPf = 1;
            this.characterPawn1.ModSize = RpgGridUserControls.GridPawn.RpgSize.Medium;
            this.characterPawn1.Name = "characterPawn1";
            this.characterPawn1.Notes = null;
            this.characterPawn1.Size = new System.Drawing.Size(53, 53);
            this.characterPawn1.TabIndex = 0;
            // 
            // grid1
            // 
            this.grid1.AllowDrop = true;
            this.grid1.DrawGrid = true;
            this.grid1.ImagePath = null;
            this.grid1.Location = new System.Drawing.Point(3, 3);
            this.grid1.Name = "grid1";
            this.grid1.PanningSensibilityFactor = 50F;
            this.grid1.Size = new System.Drawing.Size(610, 380);
            this.grid1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkToggleGrid
            // 
            this.chkToggleGrid.AutoSize = true;
            this.chkToggleGrid.Checked = true;
            this.chkToggleGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkToggleGrid.Location = new System.Drawing.Point(3, 435);
            this.chkToggleGrid.Name = "chkToggleGrid";
            this.chkToggleGrid.Size = new System.Drawing.Size(45, 17);
            this.chkToggleGrid.TabIndex = 3;
            this.chkToggleGrid.Text = "Grid";
            this.chkToggleGrid.UseVisualStyleBackColor = true;
            this.chkToggleGrid.CheckedChanged += new System.EventHandler(this.chkToggleGrid_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pagMap);
            this.tabControl1.Controls.Add(this.pagInvitation);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(791, 495);
            this.tabControl1.TabIndex = 4;
            // 
            // pagMap
            // 
            this.pagMap.Controls.Add(this.tableLayoutPanel1);
            this.pagMap.Location = new System.Drawing.Point(4, 22);
            this.pagMap.Name = "pagMap";
            this.pagMap.Padding = new System.Windows.Forms.Padding(3);
            this.pagMap.Size = new System.Drawing.Size(783, 469);
            this.pagMap.TabIndex = 0;
            this.pagMap.Text = "Map";
            this.pagMap.UseVisualStyleBackColor = true;
            // 
            // pagInvitation
            // 
            this.pagInvitation.Controls.Add(this.button1);
            this.pagInvitation.Location = new System.Drawing.Point(4, 22);
            this.pagInvitation.Name = "pagInvitation";
            this.pagInvitation.Padding = new System.Windows.Forms.Padding(3);
            this.pagInvitation.Size = new System.Drawing.Size(192, 158);
            this.pagInvitation.TabIndex = 1;
            this.pagInvitation.Text = "Invite";
            this.pagInvitation.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.54546F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.45455F));
            this.tableLayoutPanel1.Controls.Add(this.grid1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(777, 463);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.chkToggleGrid, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.characterPawn1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(621, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(153, 457);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 495);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.pagMap.ResumeLayout(false);
            this.pagInvitation.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private RpgGridUserControls.CharacterPawn characterPawn1;
        private RpgGridUserControls.Grid grid1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkToggleGrid;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pagMap;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TabPage pagInvitation;
    }
}

