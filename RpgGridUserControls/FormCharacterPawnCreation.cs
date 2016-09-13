using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public partial class FormCharacterPawnCreation : Form
    {
        public CharacterPawn CreatedPawn
        {
            get;
            private set;
        }

        public FormCharacterPawnCreation()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            CreatedPawn = new CharacterPawn();
            gridPawnValueController1.RegisterCharacterPawn(CreatedPawn);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
