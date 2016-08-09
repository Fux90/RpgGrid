namespace RpgGrid
{
    partial class FrmMain
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
            this.btnAddTcpListener = new System.Windows.Forms.Button();
            this.chkToggleGrid = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pagMap = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkMaster = new System.Windows.Forms.CheckBox();
            this.pagInvitation = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.grpMaster = new System.Windows.Forms.GroupBox();
            this.btnStartTcpListener = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtPlayer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSendInvite = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.txtMyMail = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpPlayer = new System.Windows.Forms.GroupBox();
            this.btnPing = new System.Windows.Forms.Button();
            this.btnAcceptInvite = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtPortServer = new System.Windows.Forms.TextBox();
            this.txtIpServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.grid1 = new RpgGridUserControls.Grid();
            this.pawnManager1 = new RpgGridUserControls.PawnManager();
            this.pawnContainer1 = new RpgGridUserControls.PawnContainer();
            this.tabControl1.SuspendLayout();
            this.pagMap.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pagInvitation.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.grpMaster.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpPlayer.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddTcpListener
            // 
            this.btnAddTcpListener.Location = new System.Drawing.Point(6, 136);
            this.btnAddTcpListener.Name = "btnAddTcpListener";
            this.btnAddTcpListener.Size = new System.Drawing.Size(94, 23);
            this.btnAddTcpListener.TabIndex = 2;
            this.btnAddTcpListener.Text = "Send Mail";
            this.btnAddTcpListener.UseVisualStyleBackColor = true;
            this.btnAddTcpListener.Click += new System.EventHandler(this.btnSendMail_Click);
            // 
            // chkToggleGrid
            // 
            this.chkToggleGrid.AutoSize = true;
            this.chkToggleGrid.Checked = true;
            this.chkToggleGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkToggleGrid.Location = new System.Drawing.Point(3, 115);
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.22008F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.77992F));
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
            this.tableLayoutPanel2.Controls.Add(this.chkMaster, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkToggleGrid, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.pawnManager1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pawnContainer1, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(603, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 291F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(171, 457);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // chkMaster
            // 
            this.chkMaster.AutoSize = true;
            this.chkMaster.Checked = true;
            this.chkMaster.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMaster.Location = new System.Drawing.Point(3, 144);
            this.chkMaster.Name = "chkMaster";
            this.chkMaster.Size = new System.Drawing.Size(73, 17);
            this.chkMaster.TabIndex = 4;
            this.chkMaster.Text = "I\'m master";
            this.chkMaster.UseVisualStyleBackColor = true;
            this.chkMaster.CheckedChanged += new System.EventHandler(this.chkMaster_CheckedChanged);
            // 
            // pagInvitation
            // 
            this.pagInvitation.Controls.Add(this.tableLayoutPanel3);
            this.pagInvitation.Location = new System.Drawing.Point(4, 22);
            this.pagInvitation.Name = "pagInvitation";
            this.pagInvitation.Padding = new System.Windows.Forms.Padding(3);
            this.pagInvitation.Size = new System.Drawing.Size(783, 469);
            this.pagInvitation.TabIndex = 1;
            this.pagInvitation.Text = "Invite";
            this.pagInvitation.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.grpMaster, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.grpPlayer, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(777, 463);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // grpMaster
            // 
            this.grpMaster.Controls.Add(this.btnStartTcpListener);
            this.grpMaster.Controls.Add(this.groupBox2);
            this.grpMaster.Controls.Add(this.btnSendInvite);
            this.grpMaster.Controls.Add(this.btnAddTcpListener);
            this.grpMaster.Controls.Add(this.groupBox1);
            this.grpMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMaster.Location = new System.Drawing.Point(3, 3);
            this.grpMaster.Name = "grpMaster";
            this.grpMaster.Size = new System.Drawing.Size(382, 457);
            this.grpMaster.TabIndex = 0;
            this.grpMaster.TabStop = false;
            this.grpMaster.Text = "Master";
            // 
            // btnStartTcpListener
            // 
            this.btnStartTcpListener.Location = new System.Drawing.Point(6, 165);
            this.btnStartTcpListener.Name = "btnStartTcpListener";
            this.btnStartTcpListener.Size = new System.Drawing.Size(94, 23);
            this.btnStartTcpListener.TabIndex = 6;
            this.btnStartTcpListener.Text = "Start Tcp Listener";
            this.btnStartTcpListener.UseVisualStyleBackColor = true;
            this.btnStartTcpListener.Click += new System.EventHandler(this.btnStartTcpListener_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPlayer);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(6, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 43);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Player";
            // 
            // txtPlayer
            // 
            this.txtPlayer.Location = new System.Drawing.Point(58, 13);
            this.txtPlayer.Name = "txtPlayer";
            this.txtPlayer.Size = new System.Drawing.Size(190, 20);
            this.txtPlayer.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Mail";
            // 
            // btnSendInvite
            // 
            this.btnSendInvite.Location = new System.Drawing.Point(106, 137);
            this.btnSendInvite.Name = "btnSendInvite";
            this.btnSendInvite.Size = new System.Drawing.Size(157, 22);
            this.btnSendInvite.TabIndex = 3;
            this.btnSendInvite.Text = "Send Invite";
            this.btnSendInvite.UseVisualStyleBackColor = true;
            this.btnSendInvite.Click += new System.EventHandler(this.btnInvite_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPwd);
            this.groupBox1.Controls.Add(this.txtMyMail);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 62);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credentials";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(58, 36);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(190, 20);
            this.txtPwd.TabIndex = 3;
            this.txtPwd.TextChanged += new System.EventHandler(this.txtPwd_TextChanged);
            // 
            // txtMyMail
            // 
            this.txtMyMail.Location = new System.Drawing.Point(58, 13);
            this.txtMyMail.Name = "txtMyMail";
            this.txtMyMail.Size = new System.Drawing.Size(190, 20);
            this.txtMyMail.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mail";
            // 
            // grpPlayer
            // 
            this.grpPlayer.Controls.Add(this.btnPing);
            this.grpPlayer.Controls.Add(this.btnAcceptInvite);
            this.grpPlayer.Controls.Add(this.groupBox5);
            this.grpPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPlayer.Location = new System.Drawing.Point(391, 3);
            this.grpPlayer.Name = "grpPlayer";
            this.grpPlayer.Size = new System.Drawing.Size(383, 457);
            this.grpPlayer.TabIndex = 1;
            this.grpPlayer.TabStop = false;
            this.grpPlayer.Text = "Player";
            // 
            // btnPing
            // 
            this.btnPing.Location = new System.Drawing.Point(6, 115);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(157, 22);
            this.btnPing.TabIndex = 7;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            // 
            // btnAcceptInvite
            // 
            this.btnAcceptInvite.Location = new System.Drawing.Point(6, 87);
            this.btnAcceptInvite.Name = "btnAcceptInvite";
            this.btnAcceptInvite.Size = new System.Drawing.Size(157, 22);
            this.btnAcceptInvite.TabIndex = 6;
            this.btnAcceptInvite.Text = "Accept Invite";
            this.btnAcceptInvite.UseVisualStyleBackColor = true;
            this.btnAcceptInvite.Click += new System.EventHandler(this.btnAcceptInvite_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtPortServer);
            this.groupBox5.Controls.Add(this.txtIpServer);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Location = new System.Drawing.Point(6, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(257, 62);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Server";
            // 
            // txtPortServer
            // 
            this.txtPortServer.Location = new System.Drawing.Point(58, 36);
            this.txtPortServer.Name = "txtPortServer";
            this.txtPortServer.Size = new System.Drawing.Size(190, 20);
            this.txtPortServer.TabIndex = 3;
            // 
            // txtIpServer
            // 
            this.txtIpServer.Location = new System.Drawing.Point(58, 13);
            this.txtIpServer.Name = "txtIpServer";
            this.txtIpServer.Size = new System.Drawing.Size(190, 20);
            this.txtIpServer.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "IP";
            // 
            // grid1
            // 
            this.grid1.AllowDrop = true;
            this.grid1.DrawGrid = true;
            this.grid1.ImagePath = null;
            this.grid1.Location = new System.Drawing.Point(3, 3);
            this.grid1.Name = "grid1";
            this.grid1.PanningSensibilityFactor = 50F;
            this.grid1.Size = new System.Drawing.Size(594, 380);
            this.grid1.TabIndex = 1;
            // 
            // pawnManager1
            // 
            this.pawnManager1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnManager1.Location = new System.Drawing.Point(3, 3);
            this.pawnManager1.Name = "pawnManager1";
            this.pawnManager1.PawnsCellHeight = 30F;
            this.pawnManager1.PawnsCellWidth = 30F;
            this.pawnManager1.Size = new System.Drawing.Size(165, 106);
            this.pawnManager1.TabIndex = 5;
            // 
            // pawnContainer1
            // 
            this.pawnContainer1.CellHeight = 30F;
            this.pawnContainer1.CellWidth = 30F;
            this.pawnContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pawnContainer1.Location = new System.Drawing.Point(3, 169);
            this.pawnContainer1.Name = "pawnContainer1";
            this.pawnContainer1.Size = new System.Drawing.Size(165, 285);
            this.pawnContainer1.TabIndex = 6;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 495);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.pagMap.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pagInvitation.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.grpMaster.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpPlayer.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private RpgGridUserControls.Grid grid1;
        private System.Windows.Forms.Button btnAddTcpListener;
        private System.Windows.Forms.CheckBox chkToggleGrid;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pagMap;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TabPage pagInvitation;
        private System.Windows.Forms.Button btnSendInvite;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtMyMail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtPlayer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox grpMaster;
        private System.Windows.Forms.GroupBox grpPlayer;
        private System.Windows.Forms.Button btnAcceptInvite;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtPortServer;
        private System.Windows.Forms.TextBox txtIpServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkMaster;
        private System.Windows.Forms.Button btnStartTcpListener;
        private System.Windows.Forms.Button btnPing;
        private RpgGridUserControls.PawnManager pawnManager1;
        private RpgGridUserControls.PawnContainer pawnContainer1;
    }
}

