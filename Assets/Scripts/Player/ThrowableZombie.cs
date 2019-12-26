using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// spawn zombie when thrown this object
public class ThrowableZombie : ItemLauncher
{

    Zombie zombieObject;
    bool isSpawned = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (isLaunched && isSpawned)
        {
            item.Sleep();


            zombieObject = ZombiePool.Instance.SpawnFromPool("Zombie", item.transform.position, Quaternion.identity);


            if (zombieObject != null)
            {
                isSpawned = false;
            }

            print("Spawned zombie position is " + transform.position);
            
            Destroy(gameObject, 0.5f);
        }
       
    }
}
