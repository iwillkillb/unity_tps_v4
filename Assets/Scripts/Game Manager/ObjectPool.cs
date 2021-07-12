using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Singletone
    public static ObjectPool instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Object Pool.");
            return;
        }

        instance = this;
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public string name;
        public GameObject prefab;
        public int amount;
    }
    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;    // Main list.

    // Save to dictionary
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i=0; i<pool.amount; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.name, objectPool);
        }
    }

    public GameObject Call(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("Pool with name " + name + " doesn't exist.");
            return null;
        }

        // Get from dictionary.
        GameObject objectToSpawn = poolDictionary[name].Dequeue();

        // Initialization
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Interface ON
        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        // Return to dictionary.
        poolDictionary[name].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
