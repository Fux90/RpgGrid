using NetUtils;
using ResourceManagement;
using RpgGridUserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGrid
{
    public class RpgGrid : CommunicationModel
    {
        public ResourceManager ResourceManager{ get; private set; }

        public RpgGrid()
        {
            ResourceManager = ResourceManager.Current;
        }
    }
}
