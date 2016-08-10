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
    public partial class GridPawnController : UserControl, CharacterPawnController, CharacterPawnListener
    {
        private CharacterPawn currentPawn;

        public GridPawnController()
        {
            InitializeComponent();
        }

        public void RegisterCharacterPawn(CharacterPawn pawn)
        {
            currentPawn = pawn;
        }

        #region CONTROLLER

        public void SetImage(Image image)
        {
            throw new NotImplementedException();
        }

        public void SetMaxPf(int maxPf)
        {
            throw new NotImplementedException();
        }

        public void SetName(string name)
        {
            throw new NotImplementedException();
        }

        public void SetNotes(string notes)
        {
            throw new NotImplementedException();
        }

        public void SetPf(int pf)
        {
            throw new NotImplementedException();
        }

        public void SetSize(GridPawn.RpgSize size)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region LISTENER

        public void ShowAll()
        {
            throw new NotImplementedException();
        }

        public void ShowImage()
        {
            throw new NotImplementedException();
        }

        public void ShowName()
        {
            throw new NotImplementedException();
        }

        public void ShowNotes()
        {
            throw new NotImplementedException();
        }

        public void ShowPf()
        {
            throw new NotImplementedException();
        }

        public void ShowSize()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
