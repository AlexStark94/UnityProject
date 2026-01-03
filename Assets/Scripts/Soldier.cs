using UnityEngine;

/// <summary>
/// Individual soldier component. Each soldier in the squad has this script.
/// </summary>
public class Soldier : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider soldierCollider;
    [SerializeField] private Renderer soldierRenderer;

    [Header("Shooting")]
    [SerializeField] private Transform shootPoint; // Where bullets spawn from this soldier
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private string bulletTag = "Bullet";

    private float _lastShotTime;
    private bool _isShooting;

    private void Awake()
    {
        // Find components if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
        if (soldierCollider == null)
            soldierCollider = GetComponent<Collider>();
        if (soldierRenderer == null)
            soldierRenderer = GetComponent<Renderer>();

        // Create shoot point if not assigned
        if (shootPoint == null)
        {
            GameObject shootPointObj = new GameObject("ShootPoint");
            shootPointObj.transform.SetParent(transform);
            shootPointObj.transform.localPosition = Vector3.forward * 1f; // 1 unit forward
            shootPoint = shootPointObj.transform;
        }
    }

    private void Update()
    {
        // Auto-shoot forward when enabled
        if (_isShooting)
        {
            Shoot();
        }
    }

    /// <summary>
    /// Start shooting forward automatically.
    /// </summary>
    public void StartShooting()
    {
        _isShooting = true;
    }

    /// <summary>
    /// Stop shooting.
    /// </summary>
    public void StopShooting()
    {
        _isShooting = false;
    }

    /// <summary>
    /// Update animation state based on movement direction.
    /// </summary>
    public void SetMoveDirection(float direction)
    {
        if (animator != null)
        {
            animator.SetFloat("MoveDirection", direction);
        }
    }

    private void Shoot()
    {
        // Check fire rate cooldown
        if (Time.time - _lastShotTime < fireRate)
        {
            return;
        }

        _lastShotTime = Time.time;

        // Spawn bullet from shoot point
        Vector3 spawnPosition = shootPoint != null ? shootPoint.position : transform.position + transform.forward;
        GameObject bullet = CreateBullet(spawnPosition);
        bullet.tag = bulletTag;

        // Set bullet properties
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript == null)
        {
            bulletScript = bullet.AddComponent<Bullet>();
        }
        bulletScript.speed = bulletSpeed;
        bulletScript.direction = transform.forward;
    }

    private GameObject CreateBullet(Vector3 position)
    {
        if (bulletPrefab != null)
        {
            return Instantiate(bulletPrefab, position, transform.rotation);
        }

        // Create bullet dynamically if no prefab
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "Bullet";
        bullet.transform.position = position;
        bullet.transform.localScale = Vector3.one * 0.2f;

        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.speed = bulletSpeed;
        bulletScript.direction = transform.forward;

        Renderer renderer = bullet.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.yellow;
            renderer.material = mat;
        }

        return bullet;
    }

    /// <summary>
    /// Remove this soldier from the squad (when killed or removed).
    /// </summary>
    public void RemoveFromSquad()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        Destroy(gameObject);
    }
}

