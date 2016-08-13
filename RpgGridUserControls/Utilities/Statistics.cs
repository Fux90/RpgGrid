using System;
using System.Collections.Generic;
using System.Linq;
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

    public class Statistics
    {
        private const int defaultValueIfMissingStat = 0;

        private Dictionary<StatsType, int> stats;

        public Statistics()
            : this(new Dictionary<StatsType, int>())
        {
            
        }

        public Statistics(Dictionary<StatsType, int> baseStats)
        {
            stats = baseStats;
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
            }
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
