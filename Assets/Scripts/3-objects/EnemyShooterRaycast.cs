using UnityEngine;

public class EnemyShooterRaycast : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform firePoint;

    [Header("Visuals")]
    [SerializeField] private GameObject bulletVisualPrefab;

    [Header("Raycast Settings (Assignment Requirement)")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 30f;
    [SerializeField] private int damage = 1;

    [Header("Shoot Only In Radius")]
    [Tooltip("Enemy shoots only if player is within this distance")]
    [SerializeField] private float shootRadius = 8f;

    private float timer;

    private void Update()
    {
        if (player == null || firePoint == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > shootRadius)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        Vector3 targetPos = new Vector3(player.position.x, firePoint.position.y, player.position.z);
        Vector3 direction = (targetPos - firePoint.position).normalized;

        // Visual effect
        if (bulletVisualPrefab != null)
        {
            Instantiate(bulletVisualPrefab, firePoint.position, Quaternion.LookRotation(direction));
        }

        // Real raycast + Damage Application
        if (Physics.Raycast(
                firePoint.position,
                direction,
                out RaycastHit hit,
                range,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            // Check if we hit the player
            PlayerHealth hp = hit.collider.GetComponentInParent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                Debug.Log("Damage applied to player!");
            }
            else
            {
                Debug.Log("Hit is NOT player (no PlayerHealth in parents).");
            }
        }
        else
        {
            Debug.Log("Ray hit nothing");
        }

        Debug.DrawRay(firePoint.position, direction * range, Color.red, 1f);
    }
}
