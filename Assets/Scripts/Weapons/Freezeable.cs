using System.Collections;
using UnityEngine;

public class Freezeable : MonoBehaviour {

    public GameObject FreezeEffect;
    public float MaxFreezeDuration;
    public float StopEffectDelay;
    [HideInInspector]
    public bool Freezed { get; protected set; }

    ParticleSystem[] AllParticleSytems;
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
        AllParticleSytems = FreezeEffect.GetComponentsInChildren<ParticleSystem>();
    }

	void Update () {
        TimeLeftFreezing = Mathf.Max(0f, TimeLeftFreezing - Time.deltaTime);
	}

    private IEnumerator Freezing()
    {
        Freezed = true;
        bool particuleSysmtesPlaying = false;
        while(TimeLeftFreezing > StopEffectDelay)
        {
            if (!particuleSysmtesPlaying)
            {
                PlayAllParticuleSystems();
                particuleSysmtesPlaying = true;
            }
            yield return new WaitForSeconds(TimeLeftFreezing - StopEffectDelay);
        }

        StopAllParticuleSystems();

        yield return new WaitForSeconds(StopEffectDelay);

        Freezed = false;
    }

    private void PlayAllParticuleSystems()
    {
        foreach(ParticleSystem particuleSystem in AllParticleSytems)
        {
            particuleSystem.Play();
        }
    }

    private void StopAllParticuleSystems()
    {
        foreach (ParticleSystem particuleSystem in AllParticleSytems)
        {
            particuleSystem.Stop();
        }
    }
}
