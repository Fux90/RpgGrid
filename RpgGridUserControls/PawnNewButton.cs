using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RpgGridUserControls
{
    public class PawnCreateEventArgs : EventArgs
    {
        public CharacterPawn Pawn { get; private set; }

        public PawnCreateEventArgs(CharacterPawn pawn)
        {
            Pawn = pawn;
        }
    }

    public partial class PawnNewButton : BackgroundedItem
    {
        protected override string ImageResource
        {
            get
            {
                return "RpgGridUserControls.Images.plus.png";
            }
        }

        public event EventHandler<PawnCreateEventArgs> CreateNewPawn;

        public PawnNewButton()
        {
            InitializeComponent();
            CreateTooltip();
        }

        private void CreateTooltip()
        {
            ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this, "Click to create a new Character Pawn");
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            var frm = new FormCharacterPawnCreation();
            var result = frm.ShowDialog();

            if (result == DialogResult.OK)
            {
                OmCreatedNewPawn(new PawnCreateEventArgs(frm.CreatedPawn));
            }
        }

        protected void OmCreatedNewPawn(PawnCreateEventArgs pcE)
        {
            var tmp = CreateNewPawn;
            if (tmp != null)
            {
                tmp(this, pcE);
            }
        }
    }
}
