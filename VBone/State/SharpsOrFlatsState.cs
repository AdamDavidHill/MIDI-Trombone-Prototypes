using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBone.State
{
    public static class SharpsOrFlatsState
    {
        private static bool preferSharps;

        public static bool PreferSharps
        {
            get
            {
                return preferSharps;
            }

            set
            {
                preferSharps = value;
                if (PreferSharpsChanged != null)
                {
                    PreferSharpsChanged(null, null);
                }
            }
        }

        public static event EventHandler PreferSharpsChanged;
    }
}
