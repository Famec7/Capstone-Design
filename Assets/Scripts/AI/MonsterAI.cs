using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("공격 소리")]
    [SerializeField] private AudioClip effectClip;

    [Header("행동 설정")]
    [SerializeField] private bool isAggressive = true;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private float sightDistance = 10f;

    [Header("순찰 설정")]
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float waitTimeAtPatrolPoint = 3f;

    [Header("도망 설정")]
    [SerializeField] private bool isHerbivore = false;      
    [SerializeField] private float fleeDistance = 10f;      
    private Vector3 fleeTarget;                            

    private enum State { Patrol, Wait, Return, Chase, Attack, Flee }
    private State currentState = State.Patrol;
    private AudioSource audioSource;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 homePosition;
    private Vector3 patrolTarget;
    private float waitTimer;
    private float attackTimer;
    private bool isProvoked = false;
    private bool isAttacking = false;

    private MonsterStatus monsterStatus;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        homePosition = transform.position;
        monsterStatus = GetComponent<MonsterStatus>();

        var moveStat = monsterStatus.GetStat(StatType.MovementSpeed);
        if (moveStat != null)
            agent.speed = GetMovementSpeed(useMax: false);

        GoToRandomPatrolPoint();
    }

    void Update()
    {
        TryDetectPlayer();

        animator.SetFloat("Speed", agent.velocity.magnitude);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Wait:
                WaitAtPatrolPoint();
                break;
            case State.Return:
                ReturnToHome();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Flee:      
                Flee();
                break;
        }
    }

    private float GetMovementSpeed(bool useMax)
    {
        var stat = monsterStatus.GetStat(StatType.MovementSpeed);
        if (stat == null)
            return agent.speed;                   
        return useMax ? stat.maxValue : stat.currentValue; 
    }

    private void Patrol()
    {
        agent.speed = GetMovementSpeed(useMax: false);

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
        agent.speed = GetMovementSpeed(useMax: false);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToRandomPatrolPoint();
            currentState = State.Patrol;
        }
    }

    private void GoToRandomPatrolPoint()
    {
        agent.speed = GetMovementSpeed(useMax: false);

        Vector2 randCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 randPoint = homePosition + new Vector3(randCircle.x, 0, randCircle.y);

        if (NavMesh.SamplePosition(randPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
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

    private void Chase()
    {
        if (isHerbivore) return;
        agent.speed = GetMovementSpeed(useMax: true);

        float range = monsterStatus.GetStat(StatType.AttackRange)?.currentValue ?? 0f;

        if (!CanSeePlayer() && !isAggressive && !isProvoked)
        {
            GoToRandomPatrolPoint();
            currentState = State.Patrol;
            return;
        }

        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= range)
            currentState = State.Attack;
    }

    private void Attack()
    {
        if (isAttacking) return;

        float atkSpeed = monsterStatus.GetStat(StatType.AttackSpeed)?.currentValue ?? 1f;
        float cooldown = 1f / atkSpeed;

        if (attackTimer < cooldown)
        {
            attackTimer += Time.deltaTime;
            return;
        }

        float range = monsterStatus.GetStat(StatType.AttackRange)?.currentValue ?? 0f;

        if (Vector3.Distance(transform.position, player.position) <= range)
        {
            isAttacking = true;
            attackTimer = 0f;

            transform.LookAt(player);
            agent.SetDestination(transform.position);
            animator.SetTrigger("Attack");
            Debug.Log($"{name} 공격 애니메이션 시작");
            audioSource.clip = effectClip;
            audioSource.Play();
        }
        else
        {
            currentState = State.Chase;
        }
    }

    public void OnAttackHit()
    {
        float range = monsterStatus.GetStat(StatType.AttackRange)?.currentValue ?? 0f;
        if (Vector3.Distance(transform.position, player.position) <= range)
        {
            var ps = player.GetComponent<PlayerStatus>();
            if (ps != null)
            {
                float dmg = monsterStatus.GetStat(StatType.AttackPower)?.currentValue ?? 0f;
                ps.ModifyStat(StatType.Health, -dmg);
                Debug.Log($"{name} dealt {dmg} damage to player");
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        Debug.Log("EndAttack called");
    }

    private void Flee()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToRandomPatrolPoint();
            currentState = State.Patrol;
        }
    }

    private void FleeFromPlayer()
    {
        agent.speed = GetMovementSpeed(useMax: true);

        Vector3 awayDir = (transform.position - player.position).normalized;
        Vector3 candidate = transform.position + awayDir * fleeDistance;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            fleeTarget = hit.position;
        else
            fleeTarget = homePosition;


        //// ▶ (추가) 도망 지점에 디버그 구체 생성 (1초 뒤 자동 삭제)
        //GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //debugSphere.transform.position = fleeTarget;                      // 구체 위치
        //debugSphere.transform.localScale = Vector3.one * 0.5f;            // 크기 조절
        //Destroy(debugSphere.GetComponent<Collider>());                    // 충돌체 제거
        //Destroy(debugSphere, 1f);

        Debug.DrawLine(transform.position, fleeTarget, Color.cyan, 1f);
        Debug.Log($"FleeFromPlayer: from {transform.position} to {fleeTarget}");

        agent.SetDestination(fleeTarget);
        currentState = State.Flee;
    }

    private void TryDetectPlayer()
    {
        if (CanSeePlayer() && (isAggressive || isProvoked))
        {
            currentState = State.Chase;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 flatDir = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        float distance = flatDir.magnitude;
        float angle = Vector3.Angle(transform.forward, flatDir.normalized);
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        DrawSightDebug(origin);

        if (angle <= sightAngle * 0.5f && distance <= sightDistance)
        {
            if (Physics.Raycast(origin, flatDir.normalized, out RaycastHit hit, sightDistance))
            {
                bool isPlayer = false;
                if (hit.collider.gameObject == gameObject)
                     isPlayer = hit.collider.CompareTag("Player");
                Color rayColor = isPlayer ? Color.green : Color.red;

                Debug.DrawRay(origin, flatDir.normalized * sightDistance, rayColor, 0.1f);
                //Debug.Log($"{name} Raycast hit: {hit.collider.name} (Tag: {hit.collider.tag})");

                if (isPlayer)
                    return true;
            }
        }

        return false;
    }

    private void DrawSightDebug(Vector3 origin)
    {
        int segments = 10;
        float halfFOV = sightAngle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float ang = -halfFOV + (sightAngle / segments) * i;
            Quaternion rot = Quaternion.Euler(0, ang, 0);
            Vector3 dir = rot * transform.forward;
            Debug.DrawRay(origin, dir.normalized * sightDistance, Color.yellow, 0.1f);
        }
    }

    public void OnDamagedByPlayer()
    {
        if (isHerbivore)    
        {
            FleeFromPlayer();
        }
        if (!isAggressive)
        {
            isProvoked = true;
            currentState = State.Chase;
        }
    }
}
