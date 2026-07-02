[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](https://www.gnu.org/licenses/lgpl-3.0) [![Build status](https://ci.appveyor.com/api/projects/status/cko96y5mg2wm4myr/branch/master?svg=true)](https://ci.appveyor.com/api/projects/status/cko96y5mg2wm4myr/branch/master?svg=true) [![Build Status](https://dev.azure.com/BHoMBot/BHoM/_apis/build/status/Lusas_Toolkit/Lusas_Toolkit.CheckCore?branchName=master)](https://dev.azure.com/BHoMBot/BHoM/_build/latest?definitionId=108&branchName=master)

# RFEM6_Toolkit

This toolkit allows interoperability between the BHoM and (RFEM6)[https://www.dlubal.com/en/products/rfem-fea-software/rfem/what-is-rfem?srsltid=AfmBOoq3j1_L003WaFTw4thLi0UWIl8W-Yjd-JDNCkF6-5xb45P4t66Z]. It enables creation, manipulation and reading of structural finite element analysis models and analysis results. Please see the [BHoM RFEM6 Object Relations](https://github.com/BHoM/RFEM6_Toolkit/wiki/Feature-Overview) for a comprehensive list of supported BHoM objects.

### API
The RFEM6_Toolkit uses the RFEMWebServiceLibrary to connect to active sessions of RFEM. The webservice is no longer maintained or actively developed by Dlubal. Refer to:
https://github.com/dlubal-software/Dlubal_CSharp_Client#important-notice-webservice-maintenance-and-dlubal-api-transition

### Tested versions of RFEM6 for 9.2 (using `RFEM6AdapterV12_11`)

6.14.0008  
6.14.0002

To use 6.12.11 or earlier use the `RFEM6AdapterV8_2`. 

For RFEM5 refer to: https://github.com/BHoM/RFEM5_Toolkit

### Common Errors
- Not all licences support legacy API usage - you will recieve errors relating to a demo licence which limits the number of elements to 12.
- The Dlubal Grasshopper Plugin should be uninstalled as it may cause conflicts with the RFEM6_Toolkit.

### Documentation
For more information about functionality see the [RFEM6_Toolkit Wiki](https://github.com/BHoM/RFEM6_Toolkit/wiki)

---
This toolkit is part of the Buildings and Habitats object Model. Find out more on our [wiki](https://github.com/BHoM/documentation/wiki) or at [https://bhom.xyz](https://bhom.xyz/)

## Quick Start 🚀 

Grab the [latest installer](https://bhom.xyz/) and a selection of [sample scripts](https://github.com/BHoM/samples).


## Getting Started for Developers 🤖 

If you want to build the BHoM and the Toolkits from source, it's hopefully easy! 😄 
Do take a look at our specific wiki pages here: [Getting Started for Developers](https://bhom.xyz/documentation/Guides-and-Tutorials/Coding-with-BHoM/)


## Want to Contribute? ##

BHoM is an open-source project and would be nothing without its community. Take a look at our contributing guidelines and tips [here](https://github.com/BHoM/BHoM/blob/main/CONTRIBUTING.md).


## Licence ##

BHoM is free software licenced under GNU Lesser General Public Licence - [https://www.gnu.org/licenses/lgpl-3.0.html](https://www.gnu.org/licenses/lgpl-3.0.html)  
Each contributor holds copyright over their respective contributions.
The project versioning (Git) records all such contribution source information.
See [LICENSE](https://github.com/BHoM/BHoM/blob/main/LICENSE) and [COPYRIGHT_HEADER](https://github.com/BHoM/BHoM/blob/main/COPYRIGHT_HEADER.txt).
