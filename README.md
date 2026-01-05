# AtomCoLab-VR

**Collaborative Virtual Reality for Large-Scale Atomistic Visualization**

AtomCoLab-VR is a VR application for immersive exploration of molecular dynamics simulations and atomistic structures at scale. Built for Meta Quest 3/3S, it enables researchers to collaboratively visualize and discuss million-atom simulations in virtual reality.

![License](https://img.shields.io/badge/license-PolyForm%20Noncommercial-blue)
![Platform](https://img.shields.io/badge/platform-Meta%20Quest%203%2F3S-purple)
![Unity](https://img.shields.io/badge/unity-2022.3%20LTS-black)

## Features (Planned)

### Visualization
- **Massive Scale**: Render 1M+ atoms using GPU instancing
- **Custom Format**: Optimized `.aclvr` binary format for fast loading
- **Cutting Planes**: Slice through structures to reveal internal features
- **Property Coloring**: Color atoms by centro-symmetry, energy, etc.

### Collaboration
- **Multi-User**: Up to 12 users in the same virtual space
- **Voice Chat**: Spatial audio via Photon Voice 2
- **Two Modes**:
  - *Shared Object*: One person manipulates while others observe
  - *Follow Lead*: Everyone sees exactly what the presenter sees

### Data Pipeline
- **OVITO Integration**: Export directly from OVITO using Python scripts
- **LAMMPS Compatible**: Designed for MD simulation output

## Requirements

- Meta Quest 3 or Quest 3S
- Unity 2022.3 LTS (for development)
- OVITO 3.9+ (for data export)

## Documentation

- [Technical Specification](SPECIFICATION.md) - Architecture and requirements
- [File Format](Docs/FileFormat.md) - `.aclvr` binary format details

## Project Status

🚧 **Early Development** - Core rendering system in progress

See [SPECIFICATION.md](SPECIFICATION.md) for the full technical specification and roadmap.

## License

Dual licensed:
- **Noncommercial**: [PolyForm Noncommercial 1.0.0](LICENSE)
- **Commercial**: [Contact for licensing](LICENSE-COMMERCIAL.md)
