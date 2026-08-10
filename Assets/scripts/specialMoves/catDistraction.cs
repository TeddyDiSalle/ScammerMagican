using System.Collections;
using UnityEngine;

public class catDistraction : MonoBehaviour
{
    public Transform cat;
    public float distance;
    public float duration;

    private bool alreadyRunning;

    void Start()
    {
        SetupCatVisual();
    }

    public float callCat()
    {
        if (!alreadyRunning)
            StartCoroutine(catRun());

        return 0f;
    }

    IEnumerator catRun()
    {
        alreadyRunning = true;

        Vector2 goal = cat.transform.position;

        // Keep the original movement logic:
        // the cat starts from one side and charges across the screen.
        if (cat.transform.position.x < 0)
        {
            goal += new Vector2(distance, 0f);
            cat.localScale = new Vector2(1f, 1f);
        }
        else
        {
            goal -= new Vector2(distance, 0f);
            cat.localScale = new Vector2(-1f, 1f);
        }

        yield return StartCoroutine(
            GoToPos.MoveCoroutine(cat, goal, duration)
        );

        alreadyRunning = false;
    }

    private void SetupCatVisual()
    {
        if (cat == null)
            return;

        Sprite[] catFrames =
            animSprite.LoadSpritesFromResources("Art/CatAnim");

        if (catFrames == null || catFrames.Length == 0)
            return;

        // Hide the placeholder cat parts.
        foreach (SpriteRenderer oldRenderer
                 in cat.GetComponentsInChildren<SpriteRenderer>(true))
        {
            oldRenderer.enabled = false;
        }

        Transform existingVisual = cat.Find("CatVisual");
        GameObject visual;

        if (existingVisual == null)
        {
            visual = new GameObject("CatVisual");
            visual.transform.SetParent(cat, false);
        }
        else
        {
            visual = existingVisual.gameObject;
        }

        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.42f, 0.42f, 1f);

        SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = visual.AddComponent<SpriteRenderer>();

        renderer.enabled = true;
        renderer.sortingOrder = 2;
        renderer.sprite = catFrames[0];

        animSprite animator = visual.GetComponent<animSprite>();
        if (animator == null)
            animator = visual.AddComponent<animSprite>();

        animator.sprites = catFrames;
        animator.frameRate = 12f;
        animator.startAt0 = true;
        animator.holdLastFrame = false;
        animator.useUnscaledTime = true;
        animator.RestartAnimation();
    }
}
