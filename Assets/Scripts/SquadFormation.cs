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

    private void Update()
    {
        UpdateFormation();
    }

    /// <summary>
    /// Updates soldier positions in a Fermat spiral formation.
    /// </summary>
    private void UpdateFormation()
    {
        float goldenAngle = 137.5f * angleFactor;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform soldier = transform.GetChild(i);

            // Calculate position in spiral formation
            float x = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i + 1));
            float z = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i + 1));

            Vector3 targetPosition = new Vector3(x, 0, z);

            // Smoothly move to target position
            soldier.localPosition = Vector3.Lerp(soldier.localPosition, targetPosition, 0.1f);
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

        for (int i = 0; i < amount; i++)
        {
            GameObject soldierInstance = Instantiate(soldierPrefab, transform);
            soldierInstance.name = "Soldier_" + soldierInstance.transform.GetSiblingIndex();

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

        Debug.Log($"SquadFormation: Added {amount} soldiers. New count: {transform.childCount}");
    }

    /// <summary>
    /// Removes soldiers from the squad.
    /// </summary>
    public void RemoveSoldiers(int amount)
    {
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

