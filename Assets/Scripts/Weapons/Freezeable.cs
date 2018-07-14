using System.Collections;
using UnityEngine;

public class Freezeable : MonoBehaviour {

    public GameObject FreezeEffect;
    public float MaxFreezeDuration;
    [HideInInspector]
    public bool Freezed { get; protected set; }

    private float TimeLeftFreezing;

	// Use this for initialization
	public void Freeze(float freezeDuration) {
        TimeLeftFreezing = Mathf.Min(freezeDuration, MaxFreezeDuration);

        if(!Freezed)
        {
            StartCoroutine(Freezing());
        }
	}
	
    void Start()
    {
        FreezeEffect.SetActive(false);
    }

	void Update () {
        TimeLeftFreezing = Mathf.Max(0f, TimeLeftFreezing - Time.deltaTime);
	}

    private IEnumerator Freezing()
    {
        Freezed = true;

        while(TimeLeftFreezing > 0f)
        {
            FreezeEffect.SetActive(true );
            yield return new WaitForSeconds(TimeLeftFreezing);
        }

        FreezeEffect.SetActive(false);
        Freezed = false;
    }
}
