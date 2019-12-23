using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Zombie : MonoBehaviour , IPooledObject
{

    public event System.Action OnZombieDeath;
    public ZombieAnimation zombieAnimation;

    NavMeshAgent agent;

    [Header("detail")]
    public float wanderRadius = 7.0f;
    public float impactMultiplier = 200f;
    public float viewAngle = 80f;
    public float wanderTimer = 2.0f;

    private float maxWanderTimer;

    //Position 
    private Vector3 wanderPoint;
    Vector3 noPosition = new Vector3(0, -10, 0);
    public Transform target;
    Vector3 attractivePosition;
    private float enemySafeDistance =0.1f;

    public FieldOfView fowReference;

    [HideInInspector]
    public bool isAlive = true;
    public bool recentlyAttracted = false;

    public CurrentState currentState;

    private Enemy enemy;
    private Coroutine wanderHandle;


    GameObject[] potentialTargets;

    private Coroutine WanderHandle
    {
        get { return wanderHandle; }
        set
        {
            if (wanderHandle == null)
            {

                wanderHandle = value;
            }
        }
    }

    public enum CurrentState
    {
        Idle, Wander, Chase, Attack
    }

    public void TakeDamage(Vector3 forceDirection = new Vector3())
    {
        
        if (OnZombieDeath != null)
        {  
            OnZombieDeath();
            isAlive = false;
            OnZombieDeath = null;
            
            // 5 = chest part
            StopAllCoroutines();
            zombieAnimation.ragdollParts[5].AddForce(forceDirection * impactMultiplier, ForceMode.Impulse);
            Destroy(gameObject, 2);

            GameObject zombiePart =  Instantiate(ActorManager.instance.zombiePartPrefab, transform.forward + transform.position + new Vector3(0,2f,0), Quaternion.identity);

            zombiePart.GetComponent<Rigidbody>().AddForce( new Vector3 (Random.Range(-1,1), 2 * 5f, Random.Range(-1,1)), ForceMode.Impulse);
            zombiePart.GetComponent<Rigidbody>().detectCollisions = true;
            //zombiePart.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void Start()
    {
        currentState = CurrentState.Idle;
        attractivePosition = noPosition;
        wanderPoint = RandomWanderPoint();
        agent = GetComponent<NavMeshAgent>();     
        WanderHandle = StartCoroutine("FindNextWanderPoint");
    }

    public void OnObjectSpawn()
    {
        currentState = CurrentState.Idle;
        attractivePosition = noPosition;
        wanderPoint = RandomWanderPoint();
        agent = GetComponent<NavMeshAgent>();
        WanderHandle = StartCoroutine("FindNextWanderPoint");
    }

    private void Update()
    {
        if (!target)
        {
            // 눈앞에 보이는걸 타겟으로 삼자
            if (fowReference.visibleTargets.Count > 0)
            {
                target = fowReference.visibleTargets[0];
                //Attack
                if (target)
                {
                    enemy = target.GetComponentInParent<Enemy>();
                    enemy.OnEnemyDeath += TargetDead;
                }
            }
        }
        //test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage();

        }
        if (isAlive)
        {
            // 1. Enemy Target 2. Attractive Position 3. Wander
            if (target!=null)
            {
                MoveToLocation(target.position, true);

                
                if (agent.remainingDistance <= agent.stoppingDistance + enemySafeDistance)
                {
                    currentState = CurrentState.Attack;
                    enemy.TakeDamage(transform);
                }
            }
            else if (attractivePosition != noPosition)
            {
                MoveToLocation(attractivePosition);
            }
        }
    }

    public void TargetDead()
    {
        recentlyAttracted = true;
        target = null;

    }

    /// <summary>
    /// Wander around if there's nothing to do
    /// </summary>
    public void Wander()
    {
        currentState = CurrentState.Wander;
        agent.speed = 2f;
        if (Vector3.Distance(transform.position, wanderPoint) < 2f)
        {
            recentlyAttracted = false;
            wanderPoint = RandomWanderPoint();
        }
        else
        {
            MoveToLocation(wanderPoint);
        }
    }

    IEnumerator WanderWithDelay()
    {
        agent.speed = 2f;
        while (true)
        {
            agent.destination = wanderPoint;
            while (Vector3.Distance(transform.position, wanderPoint) > 1.5f && attractivePosition == noPosition && recentlyAttracted == false)
            {
                currentState = CurrentState.Wander;
                agent.isStopped = false;

                yield return null;
            }
            if (attractivePosition == noPosition)
            {
                agent.isStopped = true;
                currentState = CurrentState.Idle;
                recentlyAttracted = false;
                wanderPoint = RandomWanderPoint();
            }
            WanderHandle = null;
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator FindNextWanderPoint()
    {
        if (!target)
        {
            yield return StartCoroutine("WanderWithDelay");
        }
        StopCoroutine("FindNextWanderPoint");
        yield return StartCoroutine("FindNextWanderPoint");
    }

    public Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * wanderRadius + transform.position;
        NavMeshHit navHit;
        // returns closest position of randompoint in navmesh
        NavMesh.SamplePosition(randomPoint, out navHit, wanderRadius, -1);
        Vector3 result = new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
        return result;
    }

    public void SetTargetPosition(Vector3 position)
    {
        attractivePosition = position;
    }

    // Have target position
    public void MoveToLocation(Vector3 target, bool isTarget = false)
    {
        if (isTarget)
        {
            agent.speed = 5f;
            currentState = CurrentState.Chase;
        }
        else
        {
            agent.speed = 2f;
            currentState = CurrentState.Wander;
        }

        if (Vector3.Distance(transform.position, target) > 1f)
        {
            agent.isStopped = false;
            agent.destination = target;
        }
        else
        {
            agent.isStopped = true;
            recentlyAttracted = true;
            attractivePosition = noPosition;
        }
    }





}
