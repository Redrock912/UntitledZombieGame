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

    public List<GameObject> zombiesInRange;
    private List<Transform> ammoBoxInMap = new List<Transform>();

    private Vector3 lastLocation;


    // Tutorial 

    public bool isEasyTutorial = false;
    public bool isTutorialBot = false;

    private void Awake()
    {
        GameObject[] ammoBoxObject = GameObject.FindGameObjectsWithTag("AmmoBox");

        foreach (var ammoBox in ammoBoxObject)
        {
            ammoBoxInMap.Add(ammoBox.transform);

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        lastLocation = transform.position;
        agent = GetComponent<NavMeshAgent>();
        zombiesInRange = new List<GameObject>();
        RefillAmmo();

    }

    public void RefillAmmo()
    {
        ammo = maxAmmo;
        print("Current Ammo is " + ammo);
        agent.SetDestination(lastLocation);

    }


    void FindAmmoBox()
    {
        float nearestAmmoBox = Mathf.Infinity;
        Transform result = null;
        foreach (var ammoBox in ammoBoxInMap)
        {

            float distance = Vector3.Distance(transform.position, ammoBox.position);


            if (distance < nearestAmmoBox)
            {
                result = ammoBox;
                nearestAmmoBox = distance;
            }

        }

        ammoBoxTarget = result;
        if (ammoBoxTarget != null)
        {
            agent.SetDestination(ammoBoxTarget.position);

            // save only once...

            agent.isStopped = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTutorialBot)
        {
            if (ammo <= 0)
            {
                FindAmmoBox();
            }
            else
            {
                ShootZombieInRange();
            }
        }
        else
        {
            if (!isEasyTutorial && ammo > 0)
            {
                ShootZombieInRange();
            }
        }
    }


    void ShootZombieInRange()
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

    void UpdateTarget()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject nearestZombie = null;
        foreach (var z in zombiesInRange)
        {
            Zombie zombieObject = z.GetComponent<Zombie>();
            Player playerObject = z.GetComponent<Player>();

            if ((zombieObject && zombieObject.isAlive) || (playerObject && playerObject.isAlive))
            {

                float currentDistance = Vector3.Distance(transform.position, z.transform.position);

                if (currentDistance < nearestDistance)
                {
                    nearestDistance = currentDistance;
                    nearestZombie = z;
                }
            }
        }

        if (nearestZombie != null && nearestDistance <= weaponRange)
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

            if (target.GetComponent<Zombie>())
            {
                target.GetComponent<Zombie>().TakeDamage(dir);
            }

            if (target.GetComponent<Player>())
            {
                target.GetComponent<Player>().TakeDamage(dir);
            }
            
            ammo--;
        }
    }


    void EraseZombie(GameObject selectedZombie)
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

        if ( other.tag == "Zombie")
        {            
            Zombie nearZombie = other.GetComponentInParent<Zombie>();
            if (nearZombie != null)
            {
                bool exists = false;

                foreach (var z in zombiesInRange)
                {

                    if (z.GetComponent<Zombie>() == nearZombie && nearZombie.isAlive == true)
                    {
                        exists = true;
                    }
                }

                if (!exists && nearZombie && nearZombie.isAlive == true)
                {
                    zombiesInRange.Add(nearZombie.gameObject);
                    nearZombie.OnZombieDeath += () => EraseZombie(nearZombie.gameObject);
                }
            }
        }

        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            
            if (player != null)
            {
                bool exists = false;

                foreach (var z in zombiesInRange)
                {

                    if (z.GetComponent<Player>() == player && player.isAlive)
                    {
                        exists = true;
                    }
                }

                if (!exists && player && player.isAlive == true)
                {
                    zombiesInRange.Add(player.gameObject);
                    player.OnPlayerDeath += () => EraseZombie(player.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ( other.tag == "Zombie")
        {        
            Zombie nearZombie = other.GetComponentInParent<Zombie>();
            if (nearZombie != null)
            {
                bool exists = false;

                foreach (var z in zombiesInRange)
                {
                    if (z.GetComponent<Zombie>() == nearZombie && nearZombie.isAlive == true)
                    {
                        exists = true;
                    }
                }

                if (exists && nearZombie)
                {
                    zombiesInRange.Remove(nearZombie.gameObject);
                    nearZombie.OnZombieDeath -= () => EraseZombie(nearZombie.gameObject);
                }
            }
        }

        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
        
            if (player != null)
            {
                bool exists = false;

                foreach (var z in zombiesInRange)
                {
                    if (z.GetComponent<Player>() == player && player.isAlive)
                    {
                        exists = true;
                    }
                }

                if (exists && player)
                {
                    zombiesInRange.Remove(player.gameObject);
                    player.OnPlayerDeath -= () => EraseZombie(player.gameObject);
                }

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
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath();
        }


        isAlive = false;
        Destroy(gameObject);
    }

}
