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

        public float TemplatesCellWidth
        {
            get
            {
                return templateContainer1.CellWidth;
            }

            set
            {
                templateContainer1.CellWidth = value;
            }
        }

        public float TemplatesCellHeight
        {
            get
            {
                return templateContainer1.CellHeight;
            }

            set
            {
                templateContainer1.CellHeight = value;
            }
        }

        public PawnManager()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        public void LoadPawn(GridPawn gridPawn)
        {
            LoadPawns(new GridPawn[] { gridPawn });
        }

        public void LoadPawns(GridPawn[] gridPawns)
        {
            pawnContainer1.LoadPawns(gridPawns);
        }

        public void LoadPawnTemplates(CharacterPawnTemplate[] gridPawnTemplates)
        {
            templateContainer1.LoadPawns(gridPawnTemplates);
        }

        public GridPawn[] GetPawns()
        {
            return pawnContainer1.GetAll();
        }

        public CharacterPawnTemplate[] GetPawnTemplates()
        {
            return templateContainer1.GetAll();
        }
    }
}
