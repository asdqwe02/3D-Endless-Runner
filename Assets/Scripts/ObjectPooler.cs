using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = false;
}
public class ObjectPooler : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;
    public static ObjectPooler instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool, transform);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }
    public GameObject GetPooledObject(string tag)
    {
        foreach (GameObject pObject in pooledObjects)
        {
            if (pObject.activeInHierarchy == false && pObject.tag == tag)
                return pObject;
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
    public int ActivePooledObjectCount(string tag)
    {
        int c = 0;
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
                c++;
        }
        return c;
    }
    public List<GameObject> GetActivePoolObjects(string tag)
    {
        List<GameObject> activeObjectts = new List<GameObject>();
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
                activeObjectts.Add(pooledObjects[i]);
        }
        return activeObjectts; // if null => error
    }
    public bool DeactivatePooledObject(GameObject obj) // is this necessary ? doesn't seem like it
    {
        if (pooledObjects.Contains(obj))
        {
            Vector3 pos = obj.transform.localPosition;
            pos.x = 0; pos.z = 0; // mostly keep the y pos maybe should make another function for this
            obj.transform.parent = transform;
            obj.transform.localPosition = pos;
            obj.SetActive(false);
            return true; // deactivate successful
        }
        return false; // deactivate not successful either not exist in list or some error
    }

    public void RemoveAllObjectWithTag(string tag)
    {
        foreach (var obj in pooledObjects)
        {
            if (obj.tag == tag)
            {
                Vector3 pos = obj.transform.localPosition;
                pos.x = 0; pos.z = 0; // mostly keep the y pos maybe should make another function for this
                obj.transform.parent = transform;
                obj.transform.localPosition = pos;
                obj.SetActive(false);
            }
        }
    }
}