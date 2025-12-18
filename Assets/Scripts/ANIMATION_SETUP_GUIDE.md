# Animation Setup Guide - Step by Step

This guide will help you set up animations for your player character in a reusable way, so you can easily swap between different models (e.g., "Skinny Green Soldier" → "Fat Blue Soldier").

---

## **Step 1: Import Your Character Model**

1. **Get your character model** (e.g., "Skinny Green Soldier")
   - Place the model file (`.fbx`, `.obj`, etc.) in `Assets/Models/` (create folder if needed)
   - Unity will automatically import it

2. **Import animations** (if separate from model)
   - You need **THREE animations**:
     - **Idle** - character standing still with rifle (not shooting, when user is not touching screen)
     - **Strafe Left** - character moving left while shooting
     - **Strafe Right** - character moving right while shooting
   - Import animation files (e.g., `Idle.fbx`, `StrafeLeft.fbx`, `StrafeRight.fbx`)

---

## **Step 2: Replace the Capsule with Your Model**

1. **In the Hierarchy**, select your **Player** GameObject (currently a capsule)
2. **Delete the Capsule Mesh Renderer** component (or keep it hidden for now)
3. **Add your character model**:
   - Drag your character model from Project window into the Hierarchy as a child of Player
   - OR: Replace the Mesh Filter/Mesh Renderer with your model's components
   - **Important**: Make sure your model is positioned correctly (at origin, facing forward)

---

## **Step 3: Add Animator Component**

1. **Select your Player GameObject** (or the child GameObject with your model)
2. **Add Component** → Search for **"Animator"** → Add it
3. The Animator component will appear in the Inspector

---

## **Step 4: Create Animator Controller**

1. **Right-click in Project window** → `Create` → `Animator Controller`
2. **Name it** (e.g., `PlayerAnimatorController`)
3. **Save it** in `Assets/Animators/` (create folder if needed)
4. **Double-click** the Animator Controller to open the Animator window

---

## **Step 5: Set Up Animator States**

In the **Animator window**:

1. **Create States** (3 needed):
   - Right-click in empty space → `Create State` → `Empty` → Name it **"Idle"**
   - Right-click again → `Create State` → `Empty` → Name it **"StrafeLeft"**
   - Right-click again → `Create State` → `Empty` → Name it **"StrafeRight"**

2. **Set Default State**:
   - Right-click **"Idle"** → `Set as Layer Default State` (it should turn orange)

3. **Assign Animation Clips**:
   - Click **"Idle"** state → In Inspector, find **"Motion"** field
   - Drag your **Idle animation clip** into the Motion field
   - Click **"StrafeLeft"** state → Drag your **Strafe Left animation clip** into the Motion field
   - Click **"StrafeRight"** state → Drag your **Strafe Right animation clip** into the Motion field

4. **Create Transitions** (connect all states to each other):
   - Right-click **"Idle"** → `Make Transition` → Click **"StrafeLeft"**
   - Right-click **"Idle"** → `Make Transition` → Click **"StrafeRight"**
   - Right-click **"StrafeLeft"** → `Make Transition` → Click **"Idle"**
   - Right-click **"StrafeLeft"** → `Make Transition` → Click **"StrafeRight"**
   - Right-click **"StrafeRight"** → `Make Transition` → Click **"Idle"**
   - Right-click **"StrafeRight"** → `Make Transition` → Click **"StrafeLeft"**

---

## **Step 6: Create Animator Parameters**

In the **Animator window** (Parameters tab at top-left):

1. **Click the "+" button** → Add **Float** parameter → Name it **"MoveDirection"**
   - This is the **ONLY parameter** you need!
   - Values: **-1** = Strafe Left, **0** = Idle, **1** = Strafe Right

2. **Set up Transition Conditions** (for each transition):

   **From Idle to StrafeLeft:**
   - Click the arrow **"Idle"** → **"StrafeLeft"**
   - In Inspector, under **"Conditions"**, click **"+"**
   - Set condition: **"MoveDirection"** Less than **-0.5**
   - Uncheck **"Has Exit Time"**
   - Set **"Transition Duration"** to `0.1`

   **From Idle to StrafeRight:**
   - Click the arrow **"Idle"** → **"StrafeRight"**
   - Set condition: **"MoveDirection"** Greater than **0.5**
   - Uncheck **"Has Exit Time"**
   - Set **"Transition Duration"** to `0.1`

   **From StrafeLeft to Idle:**
   - Click the arrow **"StrafeLeft"** → **"Idle"**
   - Set condition: **"MoveDirection"** Greater than **-0.5**
   - Uncheck **"Has Exit Time"**
   - Set **"Transition Duration"** to `0.1`

   **From StrafeLeft to StrafeRight:**
   - Click the arrow **"StrafeLeft"** → **"StrafeRight"**
   - Set condition: **"MoveDirection"** Greater than **0.5**
   - Uncheck **"Has Exit Time"**
   - Set **"Transition Duration"** to `0.1`

   **From StrafeRight to Idle:**
   - Click the arrow **"StrafeRight"** → **"Idle"**
   - Set condition: **"MoveDirection"** Less than **0.5**
   - Uncheck **"Has Exit Time"**
   - Set **"Transition Duration"** to `0.1`

   **From StrafeRight to StrafeLeft:**
   - Click the arrow **"StrafeRight"** → **"StrafeLeft"**
   - Set condition: **"MoveDirection"** Less than **-0.5**
   - Uncheck **"Has Exit Time"**
   - Set **"Transition Duration"** to `0.1`

