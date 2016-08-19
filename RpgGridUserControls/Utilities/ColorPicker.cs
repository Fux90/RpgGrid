using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls.Utilities
{
    public partial class ColorPicker : Form
    {
        public enum ColorType
        {
            baseColors,
            knowColors,
        };

        private readonly Size btnSize = new Size(15, 15);

        Color[] colors;
        string[] colorNames;

        public Color ChosenColor {get; private set;}

        public ColorPicker()
            : this(ColorType.baseColors)
        {
            
        }

        public ColorPicker(ColorType clrType)
        {
            InitializeComponent();

            switch(clrType)
            {
                case ColorType.baseColors:
                    InitColors();
                    break;
                case ColorType.knowColors:
                    break;
                default:
                    throw new Exception();
            }
            
            CreateChoices();

            DialogResult = DialogResult.Cancel;
            TopMost = true;
        }

        private void CreateChoices()
        {
            var numPerRows = (int)Math.Round(Math.Sqrt(colors.Length));
            var r = -btnSize.Height;
            var c = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                if(i % numPerRows == 0)
                {
                    r += btnSize.Height;
                    c = 0;
                }
                
                var btn = new Button();
                btn.BackColor = colors[i];
                btn.FlatStyle = FlatStyle.Flat;
                btn.Size = btnSize;
                btn.Location = new Point(c, r);
                btn.KeyDown += (s, e) => { this.Close(); };
                var localI = i;
                //btn.Click += (s, e) => { MessageBox.Show(colorNames[localI]); };
                btn.Click += (s, e) => 
                {
                    ChosenColor = colors[localI];
                    this.DialogResult = DialogResult.OK;
                };
                this.Controls.Add(btn);

                c += btnSize.Width;
            }

            var W = numPerRows * btnSize.Width;
            var H = numPerRows * btnSize.Height;
            this.Size = new Size(W, H);
        }

        private void InitColors()
        {
            var colorType = typeof(System.Drawing.Color);
            // We take only static property to avoid properties like Name, IsSystemColor ...
            var propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            var lst = new List<Color>();
            var names = new List<string>();
            foreach (PropertyInfo propInfo in propInfos)
            {
                lst.Add((Color)propInfo.GetValue(null, null));
                names.Add(propInfo.Name);
            }
            colors = lst.ToArray();
            colorNames = names.ToArray();
        }

        private void InitKnownColors()
        {
            throw new NotImplementedException();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
