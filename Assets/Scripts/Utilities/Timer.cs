using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    public float TimeLeft;

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
	}
}
