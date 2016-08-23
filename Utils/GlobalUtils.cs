using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilsData
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string generateUniqueName()
        {
            return compact(DateTime.Now);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static string compact(DateTime now)
        {
            return String.Format("{0}{1}{2}{3}{4}{5}",
                                    now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);
        }
    }
}
