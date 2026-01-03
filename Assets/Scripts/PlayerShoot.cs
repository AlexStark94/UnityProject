using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting")]
    [Tooltip("Prefab for the bullet. If null, a sphere will be created automatically.")]
    public GameObject bulletPrefab;

    [Tooltip("Speed at which bullets travel forward.")]
    public float bulletSpeed = 20f;

    [Tooltip("Time between shots (in seconds).")]
    public float fireRate = 0.3f;

    [Tooltip("Forward offset from player position where bullets spawn.")]
    public float spawnOffset = 1f;

    [Header("Bullet Settings")]
    [Tooltip("Size of the bullet sphere (if using auto-created bullets).")]
    public float bulletSize = 0.2f;

    [Tooltip("Tag to assign to bullets.")]
    public string bulletTag = "Bullet";

    private float _lastShotTime;
    private bool _wasTouching;

    private void Update()
    {
        HandleShootInput();
    }

    private void HandleShootInput()
    {
        bool isTouching = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Mouse input for testing on PC - fire while button is held
        if (Input.GetMouseButton(0))
    {
            isTouching = true;
        }
#else
        // Touch input for mobile - fire while at least one finger is on screen
        if (Input.touchCount > 0)
        {
            isTouching = true;
        }
#endif

        // While touching/pressing, keep shooting, controlled by fireRate cooldown
        if (isTouching)
        {
            Shoot();
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

        // Calculate spawn position (forward from player)
        Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;

        GameObject bullet;

        if (bulletPrefab != null)
        {
            // Use prefab if assigned
            bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);
        }
        else
        {
            // Create bullet dynamically
            bullet = CreateBullet(spawnPosition);
        }

        // Set tag
        bullet.tag = bulletTag;

        // Get or add Bullet component and set speed
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
        // Create sphere GameObject
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "Bullet";
        bullet.transform.position = position;
        bullet.transform.localScale = Vector3.one * bulletSize;

        // Add Rigidbody for physics
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add Bullet script
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.speed = bulletSpeed;
        bulletScript.direction = transform.forward;

        // Optional: Add a material to make it visible
        Renderer renderer = bullet.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.yellow; // Yellow bullets for visibility
            renderer.material = mat;
        }

        return bullet;
    }
}

// Bullet behaviour used by PlayerShoot. Kept in the same file so it's always available.
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed at which the bullet travels.")]
    public float speed = 20f;

    [Tooltip("Direction the bullet travels.")]
    public Vector3 direction = Vector3.forward;

    [Header("Lifetime")]
    [Tooltip("How long the bullet exists before auto-destroying (in seconds).")]
    public float lifetime = 5f;

    [Header("Damage")]
    [Tooltip("Damage this bullet deals to enemies.")]
    public int damage = 1;

    private float _spawnTime;

    private void Start()
    {
        _spawnTime = Time.time;
    }

    private void Update()
    {
        // Move bullet forward
        transform.position += direction * speed * Time.deltaTime;

        // Auto-destroy after lifetime
        if (Time.time - _spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Don't collide with player, squad, or other bullets
        if (other.CompareTag("Player") || other.CompareTag("Bullet") || other.CompareTag("Squad"))
        {
            return;
        }

        // Check if we hit an enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Destroy bullet on any other collision
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision-based hits (non-trigger colliders)
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }

        Destroy(gameObject);
    }
}

