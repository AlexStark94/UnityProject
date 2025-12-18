using UnityEngine;

/// <summary>
/// Reusable animation data container. Create one for each character model (e.g., "SkinnyGreenSoldier", "FatBlueSoldier").
/// Assign this to PlayerMovement to easily swap between different character models and their animations.
/// </summary>
[CreateAssetMenu(fileName = "New Player Animation Data", menuName = "Player/Animation Data")]
public class PlayerAnimationData : ScriptableObject
{
    [Header("Animator Controller")]
    [Tooltip("The Animator Controller for this character model. Must be assigned.")]
    public RuntimeAnimatorController animatorController;

    [Header("Animation Clips")]
    [Tooltip("Idle animation clip (standing still with rifle, not shooting).")]
    public AnimationClip idleClip;

    [Tooltip("Strafe Left animation clip (moving left while shooting).")]
    public AnimationClip strafeLeftClip;

    [Tooltip("Strafe Right animation clip (moving right while shooting).")]
    public AnimationClip strafeRightClip;

    [Header("Animation Parameters")]
    [Tooltip("Name of the 'MoveDirection' parameter in the Animator Controller (Float: -1 = left, 0 = idle, 1 = right).")]
    public string moveDirectionParameter = "MoveDirection";

    /// <summary>
    /// Validates that required fields are assigned.
    /// </summary>
    public bool IsValid()
    {
        return animatorController != null;
    }
}

