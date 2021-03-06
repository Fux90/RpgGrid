﻿using System;
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
    public partial class TemplateContainer : UserControl
#else
    public partial class TemplateContainer : UserControl
#endif
    {
#if TEST_NO_TEMPLATE
        private ScrollableContainer scrollableContainerGridPawns;
#else
        private ScrollableContainer<CharacterPawnTemplate> scrollableContainerGridPawnTemplates;
#endif

        public event EventHandler<SavingPawnTemplateEventArgs> SaveTemplate
        {
            add
            {
                pawnTemplateSaver1.SaveTemplate += value;
            }

            remove
            {
                pawnTemplateSaver1.SaveTemplate -= value;
            }
        }

        public event EventHandler<PawnTemplateCreateEventArgs> CreateNewTemplate
        {
            add
            {
                pawnTemplateNewButton1.CreateNewTemplate += value;
            }

            remove
            {
                pawnTemplateNewButton1.CreateNewTemplate -= value;
            }
        }

        public TemplateContainer()
        {
#if TEST_NO_TEMPLATE
            scrollableContainerGridPawnTemplates = new ScrollableContainer();
#else
            scrollableContainerGridPawnTemplates = new ScrollableContainer<CharacterPawnTemplate>();
#endif

            InitializeComponent();

            this.tableLayoutPanel1.Controls.Add(scrollableContainerGridPawnTemplates, 0, 0);
            scrollableContainerGridPawnTemplates.Dock = DockStyle.Fill;
        }



        #region DELEGATION

        public float CellHeight
        {
            get { return scrollableContainerGridPawnTemplates.CellHeight; }
            set { scrollableContainerGridPawnTemplates.CellHeight = value; }
        }

        public float CellWidth
        {
            get { return scrollableContainerGridPawnTemplates.CellWidth; }
            set { scrollableContainerGridPawnTemplates.CellWidth = value; }
        }

        public void LoadPawns(CharacterPawnTemplate[] gridPawns)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                for (int i = 0; i < gridPawns.Length; i++)
                {
                    scrollableContainerGridPawnTemplates.ThreadSafeAdd(gridPawns[i]);
                }
            };
            bw.RunWorkerAsync();
        }

        public CharacterPawnTemplate[] GetAll()
        {
            return scrollableContainerGridPawnTemplates.All();
        }

        #endregion
    }
}
