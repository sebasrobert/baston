using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectableSpawner : MonoBehaviour {

    public GameObject[] collectables = new GameObject[0];
    public Transform[] spawnPoints = new Transform[0];
    [Range(1, 4)]
    public int spawnCount;
    public float startWait;
    public float spawnWait;

    private HashSet<int> spawnPointExclusion = new HashSet<int>();

    void Awake()
    {
        // To avoid infine loop
        Assert.IsTrue(spawnCount <= spawnPoints.Length);
    }

    // Use this for initialization
    void Start () {
        if (collectables.Length > 0 && spawnPoints.Length > 0)
        {
            StartCoroutine(SpawnCollectables());
        }
	}
	
    IEnumerator SpawnCollectables () {
        yield return new WaitForSeconds(startWait);

        GameObject[] collectableObjects = new GameObject[spawnCount];
        while(true) {
            DestroyCollectables(collectableObjects);

            spawnPointExclusion.Clear();
            for (int i = 0; i < spawnCount; i++)
            {
                int randomSpawnPointIndex = getRandomIntExcept(0, spawnPoints.Length, spawnPointExclusion);
                spawnPointExclusion.Add(randomSpawnPointIndex);
                Transform spawnPoint = spawnPoints[randomSpawnPointIndex];
                GameObject collectable = collectables[Random.Range(0, collectables.Length)];
                collectableObjects[i] = Instantiate(collectable, spawnPoint.position, spawnPoint.rotation);
            }

            yield return new WaitForSeconds(spawnWait);
        }
	}

    private int getRandomIntExcept(int min, int max, HashSet<int> except)
    {
        int randomInt;
        do
        {
            randomInt = Random.Range(min, max);
        } while (except.Contains(randomInt));

        return randomInt;
    }

    private void DestroyCollectables(GameObject[] collectableObjects)
    {
        for (int i = 0; i < collectableObjects.Length; i++)
        {
            GameObject collectable = collectableObjects[i];
            if (collectable != null)
            {
                Destroy(collectable);
            }
        }
    }
}
