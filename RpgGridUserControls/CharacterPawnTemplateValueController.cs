using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilsData;
using RpgGridUserControls.Utilities;

namespace RpgGridUserControls
{
    public partial class CharacterPawnTemplateValueController : UserControl, CharacterPawnTemplateController, CharacterPawnTemplateListener
    {
        public event EventHandler<CharacterPawnTemplateBuilderValueChangedEventArgs> ValueChanged;

        public CharacterPawnTemplateValueController()
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
        }

        private void cmbSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
                var newSize = (GridPawn.RpgSize)cmbSizes.SelectedIndex;
                SetSize(newSize);
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            var txt = (TextBox)sender;

            if (e.KeyCode == Keys.Enter)
            {
                behavioursByTextBox[txt](txt);
                txt.ReadOnly = true;
            }
        }

        private void txt_MouseClick(object sender, MouseEventArgs e)
        {
            var txt = (TextBox)sender;
            if (txt.ReadOnly)
            {
                txt.ReadOnly = false;
            }
        }

        private void picPawn_Click(object sender, EventArgs e)
        {
            var mE = (MouseEventArgs)e;
            if (mE.Button == MouseButtons.Right)
            {
                using (var oDlg = new OpenFileDialog())
                {
                    oDlg.Title = "Change template's image";
                    oDlg.Multiselect = false;
                    oDlg.Filter = "Jpg Files|*.jpg|Png Files|*.png|Bmp Files|*.bmp|All Files|*.*";

                    if (oDlg.ShowDialog() == DialogResult.OK)
                    {
                        //currentPawn.Image = Image.FromFile(oDlg.FileName);
                        SetImage(Image.FromFile(oDlg.FileName));
                        ShowImage();
                    }
                }
            }
        }

        private void setIntValue(TextBox inputTxt, setIntMethod setter)
        {
            int newValue;
            if (int.TryParse(inputTxt.Text, out newValue))
            {
                setter(newValue);
            }
        }

        #endregion

        protected void OnValueChanged(CharacterPawnTemplateBuilderValueChangedEventArgs e)
        {
            var tmp = ValueChanged;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        #region CONTROLLER

        public void SetName(string name)
        {
            CharacterPawnTemplate.Builder.BuildName = name;
        }

        public void SetImage(Image image)
        {
            CharacterPawnTemplate.Builder.BuildImage = image;
        }

        public void SetSize(GridPawn.RpgSize size)
        {
            CharacterPawnTemplate.Builder.BuildSize = size;
        }

        public void SetNumHitDice(int numHitDice)
        {
            CharacterPawnTemplate.Builder.BuildNumHitDice = numHitDice;
        }

        public void SetBuildHealthDie(DiceTypes healthDie)
        {
            CharacterPawnTemplate.Builder.BuildHealthDie = healthDie;
        }

        public void SetDefaultStatistics(Statistics statistics)
        {
            CharacterPawnTemplate.Builder.BuildDefaultStatistics = statistics;
        }

        #endregion

        #region LISTENER

        public void ShowImage()
        {
            picTemplate.Image = CharacterPawnTemplate.Builder.BuildImage;
        }

        public void ShowName()
        {
            txtName.Text = CharacterPawnTemplate.Builder.BuildName;
        }

        public void ShowSize()
        {
            cmbSizes.SelectedIndex = (int)CharacterPawnTemplate.Builder.BuildSize;
        }

        public void ShowAll()
        {
            ShowName();
            ShowImage();
            ShowSize();

            ShowNumHitDice();
            ShowBuildHealthDie();
            ShowDefaultStatistics();
        }

        public void ShowNumHitDice()
        {
            
        }

        public void ShowBuildHealthDie()
        {
            
        }

        public void ShowDefaultStatistics()
        {
            
        }

        #endregion
    }

    public class CharacterPawnTemplateBuilderValueChangedEventArgs
    {
        public enum ChangeableItems : byte
        {
            Name,
            Image,
            Size,
        }

        public CharacterPawnTemplate TemplateBuilder { get; private set; }
        public ChangeableItems ValueChanged { get; private set; }
        public object Value { get; private set; }

        public byte[] GetValueBuffer(StringSerializationMethod strConversion)
        {
            switch (ValueChanged)
            {
                case ChangeableItems.Image:
                    return Utils.serializeImage((Image)Value);
                case ChangeableItems.Name:
                    return Utils.serializeString((string)Value, strConversion);
                case ChangeableItems.Size:
                    return serializeRpgSize((GridPawn.RpgSize)Value);
                default:
                    throw new Exception("Unexpected changed value");
            }
        }

        public static object Deserialize(ChangeableItems valueType, byte[] buffer, StringDeserializationMethod strConversion)
        {
            switch (valueType)
            {
                case ChangeableItems.Image:
                    return Utils.deserializeImage(buffer);
                case ChangeableItems.Name:
                    return Utils.deserializeString(buffer, strConversion);
                case ChangeableItems.Size:
                    return deserializeRpgSize(buffer);
                default:
                    throw new Exception("Unexpected changed value");
            }
        }

        private static GridPawn.RpgSize deserializeRpgSize(byte[] buffer)
        {
            return (GridPawn.RpgSize)buffer[0];
        }

        private byte[] serializeRpgSize(GridPawn.RpgSize value)
        {
            return new byte[] { (byte)value };
        }

        public CharacterPawnTemplateBuilderValueChangedEventArgs(CharacterPawnTemplate.Builder templateBuilder, ChangeableItems valueChanged, object value)
        {
            TemplateBuilder = templateBuilder;
            ValueChanged = valueChanged;
            Value = value;
        }
    }
}
