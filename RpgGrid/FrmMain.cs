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

        public FrmMain()
        {
            InitializeComponent();
            grid = new RpgGrid();

#if DEBUG
            this.MouseClick += (s, e) =>
            {
                var colorPicker = new ColorPicker();
                colorPicker.ShowDialog();
            };
#endif
        }

        private void Form1_Load(object sender, EventArgs e)
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

            // Grid ----------------------------------------
            grid1.ImagePath = @"dnd_map_1.jpg";
            //grid1.ImagePath = @"placeholder.png";
            grid1.PawnListener = this.gridPawnController1;
            grid1.PawnController = this.gridPawnController1;

            HidePlayerControls();
            ShowMasterControls();
        }

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
            pawnManager1.Visible = false;
        }

        private void ShowMasterControls()
        {
            grpMaster.Enabled = true;
            pawnManager1.Visible = true;
        }

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
            var inviteInfo = Connections.Current.InvitePlayer(out sockID, out waitPlayerBw);
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
            var inviteInfo = Connections.Current.InvitePlayer(out sockID, out waitPlayerBw);
            waitPlayerBw.RunWorkerAsync();
            MessageBox.Show("Opened socket");
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            if(!Connections.Current.PingServer())
            {
                MessageBox.Show("I cannot ping");
            }
        }
    }
}
