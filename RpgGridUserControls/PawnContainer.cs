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
            InitializeComponent();
        }
    }
}
