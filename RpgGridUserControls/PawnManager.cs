﻿using System;
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

        public event EventHandler<SavingPawnEventArgs> SavePawn
        {
            add
            {
                pawnContainer1.SavePawn += value;
            }

            remove
            {
                pawnContainer1.SavePawn -= value;
            }
        }

        public event EventHandler<PawnCreateEventArgs> CreateNewPawn
        {
            add
            {
                pawnContainer1.CreateNewPawn += value;
            }

            remove
            {
                pawnContainer1.CreateNewPawn -= value;
            }
        }

        public event EventHandler<SavingPawnTemplateEventArgs> SaveTemplate
        {
            add
            {
                templateContainer1.SaveTemplate += value;
            }

            remove
            {
                templateContainer1.SaveTemplate -= value;
            }
        }

        public event EventHandler<PawnTemplateCreateEventArgs> CreateNewTemplate
        {
            add
            {
                templateContainer1.CreateNewTemplate += value;
            }

            remove
            {
                templateContainer1.CreateNewTemplate -= value;
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

        public void LoadPawnTemplate(CharacterPawnTemplate gridPawnTemplate)
        {
            LoadPawnTemplates(new CharacterPawnTemplate[] { gridPawnTemplate });
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

        public GridPawn GetByUniqueID(string ID)
        {
            return pawnContainer1.GetByUniqueID(ID);
        }
    }
}
