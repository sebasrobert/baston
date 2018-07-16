using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public float TimeLeft;
    public Text TimeLeftText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TimeLeft -= Time.deltaTime;
        if(TimeLeft < 0)
        {
            TimeLeft = 0;
        }

        string minutes = Mathf.Floor(TimeLeft / 60).ToString("00");
        string seconds = Mathf.Floor(TimeLeft % 60).ToString("00");

        TimeLeftText.text = string.Format("{0}:{1}", minutes, seconds);
	}
}
