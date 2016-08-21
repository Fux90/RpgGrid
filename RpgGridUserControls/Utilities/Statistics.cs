using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RpgGridUserControls.Utilities
{
    public enum StatsType
    {
        Strength,
        Dexterity,
        Constitution,
        Wisdom,
        Intelligence,
        Charisma
    }

    [Serializable]
    public class Statistics : ISerializable
    {
        private const string StatsSerializationName = "stats";
        private const string CurrentPawnSerializationName = "pawn";

        private const int defaultValueIfMissingStat = 10;
        private static StatsType[] allTypes;

        private CharacterPawn currentPawn;
        private Dictionary<StatsType, int> stats;

        static Statistics()
        {
            allTypes = (StatsType[])Enum.GetValues(typeof(StatsType));
        }

        public Statistics(CharacterPawn pawn)
            : this(pawn, new Dictionary<StatsType, int>())
        {
            
        }

        public Statistics(CharacterPawn pawn, Dictionary<StatsType, int> baseStats)
        {
            stats = baseStats;
            currentPawn = pawn;
        }

        public Statistics(CharacterPawn pawn, Statistics model)
            : this(pawn)
        {
            foreach (var key in allTypes)
            {
                this[key] = model[key];
            }
        }

        public Statistics(SerializationInfo info, StreamingContext context)
        {
            stats = (Dictionary<StatsType, int>)info.GetValue(StatsSerializationName, typeof(Dictionary<StatsType, int>));
            currentPawn = (CharacterPawn)info.GetValue(CurrentPawnSerializationName, typeof(CharacterPawn));
        }

        public int this[StatsType type]
        {
            get
            {
                if (!stats.ContainsKey(type))
                {
                    return defaultValueIfMissingStat;
                }
                else
                {
                    return stats[type];
                }
            }

            set
            {
                stats[type] = value;
                currentPawn.InvalidateTooltipDescription();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(StatsSerializationName, stats, typeof(Dictionary<StatsType, int>));
            info.AddValue(CurrentPawnSerializationName, currentPawn, typeof(CharacterPawn));
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool minimal)
        {
            var strB = new StringBuilder();

            var keys = minimal ? stats.Keys.ToArray() : allTypes;

            foreach (var key in keys)
            {
                var value = this[key];
                var mod = value.Modifier();
                strB.AppendLine();
                strB.AppendFormat("{0}: {1} ({2:+0;-0;0})", key, value, mod);
            }
            return strB.ToString();
        }
    }

    public static class StatisticsExt
    {
        public static int Modifier(this int statValue)
        {
            return (int)Math.Floor(statValue / 2.0f - 5.0f);
        }
    }
}
