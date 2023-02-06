using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Bar> ReadBar(List<string> ids = null)
        {

            List<Bar> barList = new List<Bar>();

            
            rfModel.object_with_children[] memberNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER);
            List<rfModel.member> foundMembers = memberNumbers.ToList().Select(n => model.get_member(n.no)).ToList();
            var foundMembers2 = new List<rfModel.member>();


            foreach (var f in memberNumbers.ToList()) {

                foundMembers2.Add(model.get_member(f.no));

            }
 
            foreach (rfModel.member m in foundMembers) {


                rfModel.section rfSection = model.get_section(m.section_start);
                rfModel.material rfMaterial = model.get_material(rfSection.material);
                rfModel.line rfLine = model.get_line(m.line);
                rfModel.node rfNode1=model.get_node(rfLine.definition_nodes[0]);
                rfModel.node rfNode2 = model.get_node(rfLine.definition_nodes[1]);

                barList.Add(m.FromRFEM(rfNode1,rfNode2, rfMaterial,rfSection));
               
            }

            return barList;
        }

    }
}
