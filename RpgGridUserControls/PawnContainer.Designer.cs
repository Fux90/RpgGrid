#define TEST_NO_TEMPLATE

using System.Windows.Forms;

namespace RpgGridUserControls
{
#if TEST_NO_TEMPLATE
    public partial class PawnContainer : ScrollableContainer
#else
    public partial class PawnContainer : ScrollableContainer<GridPawn>
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
        public override void InitializeComponent()
        {
            base.InitializeComponent();

            this.SuspendLayout();

            //
            // pnlMain
            //
            //this.pnlMain.DragDrop += new DragEventHandler(pnlMain_DragDrop);
            //this.pnlMain.DragEnter += new DragEventHandler(pnlMain_DragEnter);
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
