using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{

    public event System.Action OnBoxDestroyed;

    public float refillDistance;
    

    private int ammo;
    public int maxAmmo;

    List<Enemy> enemies = new List<Enemy>();

    private void OnTriggerEnter(Collider other)
    {
        
    }


    private void Awake()
    {

        
        GameObject[] enemiesObject = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach(var enemyObject in enemiesObject)
        {
            enemies.Add(enemyObject.GetComponent<Enemy>());
        }
    }


    public void CheckForDistanceAndBlock()
    {
        
        foreach(var enemy in enemies)
        {
            print(enemies.Count);
            print(enemy);

           if(Vector3.Distance(transform.position, enemy.transform.position) < refillDistance)
           {
                enemy.RefillAmmo();
           }
        }
        
    }
    
    private void Update()
    {
        CheckForDistanceAndBlock();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, refillDistance);
    }

    public void BoxDestroyed()
    {
        if(OnBoxDestroyed != null)
        {
            OnBoxDestroyed();
        }

        Destroy(gameObject);
    }

}
