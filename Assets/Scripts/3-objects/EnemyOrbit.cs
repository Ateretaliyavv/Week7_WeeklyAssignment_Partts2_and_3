using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyOrbit : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Chase")]
    [SerializeField] private float updatePathEvery = 0.15f; // כל כמה זמן לעדכן יעד

    [Header("Shooting Control")]
    [Tooltip("When enemy is within this distance, it will stop moving (so it can shoot).")]
    [SerializeField] private float stopToShootRadius = 8f;

    private NavMeshAgent agent;
    private float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // בתוך רדיוס ירי: עצירה (הירי עצמו נשאר בסקריפט שלך EnemyShooterRayCast)
        if (dist <= stopToShootRadius)
        {
            if (!agent.isStopped) agent.isStopped = true;

            // להסתכל על השחקן
            Vector3 look = player.position - transform.position;
            look.y = 0f;
            if (look.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(look);

            return;
        }

        // מחוץ לרדיוס ירי: ממשיך לרדוף חכם
        if (agent.isStopped) agent.isStopped = false;

        timer += Time.deltaTime;
        if (timer >= updatePathEvery)
        {
            timer = 0f;
            agent.SetDestination(player.position);
        }
    }
}
