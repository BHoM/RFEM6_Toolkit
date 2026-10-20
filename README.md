[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](https://www.gnu.org/licenses/lgpl-3.0)

# RFEM6_Toolkit

This toolkit connects the [Buildings and Habitats object Model (BHoM)](https://bhom.xyz) to [RFEM 6](https://www.dlubal.com/en/products/rfem-fea-software/rfem/what-is-rfem) using **Dlubal API II (gRPC)** via the [`Dlubal.Api.NetFramework`](https://www.nuget.org/packages/Dlubal.Api.NetFramework) package.

The legacy **RFEM Web Service** integration has been removed. Dlubal no longer maintains that API; see [Dlubal’s API transition notice](https://github.com/dlubal-software/Dlubal_CSharp_Client#important-notice-webservice-maintenance-and-dlubal-api-transition).

Object-level mapping notes are on the [RFEM6 Toolkit wiki](https://github.com/BHoM/RFEM6_Toolkit/wiki/Feature-Overview).

---

## Requirements

- **RFEM 6** running locally (adapter connects to the active instance or opens/creates a model).
- **API key** configured in RFEM (`config.ini`). The adapter default key name is `BHoM Test Key` (constructor `apiKeyName` parameter).
- **BHoM** assemblies under `C:\ProgramData\BHoM\Assemblies` (standard BHoM install).
- Client dependencies copied to `C:\ProgramData\BHoM\Assemblies\RFEM6_Client\` on build (see [Post-build deployment](#post-build-deployment) below).
- Development target: **.NET Framework 4.8**, RFEM **6.15.x** (constructor description).

---

## Current scope (v1)

### Supported — Push (`CreateOnly`)

Push uses `PushType.CreateOnly`: new objects are created in RFEM; the adapter does **not** update or delete existing RFEM entities through the default push path.

| Area | BHoM | Notes |
|------|------|--------|
| Geometry | `Node`, `Bar`, `Edge`, `Panel`, `Opening` | Edges become RFEM lines (polylines split per segment). |
| Properties | `IMaterialFragment`, `ISectionProperty`, `ISurfaceProperty` | Parametric / non-catalog sections export with limitations (warnings). |
| Supports | `Node.Support`, `Edge.Support` (`Constraint6DOF`) | Nodal and line supports; `Constraint6DOF.Name` → RFEM support **name**. |
| Releases | `Edge.Release` (`Constraint4DOF`) | Line release types on matched RFEM lines. |
| Bar hinges | `Bar` end releases | Member hinges via RFEM member hinge types. |
| Loading | `Loadcase`, `LoadCombination`, common `ILoad` types | As implemented in convert/CRUD. |

**Naming:** BHoM `Name` on node, edge, panel, bar, opening, material, and section maps to the RFEM **comment** field where applicable (`ToRFEM6Comment()`). Legacy `CustomData["Comment"]` is still read if `Name` is empty.

### Supported — Pull

| Area | BHoM | Notes |
|------|------|--------|
| Geometry | `Node`, `Bar`, `Edge`, `Panel`, `Opening`, `RFEMLine` | RFEM comments map back to BHoM `Name` where implemented. |
| Supports | `Node.Support`, `Edge.Support` | Stiffnesses and user-defined support **names** (`UserDefinedNameEnabled` in RFEM). |
| Properties & loads | Materials, sections, surfaces, load cases, loads | Per existing read/convert methods. |

### Not supported (removed or not migrated)

| Feature | Status |
|---------|--------|
| **WipeOut** (`adapter.Wipeout()`) | Clears the active model via API II `delete_all_objects` (integration tests use this in teardown). |
| **Update** / **Delete** CRUD | Not implemented for API II in this release. |
| **Results** (bar forces, nodal reactions, etc.) | Read/convert code removed; not in v1. |
| Standalone push of `RFEMNodalSupport` / `RFEMLineSupport` fragments | Supports are created from `Node` / `Edge` instead. |
| Full parametric section library parity | Many custom sections push with simplified RFEM definitions. |

Unit tests under `.ci/unit-tests` still target the old Web Service workflow in places; expect failures until they are migrated to API II.

---

## Usage sketch

```csharp
var adapter = new RFEM6Adapter(filePath: "", active: true, apiKeyName: "BHoM Test Key");
adapter.Push(objects);
var pulled = adapter.Pull(typeof(Node));
```

Call **`adapter.Wipeout()`** between integration tests (or use a dedicated RFEM model) to clear the active model.

---

## Post-build deployment

The adapter project defines a **PostBuild** target that runs after each successful build:

1. Copies `RFEM6_Adapter.dll` to `C:\ProgramData\BHoM\Assemblies\`
2. Copies dependent DLLs from the build output to `C:\ProgramData\BHoM\Assemblies\RFEM6_Client\`

This lets a local BHoM install (Rhino/Grasshopper, Excel, etc.) load the adapter and gRPC client without manual copying.

**Local builds:** If you do not want files copied to `ProgramData` (permissions, multiple toolkits, or side-by-side versions), build with:

```text
dotnet build RFEM6_Adapter\RFEM6_Adapter.csproj -p:RunPostBuildEvent=Never
```

and copy `Build\net48\` outputs yourself.

**CI:** Build agents often have no RFEM install and may block writes to `ProgramData`. Pipelines should use `RunPostBuildEvent=Never` (or remove/condition the target) and publish artefacts from `Build\net48\` instead.

**Rhino / plug-in conflicts:** Other plug-ins can load different `Grpc.Core` or `Google.Protobuf` versions. The adapter preloads assemblies from `RFEM6_Client` to reduce binding conflicts; uninstall conflicting Dlubal Grasshopper plug-ins if you see client load errors.

---

## Common issues

- **Demo licence / element limits** — API access depends on your RFEM licence; demo mode caps model size.
- **RFEM not running** — Start RFEM before push/pull; the adapter checks for the `RFEM6` process.
- **gRPC / assembly conflicts** — See post-build `RFEM6_Client` folder and error text from `Connect()`.

---

## Developers

See [Coding with BHoM](https://bhom.xyz/documentation/Guides-and-Tutorials/Coding-with-BHoM/).

Build the solution (adapter, engine, oM). Ensure BHoM DLLs are installed under `ProgramData` as for a normal BHoM deployment.

---

## Contributing

See the BHoM [contributing guidelines](https://github.com/BHoM/BHoM/blob/main/CONTRIBUTING.md).

---

## Licence

BHoM is free software licensed under the [GNU LGPL v3](https://www.gnu.org/licenses/lgpl-3.0.html).  
Each contributor holds copyright over their respective contributions.  
See [LICENSE](https://github.com/BHoM/BHoM/blob/main/LICENSE) and [COPYRIGHT_HEADER](https://github.com/BHoM/BHoM/blob/main/COPYRIGHT_HEADER.txt).
