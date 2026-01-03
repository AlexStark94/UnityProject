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

    [Header("Animation Smoothing")]
    [Tooltip("Minimum input movement per frame to update animation (prevents jitter during very slow movement).")]
    public float minMovementPerFrame = 2f; // In screen pixels per frame

    private Camera _mainCamera;
    private bool _isDragging;
    private float _startTouchX;
    private float _startPlayerX;
    private float _previousPlayerX; // Track previous player position for stable direction detection
    private float _moveDirection; // -1 = left, 0 = idle, 1 = right
    private float _lastDirection; // Track last direction to prevent rapid flips
    private float _previousInputX; // Track previous input position for per-frame movement calculation

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
            _previousPlayerX = transform.position.x;
            UpdateAnimationState(0f); // Idle when not touching
            return;
        }

        Touch touch = Input.GetTouch(0);
        float currentTouchX = touch.position.x;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _isDragging = false;
                _startTouchX = currentTouchX;
                _previousInputX = currentTouchX;
                _startPlayerX = transform.position.x;
                _previousPlayerX = transform.position.x;
                _lastDirection = 0f;
                UpdateAnimationState(0f); // Idle when just touching (not moving yet)
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                float deltaX = currentTouchX - _startTouchX;
                float frameDeltaX = currentTouchX - _previousInputX;
                ApplyDrag(deltaX, frameDeltaX);
                _previousInputX = currentTouchX;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                _isDragging = false;
                _previousPlayerX = transform.position.x;
                _lastDirection = 0f;
                UpdateAnimationState(0f); // Idle when touch ends
                break;
        }
    }

    private void HandleMouseInput()
    {
        float currentMouseX = Input.mousePosition.x;

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = false;
            _startTouchX = currentMouseX;
            _previousInputX = currentMouseX;
            _startPlayerX = transform.position.x;
            _previousPlayerX = transform.position.x;
            _lastDirection = 0f;
            UpdateAnimationState(0f); // Idle when just clicking (not moving yet)
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = currentMouseX - _startTouchX;
            float frameDeltaX = currentMouseX - _previousInputX;
            ApplyDrag(deltaX, frameDeltaX);
            _previousInputX = currentMouseX;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _previousPlayerX = transform.position.x;
            _lastDirection = 0f;
            UpdateAnimationState(0f); // Idle when mouse released
        }
        else
        {
            _previousPlayerX = transform.position.x;
            _lastDirection = 0f;
            UpdateAnimationState(0f); // Idle when not pressing mouse
        }
    }

    private void ApplyDrag(float deltaX, float frameDeltaX)
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

        // Apply movement based on total delta (for smooth position updates)
        float screenWidth = Screen.width;
        float normalizedDelta = deltaX / screenWidth; // -1 .. 1 approx for full-width drag

        float targetX = _startPlayerX + normalizedDelta * moveSpeed;

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(targetX, minX, maxX);
        transform.position = position;

        // Calculate direction based on input movement
        // Use frameDeltaX for instant direction detection when switching
        float newDirection = 0f;
        
        // If moving significantly this frame, use frame direction (instant response)
        if (Mathf.Abs(frameDeltaX) > minMovementPerFrame)
        {
            newDirection = Mathf.Sign(frameDeltaX);
        }
        // If barely moving this frame but overall movement exists, use overall direction
        else if (Mathf.Abs(deltaX) > dragThreshold)
        {
            newDirection = Mathf.Sign(deltaX);
        }
        // If not moving, keep last direction (prevents flickering to idle during slow movement)
        else
        {
            newDirection = _lastDirection;
        }

        // Always update animation - this gives instant response when switching directions
        // The minMovementPerFrame threshold prevents jitter during very slow movement
        if (newDirection != _lastDirection || Mathf.Abs(frameDeltaX) > minMovementPerFrame)
        {
            UpdateAnimationState(newDirection);
            _lastDirection = newDirection;
        }
        
        _previousPlayerX = position.x;
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