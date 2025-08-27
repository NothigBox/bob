using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool<T> : Object where T : PoolObject
{
    private T objectPrefab;

    private List<T> allObjects;
    private List<T> availableObjects;
    private List<T> unavailableObjects;

    public List<T> AllObjects
    {
        get
        {
            if (allObjects == null)
            {
                allObjects = new List<T>();
            }

            return allObjects;
        }
        set
        {
            allObjects = value;
        }
    }
    public List<T> AvailableObjects
    {
        get
        {
            if (availableObjects == null)
            {
                availableObjects = new List<T>();
            }

            return availableObjects;
        }
        set
        {
            availableObjects = value;
        }
    }
    public List<T> UnavailableObjects
    {
        get
        {
            if (unavailableObjects == null)
            {
                unavailableObjects = new List<T>();
            }

            return unavailableObjects;
        }
        set
        {
            unavailableObjects = value;
        }
    }

    public ObjectsPool(T prefab)
    {
        objectPrefab = prefab;
    }

    private T GetNewObject(Vector3 position)
    {
        T newObject = Instantiate(objectPrefab, position, Quaternion.identity);

        newObject.OnDisabled += ReceiptObject;

        AllObjects.Add(newObject);
        UnavailableObjects.Add(newObject);

        return newObject;
    }

    public T GetObject(Vector3 position)
    {
        T thisObject = default;

        if (AvailableObjects.Count <= 0)
        {
            thisObject = GetNewObject(position);
        }
        else
        {
            thisObject = AvailableObjects[0];

            AvailableObjects.RemoveAt(0);
            UnavailableObjects.Add(thisObject);

            thisObject.transform.position = position;
        }

        thisObject.gameObject.SetActive(true);

        return thisObject;
    }

    private void ReceiptObject(PoolObject thisObject)
    {
        UnavailableObjects.Remove(thisObject as T);
        AvailableObjects.Add(thisObject as T);
    }
}
