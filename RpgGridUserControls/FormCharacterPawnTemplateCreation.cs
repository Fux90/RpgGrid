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
    public partial class FormCharacterPawnTemplateCreation : Form
    {
        public CharacterPawnTemplate CreatedPawnTemplate { get; private set; }

        public FormCharacterPawnTemplateCreation()
        {
            InitializeComponent();

            //CreatedPawnTemplate = ...
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
