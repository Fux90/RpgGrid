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
            private static string buildName;
            private static Image buildImage;
            private static GridPawn.RpgSize buildSize;
            private static int buildNumHitDice;
            private static DiceTypes buildHealthDie;
            private static Statistics buildDefaultStatistics;

            static Builder()
            {
                SetDefults();
            }

            #region DEFAULTS
            
            private static void SetDefults()
            {
                SetDefaultName();
                SetDefaultImage();
                SetDefaultSize();
                SetDefaultNumHitDice();
                SetDefaultHealthDie();
                SetDefaultStatistics();
            }

            private static void SetDefaultName()
            {
                BuildName = "Nog";
            }

            private static void SetDefaultImage()
            {
                // Change with resource
                BuildImage = Image.FromFile(@"character2.jpg");
            }

            private static void SetDefaultSize()
            {
                BuildSize = GridPawn.RpgSize.Medium;
            }

            private static void SetDefaultNumHitDice()
            {
                buildNumHitDice = 1;
            }

            private static void SetDefaultHealthDie()
            {
                BuildHealthDie = DiceTypes.d8;
            }

            private static void SetDefaultStatistics()
            {
                BuildStatistics = new Statistics(null, new Dictionary<StatsType, int>()
                {
                    {StatsType.Strength, 14},
                    {StatsType.Constitution, 12},
                });
            }

            #endregion

            #region PROPERTIES

            public static string BuildName
            {
                get
                {
                    return buildName;
                }

                set
                {
                    buildName = value;
                    var invalidName = buildName == null || buildName == "";

                    if (invalidName)
                    {
                        buildName = "Npg";
                    }
                }
            }

            public static Image BuildImage
            {
                get
                {
                    return buildImage;
                }

                set
                {
                    buildImage = value;
                    var invalidImage = buildImage == null;

                    if (invalidImage)
                    {
                        buildImage = Image.FromFile(@"character2.jpg");
                    }
                }
            }

            public static GridPawn.RpgSize BuildSize
            {
                get
                {
                    return buildSize;
                }

                set
                {
                    buildSize = value;
                }
            }

            public static int BuildNumHitDice
            {
                get
                {
                    return buildNumHitDice;
                }

                set
                {
                    buildNumHitDice = value;
                    var invalidNumHitDice = buildNumHitDice < 1;

                    if (invalidNumHitDice)
                    {
                        buildNumHitDice = 1;
                    }
                }
            }

            public static DiceTypes BuildHealthDie
            {
                get
                {
                    return buildHealthDie;
                }

                set
                {
                    buildHealthDie = value;
                }
            }

            public static Statistics BuildStatistics
            {
                get
                {
                    return buildDefaultStatistics;
                }

                set
                {
                    buildDefaultStatistics = value;
                }
            }

            #endregion

            public static CharacterPawnTemplate Create()
            {
                return new CharacterPawnTemplate()
                {
                    DefaultName = BuildName,
                    DefaultImage = BuildImage,
                    DefaultSize = BuildSize,
                    NumHitDice = BuildNumHitDice,
                    HealthDie = BuildHealthDie,
                    DefaultStatistics = BuildStatistics,
                };
            }
        }

        private const string NameSerializationKey = "name";
        private const string ImageSerializationKey = "image";
        private const string ModSizeSerializationKey = "size";
        private const string NumHitDiceSerializationKey = "numDice";
        private const string HealthDieSerializationKey = "diceType";
        private const string StatisticsSerializationKey = "stats";

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
                RpgGridControlUtils.ApplyCircleMask((Bitmap)value, (res) =>
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
            init();
        }

        public CharacterPawnTemplate(SerializationInfo info, StreamingContext context)
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            DefaultName = info.GetString(NameSerializationKey);
            DefaultImage = (Image)info.GetValue(ImageSerializationKey, typeof(Image));
            DefaultSize = (GridPawn.RpgSize)info.GetValue(ModSizeSerializationKey, typeof(GridPawn.RpgSize));
            NumHitDice = info.GetInt32(NumHitDiceSerializationKey);
            HealthDie = (DiceTypes)info.GetValue(HealthDieSerializationKey, typeof(DiceTypes));
            DefaultStatistics = (Statistics)info.GetValue(StatisticsSerializationKey, typeof(Statistics));

            ComputeRectImage();
        }

        private void init()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void CreateToolTip()
        {
            // Create the ToolTip and associate with the Form container.
            var toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this, this.ToString());
        }

        private void CharacterPawnTemplate_Load(object sender, EventArgs e)
        {
            ComputeRectImage();
            CreateToolTip();
        }

        private void CharacterPawnTemplate_Resize(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        public string generateUniqueName()
        {
            return UtilsData.Utils.generateUniqueName();
        }

        //public string generateUniqueName()
        //{
        //    return compact(DateTime.Now);
        //}

        //private string compact(DateTime now)
        //{
        //    return String.Format(   "{0}{1}{2}{3}{4}{5}", 
        //                            now.Year, now.Month, now.Day, 
        //                            now.Hour, now.Minute, now.Second);
        //}

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
            var newPawn = new CharacterPawn(true);

            newPawn.Name = String.Format("{0}_{1}", DefaultName, generateUniqueName());
            newPawn.Image = DefaultImage;
            newPawn.ModSize = DefaultSize;
            newPawn.MaxPf = randomBuild ? randomPf(NumHitDice, HealthDie) : NumHitDice * (int)HealthDie;
            newPawn.MaxPf += NumHitDice * DefaultStatistics[StatsType.Constitution].Modifier();
            newPawn.CurrentPf = newPawn.MaxPf;
            newPawn.Stats = new Statistics(newPawn, DefaultStatistics);

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
            info.AddValue(ImageSerializationKey, DefaultImage, typeof(Image));
            info.AddValue(ModSizeSerializationKey, DefaultSize, typeof(GridPawn.RpgSize));
            info.AddValue(NumHitDiceSerializationKey, NumHitDice, typeof(int));
            info.AddValue(HealthDieSerializationKey, HealthDie, typeof(DiceTypes));
            info.AddValue(StatisticsSerializationKey, DefaultStatistics, typeof(Statistics));

        }

        public override string ToString()
        {
            var strB = new StringBuilder();

            strB.AppendLine(DefaultName);
            strB.AppendLine(DefaultSize.ToString());
            strB.AppendFormat("{0}{1}", NumHitDice, HealthDie.ToString());
            strB.AppendLine(DefaultStatistics.ToString());
            strB.AppendLine();

            return strB.ToString();
        }
    }
}
