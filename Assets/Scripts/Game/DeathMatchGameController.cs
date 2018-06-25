using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class DeathMatchGameController : MonoBehaviour {

    public GameObject PlayerPrefab;
    [Range(1, 4)]
    public int NumberOfPlayers;
    public Transform[] SpawnPoints = new Transform[4];
    public int GameDuration;
    public int NumberOfKillsToWin;

    private GameObject[] Players=  new GameObject[4];

	// Use this for initialization
	void Start () {
        EventManager.StartListening<WeaponHitPlayerEvent>(OnWeaponHitPlayerEvent);
        EventManager.StartListening<PlayerDieEvent>(OnPlayerDieEvent);

        SpawnAllPlayers();
	}
	
    public void OnWeaponHitPlayerEvent(WeaponHitPlayerEvent gameEvent)
    {
        Debug.Log("OnWeaponHitPlayerEvent player " + gameEvent.Source.name + ", hit player " + gameEvent.Target.name);
    }

    public void OnPlayerDieEvent(PlayerDieEvent gameEvent)
    {
        Debug.Log("OnPlayerDieEvent " + gameEvent.Player.name);
    }

    private void SpawnAllPlayers() 
    {
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            Transform spawnPoint = SpawnPoints[i];
            Players[i] = Instantiate(PlayerPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            Players[i].name = "Player" + (i + 1);

            F3DCharacter playerCharacter = Players[i].GetComponent<F3DCharacter>();
            if (i == 0)
            {
                playerCharacter.inputControllerType = F3DCharacter.InputType.KEYBOAD_MOUSE;
                playerCharacter.inputControllerName = "";
            }
            else 
            {
                playerCharacter.inputControllerType = F3DCharacter.InputType.GAMING_CONTROLLER;
                playerCharacter.inputControllerName = "Gamepad" + i;
            }
        }
    }

    private void GameOver()
    {        
    }
}
