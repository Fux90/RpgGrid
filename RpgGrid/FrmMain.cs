#define VERBOSE_DEBUGGING

using MailSenderLib;
using NetUtils;
using RpgGridUserControls.Utilities;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RpgGrid
{
    public partial class FrmMain : Form
    {
        RpgGrid grid;
#if DEBUG && VERBOSE_DEBUGGING
        Form frmVerboseDebugging;
#endif

        public FrmMain()
        {
            InitializeComponent();

            grid = new RpgGrid( this,
                                grid1,
                                pawnManager1);

#if DEBUG && VERBOSE_DEBUGGING
            ShowVerboseDebuggingForm();
#endif
        }

#if DEBUG && VERBOSE_DEBUGGING
        private void ShowVerboseDebuggingForm()
        {
            frmVerboseDebugging = new Form()
            {
                Text = "Verbose Debugging",
                TopMost = true,
            };

            var txt = new TextBox()
            {
                Name = "txtDebug",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Multiline = true,
            };

            Func<string, string> appendLine = (string str) => str + Environment.NewLine;

            frmVerboseDebugging.Controls.Add(txt);

            frmVerboseDebugging.FormClosing += (s, e) =>
            {
                txt.AppendText(appendLine("Can't close Verbose Debugging form."));
                e.Cancel = true;
            };

            frmVerboseDebugging.Show();

            this.grid.VerboseDebugging += (s, e) => txt.AppendText(appendLine(e.Message));
        }
#endif

        private void Form1_Load(object sender, EventArgs e)
        {
            this.chkMaster.Checked = false;
            this.chkPawnController.Checked = false;

            //grid1.ImagePath = @"placeholder.png";
            grid1.PawnListener = this.gridPawnController1;
            grid1.PawnController = this.gridPawnController1;
        }

        // TODO: Move to RpgGrid class?
        private void SendMail(  string from, 
                                string to, 
                                string content = "", 
                                Int32 sockID = -1,
                                BackgroundWorker waitPlayerBw = null)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                var arguments = (object[])e.Argument;

                var mS = MailSender.Current;
                mS.MyAddress = from;

                var sent = false;

                try
                {
                    if (mS.SendTo(to, "GdrGrid Invite", String.Format("Here's your invitation ({0})", content)))
                    {
                        sent = true;
                    }
                }
                catch(Exception ex)
                {
                    sent = false;
                    MessageBox.Show(ex.Message);
                }

                e.Result = new object[]
                {
                    sent,
                    sockID,
                };
            };

            bw.RunWorkerCompleted += (s, e) =>
            {
                if(e.Cancelled)
                {
                    //??
                }
                else
                {
                    btnSendInvite.Enabled = true;
                    var msg = "";

                    var args = (object[])e.Result;

                    var sent = (bool)args[0];
                    var ID = (int)args[1];

                    if(e.Error != null)
                    {
                        msg = e.Error.Message;
                        sent = false;
                    }

                    if (sent)
                    {
                        msg = String.Format("Succesfully invited {0}", txtPlayer.Text);
                        txtPlayer.Text = "";
                        waitPlayerBw?.RunWorkerAsync();
                    }
                    else
                    {
                        msg = String.Format("Issues: closed port #{0}", ID);
                        Connections.Current.CloseByID(ID);
                    }

                    this.Cursor = Cursors.Default;
                    MessageBox.Show(msg);
                }
            };

            this.Cursor = Cursors.WaitCursor;
            bw.RunWorkerAsync();
        }

        private void HideMasterControls()
        {
            grpMaster.Enabled = false;
#if !DEBUG
            pawnManager1.Visible = false;
#endif
        }

        private void ShowMasterControls()
        {
            grpMaster.Enabled = true;
            pawnManager1.Visible = true;

#if DEBUG
            LoadDefaults();
#endif
        }

#if DEBUG
        private void LoadDefaults()
        {
            // Pawn ----------------------------------------
            grid.ResourceManager.AsyncRetrievePawns(
                (pawns) =>
                {
                    //pawnContainer1.LoadPawns(pawns);
                    pawnManager1.LoadPawns(pawns);
                },
                (ex) =>
                {
                    MessageBox.Show(ex.Message);
                }
            );

            // Templates -----------------------------------
            grid.ResourceManager.AsyncRetrievePawnTemplates(
                (templates) =>
                {
                    pawnManager1.LoadPawnTemplates(templates);
                },
                (ex) =>
                {
                    MessageBox.Show(ex.Message);
                }
            );

            // Grid ----------------------------------------
            grid1.ImagePath = @"dnd_map_1.jpg";
        }
#endif

        private void HidePlayerControls()
        {
            grpPlayer.Enabled = false;
        }

        private void ShowPlayerControls()
        {
            grpPlayer.Enabled = true;
        }

        private void btnSendMail_Click(object sender, EventArgs e)
        {
            SendMail(txtMyMail.Text, txtPlayer.Text);
        }

        private void btnInvite_Click(object sender, EventArgs e)
        {
            btnSendInvite.Enabled = false;

            Int32 sockID;
            BackgroundWorker waitPlayerBw;
            Button closeConnectionBtn;
            var inviteInfo = Connections.Current.InvitePlayer(out sockID, out waitPlayerBw, out closeConnectionBtn);
            ManageCloseConnectionButton(closeConnectionBtn, inviteInfo);
            SendMail(txtMyMail.Text, txtPlayer.Text, inviteInfo, sockID, waitPlayerBw);
        }

        private void chkToggleGrid_CheckedChanged(object sender, EventArgs e)
        {
            grid1.DrawGrid = chkToggleGrid.Checked;
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            MailSender.Current.Pwd = txtPwd.Text;
        }

        private void chkMaster_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMaster.Checked)
            {
                ShowMasterControls();
                HidePlayerControls();
            }
            else
            {
                HideMasterControls();
                ShowPlayerControls();
            }
        }

        private void btnAcceptInvite_Click(object sender, EventArgs e)
        {
            Connections.Current.AcceptInvite(txtIpServer.Text, txtPortServer.Text);
        }

        private void btnStartTcpListener_Click(object sender, EventArgs e)
        {
            int sockID;
            BackgroundWorker waitPlayerBw;
            Button closeConnectionBtn;
            var inviteInfo = Connections.Current.InvitePlayer(out sockID, out waitPlayerBw, out closeConnectionBtn);
            waitPlayerBw.RunWorkerAsync();
            //MessageBox.Show("Opened socket");
            lblConnectionAddress.Text = String.Format("Address {0}", inviteInfo);

            ManageCloseConnectionButton(closeConnectionBtn, inviteInfo);
        }

        private void ManageCloseConnectionButton(Button closeConnectionBtn, string inviteInfo)
        {
            grpConnections.Controls.Add(closeConnectionBtn);
            closeConnectionBtn.Dock = DockStyle.Top;
            closeConnectionBtn.Text = String.Format("Awaiting on {0}...", inviteInfo);
            closeConnectionBtn.EnabledChanged += (s, eEnabled) =>
            {
                closeConnectionBtn.Text = String.Format("Closing on {0}...", inviteInfo);
            };
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            if(!Connections.Current.PingServer())
            {
                MessageBox.Show("I cannot ping");
            }
        }

        private void chkPawnController_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel1.ColumnStyles[0].Width = chkPawnController.Checked ? 388 : 0;
        }
    }
}
