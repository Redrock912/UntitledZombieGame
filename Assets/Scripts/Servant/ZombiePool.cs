using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour 
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public Zombie prefab;
        public int size;
    }

    #region Singleton
    public static ZombiePool Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<Zombie>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<Zombie>>();

        foreach(Pool pool in pools)
        {
            Queue<Zombie> objectPool = new Queue<Zombie>();

            for(int i = 0; i < pool.size; i++)
            {
                Zombie obj = Instantiate(pool.prefab);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);

            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public Zombie SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag "+ tag + " doesn't exist");
            return null;
        }

        Zombie zombieToSpawn = poolDictionary[tag].Dequeue();

        zombieToSpawn.gameObject.SetActive(true);
        zombieToSpawn.transform.position = position;
        zombieToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = zombieToSpawn.GetComponent<IPooledObject>();

        if(pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }


        poolDictionary[tag].Enqueue(zombieToSpawn);

        return zombieToSpawn;
    }
}
