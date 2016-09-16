using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGridUserControls
{
    public interface CharacterPawnTemplateController
    {
        void SetName(string name);
        void SetImage(Image image);
        void SetSize(GridPawn.RpgSize size);

        void SetNumHitDice(int numHitDice);
        void SetBuildHealthDie(RpgGridUserControls.Utilities.DiceTypes healthDie);
        void SetDefaultStatistics(RpgGridUserControls.Utilities.Statistics statistics);
    }
}
