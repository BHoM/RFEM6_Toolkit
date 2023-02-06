using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<Bar> bhBars)
        {


            foreach (Bar bhBar in bhBars)
            {

                //Find Corresponding Nodes
                rfModel.object_with_children[] rfNodesByType = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
                IEnumerable<int> rfNodeNumbers = rfNodesByType.ToList().Select(m => m.no);

                List<rfModel.node> nodeList = new List<rfModel.node>();

                rfModel.node start = getNodeFromRFModel(bhBar.StartNode);
                rfModel.node end = getNodeFromRFModel(bhBar.EndNode);

                if (start != null && end != null)
                {

                    //rfModel.line rfLine = new rfModel.line();

                    if (!lineDoesExist(start, end))
                    {

                        rfModel.line rfLine = new rfModel.line()
                        {
                            no = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_LINE, 0),
                            definition_nodes = new int[] { start.no, end.no },
                            type = rfModel.line_type.TYPE_POLYLINE,
                        };

                        model.set_line(rfLine);


                        rfModel.member rfMember = bhBar.ToRFEM6(model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_MEMBER, 0), rfLine, getRFSection(bhBar.SectionProperty));

                        model.set_member(rfMember);

                    }

                }

            }

            return true;
        }

        private bool lineDoesExist(rfModel.node n0, rfModel.node n1)
        {

            rfModel.object_with_children[] rfLineByType = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
            IEnumerable<int> rfLineNumbers = rfLineByType.ToList().Select(m => m.no);

            foreach (int l in rfLineNumbers)
            {


                rfModel.line currentLine = model.get_line(l);

                HashSet<int> nodeIds = currentLine.definition_nodes.ToHashSet();


                if (nodeIds.Contains(n0.no) && nodeIds.Contains(n1.no))
                {

                    return true;

                }

            }

            return false;
        }

        public rfModel.section getRFSection(ISectionProperty bhSection)
        {



            rfModel.object_with_children[] rfSectionsByType = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
            IEnumerable<int> rfSectionNumbers = rfSectionsByType.ToList().Select(m => m.no);
            string bhSectionName = String.Concat(bhSection.Name.Where(c => !Char.IsWhiteSpace(c))).Split('|')[0].ToUpper();

            foreach (int n in rfSectionNumbers)
            {

                rfModel.section currSection = model.get_section(n);

                string rfSectionName = String.Concat(currSection.name.Where(c => !Char.IsWhiteSpace(c))).Split('|')[0].ToUpper(); ;

                if (rfSectionName.Equals(bhSectionName))
                {
                    return model.get_section(n);
                }

            }

            return null;



        }
    }
}
