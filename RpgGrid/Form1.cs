using MailSenderLib;
using NetUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGrid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Pawn ----------------------------------------
            characterPawn1.CurrentPf = 59;
            characterPawn1.MaxPf = 60;
            characterPawn1.Image = Image.FromFile(@"character1.png");
            //characterPawn1.ModSize = RpgGridUserControls.GridPawn.RpgSize.Large_Long;

            // Grid ----------------------------------------
            grid1.ImagePath = @"dnd_map_1.jpg";
            //grid1.ImagePath = @"placeholder.png";

            HidePlayerControls();
            ShowMasterControls();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMail(txtMyMail.Text, txtPlayer.Text);
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

        private void HideMasterControls()
        {
            grpMaster.Enabled = false;
        }

        private void ShowMasterControls()
        {
            grpMaster.Enabled = true;
        }

        private void HidePlayerControls()
        {
            grpPlayer.Enabled = false;
        }

        private void ShowPlayerControls()
        {
            grpPlayer.Enabled = true;
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
