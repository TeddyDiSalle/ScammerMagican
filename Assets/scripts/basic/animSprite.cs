using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public float frameRate = 10f;
    public bool startAt0;
    public bool holdLastFrame;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        int frame = Random.Range(0,sprites.Length);
        if (startAt0)
            frame = 0;

        while (true)
        {
            spriteRenderer.sprite = sprites[frame];

            if (holdLastFrame&&frame==sprites.Length-1)
                yield break;

            frame = (frame + 1) % sprites.Length;

            yield return new WaitForSeconds(1f / frameRate);
        }
    }
}
