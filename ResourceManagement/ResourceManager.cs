using RpgGridUserControls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement
{
    public sealed partial class ResourceManager
    {
        private static ResourceManager current;
        public static ResourceManager Current
        {
            get
            {
                if(current == null)
                {
                    current = new ResourceManager();
                }
                return current;
            }
        }

        public bool ParallelExecution { get; set; }

        private ResourceManager()
        {
            ParallelExecution = true;   
        }

        public delegate void GridPawnResultCallback(GridPawn[] pawns);
        public delegate void ErrorCallback(Exception ex);

        public GridPawn[] RetrievePawns()
        {
            // TODO: retrieve images from zip file and info from XML/db/whatever
            //return new GridPawn[]
            //{
            //    new CharacterPawn()
            //    {
            //        CurrentPf = 59,
            //        MaxPf = 60,
            //        Image = Image.FromFile(@"character1.png"),
            //        //ModSize = RpgGridUserControls.GridPawn.RpgSize.Large_Long,
            //    }
            //};

            var pawns = new GridPawn[30];

            var partition = Partitioner.Create(0, pawns.Length);
            var parOption = new ParallelOptions()
            {
                MaxDegreeOfParallelism = ParallelExecution ? -1 : 1
            };

            Parallel.ForEach(partition, parOption, p => {
                for (int i = p.Item1; i < p.Item2; i++)
                {
                    pawns[i] = new CharacterPawn()
                    {
                        CurrentPf = 59,
                        MaxPf = 60,
                        Image = Image.FromFile(@"character1.png"),
                        ModSize = i > 10 ? RpgGridUserControls.GridPawn.RpgSize.Large : RpgGridUserControls.GridPawn.RpgSize.Medium,
                    };
                };
            });

            return pawns;
        }

        public void AsyncRetrievePawns(GridPawnResultCallback callback, ErrorCallback onError = null)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                e.Result = RetrievePawns();
            };
            bw.RunWorkerCompleted += (s, e) =>
            {
                if(e.Error == null)
                {
                    callback((GridPawn[])e.Result);
                }
                else
                {
                    if (onError != null)
                    {
                        onError(e.Error);
                    }
                }
            };

            bw.RunWorkerAsync();
        }
    }
}
