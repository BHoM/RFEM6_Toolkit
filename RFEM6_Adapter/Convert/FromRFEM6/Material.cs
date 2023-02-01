using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static IMaterialFragment FromRFEM(this rfModel.material rfMaterial)
        {

            string s = rfMaterial.generating_object_info;
            IMaterialFragment bhMaterial = null;

            String[] matParaArray = rfMaterial.comment.Split('|');


            if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_STEEL) || rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_REINFORCING_STEEL))
            {

                double density = (matParaArray.Length > 1) ? Double.Parse(matParaArray[1]) : 0;
                double dambingRatio = (matParaArray.Length > 1) ? Double.Parse(matParaArray[2]) : 0;
                double poissonsRatio = (matParaArray.Length > 1) ? Double.Parse(matParaArray[3]) : 0;
                double thermalExpansionCoeff = (matParaArray.Length > 1) ? Double.Parse(matParaArray[4]) : 0;
                double youngsModulus = (matParaArray.Length > 1) ? Double.Parse(matParaArray[5]) : 0;
                double yieldStress = (matParaArray.Length > 1) ? Double.Parse(matParaArray[6]) : 0;
                double ultimateStress = (matParaArray.Length > 1) ? Double.Parse(matParaArray[7]) : 0;

                bhMaterial = Engine.Structure.Create.Steel(matParaArray[0], youngsModulus, poissonsRatio, thermalExpansionCoeff, density, dambingRatio, yieldStress, ultimateStress);



            }

            else if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_CONCRETE))
            {

                double density = (matParaArray.Length > 1) ? Double.Parse(matParaArray[1]) : 0;
                double dambingRatio = (matParaArray.Length > 1) ? Double.Parse(matParaArray[2]) : 0;
                double poissonsRatio = (matParaArray.Length > 1) ? Double.Parse(matParaArray[3]) : 0;
                double thermalExpansionCoeff = (matParaArray.Length > 1) ? Double.Parse(matParaArray[4]) : 0;
                double youngsModulus = (matParaArray.Length > 1) ? Double.Parse(matParaArray[5]) : 0;
                double cubeStrength = (matParaArray.Length > 1) ? Double.Parse(matParaArray[6]) : 0;
                double cylinderStrength = (matParaArray.Length > 1) ? Double.Parse(matParaArray[7]) : 0;

                bhMaterial = Engine.Structure.Create.Concrete(matParaArray[0], youngsModulus, poissonsRatio, thermalExpansionCoeff, density, dambingRatio, cubeStrength, cylinderStrength);

            }



            return bhMaterial;
        }

    }
}
