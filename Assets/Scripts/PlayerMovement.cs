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

    private Camera _mainCamera;
    private bool _isDragging;
    private float _startTouchX;
    private float _startPlayerX;

    private void Awake()
    {
        _mainCamera = Camera.main;
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
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _isDragging = false;
                _startTouchX = touch.position.x;
                _startPlayerX = transform.position.x;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                float deltaX = touch.position.x - _startTouchX;
                ApplyDrag(deltaX);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                _isDragging = false;
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
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.mousePosition.x - _startTouchX;
            ApplyDrag(deltaX);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
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
            return;

        float screenWidth = Screen.width;
        float normalizedDelta = deltaX / screenWidth; // -1 .. 1 approx for full-width drag

        float targetX = _startPlayerX + normalizedDelta * moveSpeed;

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(targetX, minX, maxX);
        transform.position = position;
    }
}