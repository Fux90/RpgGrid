using RpgGridUserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement
{
    public sealed class ResourceManager
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

        private ResourceManager()
        {

        }

        public GridPawn[] RetrievePawns()
        {
            // TODO: retrieve images from zip file and info from XML/db/whatever
            return new GridPawn[]
            {
                new CharacterPawn()
                {
                    CurrentPf = 59,
                    MaxPf = 60,
                    Image = Image.FromFile(@"character1.png"),
                    //ModSize = RpgGridUserControls.GridPawn.RpgSize.Large_Long,
                }
            };
        }
    }
}
