using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace RpgGridUserControls
{
    public partial class PawnContainer : ScrollableContainer<GridPawn>
    {
        public List<GridPawn> Pawns { get; private set; }

        public PawnContainer()
        {
            InitializeComponent();
            Pawns = new List<GridPawn>();
        }

        public void LoadPawns(GridPawn[] gridPawns)
        {
            for (int i = 0; i < gridPawns.Length; i++)
            {
                this.Add(gridPawns[i]);
            }
        }

        private void pnlMain_DragDrop(object sender, DragEventArgs e)
        {
            Type t;
            if (IsOfType(e, out t))
            {
                var ctrl = (GridPawn)e.Data.GetData(t);
                Add(ctrl);
            }
        }

        private void pnlMain_DragEnter(object sender, DragEventArgs e)
        {
            if (IsOfType(e))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
