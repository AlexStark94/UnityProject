using UnityEngine;

/// <summary>
/// Controls the squad movement (forward and side-to-side).
/// This replaces/enhances PlayerMovement for squad-based gameplay.
/// </summary>
public class SquadController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private SquadFormation squadFormation;

    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed when dragging.")]
    [SerializeField] private float horizontalSpeed = 10f;

    [Tooltip("Minimum and maximum X position the squad can move to.")]
    [SerializeField] private float minX = 495.54f;
    [SerializeField] private float maxX = 508.46f;

    [Header("Input")]
    [Tooltip("How far (in screen pixels) the finger must move to start dragging.")]
    [SerializeField] private float dragThreshold = 5f;

    [Header("Animation")]
    [Tooltip("Animation data for soldiers.")]
    [SerializeField] private PlayerAnimationData animationData;

    private bool _isDragging;
    private float _startTouchX;
    private float _startSquadX;
    private float _currentDirection; // -1 = left, 0 = idle, 1 = right

    private void Start()
    {
        // Initialize squad with one soldier if empty
        if (squadFormation != null && squadFormation.GetSoldierCount() == 0)
        {
            squadFormation.AddSoldiers(1);
        }
    }

    private void Update()
    {
        // Handle input
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif

        // Update all soldier animations
        UpdateSoldierAnimations();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            _isDragging = false;
            _currentDirection = 0f;
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _isDragging = false;
                _startTouchX = touch.position.x;
                _startSquadX = transform.position.x;
                _currentDirection = 0f;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                float deltaX = touch.position.x - _startTouchX;
                ApplyDrag(deltaX);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                _isDragging = false;
                _currentDirection = 0f;
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = false;
            _startTouchX = Input.mousePosition.x;
            _startSquadX = transform.position.x;
            _currentDirection = 0f;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.mousePosition.x - _startTouchX;
            ApplyDrag(deltaX);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _currentDirection = 0f;
        }
        else
        {
            _currentDirection = 0f;
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
            _currentDirection = 0f;
            return;
        }

        float screenWidth = Screen.width;
        float normalizedDelta = deltaX / screenWidth;

        float targetX = _startSquadX + normalizedDelta * horizontalSpeed;

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(targetX, minX, maxX);
        transform.position = position;

        // Determine direction: -1 for left, 1 for right
        _currentDirection = Mathf.Sign(deltaX);
    }

    /// <summary>
    /// Updates animation state for all soldiers in the squad.
    /// </summary>
    private void UpdateSoldierAnimations()
    {
        if (squadFormation == null) return;

        for (int i = 0; i < squadFormation.transform.childCount; i++)
        {
            Transform soldierTransform = squadFormation.transform.GetChild(i);
            Soldier soldier = soldierTransform.GetComponent<Soldier>();

            if (soldier != null)
            {
                soldier.SetMoveDirection(_currentDirection);
            }
        }
    }

    /// <summary>
    /// Gets the squad formation component.
    /// </summary>
    public SquadFormation GetSquadFormation()
    {
        return squadFormation;
    }
}

