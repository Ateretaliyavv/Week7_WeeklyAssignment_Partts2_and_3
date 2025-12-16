using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyOrbit : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player; // Drag PlayerArmature here

    [Header("Distance")]
    [SerializeField] private float desiredDistance = 8f; // המרחק הרצוי מהשחקן
    [SerializeField] private float distanceCorrectionSpeed = 6f; // כמה מהר מתקנים מרחק

    [Header("Orbit")]
    [SerializeField] private float orbitDegreesPerSecond = 60f; // מהירות סיבוב סביב השחקן

    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (player == null) return;

        // 1) מחשבים כיוון מהשחקן לאויב במישור XZ
        Vector3 toEnemy = transform.position - player.position;
        toEnemy.y = 0f;

        if (toEnemy.sqrMagnitude < 0.001f)
            toEnemy = Vector3.forward; // fallback

        Vector3 dir = toEnemy.normalized;

        // 2) מטרה: נקודה במרחק desiredDistance מהשחקן
        Vector3 desiredPos = player.position + dir * desiredDistance;
        desiredPos.y = transform.position.y; // שומרים גובה של האויב

        // 3) בנוסף: אורביט סביב השחקן (מסובבים את הכיוון מעט בכל פריים)
        float angle = orbitDegreesPerSecond * Mathf.Deg2Rad * Time.deltaTime;
        Vector3 orbitDir = Quaternion.Euler(0f, orbitDegreesPerSecond * Time.deltaTime, 0f) * dir;
        Vector3 orbitPos = player.position + orbitDir * desiredDistance;
        orbitPos.y = transform.position.y;

        // 4) משלבים: קודם אורביט, ואז תיקון קטן למרחק
        Vector3 targetPos = orbitPos;

        // תנועה עם CharacterController
        Vector3 move = (targetPos - transform.position);
        Vector3 velocity = move * distanceCorrectionSpeed; // "משיכה" לנקודה

        cc.Move(velocity * Time.deltaTime);

        // להסתכל על השחקן (רק סיבוב Y)
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look);
    }
}
