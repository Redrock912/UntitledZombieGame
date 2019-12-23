using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ZombieAnimation : MonoBehaviour
{
    const float locomotionSmoothTime = .1f;
    public List<Rigidbody> ragdollParts;
    Animator animator;
    NavMeshAgent agent;
    Zombie zombie;
    //public Transform target;
    float sliderValue;

    public float customSpeedMultiplier;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        zombie = GetComponent<Zombie>();

        

        zombie.OnZombieDeath += Death;
        

        foreach (var ragdoll in ragdollParts)
        {
            ragdoll.isKinematic = true;
        }

        agent.angularSpeed = 1500f;
    }

    private void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        float angleSpeed = Vector3.Angle(transform.forward, agent.velocity.normalized);
        Vector3 cross = Vector3.Cross(transform.forward, agent.velocity.normalized);
        if(cross.y < 0)
        {
            angleSpeed *= -1;
        }
        animator.SetFloat("speed", speedPercent, locomotionSmoothTime, 2.0f * Time.deltaTime);
        animator.SetFloat("angularspeed", angleSpeed, locomotionSmoothTime, Time.deltaTime);
        CheckZombieState();

    }

    void CheckZombieState()
    {
        switch (zombie.currentState)
        {
            case Zombie.CurrentState.Idle:
                animator.SetFloat("speedMultiplier", 0f);
                
                break;
            case Zombie.CurrentState.Wander:
                animator.SetFloat("speedMultiplier",2f);

                break;
            case Zombie.CurrentState.Chase:
                animator.SetFloat("speedMultiplier", 5f);
                break;
            case Zombie.CurrentState.Attack:
                animator.SetBool("attack", true);
                
                break;
            
        }
    }


    public void Death()
    {
        agent.enabled = false;
        animator.enabled = false;
        foreach (var ragdoll in ragdollParts)
        {
            ragdoll.isKinematic = false;
        }

        zombie.OnZombieDeath -= Death;
    }


 
}
