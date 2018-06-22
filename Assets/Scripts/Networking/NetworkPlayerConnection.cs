using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {

    public GameObject PlayerPrefab;

	void Start () {
        if (isServer) {
            GameObject playerGameObject = Instantiate(PlayerPrefab, gameObject.transform.position, Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(playerGameObject, connectionToClient);
        }
	}
	
	void Update () {
		
	}
}