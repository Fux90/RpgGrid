using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RpgGridUserControls
{
#if TEST_NO_TEMPLATE
    public partial class TemplateContainer
#else
    public partial class TemplateContainer : UserControl
#endif
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        public void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PawnContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PawnContainer";
            this.Size = new System.Drawing.Size(149, 221);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
