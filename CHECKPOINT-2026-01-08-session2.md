# AtomCoLab-VR Checkpoint - January 8, 2026 (Session 2)

## Starting State
- **Working:** VR head tracking, 2 atoms rendering with GPU instancing on Quest 3S
- **Goal:** Add VR button triggers (A/B/X/Y) to test scale (1K-500K atoms) with FPS display

## Current State: BROKEN
- App shows only Unity splash (3 dots + music)
- Scene doesn't render anything
- Meta button now works (after XR Rig conversion)
- OVRCameraRig was deleted and replaced with "Convert Main Camera to XR Rig" - this may have broken rendering

## What Was Tried (Chronological)

### 1. Added OVRInput button handling to TwoAtomDemo.cs
- **Result:** Buttons didn't work, no debug panel visible
- **Root cause discovered later:** Project uses OpenXR, not Oculus XR plugin

### 2. Added keyboard fallback (Input.GetKeyDown)
- **Result:** App crashed on Quest
- **Error:** `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package in Player Settings`
- **Lesson:** Project uses new Input System, legacy Input class crashes the app

### 3. Tried to switch from OpenXR to Oculus XR plugin
- **Result:** IMPOSSIBLE - clicking Oculus checkbox automatically selects OpenXR
- **Lesson:** This is a Unity 6 / Meta XR SDK configuration issue - they may be mutually exclusive in some setups

### 4. Removed keyboard fallback, kept only OVRInput
- **Result:** Still stuck on splash screen
- **Logs showed:** Code WAS running! Atoms and cube were created. But nothing rendered.
- **Key log output:**
  ```
  TwoAtomDemo: Created feedback cube at (-1, 1.5, 1.5)
  GPUAtomRenderer: Initialized with 2 atoms
  Created 2 atoms
  TwoAtomDemo: Initialized. Press A/B/X/Y buttons to create atoms.
  ```

### 5. Added plain Unity cube to scene
- **Result:** Cube also not visible
- **Conclusion:** Camera/rendering broken, not the atom code

### 6. Deleted OVRCameraRig, used "XR → Convert Main Camera to XR Rig"
- **Result:** Still splash screen, but Meta button now works to exit
- **This may have made things worse** - should restore OVRCameraRig

### 7. Searched for Vulkan + OpenXR black screen issue
- **Found:** Known issue where Vulkan + OpenXR = black screen
- **Suggested fix:** Change Graphics API order (OpenGLES3 before Vulkan)
- **NOT TESTED** - user pointed out they don't have black screen, they have splash screen

## Key Discoveries

### OVRInput IS compatible with OpenXR
From Meta docs: "OVR prefix naming - Several script, prefab, and component names in Meta XR SDK's are prefixed with 'OVR'... All of these are compatible with Unity's OpenXR plugin."

### The actual rendering problem
- OVRCameraRig was at position (0,0,0) ✔
- OVRManager was attached ✔
- Material was assigned to AtomRenderer ✔
- Code executed successfully ✔
- But NOTHING rendered - not even a plain cube

### Possible causes NOT yet investigated
1. **Graphics API configuration** - Player Settings → Other Settings → Graphics APIs
2. **Render pipeline compatibility** - Is project using URP/HDRP/Built-in?
3. **Camera settings** - Clear flags, culling mask, near/far clip planes
4. **OpenXR feature group configuration** - Meta Quest feature group settings
5. **Shader compatibility** - GPU instancing shader may not work on Quest with current settings

## Files That Need to Be Restored

### Original working TwoAtomDemo.cs (NO button handling)
```csharp
// The version that WAS working - just creates 2 atoms, no Update(), no input
// User uploaded this at the start of the session
// Location: Assets/Scripts/Demo/TwoAtomDemo.cs
```

### Scene setup that WAS working
- OVRCameraRig at (0,0,0) with OVRManager
- AtomRenderer with material assigned
- DemoController with TwoAtomDemo script
- Directional Light

## Critical Lessons Learned

### 1. ALWAYS look up documentation first
- Don't guess about Unity/Meta XR SDK setup
- Search for exact error messages
- Check official Meta and Unity docs before suggesting changes

### 2. Don't mix XR systems
- OVRCameraRig is for Meta XR SDK
- XR Rig / XR Origin is for Unity's generic XR system
- Mixing them causes problems

### 3. Legacy Input vs New Input System
- `Input.GetKeyDown()` crashes if project uses new Input System
- Must check Player Settings → Active Input Handling
- For Quest: use OVRInput (works with OpenXR per Meta docs)

### 4. When code runs but nothing renders
- This is a camera/rendering issue, not code issue
- Check: Camera existence, position, clear flags, culling mask
- Check: Graphics API, render pipeline, shader compatibility

### 5. Preserve working state
- Should have git committed before making changes
- Should have saved working scene as backup
- Making multiple changes at once makes debugging impossible

## To Resume Next Session

### Step 1: Restore working scene
1. Delete current XR Rig
2. Re-add OVRCameraRig prefab (search in Project window)
3. Add OVRManager component if not present
4. Position at (0,0,0)

### Step 2: Use original TwoAtomDemo.cs
- Get from git history or use the original user uploaded
- NO Update() method, NO input handling - just Start() creating 2 atoms

### Step 3: Verify atoms render again
- Build and run
- Should see 2 atoms like before
- If not, the problem is elsewhere (Graphics API, render pipeline, etc.)

### Step 4: THEN add input handling carefully
- Research proper OpenXR + Meta Quest input handling
- Check Unity docs for XR Input with OpenXR
- Consider using Unity's new Input System XR bindings instead of OVRInput
- Test ONE change at a time

## Useful Commands

```bash
# ADB path on this Mac
/Applications/Unity/Hub/Editor/6000.3.2f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb

# View Unity logs
adb logcat -d | grep -i "unity\|error\|exception" | tail -50

# Force stop app
adb shell am force-stop com.DefaultCompany.Unity

# Clear logs
adb logcat -c
```

## Project Details
- Unity version: 6000.3.2f1
- Platform: Meta Quest 3S
- XR Plugin: OpenXR with Meta XR feature group
- Input System: New Input System (NOT legacy)
- Repo: https://github.com/biterik/AtomCoLab-VR (private)

## Questions to Research Before Next Session

1. What is the correct way to handle controller input with OpenXR + Meta Quest in Unity 6?
2. Why would OVRCameraRig render controllers but not scene objects?
3. What Graphics API settings are required for Quest 3S with OpenXR?
4. Is there a Unity 6 + Meta XR SDK sample project to reference?
