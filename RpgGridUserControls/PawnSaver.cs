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
    public class SavingPawnEventArgs : EventArgs
    {
        public CharacterPawn Pawn { get; private set; }

        public SavingPawnEventArgs(CharacterPawn pawn)
        {
            Pawn = pawn;
        }
    }

    public partial class PawnSaver : BackgroundedItem
    {
        protected override string ImageResource
        {
            get
            {
                return "RpgGridUserControls.Images.1117551-floppy.png";
            }
        }

        public event EventHandler<SavingPawnEventArgs> SavePawn;

        public PawnSaver()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.AllowDrop = true;

            CreateTooltip();
        }

        private void CreateTooltip()
        {
            ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this, "Drag a pawn to save it");
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (Utils.IsOfType<CharacterPawn>(e)
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

            if (Utils.IsOfType<CharacterPawn>(drgevent, out t))
            {
                var pawn = (CharacterPawn)drgevent.Data.GetData(t);
                OnSavePawn(new SavingPawnEventArgs(pawn));
            }
        }

        protected void OnSavePawn(SavingPawnEventArgs spE)
        {
            var tmp = SavePawn;
            if (tmp != null)
            {
                tmp(this, spE);
            }
        }
    }
}
