using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {
        public static int GetRFEM6ID(this IBHoMObject obj)
        {
            return obj.AdapterId<int>(typeof(RFEM6ID));
        }
    }
}
