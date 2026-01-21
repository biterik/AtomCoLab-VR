# AtomCoLab-VR Checkpoint - January 21, 2026 (Session 3)

## Summary
Started fresh Unity project with correct configuration decisions based on current documentation. Project setup is complete, but blocked on USB debugging authorization issue with Quest 3S. User is updating Quest OS to latest version.

## Starting State
- Previous project broken after OVRCameraRig was replaced with XR Rig
- Decision made to start completely fresh

## Key Learnings (Validated with Current Documentation)

### 1. XR Plugin Selection for Unity 6+ with Quest
**Decision: Use OpenXR Plugin (not legacy Oculus XR Plugin)**
- Meta recommendation: OpenXR is standard for Unity 6+ and SDK v74+
- Oculus XR Plugin is deprecated for new features
- OVRCameraRig/OVRInput/OVRManager ARE compatible with OpenXR despite "OVR" prefix
- Unity OpenXR: Meta package provides Meta-specific extensions

### 2. Camera Rig Choice
**Decision: Use OVRCameraRig (not Unity's generic XR Origin)**
- OVRCameraRig: Quest-optimized, all Meta features work natively
- Building Blocks use OVRCameraRig
- XR Origin: Cross-platform but requires hybrid setup for Meta features
- Some experimental features only work with OVRCameraRig

### 3. Unity 6 Template Names Changed
- Template is now called **"Universal 3D"** (not "3D (URP)")
- This IS the URP template for Unity 6

### 4. Unity 6.1+ Has Dedicated Meta Quest Build Platform
- File → Build Profiles → Select **"Meta Quest"** (not generic Android)
- Automatically installs OpenXR Plugin
- Pre-configures optimal settings: Vulkan, ARM64, IL2CPP, API level 32
- Platform Browser allows selecting Meta packages during setup

## Fresh Project Setup Completed

### Project Created
- **Name:** Atom-CoLab-VR-new
- **Location:** `/Users/oq50iqeq/Desktop/PROJECTS/DEVEL/META-QUEST/AtomCoLab-VR/Atom-CoLab-VR-new/`
- **Unity Version:** 6.3 LTS (6000.3.2f1)
- **Template:** Universal 3D

### Steps Completed
1. Created new project with "Universal 3D" template
2. Unchecked Unity Version Control (using Git+GitHub instead)
3. File → Build Profiles → Selected "Meta Quest" → Switch Platform
4. Enabled "Meta XR Feature set" when prompted
5. Platform Browser: Selected **Meta XR Core SDK** only (not Interaction SDK, Audio, Avatars)
6. Switched to Meta Quest profile (now active)
7. Deleted Main Camera and Global Volume from scene
8. Meta → Tools → Building Blocks → Added "Camera Rig" (OVRCameraRig)
9. Created directory structure and copied working scripts from old project
10. Created AtomMaterial with AtomInstanced shader
11. Created AtomRenderer GameObject with GPUAtomRenderer component
12. Created DemoController GameObject with TwoAtomDemo component
13. Saved scene as MainScene.unity

### Files Copied to New Project
```
Assets/Scripts/Data/AtomData.cs
Assets/Scripts/Rendering/GPUAtomRenderer.cs
Assets/Scripts/Demo/TwoAtomDemo.cs
Assets/Shaders/AtomInstanced.shader
```

### Scene Hierarchy
- Camera Rig (OVRCameraRig from Building Blocks)
- AtomRenderer (GPUAtomRenderer component, AtomMaterial assigned)
- DemoController (TwoAtomDemo component, AtomRenderer reference)
- Directional Light

## Current Blocker: USB Debugging Authorization

### Problem
Quest 3S shows as "unauthorized" in ADB, preventing Unity builds:
```bash
adb devices
340YC10G7W03S7 unauthorized
```

### What Was Tried (All Failed)
- Deleted ADB keys: `rm -rf ~/.android/adbkey*`
- Killed and restarted ADB server
- Unplugged/replugged USB cable multiple times
- User sees "USB Detected" notification in Quest (for file access, not debugging)
- Developer Mode is ON (confirmed via Meta phone app)
- Settings menu on Quest does NOT show "Developer" section or "System" section
- Settings only shows: wifi, bluetooth, devices, link, general
- General → About only shows: MAC address, pairing code, regulatory, terms of service, privacy policy
- No visible software version or build number to tap

### Root Cause Suspected
- Quest OS needs update - settings menu structure appears incomplete
- User is currently updating Quest OS to latest version

### Quest Settings Path (According to Documentation)
The expected path is: **Settings → Advanced → Developer**
But user's Quest does NOT have "Advanced" section visible.

Alternative path mentioned in docs: **Settings → System → Developer**
But user's Quest does NOT have "System" section either.

## Technical Details
- **ADB path:** `/Applications/Unity/Hub/Editor/6000.3.2f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`
- **Quest serial:** 340YC10G7W03S7
- **Android SDK API level 32** installed (required for Meta Quest)

## Directory Structure
```
/Users/oq50iqeq/Desktop/PROJECTS/DEVEL/META-QUEST/AtomCoLab-VR/
├── AtomCoLab-VR-Files/     # Old broken project (keep for reference)
│   └── Assets/
├── Atom-CoLab-VR-new/      # NEW fresh project
│   ├── Assets/
│   │   ├── Materials/
│   │   │   └── AtomMaterial
│   │   ├── Scenes/
│   │   │   └── MainScene.unity
│   │   ├── Scripts/
│   │   │   ├── Data/
│   │   │   │   └── AtomData.cs
│   │   │   ├── Rendering/
│   │   │   │   └── GPUAtomRenderer.cs
│   │   │   └── Demo/
│   │   │       └── TwoAtomDemo.cs
│   │   └── Shaders/
│   │       └── AtomInstanced.shader
│   └── ...
└── CHECKPOINT-2026-01-21-session3.md
```

## To Resume Next Session

### Step 1: Verify Quest OS Update Completed
After OS update, check if Settings menu now shows:
- Settings → Advanced → Developer, OR
- Settings → System → Developer

### Step 2: Enable USB Debugging in Quest Settings
1. Find Developer section in Settings
2. Enable "USB Connection Dialog" 
3. Enable "USB debugging" if separate option

### Step 3: Connect and Authorize
1. Connect Quest to Mac via USB
2. Look for "Allow USB debugging?" popup in Quest
3. Check "Always allow from this computer"
4. Click Allow

### Step 4: Verify Connection
```bash
/Applications/Unity/Hub/Editor/6000.3.2f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb devices
# Should show: 340YC10G7W03S7 device (not "unauthorized")
```

### Step 5: Build and Test
1. Unity: File → Build Profiles
2. Add Open Scenes if needed
3. Set Run Device to Quest
4. Build and Run
5. Expected: See 2 atoms rendering in VR

### Step 6: If Working, Add VR Input
- Research proper OpenXR + Meta Quest input handling
- Test scale with 1K-500K atoms

## Known Issues to Watch
- Meta XR Core SDK v83 has reported license errors on Unity 6.3 LTS
- If encountered, revert to SDK v81 or v82
- USB debugging popup may not appear due to Quest firmware issues

## Repository
- **GitHub:** https://github.com/biterik/AtomCoLab-VR (private)
- **Old project folder:** `AtomCoLab-VR-Files/`
- **New project folder:** `Atom-CoLab-VR-new/`

## Memory Updates Added This Session
1. Development environment: M2 Mac with macOS Tahoe 26.2, conda-forge for Python packages, Meta Quest 3s target platform, Unity 6.3 LTS (6000.3.2f1)
2. Always search for current documentation before giving Unity/VR/Meta guidance - do not assume knowledge is up to date

## Critical Reminders for Claude
- **ALWAYS look up current documentation** before providing Unity/VR/Meta guidance
- Menu names, settings locations, and workflows change frequently
- The year is 2026 - knowledge cutoff may not reflect current state
- User's Quest has new Navigator UI (v83+) with different menu structure
