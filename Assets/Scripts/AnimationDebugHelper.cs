using UnityEngine;

/// <summary>
/// Helper script to debug animation issues. Attach this to your Player GameObject temporarily.
/// </summary>
public class AnimationDebugHelper : MonoBehaviour
{
    [Header("Debug")]
    public bool showDebugInfo = true;
    public KeyCode testLeftKey = KeyCode.A;
    public KeyCode testRightKey = KeyCode.D;
    public KeyCode testIdleKey = KeyCode.S;

    private Animator _animator;
    private PlayerMovement _playerMovement;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _playerMovement = GetComponent<PlayerMovement>();

        if (_animator == null)
        {
            Debug.LogError("AnimationDebugHelper: No Animator found!");
        }
        else
        {
            Debug.Log($"AnimationDebugHelper: Found Animator on {_animator.gameObject.name}");
            Debug.Log($"AnimationDebugHelper: Animator enabled: {_animator.enabled}");
            Debug.Log($"AnimationDebugHelper: Animator Controller: {(_animator.runtimeAnimatorController != null ? _animator.runtimeAnimatorController.name : "NULL")}");
            
            // List all parameters
            Debug.Log("AnimationDebugHelper: Animator Parameters:");
            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                Debug.Log($"  - {param.name} ({param.type})");
            }
        }
    }

    private void Update()
    {
        if (!showDebugInfo || _animator == null)
            return;

        // Test with keyboard
        if (Input.GetKey(testLeftKey))
        {
            _animator.SetFloat("MoveDirection", -1f);
            Debug.Log("AnimationDebugHelper: Testing Strafe Left (MoveDirection = -1)");
        }
        else if (Input.GetKey(testRightKey))
        {
            _animator.SetFloat("MoveDirection", 1f);
            Debug.Log("AnimationDebugHelper: Testing Strafe Right (MoveDirection = 1)");
        }
        else if (Input.GetKey(testIdleKey))
        {
            _animator.SetFloat("MoveDirection", 0f);
            Debug.Log("AnimationDebugHelper: Testing Idle (MoveDirection = 0)");
        }

        // Show current state info
        if (_animator != null && _animator.enabled)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            string currentState = "Unknown";
            if (stateInfo.IsName("Idle")) currentState = "Idle";
            else if (stateInfo.IsName("eLeft")) currentState = "StrafeLeft";
            else if (stateInfo.IsName("StrafeRig")) currentState = "StrafeRight";
            
            float moveDirValue = _animator.GetFloat("MoveDirection");
            Debug.Log($"AnimationDebugHelper: Current State: {currentState}, " +
                $"Normalized Time: {stateInfo.normalizedTime:F2}, MoveDirection: {moveDirValue}");
        }
    }

    private void OnGUI()
    {
        if (!showDebugInfo || _animator == null)
            return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Box("Animation Debug Info");
        GUILayout.Label($"Animator Enabled: {_animator.enabled}");
        GUILayout.Label($"Has Controller: {_animator.runtimeAnimatorController != null}");
        
        if (_animator.runtimeAnimatorController != null)
        {
            GUILayout.Label($"Controller: {_animator.runtimeAnimatorController.name}");
        }

        float moveDir = _animator.GetFloat("MoveDirection");
        GUILayout.Label($"MoveDirection: {moveDir}");
        
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        string stateName = "Unknown";
        if (stateInfo.IsName("Idle")) stateName = "Idle";
        else if (stateInfo.IsName("eLeft")) stateName = "StrafeLeft";
        else if (stateInfo.IsName("StrafeRig")) stateName = "StrafeRight";
        
        GUILayout.Label($"Current State: {stateName}");
        GUILayout.Label($"State Time: {stateInfo.normalizedTime:F2}");

        GUILayout.Label($"\nPress {testLeftKey.ToString()} = Left, {testRightKey.ToString()} = Right, {testIdleKey.ToString()} = Idle");
        GUILayout.EndArea();
    }
}

