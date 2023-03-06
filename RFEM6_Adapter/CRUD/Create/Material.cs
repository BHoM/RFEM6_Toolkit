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
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<IMaterialFragment> materialFragments)
        {



            foreach (IMaterialFragment bhMaterial in materialFragments) {

                rfModel.material rfMaterial = bhMaterial.ToRFEM6();
                
                m_Model.set_material(rfMaterial);
            
            }

            return true;
        }

       
    }
}
