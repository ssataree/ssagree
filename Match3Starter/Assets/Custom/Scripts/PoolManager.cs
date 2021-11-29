using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private List<PoolObject>[] poolObjects;

    public void Initialize(GameObject[] poolObjectPrefabs, int poolSize)
    {
        poolObjects = new List<PoolObject>[poolObjectPrefabs.Length];
        for (int i = 0; i < poolObjectPrefabs.Length; i++)
        {
            poolObjects[i] = new List<PoolObject>(poolSize);
            for (int sizeIdx = 0; sizeIdx < poolSize; sizeIdx++)
            {
                var gameObject = GameObject.Instantiate(poolObjectPrefabs[i]);
                var poolObject = gameObject.AddComponent<PoolObject>();
                poolObject.OnCreate();
                poolObjects[i].Add(poolObject);
            }
        }
    }

    public GameObject Get(int index, Vector3 position, Quaternion rotation)
    {
        for (int i = 0, count = poolObjects[index].Count; i < count; i++)
        {
            if (poolObjects[index][i].IsPlaying)
                continue;

            poolObjects[index][i].transform.SetPositionAndRotation(position, rotation);
            return poolObjects[index][i].OnPlay();
        }

        var gameObject = GameObject.Instantiate(poolObjects[index][0]);
        var poolObject = gameObject.GetComponent<PoolObject>();
        poolObject.OnCreate();
        poolObject.SetPositionAndRotation(position, rotation);
        poolObjects[index].Add(poolObject);
        
        return poolObject.OnPlay();
    }
}

public class PoolObject : MonoBehaviour
{
    public bool IsPlaying { get; set; }

    private GameObject go;

    private Transform tm;

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        tm.SetPositionAndRotation(position, rotation);
    }

    public void OnCreate()
    {
        go = gameObject;
        tm = transform;
        go.SetActive(false);
        IsPlaying = false;
    }

    public GameObject OnPlay()
    {
        IsPlaying = true;
        go.SetActive(true);
        return go;
    }

    public void Release(float t)
    {
        if (t > 0f)
            StartCoroutine(CorDeferDeactivate(t));
        else
        {
            IsPlaying = false;
            go.SetActive(false);
        }
    }

    private WaitForSeconds wfs;

    private IEnumerator CorDeferDeactivate(float t)
    {
        if (wfs == null)
            wfs = new WaitForSeconds(t);

        yield return wfs;
        IsPlaying = false;
        go.SetActive(false);
    }
}