using MailSenderLib;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var mS = MailSender.Current;
            mS.MyAddress = "";
            if(mS.SendTo("", "", ""))
            {
                MessageBox.Show("sent");
            }
        }

        private void chkToggleGrid_CheckedChanged(object sender, EventArgs e)
        {
            grid1.DrawGrid = chkToggleGrid.Checked;
        }
    }
}
