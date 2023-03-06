using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Update Node                               ****/
        /***************************************************/

        private bool UpdateObjects(IEnumerable<ISectionProperty> sections)
        {
            bool success = true;

            foreach (ISectionProperty section in sections)
            {
                Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();
                int materialID=section.Material.GetRFEM6ID();
                IMaterialFragment material;
                materials.TryGetValue(materialID, out material);
                String materialName=material.GetType().Name;

                m_Model.set_section(section.ToRFEM6(section.Material.GetRFEM6ID(), materialName));
            }

            return success;
        }

    }
}
