# Two-Atom Demo - Quick Setup Guide

Get atoms rendering on your Quest in ~15 minutes!

## Prerequisites

- Unity 2022.3 LTS with Android Build Support
- Meta XR SDK installed (or existing AtomCoLab-VR Unity project)
- Quest in Developer Mode, connected via USB

---

## Step 1: Copy Files to Project

Copy these files to your Unity project:

```
Assets/
├── Scripts/
│   ├── Data/
│   │   └── AtomData.cs
│   ├── Rendering/
│   │   └── GPUAtomRenderer.cs
│   └── Demo/
│       └── TwoAtomDemo.cs
└── Shaders/
    └── AtomInstanced.shader
```

---

## Step 2: Create Material

1. In Unity: **Right-click in Assets** → Create → Material
2. Name it `AtomMaterial`
3. In Inspector, change shader: **Shader dropdown** → AtomCoLab → AtomInstanced
4. Set properties:
   - Atom Scale: `1.0`
   - Smoothness: `0.5`
   - Metallic: `0.0`

---

## Step 3: Set Up Scene

### If you have an existing VR scene:

1. Create empty GameObject: **GameObject** → Create Empty
2. Name it `AtomRenderer`
3. Add Component: `GPUAtomRenderer`
4. Assign the `AtomMaterial` you created

5. Create another empty GameObject
6. Name it `DemoController`
7. Add Component: `TwoAtomDemo`
8. Drag `AtomRenderer` to the Renderer field

### If starting fresh:

1. Create new scene: **File** → New Scene → Basic (Built-in)
2. Add XR Origin (if using Meta XR SDK)
3. Follow steps above for AtomRenderer and DemoController
4. Position camera/XR Origin at (0, 0, -5) looking at origin

---

## Step 4: Test in Editor

1. Press **Play**
2. You should see 2 colored spheres (green Mg, gray Al)
3. If you see magenta spheres = shader issue
4. If you see nothing = check Console for errors

---

## Step 5: Build for Quest

1. **File** → Build Settings
2. Platform: Android (Switch if needed)
3. Connect Quest via USB
4. **Build and Run**

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| No atoms visible | Check Console for errors, verify material assigned |
| Magenta atoms | Shader not compiling - check shader errors |
| Build fails | Ensure Android SDK/NDK configured, Quest in dev mode |
| Black screen on Quest | Add OVRCameraRig or XR Origin to scene |

---

## Next Steps

Once 2 atoms work:

1. Right-click `DemoController` → "Create 1000 Random Atoms"
2. Try "Create 100,000 Atoms" to test performance
3. Build again and test on Quest

---

## Claude Code Integration Prompt

When you want Claude Code to help, use this prompt:

```
Read SPECIFICATION.md first - it's the authoritative technical spec.

Current status: Basic 2-atom demo working with GPU instancing.

Task: [describe what you need]

Constraints:
- Do NOT use Instantiate() for atoms
- MUST use ComputeBuffers + DrawMeshInstancedIndirect
- Follow existing code patterns in Assets/Scripts/
```
