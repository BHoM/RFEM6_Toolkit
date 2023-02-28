using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        protected override object NextFreeId(Type objectType, bool refresh = false)
        {

            int index = 1;

            if (!refresh && m_FreeIds.TryGetValue(objectType, out index))
            {
                index++;
                m_FreeIds[objectType] = index;
                return index;
            }
            else
            {
                rfModel.object_types? rfType = objectType.ToRFEM6();

                if (!rfType.HasValue)
                {
                    return null;
                }

                int id = model.get_first_free_number(rfType.Value, 0);
                m_FreeIds[objectType] = id;
                return id;
        }
    }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        Dictionary<Type, int> m_FreeIds = new Dictionary<Type, int>();

        /***************************************************/
    }
}
