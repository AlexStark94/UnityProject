using UnityEngine;

/// <summary>
/// Manages animations for all soldiers in the squad.
/// </summary>
public class SquadAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform soldiersParent;

    private void Start()
    {
        // Find soldiers parent if not assigned
        if (soldiersParent == null)
        {
            SquadFormation squadFormation = GetComponentInParent<SquadFormation>();
            if (squadFormation != null)
            {
                soldiersParent = squadFormation.transform;
            }
        }
    }

    /// <summary>
    /// Starts shooting animation for all soldiers.
    /// </summary>
    public void StartShooting()
    {
        if (soldiersParent == null) return;

        for (int i = 0; i < soldiersParent.childCount; i++)
        {
            Soldier soldier = soldiersParent.GetChild(i).GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.StartShooting();
            }
        }
    }

    /// <summary>
    /// Stops shooting animation for all soldiers.
    /// </summary>
    public void StopShooting()
    {
        if (soldiersParent == null) return;

        for (int i = 0; i < soldiersParent.childCount; i++)
        {
            Soldier soldier = soldiersParent.GetChild(i).GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.StopShooting();
            }
        }
    }

    /// <summary>
    /// Sets movement direction for all soldiers.
    /// </summary>
    public void SetMoveDirection(float direction)
    {
        if (soldiersParent == null) return;

        for (int i = 0; i < soldiersParent.childCount; i++)
        {
            Soldier soldier = soldiersParent.GetChild(i).GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.SetMoveDirection(direction);
            }
        }
    }
}

