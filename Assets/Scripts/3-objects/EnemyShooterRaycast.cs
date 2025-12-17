using UnityEngine;

public class EnemyShooterRayCast : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform firePoint; // מיקום יציאת הקרן

    [Header("Visuals")]
    [SerializeField] private GameObject bulletVisualPrefab; // רק בשביל היופי, לא עושה נזק

    [Header("Raycast Settings (Assignment Requirement)")]
    [SerializeField] private float fireRate = 2f; // זמן בין יריות
    [SerializeField] private float range = 30f;   // טווח הקרן
    [SerializeField] private int damage = 1;

    private float timer;

    private void Update()
    {
        if (player == null || firePoint == null) return;

        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        // --- התיקון הגדול לקפיצה ---
        // במקום לכוון ל-player.position (שזז למעלה בקפיצה),
        // אנחנו מכוונים לנקודה באותו גובה של הרובה, אבל בכיוון השחקן.
        Vector3 targetPos = new Vector3(player.position.x, firePoint.position.y, player.position.z);

        // חישוב הכיוון
        Vector3 direction = (targetPos - firePoint.position).normalized;

        // 1. יצירת אפקט ויזואלי (לא חובה להוראות, אבל עוזר להבין שירו עליך)
        if (bulletVisualPrefab != null)
        {
            Instantiate(bulletVisualPrefab, firePoint.position, Quaternion.LookRotation(direction));
        }

        // 2. דרישת התרגיל: בדיקת פגיעה ע"י הטלת קרן (Raycast)
        // אנחנו יורים את הקרן ישר קדימה מהרובה
        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, range))
        {
            // בדיקה אם פגענו בשחקן
            if (hit.collider.CompareTag("Player"))
            {
                // קריאה לסקריפט החיים (חלק ב' של ההוראות)
                PlayerHealth hp = hit.collider.GetComponentInParent<PlayerHealth>();
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                }
            }
        }

        // ציור קו אדום בסצנה כדי שתראי איפה הקרן עברה (לדיבאג)
        Debug.DrawRay(firePoint.position, direction * range, Color.red, 1f);
    }
}
