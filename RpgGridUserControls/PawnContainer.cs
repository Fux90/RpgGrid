using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace RpgGridUserControls
{
#if TEST_NO_TEMPLATE
    public partial class PawnContainer : UserControl
#else
    public partial class PawnContainer : UserControl
#endif
    {
#if TEST_NO_TEMPLATE
        private ScrollableContainer scrollableContainerGridPawns;
#else
        private ScrollableContainer<GridPawn> scrollableContainerGridPawns;
#endif

        public PawnContainer()
        {
#if TEST_NO_TEMPLATE
        scrollableContainerGridPawns = new ScrollableContainer();
#else
        scrollableContainerGridPawns = new ScrollableContainer<GridPawn>();
#endif

            InitializeComponent();

            this.Controls.Add(scrollableContainerGridPawns);
            scrollableContainerGridPawns.Dock = DockStyle.Fill;
        }



        #region DELEGATION

        public float CellHeight
        {
            get { return scrollableContainerGridPawns.CellHeight; }
            set { scrollableContainerGridPawns.CellHeight = value; }
        }

        public float CellWidth
        {
            get { return scrollableContainerGridPawns.CellWidth; }
            set { scrollableContainerGridPawns.CellWidth = value; }
        }

        public void LoadPawns(GridPawn[] gridPawns)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                for (int i = 0; i < gridPawns.Length; i++)
                {
                    scrollableContainerGridPawns.ThreadSafeAdd(gridPawns[i]);
                }
            };
            bw.RunWorkerAsync();
        }

        public GridPawn[] GetAll()
        {
            return scrollableContainerGridPawns.All();
        }

        public GridPawn GetByUniqueID(string ID)
        {
            var controls = scrollableContainerGridPawns.GridPawnControls;

            for (int i = 0; i < controls.Count; i++)
            {
                if(((GridPawn)controls[i]).UniqueID == ID)
                {
                    return (GridPawn)controls[i];
                }
            }

            return null;
        }

        #endregion
    }
}
