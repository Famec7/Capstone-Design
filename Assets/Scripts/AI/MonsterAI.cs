using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private bool isAggressive = true;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private float sightDistance = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Transform[] patrolPoints;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private Transform player;
    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private float patrolWaitTime = 2f;
    private float patrolTimer;
    private float attackTimer;
    private bool isProvoked = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                TryDetectPlayer();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }
    }

    private void TryDetectPlayer()
    {
        if (!CanSeePlayer()) return;

        if (isAggressive)
        {
            currentState = State.Chase;
        }
        else if (isProvoked)
        {
            currentState = State.Chase;
        }
    }

    private void Chase()
    {
        if (!CanSeePlayer() && !isAggressive && !isProvoked)
        {
            currentState = State.Patrol;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            return;
        }

        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Debug.Log($"{name} attacks the player!");
            // 데미지 주는 로직 나중에 추가
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = State.Chase;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).normalized;
        float distance = directionToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);

        if (angle <= sightAngle * 0.5f && distance <= sightDistance)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(origin, directionToPlayer.normalized, out RaycastHit hit, sightDistance))
            {
                Debug.DrawRay(origin, directionToPlayer.normalized * sightDistance, Color.red);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnDamagedByPlayer()
    {
        if (!isAggressive)
        {
            isProvoked = true;
            currentState = State.Chase;
        }
    }
}