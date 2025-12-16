using UnityEngine;

public class EnemyShooterRaycast : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform firePoint;

    [Header("Visual Bullet")]
    [SerializeField] private GameObject bulletVisualPrefab;

    [Header("Raycast Damage")]
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float range = 30f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayers; // Player בלבד

    private float timer;

    private void Update()
    {
        if (player == null || firePoint == null) return;

        timer += Time.deltaTime;
        if (timer < fireRate) return;
        timer = 0f;

        Shoot();
    }

    private void Shoot()
    {
        Vector3 dir = (player.position - firePoint.position).normalized;

        // 1) יצירת כדור ויזואלי
        if (bulletVisualPrefab != null)
        {
            Instantiate(
                bulletVisualPrefab,
                firePoint.position,
                Quaternion.LookRotation(dir)
            );
        }

        // 2) בדיקת פגיעה אמיתית – Raycast
        if (Physics.Raycast(firePoint.position, dir, out RaycastHit hit, range, hitLayers))
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth hp = hit.collider.GetComponentInParent<PlayerHealth>();
                if (hp != null)
                    hp.TakeDamage(damage);
            }
        }
    }
}
