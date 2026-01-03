using UnityEngine;

/// <summary>
/// Manages the formation and positioning of soldiers in the squad.
/// </summary>
public class SquadFormation : MonoBehaviour
{
    [Header("Formation Settings")]
    [Range(0f, 1f)]
    [Tooltip("Controls the spacing between soldiers in the formation.")]
    [SerializeField] private float radiusFactor = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("Controls the angle distribution in the spiral formation.")]
    [SerializeField] private float angleFactor = 1f;

    [Header("Settings")]
    [Tooltip("Prefab for individual soldiers. Must have Soldier component.")]
    [SerializeField] private GameObject soldierPrefab;

    [Header("Animation")]
    [Tooltip("Animation data for soldiers. Will be applied to all soldiers.")]
    [SerializeField] private PlayerAnimationData animationData;

    private bool _forceUpdatePositions = false; // Flag to force immediate position update
    private int _framesToSkipLerp = 0; // Number of frames to skip lerp after adding/removing
    private bool _isUpdatingFormation = false; // Prevent recursive calls
    private bool _skipFormationUpdate = false; // Skip UpdateFormation entirely when adding/removing

    private void Update()
    {
        // Skip formation update if we're in the middle of adding/removing
        if (_skipFormationUpdate)
        {
            return;
        }

        // If we're skipping lerp (just added/removed), use immediate positioning
        if (_framesToSkipLerp > 0)
        {
            ForceUpdateAllPositions();
            _framesToSkipLerp--;
            return;
        }

        if (!_isUpdatingFormation)
        {
            UpdateFormation();
        }
    }

    private void LateUpdate()
    {
        // If we just added/removed soldiers, ensure positions are correct after all updates
        if (_forceUpdatePositions || _framesToSkipLerp > 0)
        {
            ForceUpdateAllPositions();
            _forceUpdatePositions = false;
        }
    }

    /// <summary>
    /// Updates soldier positions in a Fermat spiral formation.
    /// </summary>
    private void UpdateFormation()
    {
        if (transform.childCount == 0) return;

        _isUpdatingFormation = true;

        float goldenAngle = 137.5f * angleFactor;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform soldier = transform.GetChild(i);

            // Calculate position in spiral formation
            float x = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i + 1));
            float z = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i + 1));

            Vector3 targetPosition = new Vector3(x, 0, z);

            // If force update flag is set or we're skipping lerp, set all positions immediately
            // Otherwise, smoothly lerp to target position
            if (_forceUpdatePositions || _framesToSkipLerp > 0)
            {
                soldier.localPosition = targetPosition;
            }
            else
            {
                // Smoothly move to target position
                soldier.localPosition = Vector3.Lerp(soldier.localPosition, targetPosition, 0.2f);
            }
        }

        _isUpdatingFormation = false;
    }

    /// <summary>
    /// Immediately updates all soldier positions to their correct formation positions.
    /// </summary>
    private void ForceUpdateAllPositions()
    {
        if (transform.childCount == 0) return;

        float goldenAngle = 137.5f * angleFactor;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform soldier = transform.GetChild(i);

            // Calculate position in spiral formation
            float x = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i + 1));
            float z = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i + 1));

            Vector3 targetPosition = new Vector3(x, 0, z);
            
            // Set position immediately, no lerp
            soldier.localPosition = targetPosition;
        }
    }

    /// <summary>
    /// Gets the radius of the squad formation (useful for collision detection).
    /// </summary>
    public float GetSquadRadius()
    {
        if (transform.childCount == 0)
            return 0f;
        return radiusFactor * Mathf.Sqrt(transform.childCount);
    }

    /// <summary>
    /// Adds soldiers to the squad.
    /// </summary>
    public void AddSoldiers(int amount)
    {
        if (soldierPrefab == null)
        {
            Debug.LogError("SquadFormation: soldierPrefab is not assigned! Cannot add soldiers.");
            return;
        }

        Debug.Log($"SquadFormation: Adding {amount} soldiers. Current count: {transform.childCount}");

        // Disable formation update while adding soldiers
        _skipFormationUpdate = true;

        float goldenAngle = 137.5f * angleFactor;

        for (int i = 0; i < amount; i++)
        {
            int newIndex = transform.childCount; // Index of the new soldier
            
            // Calculate target position BEFORE instantiating
            float x = radiusFactor * Mathf.Sqrt(newIndex + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (newIndex + 1));
            float z = radiusFactor * Mathf.Sqrt(newIndex + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (newIndex + 1));
            Vector3 targetPos = new Vector3(x, 0, z);
            
            // Instantiate with worldPositionStays = false to ensure it's a child immediately
            GameObject soldierInstance = Instantiate(soldierPrefab, transform, false);
            soldierInstance.name = "Soldier_" + newIndex;
            
            // Ensure parent transform is up to date
            transform.hasChanged = false;
            
            // Set position immediately - use SetLocalPositionAndRotation to ensure it's set properly
            soldierInstance.transform.SetLocalPositionAndRotation(targetPos, Quaternion.identity);
            
            // Force update to ensure position is applied
            soldierInstance.transform.localPosition = targetPos;

            // Setup soldier components
            Soldier soldierScript = soldierInstance.GetComponent<Soldier>();
            if (soldierScript == null)
            {
                soldierScript = soldierInstance.AddComponent<Soldier>();
            }

            // Setup animation if available
            if (animationData != null)
            {
                Animator animator = soldierInstance.GetComponent<Animator>();
                if (animator != null && animationData.animatorController != null)
                {
                    animator.runtimeAnimatorController = animationData.animatorController;
                }
            }

            // Start shooting
            soldierScript.StartShooting();
        }

        // Immediately update ALL positions (including existing soldiers) to ensure correct formation
        ForceUpdateAllPositions();
        
        // Skip lerp for next 3 frames to prevent position drift
        _framesToSkipLerp = 3;
        _forceUpdatePositions = true;
        
        // Re-enable formation update
        _skipFormationUpdate = false;

        Debug.Log($"SquadFormation: Added {amount} soldiers. New count: {transform.childCount}");
    }

    /// <summary>
    /// Removes soldiers from the squad.
    /// </summary>
    public void RemoveSoldiers(int amount)
    {
        // Disable formation update while removing soldiers
        _skipFormationUpdate = true;

        int soldiersToRemove = Mathf.Min(amount, transform.childCount);

        for (int i = soldiersToRemove - 1; i >= 0; i--)
        {
            if (transform.childCount > 0)
            {
                Soldier soldier = transform.GetChild(transform.childCount - 1).GetComponent<Soldier>();
                if (soldier != null)
                {
                    soldier.RemoveFromSquad();
                }
                else
                {
                    Destroy(transform.GetChild(transform.childCount - 1).gameObject);
                }
            }
        }

        // Immediately update ALL positions to reorganize formation
        ForceUpdateAllPositions();
        
        // Skip lerp for next 3 frames to prevent position drift
        _framesToSkipLerp = 3;
        _forceUpdatePositions = true;
        
        // Re-enable formation update
        _skipFormationUpdate = false;
    }

    /// <summary>
    /// Multiplies the squad size by the given factor.
    /// </summary>
    public void MultiplySquad(float multiplier)
    {
        int currentCount = transform.childCount;
        int targetCount = Mathf.RoundToInt(currentCount * multiplier);
        int soldiersToAdd = targetCount - currentCount;

        if (soldiersToAdd > 0)
        {
            AddSoldiers(soldiersToAdd);
        }
        else if (soldiersToAdd < 0)
        {
            RemoveSoldiers(-soldiersToAdd);
        }
    }

    /// <summary>
    /// Divides the squad size by the given divisor.
    /// </summary>
    public void DivideSquad(float divisor)
    {
        if (divisor <= 0) return;

        int currentCount = transform.childCount;
        int targetCount = Mathf.RoundToInt(currentCount / divisor);
        int soldiersToRemove = currentCount - targetCount;

        if (soldiersToRemove > 0)
        {
            RemoveSoldiers(soldiersToRemove);
        }
    }

    /// <summary>
    /// Gets the current number of soldiers in the squad.
    /// </summary>
    public int GetSoldierCount()
    {
        return transform.childCount;
    }
}

