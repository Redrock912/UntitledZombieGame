using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public event System.Action OnEnemyDeath;
    public float lookRadius = 10f;

    private int ammo;
    public int maxAmmo;

    float weaponRange = 10f;
    public float fireRate = 0.2f;
    private float gunCoolDown = 0f;

    public float turnTime = 10f;
    public float viewAngle;
    private NavMeshAgent agent;
    private Transform target;
    private Transform ammoBoxTarget;
    public Light spotLight;

    private Vector3 lastPosition;

    public bool isAlive = true;

    List<Zombie> zombiesInRange;
    private List<Transform> ammoBoxInMap = new List<Transform>();

    private Vector3 lastLocation;


    private void Awake()
    {
        GameObject[] ammoBoxObject = GameObject.FindGameObjectsWithTag("AmmoBox");

        foreach(var ammoBox in ammoBoxObject)
        {
            ammoBoxInMap.Add(ammoBox.transform);
           
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        lastLocation = transform.position;
        agent = GetComponent<NavMeshAgent>();
        zombiesInRange = new List<Zombie>();
        RefillAmmo();
        print(ammoBoxInMap.Count);
    }

    public void RefillAmmo()
    {
        ammo = maxAmmo;
        print("Current Ammo is " + ammo);
        agent.SetDestination(lastLocation);
        
    }


    void MakeDecision()
    {
        if(ammo <= 0)
        {
            FindAmmoBox();
            
        }
        else
        {
            CheckZombieInRange(5.0f);
        }
    }

    void CheckZombieInRange(float range)
    {
        
    }

    void FindAmmoBox()
    {
        float nearestAmmoBox = Mathf.Infinity;
        Transform result = null;
        foreach(var ammoBox in ammoBoxInMap)
        {
            
            float distance = Vector3.Distance(transform.position, ammoBox.position);
            

            if(distance < nearestAmmoBox)
            {
                result = ammoBox;
                nearestAmmoBox = distance;
            }
            
        }

        ammoBoxTarget = result;
    }

    // Update is called once per frame
    void Update()
    {

        if (ammo <= 0)
        {
            FindAmmoBox();
            if(ammoBoxTarget != null)
            {
                agent.SetDestination(ammoBoxTarget.position);

                // save only once...
                
                agent.isStopped = false;
            }
        }
        else
        {
            if (zombiesInRange.Count > 0)
            {
                UpdateTarget();

                if (gunCoolDown <= 0f)
                {

                    Shoot();
                    gunCoolDown = 1f / fireRate;

                }
            }


            if (gunCoolDown > 0)
            {
                gunCoolDown -= Time.deltaTime;
            }
        }

       
        
        
    }

   


    void UpdateTarget()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject nearestZombie = null;
        foreach(var z in zombiesInRange)
        {
            if (z.isAlive)
            {
                float currentDistance = Vector3.Distance(transform.position, z.transform.position);

                if (currentDistance < nearestDistance)
                {
                    nearestDistance = currentDistance;
                    nearestZombie = z.gameObject;

                }
            }
        }

        if(nearestZombie != null && nearestDistance <= weaponRange)
        {
            target = nearestZombie.transform;
            FaceTarget();
            
        }
        else
        {
            target = null;
        }     
    }

    void FaceTarget()
    {
        Vector3 dir = target.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot(Transform hitTarget = null)
    {
        if (hitTarget)
        {
            target = hitTarget;
        }

        if (target)
        {

            Vector3 dir = (transform.position - target.position).normalized;
            dir *= -1;
            target.GetComponent<Zombie>().TakeDamage(dir);
            ammo--;
        }
    }


    void EraseZombie(Zombie selectedZombie)
    {
        
        zombiesInRange.Remove(selectedZombie);


    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    private void OnTriggerEnter(Collider other)
    {

        Zombie nearZombie = other.GetComponentInParent<Zombie>(); 
        if (nearZombie != null)
        {
            bool exists = false;

            foreach(var z in zombiesInRange)
            {
                
                if (z == nearZombie && z.isAlive ==true)
                {
                    exists = true;
                }
            }


            if (!exists)
            {
                zombiesInRange.Add(nearZombie);
                nearZombie.OnZombieDeath += ()=>EraseZombie(nearZombie);
            }         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Zombie nearZombie = other.GetComponentInParent<Zombie>();
        if (nearZombie != null)
        {
            bool exists = false;

            foreach (var z in zombiesInRange)
            {

                if (z == nearZombie && z.isAlive == true)
                {
                    exists = true;
                }
            }


            if (exists)
            {
                zombiesInRange.Remove(nearZombie);
                nearZombie.OnZombieDeath -= () => EraseZombie(nearZombie);
            }
        }
    }

    public void TakeDamage(Transform target)
    {
        //reactive shot
        if (ammo > 0)
        {
            Shoot(target);
        }
        else
        {
            Die();
        }
        
    }

    void Die()
    {
        if(OnEnemyDeath!= null)
        {
            OnEnemyDeath();
        }


        isAlive = false;
        Destroy(gameObject);
    }

}
