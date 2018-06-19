using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour {

    public GameObject[] collectables = new GameObject[0];
    public Transform[] spawnPoints = new Transform[0];
    public float startWait;
    public float spawnWait;

	// Use this for initialization
	void Start () {
        if (collectables.Length > 0 && spawnPoints.Length > 0)
        {
            StartCoroutine(SpawnCollectables());
        }
	}
	
    IEnumerator SpawnCollectables () {
        yield return new WaitForSeconds(startWait);

        GameObject lastSpawnCollectable = null;
        while(true) {
            if(lastSpawnCollectable != null) {
                Destroy(lastSpawnCollectable); 
            }
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject collectable = collectables[Random.Range(0, collectables.Length)];
            lastSpawnCollectable = Instantiate(collectable, spawnPoint.position, spawnPoint.rotation);

            yield return new WaitForSeconds(spawnWait);
        }
	}
}
