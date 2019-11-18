using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Zombie : MonoBehaviour
{
    
    public event System.Action OnZombieDeath;
    public ZombieAnimation zombieAnimation;

    NavMeshAgent agent;

    [Header("detail")]
    public float wanderRadius =7.0f;
    public float impactMultiplier =200f;
    public float viewAngle = 80f;
    public float wanderTimer = 2.0f;
    


    //Position 
    private Vector3 wanderPoint;
    Vector3 noPosition = new Vector3(0, -10, 0);
    Transform target;
    Vector3 attractivePosition;

    public FieldOfView fowReference;

    [HideInInspector]
    public bool isAlive = true;
    public bool isWandering = false;

    public CurrentState currentState;

    GameObject[] potentialTargets;

    public enum CurrentState
    {
        Idle, Wander, Chase, Attack
    }

    public void TakeDamage(Vector3 forceDirection = new Vector3())
    {

        
       
        print("In Take");
        if(OnZombieDeath!= null)
        {
            print("Is not null");
            OnZombieDeath();
            isAlive = false;
            OnZombieDeath = null;
            // 5 = chest part
            zombieAnimation.ragdollParts[5].AddForce(forceDirection * impactMultiplier , ForceMode.Impulse);
            Destroy(gameObject, 2);
            
        }

        
    }

    public void SearchForEnemy()
    {
        float nearest = Mathf.Infinity;

        //foreach(var enemy in potentialTargets)
        //{
            
        //}
        
    }

    private void Start()
    {
        currentState = CurrentState.Idle;
        attractivePosition = noPosition;

        
        wanderPoint = RandomWanderPoint();
        agent = GetComponent<NavMeshAgent>();
        
        potentialTargets = GameObject.FindGameObjectsWithTag("Enemy");
        //StartCoroutine("FindNextWanderPoint");
    }


    private void Update()
    {
        if (!target)
        {
            // 눈앞에 보이는걸 타겟으로 삼자
            if(fowReference.visibleTargets.Count > 0)
            {
                target = fowReference.visibleTargets[0];
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
            if (target)
            {
                MoveToLocation(target.position, true);
            }
            else if (attractivePosition != noPosition)
            {
                print(agent.isStopped);
                MoveToLocation(attractivePosition);


            }
            else
            {
                print("asdf");
                print(wanderPoint);
                Wander();
                // StartCoroutine("WanderWithDelay");
                
            }
        }
        
    }

    /// <summary>
    /// Wander around if there's nothing to do
    /// </summary>
   

    public void Wander()
    {
        currentState = CurrentState.Wander;
        agent.speed = 0.5f;
        if(Vector3.Distance(transform.position, wanderPoint) < 2f)
        {
            wanderPoint = RandomWanderPoint();
        }
        else
        {
            agent.SetDestination(wanderPoint);
            
            
        }
    }

   



    IEnumerator WanderWithDelay()
    {
        yield return new WaitForSeconds(wanderTimer);
        agent.isStopped = true;
        currentState = CurrentState.Wander;
        agent.speed = 0.5f;

        while(Vector3.Distance(transform.position,wanderPoint) > 1.5f)
        {
            isWandering = true;
            agent.isStopped = false;
            agent.SetDestination(wanderPoint);

            yield return null;
        }
        agent.isStopped = true;
        isWandering = false;
        attractivePosition = RandomWanderPoint();
        
      

    }

    IEnumerator FindNextWanderPoint()
    {
        if (!target || isWandering == false)
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
        print(result);
        return result;
    }

    public void SetTargetPosition(Vector3 position)
    {

        attractivePosition = position;
        
    }

    // Have target position
    public void MoveToLocation(Vector3 target ,bool isTarget=false)
    {
        
        if (isTarget)
        {
            agent.speed = 1f;
            currentState = CurrentState.Chase;
        }
        else
        {
            agent.speed = .5f;
            currentState = CurrentState.Wander;
        }

        if(Vector3.Distance(transform.position, target) > agent.stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target);
            
        }
        else
        {
            agent.isStopped = true;
            attractivePosition = noPosition;
            
        }
    

    }

    

   

}
