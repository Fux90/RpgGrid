using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGridUserControls
{
    public interface CharacterPawnTemplateListener
    {
        void ShowName();
        void ShowImage();
        void ShowSize();

        void ShowNumHitDice();
        void ShowBuildHealthDie();
        void ShowDefaultStatistics();

        void ShowAll();
    }
}
