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
    public class PawnTemplateCreateEventArgs : EventArgs
    {
        public CharacterPawnTemplate Template { get; private set; }

        public PawnTemplateCreateEventArgs(CharacterPawnTemplate template)
        {
            Template = template;
        }
    }

    public partial class PawnTemplateNewButton : BackgroundedItem
    {
        protected override string ImageResource
        {
            get
            {
                return "RpgGridUserControls.Images.plus.png";
            }
        }

        public event EventHandler<PawnTemplateCreateEventArgs> CreateNewTemplate;

        public PawnTemplateNewButton()
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

            var frm = new FormCharacterPawnTemplateCreation();
            var result = frm.ShowDialog();

            if (result == DialogResult.OK)
            {
                OmCreatedNewPawnTemplate(new PawnTemplateCreateEventArgs(frm.CreatedPawnTemplate));
            }
        }

        protected void OmCreatedNewPawnTemplate(PawnTemplateCreateEventArgs ptcE)
        {
            var tmp = CreateNewTemplate;
            if (tmp != null)
            {
                tmp(this, ptcE);
            }
        }
    }
}
