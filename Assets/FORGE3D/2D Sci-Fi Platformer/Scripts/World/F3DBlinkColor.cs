using System;
using UnityEngine;
using System.Collections;

public class F3DBlinkColor : MonoBehaviour
{
    
    public Color ColorA, ColorB;
    public float Rate;
    public float lerpLength = 1f;
    public LerpFunctions lerpFunctions = LerpFunctions.PingPong;

    public enum LerpFunctions
    {
        PingPong,
        Repeat
    }

    //
    private SpriteRenderer _sprite;

    private float _time;

    void Awake()
	{
	    _sprite = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
        if (!_sprite) return;
        _time += Time.deltaTime;
        switch (lerpFunctions)
        {
            case LerpFunctions.PingPong:
                _sprite.color = Color.Lerp(ColorA, ColorB, Mathf.PingPong(Time.time * Rate, lerpLength));
                break;
            case LerpFunctions.Repeat:
                _sprite.color = Color.Lerp(ColorA, ColorB, Mathf.Repeat(Time.time * Rate, lerpLength));
                break;
        }
    }
}
