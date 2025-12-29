# Contributing to AtomCoLab-VR

Thank you for your interest in contributing to AtomCoLab-VR! This document provides guidelines and instructions for contributing.

## Code of Conduct

By participating in this project, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

## License Agreement

By contributing to AtomCoLab-VR, you agree that your contributions will be licensed under the PolyForm Noncommercial License 1.0.0, and you grant the project maintainers the right to include your contributions in commercially licensed versions of the software.

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](https://github.com/your-org/AtomCoLab-VR/issues)
2. If not, create a new issue with:
   - Clear, descriptive title
   - Steps to reproduce
   - Expected vs actual behavior
   - Unity version and Quest device model
   - Screenshots or video if applicable

### Suggesting Features

1. Check existing [Discussions](https://github.com/your-org/AtomCoLab-VR/discussions) for similar ideas
2. Create a new discussion with:
   - Clear description of the feature
   - Use case and motivation
   - Potential implementation approach

### Pull Requests

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes following our coding standards
4. Write or update tests as needed
5. Commit with clear messages: `git commit -m "Add: brief description"`
6. Push to your fork: `git push origin feature/your-feature-name`
7. Open a Pull Request

## Development Setup

### Prerequisites

- Unity 2022.3 LTS
- Meta XR SDK
- Android SDK with Quest support
- Git LFS (for large assets)

### Getting Started

```bash
# Clone your fork
git clone https://github.com/YOUR-USERNAME/AtomCoLab-VR.git
cd AtomCoLab-VR

# Add upstream remote
git remote add upstream https://github.com/your-org/AtomCoLab-VR.git

# Install Git LFS
git lfs install
git lfs pull
```

## Coding Standards

### C# Style Guide

- Use PascalCase for public members, camelCase for private
- Prefix private fields with underscore: `_privateField`
- Use meaningful, descriptive names
- Keep methods focused and under 50 lines
- Add XML documentation for public APIs

```csharp
namespace AtomCoLab.Visualization
{
    /// <summary>
    /// Renders molecular structures in VR.
    /// </summary>
    public class MoleculeRenderer : MonoBehaviour
    {
        [SerializeField]
        private Material _atomMaterial;

        private MeshFilter _meshFilter;

        /// <summary>
        /// Sets the render mode for molecule visualization.
        /// </summary>
        /// <param name="mode">The desired render mode.</param>
        public void SetRenderMode(RenderMode mode)
        {
            // Implementation
        }
    }
}
```

### Commit Messages

Use conventional commits format:

- `Add:` New feature
- `Fix:` Bug fix
- `Update:` Enhancement to existing feature
- `Refactor:` Code restructuring
- `Docs:` Documentation changes
- `Test:` Test additions or modifications
- `Chore:` Build, CI, or tooling changes

### Unity Best Practices

- Use SerializeField instead of public fields
- Prefer composition over inheritance
- Use ScriptableObjects for configuration data
- Keep prefabs modular and reusable
- Optimize for Quest hardware limitations

## Testing

### Running Tests

```bash
# Run all tests via Unity Test Runner
# Open Window > General > Test Runner in Unity
```

### Writing Tests

- Place tests in `Tests/` directory
- Use Unity Test Framework
- Cover edge cases and error conditions
- Test VR interactions in Play Mode tests

## Review Process

1. All PRs require at least one maintainer review
2. CI checks must pass (build, tests, linting)
3. Documentation must be updated if applicable
4. Breaking changes require discussion first

## Questions?

- Open a [Discussion](https://github.com/your-org/AtomCoLab-VR/discussions)
- Join our community chat
- Email: contributors@atomcolab-vr.com

Thank you for helping make AtomCoLab-VR better!
