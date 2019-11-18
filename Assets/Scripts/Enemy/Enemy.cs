using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float lookRadius = 10f;

    int ammo;
    public int maxAmmo;

    float weaponRange = 10f;
    public float fireRate = 1f;
    private float gunCoolDown = 0f;

    public float turnTime = 10f;
    public float viewAngle;
    Transform target;
    public Light spotLight;


    List<Zombie> zombiesInRange;


    // Start is called before the first frame update
    void Start()
    {
        zombiesInRange = new List<Zombie>();
        
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

    }

    // Update is called once per frame
    void Update()
    {
       if(zombiesInRange.Count > 0)
        {
            UpdateTarget();

            if(gunCoolDown <= 0f)
            {
                //Shoot();
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
        foreach(var z in zombiesInRange)
        {
            float currentDistance = Vector3.Distance(transform.position, z.transform.position);

            if(currentDistance < nearestDistance)
            {
                nearestDistance = currentDistance;
                nearestZombie = z.gameObject;

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

    void Shoot()
    {
        Vector3 dir = (transform.position - target.position).normalized;
        dir *= -1;
        target.GetComponent<Zombie>().TakeDamage(dir);
    }


    void EraseZombie(Zombie selectedZombie)
    {
        print("Erased Zombie");
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


}
