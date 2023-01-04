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

            foreach (ISectionProperty section in sectionProperties) {

                if (csDoesAlreadyExist(section)) { continue; }

                rfModel.object_with_children[] materials = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
                var materialNumbers = materials.ToList().Select(m => m.no);
                int matNo = 1;

                foreach (int n in materialNumbers) {

                    var rfMatrial = model.get_material(n);
                    string rfMatName = rfMatrial.name.Split('|')[0].Trim(new Char[] { ' '});

                    if (rfMatName.Equals(section.Material.Name)) {
                        matNo = n;
                    
                        break;
                    }
                }

                bool csDoesExist=csDoesAlreadyExist(section);

                // create section
                //rfModel.section sectionSteelCSBeam = new rfModel.section
                //{
                //    no = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_SECTION, 0),
                //    material = matNo,
                //    materialSpecified = true,
                //    name = section.Name,
                //    typeSpecified = true,
                //    type = rfModel.section_type.TYPE_STANDARDIZED_STEEL,
                //    manufacturing_type = rfModel.section_manufacturing_type.MANUFACTURING_TYPE_HOT_ROLLED,
                //    manufacturing_typeSpecified = true,
                //    thin_walled_model = true,
                //    thin_walled_modelSpecified = true,
                //};

                rfModel.section sectionSteelCSBeam = section.ToRFEM6(model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_SECTION, 0),matNo);

                model.set_section(sectionSteelCSBeam);

            }

            return true;

        }

        private bool csDoesAlreadyExist(ISectionProperty bhSec) {

            rfModel.object_with_children[] sec = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
            var secNum = sec.ToList().Select(m => m.no).ToList();
            int matNo = 1;

            foreach (int n in secNum)
            {
                var rfSec = model.get_section(n);
                string rfSecName = rfSec.name.Split('|')[0].Trim(new Char[] { ' ' });

                if (rfSecName.Equals(bhSec.Name))
                {
                    return true;
                }

            }

            return false;

        }

    }
}
