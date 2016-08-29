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
            InitTextBoxBehaviours();
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

        public event EventHandler<GridPawnValueChangedEventArgs> ValueChanged;

        #region CONTROLLER

        public void SetImage(Image image)
        {
            currentPawn.Image = image;
            OnValueChanged(new GridPawnValueChangedEventArgs(currentPawn, GridPawnValueChangedEventArgs.ChangeableItems.Image, image));
        }

        public void SetPf(int pf)
        {
            currentPawn.CurrentPf = pf;
            OnValueChanged(new GridPawnValueChangedEventArgs(currentPawn, GridPawnValueChangedEventArgs.ChangeableItems.Pf, pf));
        }

        public void SetMaxPf(int maxPf)
        {
            currentPawn.MaxPf = maxPf;
            OnValueChanged(new GridPawnValueChangedEventArgs(currentPawn, GridPawnValueChangedEventArgs.ChangeableItems.MaxPf, maxPf));
        }

        public void SetName(string name)
        {
            currentPawn.Name = name;
            OnValueChanged(new GridPawnValueChangedEventArgs(currentPawn, GridPawnValueChangedEventArgs.ChangeableItems.Name, name));
        }

        public void SetNotes(string notes)
        {
            currentPawn.Notes = notes;
            OnValueChanged(new GridPawnValueChangedEventArgs(currentPawn, GridPawnValueChangedEventArgs.ChangeableItems.Notes, notes));
        }

        public void SetSize(GridPawn.RpgSize size)
        {
            currentPawn.ModSize = size;
            OnValueChanged(new GridPawnValueChangedEventArgs(currentPawn, GridPawnValueChangedEventArgs.ChangeableItems.Size, size));
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
            txtNotes.Text = currentPawn.Notes == null ? "" : currentPawn.Notes;
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

        #region EVENTS

        private delegate void setIntMethod(int newValue);
        private delegate void textBoxBehaviour(TextBox txt);
        Dictionary<TextBox, textBoxBehaviour> behavioursByTextBox;

        private void InitTextBoxBehaviours()
        {
            behavioursByTextBox = new Dictionary<TextBox, textBoxBehaviour>();
            var readonlyTextboxes = new TextBox[]
            {
                txtName,
                txtCurrPf,
                txtMaxPf,
                txtNotes,
            };

            for (int i = 0; i < readonlyTextboxes.Length; i++)
            {
                readonlyTextboxes[i].ReadOnly = true;
            }

            behavioursByTextBox[txtName] = (txt) =>
            {
                SetName(txt.Text);
            };

            behavioursByTextBox[txtCurrPf] = (txt) =>
            {
                setIntValue(txt, SetPf);
            };

            behavioursByTextBox[txtMaxPf] = (txt) =>
            {
                setIntValue(txt, SetMaxPf);
            };

            behavioursByTextBox[txtNotes] = (txt) =>
            {
                SetNotes(txt.Text);
            };
        }

        private void cmbSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentPawn != null)
            {
                var newSize = (GridPawn.RpgSize)cmbSizes.SelectedIndex;
                //MessageBox.Show(String.Format("->{0}", newSize));
                currentPawn.ModSize = newSize;
            }
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            var txt = (TextBox)sender;

            if(e.KeyCode == Keys.Enter)
            {
                behavioursByTextBox[txt](txt);
                txt.ReadOnly = true;
            }
        }

        private void txt_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentPawn != null)
            {
                var txt = (TextBox)sender;
                if (txt.ReadOnly)
                {
                    txt.ReadOnly = false;
                }
            }
        }

        private void picPawn_Click(object sender, EventArgs e)
        {
            if (currentPawn != null)
            {
                var mE = (MouseEventArgs)e;
                if (mE.Button == MouseButtons.Right)
                {
                    using (var oDlg = new OpenFileDialog())
                    {
                        oDlg.Title = String.Format("Change {0}'s image", currentPawn.Name);
                        oDlg.Multiselect = false;
                        oDlg.Filter = "Jpg Files|*.jpg|Png Files|*.png|Bmp Files|*.bmp|All Files|*.*";

                        if(oDlg.ShowDialog() == DialogResult.OK)
                        {
                            currentPawn.Image = Image.FromFile(oDlg.FileName);
                            ShowImage();
                        }
                    }
                }
            }
        }

        private void setIntValue(TextBox inputTxt, setIntMethod setter)
        {
            int newValue;
            if(int.TryParse(inputTxt.Text, out newValue))
            {
                setter(newValue);
            }
        }

        #endregion

        protected void OnValueChanged(GridPawnValueChangedEventArgs e)
        {
            var tmp = ValueChanged;
            if(tmp != null)
            {
                tmp(this, e);
            }
        }
    }

    public class GridPawnValueChangedEventArgs
    {
        public enum ChangeableItems
        {
            Name,
            Pf,
            MaxPf,
            Image,
            Size,
            Notes,
        }

        public GridPawn Pawn { get; private set; }
        public ChangeableItems ValueChanged { get; private set; }
        public object Value { get; private set; }

        public GridPawnValueChangedEventArgs(GridPawn pawn, ChangeableItems valueChanged, object value)
        {
            Pawn = pawn;
            ValueChanged = valueChanged;
            Value = value;
        }
    }
}
