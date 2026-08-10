using System.Collections;
using UnityEngine;

public class gameOrder : MonoBehaviour
{
    public setCups cupSetter;
    public LowerCup cupLowerer;

    private const string FullBackgroundResource =
        "Art/Background/ScammerMagician_Background";

    void Start()
    {
        SetupSingleBackground();
        callGame();
    }

    public void callGame()
    {
        cupSetter.MakeCups();
        StartCoroutine(cupLowerer.StartRound());
    }

    private void SetupSingleBackground()
    {
        // Remove the separate ocean/grass/cardboard decor created by
        // the previous visual-art pack.
        GameObject oldDecor = GameObject.Find("WorldDecorRoot");
        if (oldDecor != null)
            Destroy(oldDecor);

        // Disable the old painted background sprite so it does not show
        // through or compete with the new complete background image.
        SpriteRenderer[] allRenderers =
            FindObjectsOfType<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in allRenderers)
        {
            if (renderer != null &&
                renderer.gameObject.name == "background")
            {
                renderer.enabled = false;
            }
        }

        // Avoid making duplicates if this setup is called more than once.
        GameObject existing = GameObject.Find("FullBackground");
        if (existing != null)
            Destroy(existing);

        Sprite backgroundSprite =
            Resources.Load<Sprite>(FullBackgroundResource);

        if (backgroundSprite == null)
        {
            Debug.LogError(
                "Could not load the new full background at Resources/" +
                FullBackgroundResource
            );
            return;
        }

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError(
                "Cannot place the new background because Main Camera was not found."
            );
            return;
        }

        GameObject backgroundObject =
            new GameObject("FullBackground");

        SpriteRenderer backgroundRenderer =
            backgroundObject.AddComponent<SpriteRenderer>();

        backgroundRenderer.sprite = backgroundSprite;
        backgroundRenderer.sortingOrder = -100;

        // Center it directly on the camera.
        backgroundObject.transform.position =
            new Vector3(
                cam.transform.position.x,
                cam.transform.position.y,
                0f
            );

        // Automatically scale the 16:9 image so it fills the entire
        // orthographic camera view without requiring Inspector adjustments.
        if (cam.orthographic)
        {
            float cameraHeight = cam.orthographicSize * 2f;
            float cameraWidth = cameraHeight * cam.aspect;

            Vector2 spriteSize = backgroundSprite.bounds.size;

            if (spriteSize.x > 0f && spriteSize.y > 0f)
            {
                float widthScale =
                    cameraWidth / spriteSize.x;

                float heightScale =
                    cameraHeight / spriteSize.y;

                // Cover the whole screen; crop a little if the window
                // aspect ratio is not exactly 16:9.
                float finalScale =
                    Mathf.Max(widthScale, heightScale);

                backgroundObject.transform.localScale =
                    new Vector3(
                        finalScale,
                        finalScale,
                        1f
                    );
            }
        }
    }
}
