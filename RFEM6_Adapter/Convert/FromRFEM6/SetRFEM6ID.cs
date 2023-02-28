using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Base;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static void SetRFEM6ID(this IBHoMObject obj, object id)
        {
            obj.SetAdapterId(typeof(RFEM6ID), id);
        }

    }
}
