using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        //Has been implemented inside of Nodes.cs

        private bool CreateCollection(IEnumerable<ISectionProperty> sectionProperties)
        {

            foreach (ISectionProperty section in sectionProperties)
            {

               // if (csDoesAlreadyExist(section)) { continue; }

                rfModel.object_with_children[] materials = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
                var materialNumbers = materials.ToList().Select(m => m.no);
                int matNo = m_Model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_MATERIAL, 0);

                foreach (int n in materialNumbers)
                {

                    var rfMatrial = m_Model.get_material(n);
                    string rfMatName = rfMatrial.name.Split('|')[0].Trim(new Char[] { ' ' });

                    if (rfMatName.Equals(section.Material.Name))
                    {
                        matNo = n;

                        break;
                    }
                }

                rfModel.section rfSection = section.ToRFEM6( matNo, section.Material.GetType().Name); ;

                m_Model.set_section(rfSection);

            }

            return true;

        }

        //private bool csDoesAlreadyExist(ISectionProperty bhSec)
        //{

        //    rfModel.object_with_children[] sec = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
        //    var secNum = sec.ToList().Select(m => m.no).ToList();
        //    int matNo = 1;

        //    foreach (int n in secNum)
        //    {

        //        var x = bhSec.ToRFEM6(n, matNo, bhSec.Material.GetType().Name.ToString());

        //        var rfSec = model.get_section(n);
        //        string rfSecName = rfSec.name.Split('|')[0].Trim(new Char[] { ' ' });

        //        if (rfSecName.Equals(bhSec.Name) || rfSecName.Equals(x.name))
        //        {
        //            return true;
        //        }

        //    }

        //    return false;

        //}

    }
}
