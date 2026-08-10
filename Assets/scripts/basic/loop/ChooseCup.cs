using System.Collections;
using UnityEngine;

public class ChooseCup : MonoBehaviour
{
    public float revealDelay;
    public GameObject clickBlocker;

    private lvlProgress progress;
    private LowerCup cupRaiser;
    private fightManager bossFight;

    void Start()
    {
        cupRaiser = FindObjectOfType<LowerCup>();
        progress = FindObjectOfType<lvlProgress>();
        bossFight = FindObjectOfType<fightManager>();

        // Don't rely on GameObject.Find because clickBlocker
        // may be inactive when this cup is created.
        if (clickBlocker == null)
        {
            Shuffle shuffle = FindObjectOfType<Shuffle>();

            if (shuffle != null)
                clickBlocker = shuffle.clickBlocker;
        }

        if (clickBlocker == null)
            Debug.LogError("ChooseCup could not find clickBlocker.", this);
    }

    void OnMouseDown()
    {
        // Prevent a NullReferenceException.
        if (clickBlocker == null)
            return;

        // Only allow cup selection at the proper time.
        if (!clickBlocker.activeSelf)
        {
            // If there is no boss manager, normal gameplay still works.
            if (bossFight == null || !bossFight.haltCup(transform))
            {
                // Leave selection music and return to the main gameplay loop
                // as soon as the player commits to a cup.
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayGameplayMusic();

                StartCoroutine(DoReveal());
            }
        }
    }

    IEnumerator DoReveal()
    {
        if (clickBlocker == null ||
            cupRaiser == null ||
            progress == null)
        {
            yield break;
        }

        clickBlocker.SetActive(true);

        // The ball is hidden only during the actual shuffle.
        // Turn it back on BEFORE any cup begins its reveal/lift animation.
        if (cupRaiser.reparenter != null &&
            cupRaiser.reparenter.ball != null)
        {
            SpriteRenderer ballRenderer =
                cupRaiser.reparenter.ball.GetComponent<SpriteRenderer>();

            if (ballRenderer != null)
                ballRenderer.enabled = true;
        }

        bool won = false;

        if (transform.childCount > 0 &&
            transform.GetChild(0).gameObject.name == "ball")
        {
            transform.GetChild(0).SetParent(null, true);
            won = true;
        }

        cupRaiser.lowerCup(transform, true);

        yield return new WaitForSeconds(
            cupRaiser.duration + revealDelay
        );

        progress.initialDone();

        yield return StartCoroutine(
            cupRaiser.DoLower(true, transform)
        );

        if (won)
            progress.won();
        else
            progress.lost();
    }
}
