using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Update Node                               ****/
        /***************************************************/

        private bool UpdateObjects(IEnumerable<IMaterialFragment> materials)
        {
            bool success = true;

            foreach (IMaterialFragment material in materials)
            {
                model.set_material(material.ToRFEM6());
            }

            return success;
        }

    }
}