---

## **Step 7: Create PlayerAnimationData ScriptableObject**

1. **Right-click in Project window** → `Create` → `Player` → `Animation Data`
   - (This option appears because of the `[CreateAssetMenu]` attribute)

2. **Name it** (e.g., `SkinnyGreenSoldier_AnimationData`)

3. **Configure it**:
   - **Animator Controller**: Drag your `PlayerAnimatorController` here
   - **Idle Clip**: Drag your **Idle** animation clip
   - **Strafe Left Clip**: Drag your **Strafe Left** animation clip
   - **Strafe Right Clip**: Drag your **Strafe Right** animation clip
   - **Move Direction Parameter**: Should be `"MoveDirection"` (default - this is the only parameter needed)

4. **Save it** in `Assets/AnimationData/` (create folder if needed)

---

## **Step 8: Assign Animation Data to PlayerMovement**

1. **Select your Player GameObject** in Hierarchy
2. **In Inspector**, find the **PlayerMovement** component
3. **Drag your `SkinnyGreenSoldier_AnimationData`** into the **"Animation Data"** field
4. **If Animator is on a child object**, drag that child GameObject into the **"Animator"** field
   - Otherwise, it will find it automatically

---

## **Step 9: Test It!**

1. **Press Play** in Unity
2. **Don't touch/click** → Should see **Idle** animation (standing with rifle)
3. **Click and hold** (player starts shooting) → Should see **Idle** animation (still idle, but shooting)
4. **Drag left** while holding → Should see **StrafeLeft** animation
5. **Drag right** while holding → Should see **StrafeRight** animation
6. **Stop dragging** (but keep holding) → Should return to **Idle** animation
7. **Release** → Should return to **Idle** animation (not shooting)

---

## **Step 10: Swap to Different Character (Reusability)**

When you want to use a **"Fat Blue Soldier"** instead:

1. **Create a new Animator Controller** for the new character
2. **Set it up** with the same parameter name (`MoveDirection` - that's all you need!)
3. **Create a new PlayerAnimationData** → Name it `FatBlueSoldier_AnimationData`
4. **Assign the new Animator Controller and the three animation clips** (Idle, StrafeLeft, StrafeRight)
5. **In PlayerMovement**, simply **drag the new `FatBlueSoldier_AnimationData`** into the **"Animation Data"** field
6. **Done!** The system automatically switches everything

**OR** use code at runtime:
```csharp
// In any script:
PlayerMovement player = FindObjectOfType<PlayerMovement>();
PlayerAnimationData newData = Resources.Load<PlayerAnimationData>("FatBlueSoldier_AnimationData");
player.SetAnimationData(newData);
```

---

## **Troubleshooting**

- **Animations not playing?**
  - Check that Animator Controller is assigned to Animator component
  - Check that all three animation clips (Idle, StrafeLeft, StrafeRight) are assigned to states
  - Check that parameter name `MoveDirection` matches exactly (case-sensitive)

- **Transitions not working?**
  - Make sure "Has Exit Time" is unchecked
  - Check that conditions use `MoveDirection` parameter with correct thresholds (-0.5 and 0.5)
  - Verify parameter name matches in PlayerAnimationData
  - Make sure all 6 transitions are set up (Idle ↔ StrafeLeft, Idle ↔ StrafeRight, StrafeLeft ↔ StrafeRight)

- **Model not visible?**
  - Check that Mesh Renderer is enabled
  - Check that model is a child of Player or components are on Player

---

## **Summary**

✅ **Reusable System**: Create one `PlayerAnimationData` per character model  
✅ **Easy Swapping**: Just change the Animation Data field in Inspector  
✅ **Runtime Swapping**: Use `SetAnimationData()` method in code  
✅ **Flexible**: Works with any character model and animation set

