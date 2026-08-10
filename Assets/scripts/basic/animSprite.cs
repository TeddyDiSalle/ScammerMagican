using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class animSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Image uiImage;
    private Coroutine animateRoutine;

    public Sprite[] sprites;
    public float frameRate = 10f;
    public bool startAt0;
    public bool holdLastFrame;
    public bool useUnscaledTime;

    private Sprite[] defaultSprites;
    private float defaultFrameRate;
    private bool defaultStartAt0;
    private bool defaultHoldLastFrame;
    private bool defaultsCaptured;

    // Used to keep reaction animations (which may have different source
    // resolutions) the same apparent world size as the original sprite.
    private Vector3 defaultLocalScale;
    private Vector2 defaultWorldSpriteSize;
    private bool hasDefaultWorldSpriteSize;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();

        defaultLocalScale = transform.localScale;

        CaptureDefaults();
        CaptureDefaultWorldSpriteSize();
    }

    void Start()
    {
        RestartAnimation();
    }

    void OnEnable()
    {
        if (spriteRenderer != null || uiImage != null)
        {
            if (sprites != null && sprites.Length > 0)
                RestartAnimation();
        }
    }

    void OnDisable()
    {
        if (animateRoutine != null)
        {
            StopCoroutine(animateRoutine);
            animateRoutine = null;
        }
    }

    private void CaptureDefaults()
    {
        if (defaultsCaptured)
            return;

        if (sprites != null)
        {
            defaultSprites = new Sprite[sprites.Length];
            Array.Copy(sprites, defaultSprites, sprites.Length);
        }

        defaultFrameRate = frameRate;
        defaultStartAt0 = startAt0;
        defaultHoldLastFrame = holdLastFrame;
        defaultsCaptured = true;
    }

    private void CaptureDefaultWorldSpriteSize()
    {
        // This normalization is only for world-space SpriteRenderers.
        // UI Images use RectTransform sizing and should not be changed here.
        if (spriteRenderer == null)
            return;

        Sprite reference = spriteRenderer.sprite;

        if (reference == null &&
            sprites != null &&
            sprites.Length > 0)
        {
            reference = sprites[0];
        }

        if (reference == null)
            return;

        defaultWorldSpriteSize = reference.bounds.size;
        hasDefaultWorldSpriteSize =
            defaultWorldSpriteSize.x > 0f &&
            defaultWorldSpriteSize.y > 0f;
    }

    public void RestartAnimation()
    {
        if (!isActiveAndEnabled)
            return;

        if (animateRoutine != null)
            StopCoroutine(animateRoutine);

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (uiImage == null)
            uiImage = GetComponent<Image>();

        animateRoutine = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (sprites == null || sprites.Length == 0)
            yield break;

        if (spriteRenderer == null && uiImage == null)
            yield break;

        int frame = UnityEngine.Random.Range(0, sprites.Length);

        if (startAt0)
            frame = 0;

        while (true)
        {
            if (frame < 0 || frame >= sprites.Length)
                frame = 0;

            SetDisplayedSprite(sprites[frame]);

            if (holdLastFrame && frame == sprites.Length - 1)
                yield break;

            frame = (frame + 1) % sprites.Length;

            if (frameRate <= 0f)
            {
                yield return null;
            }
            else if (useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(1f / frameRate);
            }
            else
            {
                yield return new WaitForSeconds(1f / frameRate);
            }
        }
    }

    private void SetDisplayedSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;

        if (uiImage != null)
            uiImage.sprite = sprite;
    }

    public void SetSprites(
        Sprite[] newSprites,
        bool restartAtZero = true,
        bool newHoldLastFrame = false,
        float newFrameRate = -1f)
    {
        if (newSprites == null || newSprites.Length == 0)
            return;

        if (!defaultsCaptured)
            CaptureDefaults();

        // The normal magician art is 1820x1024, while the supplied reaction
        // animations are 560x315. With the same Unity Pixels Per Unit, the
        // reaction sprite would render about 3.25x smaller.
        //
        // Scale world-space SpriteRenderer animations to match the original
        // sprite's world-space dimensions automatically.
        if (spriteRenderer != null &&
            hasDefaultWorldSpriteSize &&
            newSprites[0] != null)
        {
            Vector2 newSize = newSprites[0].bounds.size;

            if (newSize.x > 0f && newSize.y > 0f)
            {
                float widthRatio =
                    defaultWorldSpriteSize.x / newSize.x;

                float heightRatio =
                    defaultWorldSpriteSize.y / newSize.y;

                // They have essentially the same aspect ratio, but averaging
                // protects against tiny rounding differences.
                float scaleRatio =
                    (widthRatio + heightRatio) * 0.5f;

                transform.localScale =
                    defaultLocalScale * scaleRatio;
            }
        }

        sprites = newSprites;
        startAt0 = restartAtZero;
        holdLastFrame = newHoldLastFrame;

        if (newFrameRate > 0f)
            frameRate = newFrameRate;

        SetDisplayedSprite(sprites[0]);
        RestartAnimation();
    }

    public void ResetToDefault()
    {
        if (!defaultsCaptured ||
            defaultSprites == null ||
            defaultSprites.Length == 0)
        {
            return;
        }

        // Restore the magician/object to its original scale when returning
        // to the default idle animation.
        transform.localScale = defaultLocalScale;

        sprites = new Sprite[defaultSprites.Length];
        Array.Copy(defaultSprites, sprites, defaultSprites.Length);

        frameRate = defaultFrameRate;
        startAt0 = defaultStartAt0;
        holdLastFrame = defaultHoldLastFrame;

        RestartAnimation();
    }

    public static Sprite[] LoadSpritesFromResources(string path)
    {
        Sprite[] loaded = Resources.LoadAll<Sprite>(path);

        if (loaded == null || loaded.Length == 0)
            return loaded;

        Array.Sort(
            loaded,
            (a, b) => string.CompareOrdinal(a.name, b.name)
        );

        return loaded;
    }
}
