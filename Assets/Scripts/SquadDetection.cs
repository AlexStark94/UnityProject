using UnityEngine;

/// <summary>
/// Handles collision detection for the squad (enemies, power-ups, obstacles).
/// </summary>
public class SquadDetection : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private SquadFormation squadFormation;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask powerUpLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Debug")]
    [Tooltip("Press this key to add 5 soldiers (for testing).")]
    [SerializeField] private KeyCode debugAddKey = KeyCode.Equals; // = key (same as + without shift)

    [Tooltip("Press this key to remove 5 soldiers (for testing).")]
    [SerializeField] private KeyCode debugRemoveKey = KeyCode.Minus; // - key

    [Tooltip("Press this key to multiply squad by 2 (for testing).")]
    [SerializeField] private KeyCode debugMultiplyKey = KeyCode.M;

    [Tooltip("Press this key to divide squad by 2 (for testing).")]
    [SerializeField] private KeyCode debugDivideKey = KeyCode.D;

    private void Start()
    {
        // Find squad formation if not assigned
        if (squadFormation == null)
        {
            squadFormation = GetComponentInParent<SquadFormation>();
            if (squadFormation == null)
            {
                squadFormation = FindObjectOfType<SquadFormation>();
            }
        }

        // Debug info
        if (squadFormation == null)
        {
            Debug.LogError("SquadDetection: Could not find SquadFormation! Make sure it's assigned or in the scene.");
        }
        else
        {
            Debug.Log($"SquadDetection: Found SquadFormation. Current soldier count: {squadFormation.GetSoldierCount()}");
        }
    }

    private void Update()
    {
        // Debug keys for testing
        HandleDebugInput();

        // Detection checks
        DetectEnemies();
        DetectPowerUps();
        DetectObstacles();
    }

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(debugAddKey) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Debug.Log("Debug: Adding 1 soldiers");
            if (squadFormation != null)
            {
                int beforeCount = squadFormation.GetSoldierCount();
                squadFormation.AddSoldiers(1);
                int afterCount = squadFormation.GetSoldierCount();
                Debug.Log($"Squad size: {beforeCount} -> {afterCount}");
            }
            else
            {
                Debug.LogError("SquadDetection: SquadFormation is null! Cannot add soldiers.");
            }
        }

        if (Input.GetKeyDown(debugRemoveKey) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Debug.Log("Debug: Removing 1 soldiers");
            if (squadFormation != null)
            {
                int beforeCount = squadFormation.GetSoldierCount();
                squadFormation.RemoveSoldiers(1);
                int afterCount = squadFormation.GetSoldierCount();
                Debug.Log($"Squad size: {beforeCount} -> {afterCount}");
            }
            else
            {
                Debug.LogError("SquadDetection: SquadFormation is null! Cannot remove soldiers.");
            }
        }

        if (Input.GetKeyDown(debugMultiplyKey))
        {
            Debug.Log("Debug: Multiplying squad by 2");
            if (squadFormation != null)
            {
                int beforeCount = squadFormation.GetSoldierCount();
                squadFormation.MultiplySquad(2f);
                int afterCount = squadFormation.GetSoldierCount();
                Debug.Log($"Squad size: {beforeCount} -> {afterCount}");
            }
            else
            {
                Debug.LogError("SquadDetection: SquadFormation is null! Cannot multiply squad.");
            }
        }

        if (Input.GetKeyDown(debugDivideKey))
        {
            Debug.Log("Debug: Dividing squad by 2");
            if (squadFormation != null)
            {
                int beforeCount = squadFormation.GetSoldierCount();
                squadFormation.DivideSquad(2f);
                int afterCount = squadFormation.GetSoldierCount();
                Debug.Log($"Squad size: {beforeCount} -> {afterCount}");
            }
            else
            {
                Debug.LogError("SquadDetection: SquadFormation is null! Cannot divide squad.");
            }
        }
    }

    private void DetectEnemies()
    {
        if (squadFormation == null) return;

        float detectionRadius = squadFormation.GetSquadRadius() + 1f;
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            // Enemy hit by bullets (handled by Bullet script)
            // This could be used for melee enemies or other interactions
        }
    }

    private void DetectPowerUps()
    {
        if (squadFormation == null) return;

        float detectionRadius = squadFormation.GetSquadRadius() + 0.5f;
        Collider[] powerUps = Physics.OverlapSphere(transform.position, detectionRadius, powerUpLayer);

        foreach (Collider powerUp in powerUps)
        {
            PowerUp powerUpScript = powerUp.GetComponent<PowerUp>();
            if (powerUpScript != null)
            {
                ApplyPowerUpEffect(powerUpScript);
                Destroy(powerUp.gameObject);
            }
        }
    }

    private void DetectObstacles()
    {
        if (squadFormation == null) return;

        float detectionRadius = squadFormation.GetSquadRadius() + 0.5f;
        Collider[] obstacles = Physics.OverlapSphere(transform.position, detectionRadius, obstacleLayer);

        foreach (Collider obstacle in obstacles)
        {
            Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();
            if (obstacleScript != null)
            {
                ApplyObstacleEffect(obstacleScript);
            }
        }
    }

    private void ApplyPowerUpEffect(PowerUp powerUp)
    {
        switch (powerUp.effectType)
        {
            case PowerUpType.Add:
                squadFormation.AddSoldiers(powerUp.amount);
                Debug.Log($"PowerUp: Added {powerUp.amount} soldiers");
                break;

            case PowerUpType.Multiply:
                squadFormation.MultiplySquad(powerUp.multiplier);
                Debug.Log($"PowerUp: Multiplied squad by {powerUp.multiplier}");
                break;
        }
    }

    private void ApplyObstacleEffect(Obstacle obstacle)
    {
        switch (obstacle.effectType)
        {
            case ObstacleType.Subtract:
                squadFormation.RemoveSoldiers(obstacle.amount);
                Debug.Log($"Obstacle: Removed {obstacle.amount} soldiers");
                break;

            case ObstacleType.Divide:
                squadFormation.DivideSquad(obstacle.divisor);
                Debug.Log($"Obstacle: Divided squad by {obstacle.divisor}");
                break;
        }
    }
}

/// <summary>
/// Power-up component that can be added to GameObjects.
/// </summary>
public class PowerUp : MonoBehaviour
{
    public PowerUpType effectType;
    public int amount; // For Add
    public float multiplier; // For Multiply
}

public enum PowerUpType
{
    Add,
    Multiply
}

/// <summary>
/// Obstacle component that can be added to GameObjects.
/// </summary>
public class Obstacle : MonoBehaviour
{
    public ObstacleType effectType;
    public int amount; // For Subtract
    public float divisor; // For Divide
}

public enum ObstacleType
{
    Subtract,
    Divide
}

