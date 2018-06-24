//using System;
using UnityEngine;

public class F3DBlinkColor : MonoBehaviour
{
    
    public Color ColorA, ColorB;
    public float Rate;
    public float lerpLength = 1f;
    public float randomLerpLengthModifier = 0f;
    public LerpFunctions lerpFunctions = LerpFunctions.PingPong;

    public enum LerpFunctions
    {
        PingPong,
        Repeat
    }

    //
    private SpriteRenderer _sprite;

    private float _time;

    private float _lerp;

    void Awake()
	{
	    _sprite = GetComponent<SpriteRenderer>();
        _lerp = lerpLength + Random.Range(0f, randomLerpLengthModifier);
	}

    // Update is called once per frame
    void Update()
    {
        if (!_sprite) return;
        _time += Time.deltaTime;


        switch (lerpFunctions)
        {
            case LerpFunctions.PingPong:
                _sprite.color = Color.Lerp(ColorA, ColorB, Mathf.PingPong(Time.time * Rate, _lerp));
                break;
            case LerpFunctions.Repeat:
                _sprite.color = Color.Lerp(ColorA, ColorB, Mathf.Repeat(Time.time * Rate, _lerp));
                break;
        }
    }
}
