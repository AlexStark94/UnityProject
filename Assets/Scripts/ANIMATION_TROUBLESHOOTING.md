# Animation Troubleshooting - T-Pose Fix

If your character is stuck in T-pose, follow these steps:

## **Step 1: Verify Animation Clips are Assigned to States**

1. **Open your Animator Controller** (`PlayerAnimatorController`)
2. **Click on each state** (Idle, StrafeLeft, StrafeRight) in the Animator window
3. **In the Inspector**, check the **"Motion"** field:
   - **Idle state** → Should have your **Idle** animation clip assigned
   - **StrafeLeft state** → Should have your **StrafeL** animation clip assigned
   - **StrafeRight state** → Should have your **StrafeR** animation clip assigned
4. **If empty**: Drag the animation clips from Project window into the Motion field

## **Step 2: Check Animation Clip Import Settings**

1. **Select your animation clips** in Project window (StrafeL, StrafeR, Idle)
2. **In Inspector**, check these settings:
   - **Loop Time**: ✅ **CHECKED** (animations should loop)
   - **Root Transform Rotation**: Usually "Bake Into Pose" ✅
   - **Root Transform Position (Y)**: Usually "Bake Into Pose" ✅
   - **Root Transform Position (XZ)**: Usually "Bake Into Pose" ✅
   - **Avatar**: Should match your character's Avatar (if using Humanoid)
   - **Rig**: Should be set correctly (Humanoid/Generic)

## **Step 3: Verify Avatar is Set Up**

1. **Select your character model** (Soldier1) in Project window
2. **In Inspector**, go to **Rig** tab:
   - **Animation Type**: Should be **"Humanoid"** or **"Generic"**
   - If Humanoid, make sure **"Configure"** button shows "Avatar is valid"
3. **Select the Animator component** on Soldier1 in Hierarchy
4. **In Inspector**, check **"Avatar"** field:
   - Should be assigned (usually auto-assigned from the model)
   - If empty, drag the Avatar from your model into this field

## **Step 4: Check Animator Component Settings**

1. **Select Soldier1** (the GameObject with Animator) in Hierarchy
2. **In Inspector**, find **Animator** component:
   - **Controller**: Should be assigned (`PlayerAnimatorController`)
   - **Avatar**: Should be assigned (from your model)
   - **Apply Root Motion**: Usually **UNCHECKED** for this type of movement
   - **Update Mode**: "Normal"
   - **Culling Mode**: "Always Animate" (so it animates even when off-screen)

## **Step 5: Verify Parameter is Being Set**

1. **Press Play** in Unity
2. **Open Animator window** (keep it open while playing)
3. **Watch the "MoveDirection" parameter** value:
   - Should change when you drag left/right
   - Should be **-1** when dragging left
   - Should be **1** when dragging right
   - Should be **0** when idle
4. **Watch the active state** (should highlight in orange):
   - Should change from Idle → StrafeLeft/StrafeRight when dragging

## **Step 6: Check Transition Settings**

For each transition in your Animator Controller:

1. **Click the transition arrow** (e.g., "Idle → StrafeRight")
2. **In Inspector**, verify:
   - **Has Exit Time**: ❌ **UNCHECKED** (for instant transitions)
   - **Transition Duration**: `0.1` (smooth blend)
   - **Conditions**: Should have correct condition
     - Idle → StrafeLeft: `MoveDirection < -0.5`
     - Idle → StrafeRight: `MoveDirection > 0.5`
     - StrafeLeft → Idle: `MoveDirection > -0.5`
     - StrafeRight → Idle: `MoveDirection < 0.5`

## **Step 7: Test with Debug Helper**

1. **Add AnimationDebugHelper** component to Player GameObject
2. **Press Play**
3. **Press A, D, S keys** to manually test animations
4. **Check Console** for debug messages
5. **Check on-screen debug info** to see current state and parameter values

## **Common Issues:**

### Issue: Animations play but character stays in T-pose
**Solution**: Check Avatar assignment on Animator component

### Issue: Only one animation plays
**Solution**: Check transition conditions and make sure all transitions are set up

### Issue: Animations don't loop
**Solution**: Check "Loop Time" in animation clip import settings

### Issue: Character moves but no animation
**Solution**: Make sure animation clips are assigned to states in Animator Controller

### Issue: Parameter doesn't change
**Solution**: Check Console for errors, verify AnimationData is assigned in PlayerMovement

## **Quick Test:**

1. **Select Soldier1** in Hierarchy
2. **In Animator window**, right-click on **"Idle"** state → **"Set as Layer Default State"**
3. **Click "Idle"** state → In Inspector, verify Motion field has an animation clip
4. **Press Play** → Character should play Idle animation immediately
5. **If still T-pose**: The animation clip isn't assigned or Avatar is missing

