using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGridUserControls
{
    public interface CharacterPawnController
    {
        void RegisterCharacterPawn(CharacterPawn pawn);

        void SetName(string name);
        void SetPf(int pf);
        void SetMaxPf(int maxPf);
        void SetImage(Image image);
        void SetSize(GridPawn.RpgSize size);
        void SetNotes(string notes); 
    }
}
