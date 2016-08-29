using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGridUserControls
{
    public class GridPawnValueChanger : CharacterPawnController
    {
        private CharacterPawn currentPawn;

        public static GridPawnValueChanger current;
        public static GridPawnValueChanger GetCurrent(CharacterPawn pawn)
        {
            if(current == null)
            {
                current = new GridPawnValueChanger(pawn);
            }

            return current;
        }

        private GridPawnValueChanger(CharacterPawn pawn)
        {
            current.RegisterCharacterPawn(pawn);
        }

        public void RegisterCharacterPawn(CharacterPawn pawn)
        {
            currentPawn = pawn;
        }

        #region SET

        public void SetImage(Image image)
        {
            throw new NotImplementedException();
        }

        public void SetMaxPf(int maxPf)
        {
            throw new NotImplementedException();
        }

        public void SetName(string name)
        {
            throw new NotImplementedException();
        }

        public void SetNotes(string notes)
        {
            throw new NotImplementedException();
        }

        public void SetPf(int pf)
        {
            throw new NotImplementedException();
        }

        public void SetSize(GridPawn.RpgSize size)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void SetValueByType(GridPawnValueChangedEventArgs.ChangeableItems valueType, object value)
        {
            switch (valueType)
            {
                case GridPawnValueChangedEventArgs.ChangeableItems.Name:
                    SetName((string)value);
                    break;
                case GridPawnValueChangedEventArgs.ChangeableItems.Pf:
                    SetPf((int)value);
                    break;
                case GridPawnValueChangedEventArgs.ChangeableItems.MaxPf:
                    SetMaxPf((int)value);
                    break;
                case GridPawnValueChangedEventArgs.ChangeableItems.Image:
                    SetImage((Image)value);
                    break;
                case GridPawnValueChangedEventArgs.ChangeableItems.Size:
                    SetSize((GridPawn.RpgSize)value);
                    break;
                case GridPawnValueChangedEventArgs.ChangeableItems.Notes:
                    SetNotes((string)value);
                    break;
                default:
                    break;
            }
        }
    }
}
