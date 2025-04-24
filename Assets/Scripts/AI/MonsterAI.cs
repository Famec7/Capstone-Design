using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("행동 설정")]
    [SerializeField] private bool isAggressive = true;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private float sightDistance = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("순찰 설정")]
    [SerializeField] private float patrolRadius = 5f;         
    [SerializeField] private float waitTimeAtPatrolPoint = 3f; 

    private enum State { Patrol, Wait, Return, Chase, Attack }
    private State currentState = State.Patrol;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 homePosition;
    private Vector3 patrolTarget;
    private float waitTimer;
    private float attackTimer;
    private bool isProvoked = false;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        homePosition = transform.position;
        GoToRandomPatrolPoint();
    }

    void Update()
    {
        float moveSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", moveSpeed);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                TryDetectPlayer();
                break;
            case State.Wait:
                WaitAtPatrolPoint();
                TryDetectPlayer();
                break;
            case State.Return:
                ReturnToHome();
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
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Wait;
            waitTimer = 0f;
        }
    }

    private void WaitAtPatrolPoint()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTimeAtPatrolPoint)
        {
            agent.SetDestination(homePosition);
            currentState = State.Return;
        }
    }

    private void ReturnToHome()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToRandomPatrolPoint();
            currentState = State.Patrol;
        }
    }

    private void GoToRandomPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 randomPoint = homePosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            agent.SetDestination(patrolTarget);
        }
        else
        {
            Debug.LogWarning("유효한 순찰 지점을 찾지 못했습니다. 홈 포지션 유지.");
            agent.SetDestination(homePosition);
        }
    }

    private void Attack()
    {
        if (isAttacking) return;

        if (attackTimer < attackCooldown)
        {
            attackTimer += Time.deltaTime;
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            isAttacking = true;
            attackTimer = 0f;

            transform.LookAt(player);
            agent.SetDestination(transform.position);
            animator.SetTrigger("Attack");

            Debug.Log($"{name} attacks the player!");
        }
        else
        {
            currentState = State.Chase;
        }
    }

    public void EndAttack()
    {
        Debug.Log("EndAttack called");
        isAttacking = false;
    }

    private void TryDetectPlayer()
    {
        if (!CanSeePlayer()) return;

        if (isAggressive || isProvoked)
        {
            currentState = State.Chase;
        }
    }

    private void Chase()
    {
        if (!CanSeePlayer() && !isAggressive && !isProvoked)
        {
            GoToRandomPatrolPoint();
            currentState = State.Patrol;
            return;
        }

        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 flatDirectionToPlayer = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        float distance = flatDirectionToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, flatDirectionToPlayer.normalized);

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        DrawSightDebug(origin);

        if (angle <= sightAngle * 0.5f && distance <= sightDistance)
        {
            if (Physics.Raycast(origin, flatDirectionToPlayer.normalized, out RaycastHit hit, sightDistance))
            {
                Debug.DrawRay(origin, flatDirectionToPlayer.normalized * sightDistance, Color.red, 0.1f);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DrawSightDebug(Vector3 origin)
    {
        int segmentCount = 10; 
        float halfFOV = sightAngle * 0.5f;

        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = -halfFOV + (sightAngle / segmentCount) * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * transform.forward;

            Debug.DrawRay(origin, direction.normalized * sightDistance, Color.yellow, 0.1f);
        }
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