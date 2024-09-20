using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    int initialCount = 1;
    [System.Serializable]
    public class pool
    {
        public levelTypes type;
        public GameObject[] prefab;
        public int count;
    }
    public List<pool> pools;
    private Dictionary<levelTypes, List<GameObject>> poolDictionary = new Dictionary<levelTypes, List<GameObject>>();

    public static Action coinConsumedEvent;

    private void Start()
    {
        KeepAllItemInPool();
        InitialLevel();
    }

    void InitialLevel()
    {
        for (int i = 0; i < 4; i++)
        {
            SpawnFromPool(levelTypes.Easy);
        }
        UIManager.Instance.loadingPanel.SetActive(false);
    }
    private void SpawnFromPool(levelTypes tag)
    {
        if (!poolDictionary.ContainsKey(tag)) return;
        List<GameObject> objToSpawnList = poolDictionary[tag];

        int value = UnityEngine.Random.Range(0, objToSpawnList.Count);
        GameObject objToSpawn = objToSpawnList[value];
        objToSpawnList.RemoveAt(value);
        objToSpawn.SetActive(true);
        if (PlayerController.Instance.canSpawnCoin)
        {
            objToSpawn.GetComponent<PlatformProperty>().ActivateCoin();
            PlayerController.Instance.canSpawnCoin = false;
            PlayerController.Instance.lastCoinPlaformValue = 0;
        }
        else
        {
            objToSpawn.GetComponent<PlatformProperty>().DeactivateCoin();
        }
        objToSpawn.transform.position = new Vector3(0, 0, initialCount * 164f+54f);
        initialCount++;
    }
    public void KeepAllItemInPool()
    {
        foreach (var item in pools)
        {
            List<GameObject> objToSpawnList=new List<GameObject>();
            for (int i = 0; i < item.count; i++)
            {
                foreach (var g in item.prefab)
                {
                    GameObject obj = Instantiate(g, new Vector3(0, 0, -1000f), Quaternion.identity);
                    obj.GetComponent<PlatformProperty>().type = item.type;
                    objToSpawnList.Add(obj);
                    obj.SetActive(false);
                }
            }
            poolDictionary.Add(item.type, objToSpawnList);
        }
    }

    public void AddToPool(GameObject obj, levelTypes tag)
    {
        obj.SetActive(false);
        obj.transform.position = new Vector3(0, 0, -1000f);
        poolDictionary[tag].Add(obj);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Regeneration"))
        {
            if (PlayerController.Instance.lastTrackScore > Random.Range(7,15))
            {
                PlayerController.Instance.lastTrackScore = 0;
                SpawnFromPool(levelTypes.Medium);
            }
            else
            {
                SpawnFromPool(levelTypes.Easy);
            }

            if(PlayerController.Instance.lastCoinPlaformValue > Random.Range(2, 4))
            {
                PlayerController.Instance.canSpawnCoin = true;
            }
        }
        if (other.GetComponent<IConsumables>() != null)
        {
            if (other.GetComponent<Coin>() != null)
            {
                coinConsumedEvent?.Invoke();
            }
            else
            {
                Vector3 pos = other.ClosestPoint(transform.position);
                PlayerController.Instance.explosion.transform.position = pos;
            }

            other.GetComponent<IConsumables>().ConsumedEvent();
        }
    }
}
