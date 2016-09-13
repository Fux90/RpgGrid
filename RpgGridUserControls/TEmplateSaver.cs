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
using UtilsData;

namespace RpgGridUserControls
{
    public class SavingPawnTemplateEventArgs : EventArgs
    {
        public CharacterPawnTemplate Template { get; private set; }

        public SavingPawnTemplateEventArgs(CharacterPawnTemplate template)
        {
            Template = template;
        }
    }

    public partial class PawnTemplateSaver : BackgroundedItem
    {
        protected override string ImageResource
        {
            get
            {
                return "RpgGridUserControls.Images.1117551-floppy.png";
            }
        }

        public event EventHandler<SavingPawnTemplateEventArgs> SaveTemplate;

        public PawnTemplateSaver()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.AllowDrop = true;

            CreateTooltip();
        }

        private void CreateTooltip()
        {
            ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this, "Drag a pawn template to save it");
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (Utils.IsOfType<CharacterPawnTemplate>(e)
                && e.AllowedEffect == (DragDropEffects.Move | DragDropEffects.Copy))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            Type t;
            var pt = new Point(drgevent.X, drgevent.Y);

            if (Utils.IsOfType<CharacterPawnTemplate>(drgevent, out t))
            {
                var template = (CharacterPawnTemplate)drgevent.Data.GetData(t);
                OnSavePawnTemplate(new SavingPawnTemplateEventArgs(template));
            }
        }

        protected void OnSavePawnTemplate(SavingPawnTemplateEventArgs sptE)
        {
            var tmp = SaveTemplate;
            if (tmp != null)
            {
                tmp(this, sptE);
            }
        }
    }
}
