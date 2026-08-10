using System.Collections;
using UnityEngine;

public class finale : MonoBehaviour
{
    public Transform finaleCupProgression;
    public Transform ball;

    private Collider2D finalCupCollider;
    private fadeWClick finalClick;
    private bool prepared;

    void Start()
    {
        // The normal cup-selection scripts should no longer control the
        // surviving boss cup once the finale begins.
        ChooseCup chooseCup = GetComponent<ChooseCup>();
        if (chooseCup != null)
            Destroy(chooseCup);

        cupOverlap overlap = GetComponent<cupOverlap>();
        if (overlap != null)
            Destroy(overlap);

        finalCupCollider = GetComponent<Collider2D>();
        if (finalCupCollider != null)
            finalCupCollider.enabled = false;

        PrepareNestedCups();

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sortingOrder = 0;

        finalClick = GetComponent<fadeWClick>();
        if (finalClick == null)
            finalClick = gameObject.AddComponent<fadeWClick>();

        // The boss fight's final-ball movement uses ball.localPosition.
        // Parent the ball to this cup first so that movement is relative
        // to the surviving cup instead of world-space (0,0).
        if (ball != null)
            ball.SetParent(transform, true);

        StartCoroutine(FinishFinaleSetup());
    }

    private void PrepareNestedCups()
    {
        GameObject progressionObject =
            GameObject.Find("finalCupProgression");

        if (progressionObject == null)
        {
            Debug.LogWarning(
                "Finale: finalCupProgression was not found. " +
                "The final cup will still end the game when clicked."
            );
            return;
        }

        finaleCupProgression = progressionObject.transform;

        // Put the progression at the surviving cup's center.
        finaleCupProgression.SetParent(transform, false);
        finaleCupProgression.localPosition = Vector3.zero;
        finaleCupProgression.localRotation = Quaternion.identity;
        finaleCupProgression.localScale = Vector3.one;

        // The scene stores an empty wrapper named finalCupProgression around
        // the first nested cup. Flatten that wrapper so every click can simply
        // reveal the next cup.
        if (finaleCupProgression.childCount > 0)
        {
            Transform firstNestedCup =
                finaleCupProgression.GetChild(0);

            firstNestedCup.SetParent(transform, false);

            // Do not destroy immediately before the child reparent settles.
            Destroy(finaleCupProgression.gameObject);
            finaleCupProgression = firstNestedCup;
        }
    }

    private IEnumerator FinishFinaleSetup()
    {
        if (ball != null)
        {
            // fightManager waits one second before moving the final ball,
            // then accelerates it toward localPosition zero.
            float timeout = 3.5f;
            float elapsed = 0f;

            while (ball != null &&
                   Vector2.Distance(ball.localPosition, Vector2.zero) > 0.15f &&
                   elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (ball != null)
            {
                Transform deepestCup = FindDeepestNestedCup();

                if (deepestCup != null)
                {
                    // Preserve the ball's apparent size while nesting it,
                    // then center it under the smallest cup.
                    ball.SetParent(deepestCup, true);
                    ball.localPosition = Vector3.zero;
                }

                SpriteRenderer ballRenderer =
                    ball.GetComponent<SpriteRenderer>();

                if (ballRenderer != null)
                {
                    ballRenderer.enabled = true;
                    ballRenderer.sortingOrder = 5;
                }
            }
        }

        if (finalCupCollider != null)
            finalCupCollider.enabled = true;

        prepared = true;

        Debug.Log(
            "Finale ready: click through the nested cups to finish the game."
        );
    }

    private Transform FindDeepestNestedCup()
    {
        Transform current = transform;
        Transform deepest = null;

        while (true)
        {
            Transform next = null;

            for (int i = 0; i < current.childCount; i++)
            {
                Transform child = current.GetChild(i);

                // Nested finale cups already have fadeWClick in the scene.
                // The ball does not, so this cleanly ignores the ball.
                if (child.GetComponent<fadeWClick>() != null)
                {
                    next = child;
                    break;
                }
            }

            if (next == null)
                break;

            deepest = next;
            current = next;
        }

        return deepest;
    }

    public bool IsPrepared()
    {
        return prepared;
    }
}
