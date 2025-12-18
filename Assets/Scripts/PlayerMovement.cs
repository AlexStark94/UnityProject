using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Horizontal speed in units per second when dragging.")]
    public float moveSpeed = 10f;

    [Tooltip("Minimum and maximum X position the player can move to.")]
    public float minX = 495.54f;
    public float maxX = 508.46f;

    [Header("Input")]
    [Tooltip("How far (in screen pixels) the finger must move to start dragging.")]
    public float dragThreshold = 5f;

    [Header("Animation")]
    [Tooltip("Animation data for this character model. Create one in Assets and assign it here.")]
    public PlayerAnimationData animationData;

    [Tooltip("Reference to the Animator component. Will be found automatically if not assigned.")]
    public Animator animator;

    private Camera _mainCamera;
    private bool _isDragging;
    private float _startTouchX;
    private float _startPlayerX;
    private float _moveDirection; // -1 = left, 0 = idle, 1 = right

    private void Awake()
    {
        _mainCamera = Camera.main;

        // Find Animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        // Setup animation data if assigned
        if (animationData != null && animator != null)
        {
            SetupAnimator();
        }
    }

    private void SetupAnimator()
    {
        if (animationData.animatorController != null)
        {
            animator.runtimeAnimatorController = animationData.animatorController;
            Debug.Log($"PlayerMovement: Animator Controller assigned: {animationData.animatorController.name}");
        }
        else
        {
            Debug.LogWarning("PlayerMovement: Animator Controller is null in AnimationData!");
        }

        // Check if Animator has an Avatar (required for animations to play)
        if (animator != null)
        {
            if (animator.avatar == null)
            {
                Debug.LogError("PlayerMovement: Animator has no Avatar assigned! Animations won't play. " +
                    "Assign the Avatar from your character model to the Animator component.");
            }
            else
            {
                Debug.Log($"PlayerMovement: Avatar assigned: {animator.avatar.name}");
            }

            if (!animator.enabled)
            {
                Debug.LogWarning("PlayerMovement: Animator is disabled! Enabling it now.");
                animator.enabled = true;
            }
        }

        // Verify parameter exists
        if (animator != null && !string.IsNullOrEmpty(animationData.moveDirectionParameter))
        {
            bool hasParameter = false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == animationData.moveDirectionParameter && param.type == AnimatorControllerParameterType.Float)
                {
                    hasParameter = true;
                    break;
                }
            }

            if (!hasParameter)
            {
                Debug.LogError($"PlayerMovement: Parameter '{animationData.moveDirectionParameter}' not found in Animator Controller! " +
                    $"Make sure the parameter exists and is a Float type.");
            }
            else
            {
                Debug.Log($"PlayerMovement: Parameter '{animationData.moveDirectionParameter}' found in Animator Controller.");
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            _isDragging = false;
            UpdateAnimationState(0f); // Idle when not touching
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _isDragging = false;
                _startTouchX = touch.position.x;
                _startPlayerX = transform.position.x;
                UpdateAnimationState(0f); // Idle when just touching (not moving yet)
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                float deltaX = touch.position.x - _startTouchX;
                ApplyDrag(deltaX);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                _isDragging = false;
                UpdateAnimationState(0f); // Idle when touch ends
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = false;
            _startTouchX = Input.mousePosition.x;
            _startPlayerX = transform.position.x;
            UpdateAnimationState(0f); // Idle when just clicking (not moving yet)
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.mousePosition.x - _startTouchX;
            ApplyDrag(deltaX);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            UpdateAnimationState(0f); // Idle when mouse released
        }
        else
        {
            UpdateAnimationState(0f); // Idle when not pressing mouse
        }
    }

    private void ApplyDrag(float deltaX)
    {
        // If finger/mouse moved enough, start dragging
        if (!_isDragging && Mathf.Abs(deltaX) > dragThreshold)
        {
            _isDragging = true;
        }

        if (!_isDragging)
        {
            UpdateAnimationState(0f); // Idle when touching but not dragging enough
            return;
        }

        float screenWidth = Screen.width;
        float normalizedDelta = deltaX / screenWidth; // -1 .. 1 approx for full-width drag

        float targetX = _startPlayerX + normalizedDelta * moveSpeed;

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(targetX, minX, maxX);
        transform.position = position;

        // Determine direction: -1 for left, 1 for right
        float direction = Mathf.Sign(deltaX);
        UpdateAnimationState(direction);
    }

    private void UpdateAnimationState(float direction)
    {
        if (animator == null)
        {
            Debug.LogWarning("PlayerMovement: Animator is null!");
            return;
        }

        if (animationData == null)
        {
            Debug.LogWarning("PlayerMovement: AnimationData is null!");
            return;
        }

        if (!animator.enabled)
        {
            Debug.LogWarning("PlayerMovement: Animator is disabled!");
            return;
        }

        _moveDirection = direction;

        // Update Animator parameter: -1 = left, 0 = idle, 1 = right
        if (!string.IsNullOrEmpty(animationData.moveDirectionParameter))
        {
            animator.SetFloat(animationData.moveDirectionParameter, direction);
            
            // Debug log to verify parameter is being set
            if (Application.isPlaying)
            {
                float currentValue = animator.GetFloat(animationData.moveDirectionParameter);
                Debug.Log($"PlayerMovement: Setting MoveDirection to {direction}, Current value in Animator: {currentValue}");
            }
        }
        else
        {
            Debug.LogWarning($"PlayerMovement: moveDirectionParameter is empty! Check AnimationData.");
        }
    }

    /// <summary>
    /// Call this to swap to a different character model/animation set at runtime.
    /// </summary>
    public void SetAnimationData(PlayerAnimationData newAnimationData)
    {
        animationData = newAnimationData;
        if (animationData != null && animator != null)
        {
            SetupAnimator();
        }
    }
}