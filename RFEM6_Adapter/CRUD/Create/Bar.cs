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

        private bool CreateCollection(IEnumerable<Bar> bhBars)
        {


            foreach (Bar bhBar in bhBars) {

                //Find Corresponding Nodes
                rfModel.object_with_children[] rfNodesByType =  model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
                IEnumerable<int> rfNodeNumbers = rfNodesByType.ToList().Select(m => m.no);

                List<rfModel.node> nodeList = new List<rfModel.node>();

                rfModel.node start = getNodeFromRFModel(bhBar.StartNode);
                rfModel.node end = getNodeFromRFModel(bhBar.EndNode);

                if (start!=null && end!=null) { 
                
                    //rfModel.line rfLine = new rfModel.line();

                    rfModel.line rfLine = new rfModel.line()
                    {
                        no = 1,
                        definition_nodes = new int[] { start.no, end.no },
                        type = rfModel.line_type.TYPE_POLYLINE,
                    };

                    model.set_line(rfLine);
                }

            }

            //Create Lines
            

            //Create Material
            //Create Sections
            //Create Bar


            return true;
        }

    }
}
