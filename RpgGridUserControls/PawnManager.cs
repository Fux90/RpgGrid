using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public partial class PawnManager : UserControl
    {
        public float PawnsCellWidth
        {
            get
            {
                return pawnContainer1.CellWidth;
            }

            set
            {
                pawnContainer1.CellWidth = value;
            }
        }

        public float PawnsCellHeight
        {
            get
            {
                return pawnContainer1.CellHeight;
            }

            set
            {
                pawnContainer1.CellHeight = value;
            }
        }

        public PawnManager()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        public void LoadPawns(GridPawn[] gridPawns)
        {
            pawnContainer1.LoadPawns(gridPawns);
        }
    }
}
