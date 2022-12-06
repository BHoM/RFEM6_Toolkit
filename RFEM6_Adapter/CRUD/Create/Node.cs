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
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {

            //NOTE:A geometric object has, in general, a parent_no = 0. The parent_no parameter becomes significant for example with loads.
            int nodeId = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_NODE, 0);

            int counter = 0;

            for (int i = 0; i < bhNodes.Count(); i++)
            {
                Node bhNode = bhNodes.ToList()[i];

                rfModel.node rfNd= modelDoesContainNode(bhNode);
               
                //check if node does exist
                if (rfNd == null) {


                    rfModel.node rfNode = new rfModel.node()
                    {
                        no = nodeId + i -counter,
                        coordinates = new rfModel.vector_3d() { x = bhNode.Position.X, y = bhNode.Position.Y, z = bhNode.Position.Z },
                        coordinate_system_type = rfModel.node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "concrete part"
                    };
                    model.set_node(rfNode);

                }

                //does node have a support?
                if (bhNode.Support!=null)
                {

                    int nodalSupportId = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT, 0);

                    rfModel.node constraintLocationNode = modelDoesContainNode(bhNode);

                    //check if support already exist
                    var existinConstraint = checkForExistenceOfConstraint(bhNode.Support);
                    if (existinConstraint == null)
                    {
                        rfModel.nodal_support rfNodalSuuuport = new rfModel.nodal_support()
                        {
                            no = nodalSupportId,
                            nodes = new int[] { constraintLocationNode.no },
                            spring = new rfModel.vector_3d() { x = stiffnessTranslationBHToRF("" + bhNode.Support.TranslationX), y = stiffnessTranslationBHToRF("" + bhNode.Support.TranslationY), z = stiffnessTranslationBHToRF("" + bhNode.Support.TranslationZ) },
                            rotational_restraint = new rfModel.vector_3d() { x = stiffnessTranslationBHToRF("" + bhNode.Support.RotationX), y = stiffnessTranslationBHToRF("" + bhNode.Support.RotationY), z = stiffnessTranslationBHToRF("" + bhNode.Support.RotationZ) },
                        };

                        model.set_nodal_support(rfNodalSuuuport);
                    }
                    else
                    {
                        var a = modelDoesContainNode(bhNode);
                        var b = existinConstraint.nodes.ToList();
                        b.Add(a.no);
                        existinConstraint.nodes=b.ToArray();
                        model.delete_object(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT, existinConstraint.no, 0);
                        model.set_nodal_support(existinConstraint);
                    }
                }
            }

            return true;
        }

        private rfModel.node modelDoesContainNode(Node bhNode)
        {

            var numbers=model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODE);
            var foundNode=numbers.ToList().Select(n => model.get_node(n.no));

            var collectedNode = foundNode.Where(n => (n.coordinate_1.Equals(bhNode.Position.X) && n.coordinate_2.Equals(bhNode.Position.Y) && n.coordinate_3.Equals(bhNode.Position.Z)));

            if (collectedNode.ToList().Count>0)
            {
                return collectedNode.ToList().First(); 
            }

            return null;
        }

        private Double stiffnessTranslationBHToRF(String stiffness)
        {

            Double result = stiffness=="Free"?0.0:Double.PositiveInfinity;

            return result;
        }

        private rfModel.nodal_support checkForExistenceOfConstraint(BH.oM.Structure.Constraints.Constraint6DOF constraint)
        {

            //gettin all nodalsupport from model
            var numbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
            var allSupports = numbers.ToList().Select(n => model.get_nodal_support(n.no));

            //check if there is one support in model that does fullfill all criterial for equality
            foreach (var s in allSupports)
            {
               bool x = s.spring.x.Equals(stiffnessTranslationBHToRF("" + constraint.TranslationX));
               bool y = s.spring.y.Equals(stiffnessTranslationBHToRF("" + constraint.TranslationY));
               bool z = s.spring.z.Equals(stiffnessTranslationBHToRF("" + constraint.TranslationZ));
               bool xx = s.rotational_restraint.x.Equals(stiffnessTranslationBHToRF("" + constraint.RotationX));
               bool yy = s.rotational_restraint.y.Equals(stiffnessTranslationBHToRF("" + constraint.RotationY));
               bool zz = s.rotational_restraint.z.Equals(stiffnessTranslationBHToRF("" + constraint.RotationZ));

                if (x&&y&&z&&xx&&yy&&zz)
                {
                    return s;
                }

            }

            return null;
        }

    }
}
