using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameEvents;

public class DeathMatchGameController : GameController {

    public GameObject PlayerPrefab;
    [Range(1, 4)]
    public int NumberOfPlayers;
    public Transform[] SpawnPoints = new Transform[4];
    public int NumberOfKillsToWin;
    public int PointsForKill;
    public int PointsForHit;
    public float WaitTimeBeforeRespawn;
    public GameObject GameOverObject;

    private Timer Timer;
    private int MaxKills;
    private bool GameOver;

    private void Awake()
    {
        Timer = GetComponent<Timer>();
        EventManager.StartListening<WeaponHitPlayerEvent>(OnWeaponHitPlayerEvent);
        EventManager.StartListening<PlayerDieEvent>(OnPlayerDieEvent);
        Players = new GameObject[NumberOfPlayers];
        SpawnAllPlayers();
        MaxKills = 0;
        GameOver = false;
        GameOverObject.SetActive(false);
    }

    void Start () 
    {        
	}

    private void Update()
    {
        if(Timer.TimeLeft < float.Epsilon || MaxKills >= NumberOfKillsToWin) {
            MakeGameOver();
        }       

        if (GameOver && Input.GetKeyDown(KeyCode.R))
        {            
            RestartGame();
        }
    }

    public void OnWeaponHitPlayerEvent(WeaponHitPlayerEvent gameEvent)
    {
        if(GameOver)
        {
            return;
        }

        UpdateScoreForHit(gameEvent.Shooter, PointsForHit);
    }

    public void OnPlayerDieEvent(PlayerDieEvent gameEvent)
    {
        if (GameOver)
        {
            return;
        }

        UpdateScoreForKill(gameEvent.Killer, gameEvent.Dead, PointsForKill);

        StartCoroutine(RespawnPlayer(gameEvent.Dead));
    }

    private void SpawnAllPlayers() 
    {        
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            Transform spawnPoint = SpawnPoints[i];
            string playerName = "Player" + (i + 1);
            GameObject player = Instantiate(PlayerPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            Players[i] = player;
            player.name = playerName;

            F3DCharacter playerCharacter = player.GetComponent<F3DCharacter>();
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

    private void MakeGameOver()
    {
        GameOver = true;
        GameOverObject.SetActive(true);
        StartCoroutine(PauseSceneSmoothly());
    }

    private IEnumerator PauseSceneSmoothly()
    {
        while (Time.timeScale > float.Epsilon)
        {
            yield return new WaitForSeconds(0.1f);

            var newTimeScale = Time.timeScale - 0.03f;
            if(newTimeScale < float.Epsilon)
            {
                newTimeScale = 0f;
            }

            Time.timeScale = newTimeScale;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    private void UpdateScoreForHit(GameObject player, int points)
    {
        PlayerScore playerScore = GetPlayerScore(player);
        playerScore.Points += points;
    }

    private void UpdateScoreForKill(GameObject killer, GameObject dead, int points)
    {
        PlayerScore killerPlayerScore = GetPlayerScore(killer);
        killerPlayerScore.Points += points;
        killerPlayerScore.NumberOfKills++;
        if(killerPlayerScore.NumberOfKills > MaxKills)
        {
            MaxKills = killerPlayerScore.NumberOfKills;
        }

        PlayerScore killedPlayerScore = GetPlayerScore(dead);
        killedPlayerScore.NumberOfDeaths++;
    }

    private PlayerScore GetPlayerScore(GameObject player)
    {
        return player.GetComponent<PlayerScore>();
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        yield return new WaitForSeconds(WaitTimeBeforeRespawn);

        F3DCharacter character = player.GetComponent<F3DCharacter>();
        character.RespawnAtInitialPosition();
    }
}
