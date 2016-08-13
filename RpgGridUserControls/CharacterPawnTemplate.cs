using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Drawing.Imaging;
using System.Threading;
using RpgGridUserControls.Utilities;

namespace RpgGridUserControls
{
    [Serializable]
    public partial class CharacterPawnTemplate : UserControl, GridPawnBuilder, ISerializable
    {
        public class Builder : CharacterPawnTemplate
        {
            public static CharacterPawnTemplate Create()
            {
                return new CharacterPawnTemplate()
                {
                    DefaultName = "Npg",
                    DefaultImage = Image.FromFile(@"character2.jpg"),
                    DefaultSize = GridPawn.RpgSize.Medium,
                    NumHitDice = 1,
                    HealthDie = DiceTypes.d8,
                    DefaultStatistics = new Statistics(new Dictionary<StatsType, int>()
                    {
                        {StatsType.Strength, 14},
                        {StatsType.Constitution, 12},
                    }),
                };
            }
        }

        private const string NameSerializationKey = "name";
        private const string ImageSerializationName = "image";
        private const string ModSizeSerializationName = "size";
        private const string MaxPfSerializationName = "maxPf";

        private readonly static Dictionary<GridPawn.RpgSize, Brush> brushBySize;
        private readonly Pen circlePen = new Pen(Brushes.Black, 2.0f);
        private readonly Pen highlightPen = new Pen(Brushes.YellowGreen, 2.0f);

        private string defaultName;
        private Image defaultImage;
        private GridPawn.RpgSize defaultSize;
        private int numHitDice;
        private DiceTypes healthDie;
        private Statistics defaultStats;

        public string DefaultName
        {
            get
            {
                return defaultName;
            }

            protected set
            {
                defaultName = value;
            }
        }

        public Image DefaultImage
        {
            get
            {
                return defaultImage;
            }

            protected set
            {
                //defaultImage = Utils.ApplyCircleMask((Bitmap)value);
                var sem = new Semaphore(0,1);
                Utils.ApplyCircleMask((Bitmap)value, (res) =>
                {
                    if(res == null)
                    {
                        MessageBox.Show("No");
                    }
                    defaultImage = res;
                    sem.Release();
                    this.Invalidate();
                });
                sem.WaitOne();
            }
        }

        public new GridPawn.RpgSize DefaultSize
        {
            get
            {
                return defaultSize;
            }

            protected set
            {
                defaultSize = value;
            }
        }

        public Statistics DefaultStatistics
        {
            get
            {
                //if(defaultStats == null)
                //{
                //    defaultStats = new Statistics();
                //}

                return defaultStats;
            }

            protected set
            {
                defaultStats = value;
            }
        }

        public int NumHitDice
        {
            get
            {
                return numHitDice;
            }

            protected set
            {
                numHitDice = Math.Max(1, value);
            }
        }

        public DiceTypes HealthDie
        {
            get
            {
                return healthDie;
            }

            protected set
            {
                healthDie = value;
            }
        }

        public bool MouseIsOver { get; private set; }

        private Rectangle rectImage;
        private Rectangle rectPie;

        static CharacterPawnTemplate()
        {
            brushBySize = new Dictionary<GridPawn.RpgSize, Brush>();

            brushBySize[GridPawn.RpgSize.Medium] = Brushes.Green;
            brushBySize[GridPawn.RpgSize.Large] = Brushes.Orange;
            brushBySize[GridPawn.RpgSize.Large_Long] = Brushes.Red;
        }

        protected CharacterPawnTemplate()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void CharacterPawnTemplate_Load(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        private void CharacterPawnTemplate_Resize(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        public string generateUniqueName()
        {
            return compact(DateTime.Now);
        }

        private string compact(DateTime now)
        {
            return String.Format(   "{0}{1}{2}{3}{4}{5}", 
                                    now.Year, now.Month, now.Day, 
                                    now.Hour, now.Minute, now.Second);
        }

        private void ComputeRectImage()
        {
            this.Width = this.Height;
            
            int borderX_2, dimX;
            GetDim(Width, out borderX_2, out dimX);
            int borderY_2, dimY;
            GetDim(Height, out borderY_2, out dimY);

            rectImage = new Rectangle(borderX_2, borderY_2, dimX, dimY);
            rectPie = new Rectangle(0, 0, Width, Height);

            var mod = 1;
            var mod2 = 2 * mod;

            var size = new Size(borderX_2 - mod2, borderY_2 - mod2);

            this.Invalidate();
        }

        private void GetDim(int baseDim, out int border_2, out int dim)
        {
            var border = (int)Math.Round(0.30 * baseDim);
            border_2 = border / 2;
            dim = baseDim - 2 * border_2;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.DoDragDrop(this, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            setMouseOver(true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            setMouseOver(false);
        }

        private void setMouseOver(bool value)
        {
            MouseIsOver = value;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.FillEllipse(brushBySize[DefaultSize], rectPie);
            
            if (DefaultImage != null)
            {
                g.DrawImage(DefaultImage, rectImage);
            }

            if (MouseIsOver)
            {
                g.DrawEllipse(highlightPen, rectPie);
            }
            else
            {
                g.DrawEllipse(circlePen, rectPie);
            }
        }

        public GridPawn Build(bool randomBuild = false)
        {
            var newPawn = new CharacterPawn();

            newPawn.Name = String.Format("{0}_{1}", DefaultName, generateUniqueName());
            newPawn.Image = DefaultImage;
            newPawn.ModSize = DefaultSize;
            newPawn.MaxPf = randomBuild ? randomPf(NumHitDice, HealthDie) : NumHitDice * (int)HealthDie;
            newPawn.MaxPf += NumHitDice * DefaultStatistics[StatsType.Constitution].Modifier();
            newPawn.CurrentPf = newPawn.MaxPf;

            return newPawn;
        }

        private int randomPf(int numHitDice, DiceTypes healthDie)
        {
            var rnd = new Random();
            var pf = (int)healthDie;

            for (int i = 1; i < numHitDice; i++)
            {
                pf += (rnd.Next((int)healthDie) + 1);
            }

            return pf;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationKey, DefaultName, typeof(string));
            info.AddValue(ImageSerializationName, DefaultImage, typeof(Image));
            info.AddValue(ModSizeSerializationName, DefaultSize, typeof(GridPawn.RpgSize));
        }
    }
}
