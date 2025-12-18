# Avatar Setup Guide

An Avatar is required for animations to play on your character. Follow these steps:

## **Step 1: Check Your Model's Avatar**

1. **Select your character model** in Project window (e.g., `Assets/Models/Soldier1`)
2. **In Inspector**, click the **"Rig"** tab (next to "Model", "Materials", etc.)
3. **Check "Animation Type"**:
   - If it says **"Humanoid"** → Good! Go to Step 2
   - If it says **"Generic"** → Also works, but Humanoid is better for human characters
   - If it says **"None"** → Change it to **"Humanoid"** or **"Generic"**

## **Step 2: Configure Avatar (if Humanoid)**

1. **With your model selected**, in the **"Rig"** tab:
   - **Animation Type**: Select **"Humanoid"**
   - Click **"Apply"** button at the bottom
2. **Unity will analyze your model** and create an Avatar
3. **Check the "Configure" button**:
   - If it says **"Configure..."** → Click it to set up bone mapping
   - If it says **"Avatar is valid"** → Perfect! Your Avatar is ready

### **If "Configure..." appears:**

1. **Click "Configure..."** button
2. **Unity will open the Avatar configuration window**
3. **Check the bone mapping** (red/green dots on your character):
   - **Green dots** = bones are mapped correctly
   - **Red dots** = bones need mapping
4. **If there are red dots**:
   - Unity usually auto-maps them, but you can manually drag bones
   - Or click **"Pose" → "Enforce T-Pose"** to help Unity understand the pose
5. **Click "Done"** when finished

## **Step 3: Assign Avatar to Animator**

1. **Select Soldier1** in Hierarchy (the GameObject with your character model and Animator)
2. **In Inspector**, find the **Animator** component
3. **Find the "Avatar" field** (below "Controller")
4. **Drag your model** from Project window into the "Avatar" field
   - OR: Click the circle icon next to "Avatar" → Select your model
   - Unity will automatically use the Avatar from that model

## **Alternative: If Your Model is Generic**

If your model uses **"Generic"** animation type:

1. **Select your model** in Project → **"Rig"** tab
2. **Animation Type**: **"Generic"**
3. **Avatar Definition**: **"Create From This Model"**
4. **Click "Apply"**
5. **Assign to Animator** (same as Step 3 above)

## **Step 4: Verify It Works**

1. **Select Soldier1** in Hierarchy
2. **In Inspector**, Animator component should show:
   - **Controller**: `PlayerAnimatorController` ✅
   - **Avatar**: `Soldier1` (or your model name) ✅
3. **Press Play**
4. **Character should animate** (no longer in T-pose)

## **Troubleshooting:**

### **"Avatar is invalid" error:**
- Make sure your model has bones/skeleton
- Try switching to "Generic" instead of "Humanoid"
- Check that bone hierarchy is correct

### **Avatar field is grayed out:**
- Make sure you selected the GameObject with the Animator component
- Check that the Animator component is enabled

### **Still T-pose after assigning Avatar:**
- Make sure animation clips are assigned to states in Animator Controller
- Check that animation clips have "Loop Time" enabled
- Verify the Animator Controller is assigned

