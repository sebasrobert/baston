using System;
using UnityEngine;
using System.Collections;

public class F3DCharacterAvatar : MonoBehaviour
{
    public int CharacterId;
    public SpriteRenderer Head;
    public SpriteRenderer Body;

    //
    [Serializable]
    public class CharacterArmature
    {
        public Sprite Head;
        public Sprite Body;
        public Sprite Hand1;
        public Sprite Hand2;
        public Sprite Hand3;
        public Sprite Hand4;
    }

    public CharacterArmature[] Characters;

    //
    private void Awake()
    {
        SwitchCharacter(CharacterId);
    }

    private void SwitchCharacter(int id)
    {
        if (Head == null) return;
        if (Body == null) return;
        if (Characters == null || id >= Characters.Length || id < 0) return;
        Head.sprite = Characters[CharacterId].Head;
        Body.sprite = Characters[CharacterId].Body;
    }
}