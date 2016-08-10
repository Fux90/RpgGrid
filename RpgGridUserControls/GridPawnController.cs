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
            InitComboSize();
        }

        private void InitComboSize()
        {
            var rpgSizes = (GridPawn.RpgSize[])Enum.GetValues(typeof(GridPawn.RpgSize));
            for (int i = 0; i < rpgSizes.Length; i++)
            {
                cmbSizes.Items.Add(rpgSizes[i]);
            }
            cmbSizes.SelectedIndex = 0;
        }

        public void RegisterCharacterPawn(CharacterPawn pawn)
        {
            currentPawn = pawn;
            ShowAll();
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
            ShowImage();
            ShowName();
            ShowPf();
            ShowSize();
            ShowNotes();
        }

        public void ShowImage()
        {
            picPawn.Image = currentPawn.Image;
        }

        public void ShowName()
        {
            txtName.Text = currentPawn.Name;
        }

        public void ShowNotes()
        {
            txtName.Text = currentPawn.Notes == null ? "" : currentPawn.Notes;
        }

        public void ShowPf()
        {
            txtCurrPf.Text = currentPawn.CurrentPf.ToString();
            txtMaxPf.Text = currentPawn.MaxPf.ToString();
        }

        public void ShowSize()
        {
            cmbSizes.SelectedIndex = (int)currentPawn.ModSize;
        }

        #endregion
    }
}
