using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class animSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Image uiImage;

    public Sprite[] sprites;
    public float frameRate = 10f;
    public bool startAt0;
    public bool holdLastFrame;

    // Check this for UI animations that must continue
    // while Time.timeScale = 0, like Insurance Guy.
    public bool useUnscaledTime = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (sprites == null || sprites.Length == 0)
            yield break;

        int frame = Random.Range(0, sprites.Length);

        if (startAt0)
            frame = 0;

        while (true)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = sprites[frame];

            if (uiImage != null)
                uiImage.sprite = sprites[frame];

            if (holdLastFrame && frame == sprites.Length - 1)
                yield break;

            frame = (frame + 1) % sprites.Length;

            if (useUnscaledTime)
                yield return new WaitForSecondsRealtime(1f / frameRate);
            else
                yield return new WaitForSeconds(1f / frameRate);
        }
    }
}