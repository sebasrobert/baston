using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    public GameObject[] PlayerScoreCards;
    private GameController GameController;

    void Awake()
    {
        GameController = GameObject.Find("GameController").GetComponent<GameController>();
        DisableScoreCards();
    }

    void Start() {
        StartCoroutine(UpdateScores());
    }

    private void DisableScoreCards()
    {
        for (int i = 0; i < PlayerScoreCards.Length; i++)
        {
            PlayerScoreCards[i].SetActive(false);
        }
    }

    private IEnumerator UpdateScores()
    {
        while (true)
        {
            GameObject[] players = GameController.Players;

            for (int i = 0; i < players.Length; i++)
            {
                PlayerScore playerScore = GetPlayerScore(players[i]);

                PlayerScoreCards[i].SetActive(true);
                PlayerScoreCards[i].transform.Find("ScoreValue").GetComponent<Text>().text = playerScore.Points.ToString();
                PlayerScoreCards[i].transform.Find("KDValue").GetComponent<Text>().text = playerScore.NumberOfKills.ToString() + "/" + playerScore.NumberOfDeaths.ToString();
                PlayerScoreCards[i].transform.Find("Name").GetComponent<Text>().text = players[i].name;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private PlayerScore GetPlayerScore(GameObject player)
    {
        return player.GetComponent<PlayerScore>();
    }

}
